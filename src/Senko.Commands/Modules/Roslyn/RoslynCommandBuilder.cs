using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Senko.Discord;
using Senko.Arguments;
using Senko.Common;
using Senko.Framework;
using S = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Senko.Commands.Roslyn
{
    public class RoslynCommandBuilder
    {
        private const string AssemblyName = "Senko.CompiledCommands";
        private const string Namespace = "Senko.CompiledCommands";

        private const string ArgumentContext = "context";
        private const string ArgumentModule = "module";

        private static readonly TypeSyntax StringType = S.ParseTypeName("string");
        private static readonly TypeSyntax BoolType = S.ParseTypeName("bool");
        private static readonly TypeSyntax ReadOnlyStringList = S.ParseTypeName("System.Collections.Generic.IReadOnlyList<string>");
        private static readonly TypeSyntax CommandType = S.ParseTypeName(typeof(ICommand).Name);
        private static readonly TypeSyntax TaskType = S.ParseTypeName(nameof(Task));
        private static readonly TypeSyntax MessageContextType = S.ParseTypeName(nameof(MessageContext));
        private static readonly TypeSyntax ModuleContextType = S.ParseTypeName(typeof(ModuleContext).FullName);

        private static readonly IDictionary<Type, Func<RoslynExpressionContext, ExpressionSyntax>> ValueFactories = new Dictionary<Type, Func<RoslynExpressionContext, ExpressionSyntax>>
        {
            [typeof(IDiscordGuild)] = p => S.AwaitExpression(S.InvocationExpression(p.GetRequestProperty(nameof(RequestExtensions.GetGuildAsync)), p.GetReadArguments())),
            [typeof(IDiscordUser)] = p => S.AwaitExpression(S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadUserMentionAsync)), p.GetReadArguments())),
            [typeof(IDiscordGuildUser)] = p => S.AwaitExpression(S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadGuildUserMentionAsync)), p.GetReadArguments())),
            [typeof(IDiscordRole)] = p => S.AwaitExpression(S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadRoleMentionAsync)), p.GetReadArguments())),
            [typeof(IDiscordChannel)] = p => S.AwaitExpression(S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadGuildChannelAsync)), p.GetReadArguments())),
            [typeof(long)] = p => S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadInt64)), p.GetReadArguments()),
            [typeof(ulong)] = p => S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadUInt64)), p.GetReadArguments()),
            [typeof(int)] = p => S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadInt32)), p.GetReadArguments()),
            [typeof(uint)] = p => S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadUInt32)), p.GetReadArguments()),
            [typeof(MessageContext)] = p => S.IdentifierName(ArgumentContext),
            [typeof(MessageRequest)] = p => p.GetContextProperty(nameof(MessageContext.Request)),
            [typeof(MessageResponse)] = p => p.GetContextProperty(nameof(MessageContext.Response)),
            [typeof(string)] = p =>
            {
                var isRemaining = p.Parameter.GetCustomAttributes<RemainingAttribute>().Any();
                var isUnsafe = p.Parameter.GetCustomAttributes<UnsafeAttribute>().Any();
                var escapeTypes = p.Parameter.GetCustomAttribute<EscapeAttribute>()?.Type ?? EscapeType.Default;
                var safeArgs = S.ArgumentList();

                if (escapeTypes != EscapeType.Default)
                {
                    var escapeTypesExpression = S.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, S.IdentifierName(nameof(EscapeType)), S.IdentifierName(escapeTypes.ToString()));
                    safeArgs = safeArgs.AddArguments(S.Argument(escapeTypesExpression));
                }

                // Read remaining string.
                if (isRemaining)
                {
                    if (isUnsafe)
                    {
                        return S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadUnsafeRemaining)), p.GetReadArguments());
                    }

                    return S.AwaitExpression(S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadRemainingAsync)), p.GetReadArguments(safeArgs)));
                }

                // Read an normal string.
                if (isUnsafe)
                {
                    return S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadUnsafeString)), p.GetReadArguments());
                }

                return S.AwaitExpression(S.InvocationExpression(p.GetRequestProperty(nameof(ArgumentRequestExtensions.ReadStringAsync)), p.GetReadArguments(safeArgs)));
            }
        };

        private readonly IReadOnlyList<ICommandValueProvider> _valueProviders;
        private readonly Dictionary<string, CommandInformation> _classes;
        private readonly IList<string> _assemblies = new List<string>
        {
            // .NET Core / Standard
            typeof(object).Assembly.Location,
            Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll"),
            Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "netstandard.dll"),

            typeof(IDiscordUser).Assembly.Location,                     // Senko.Discord.Common
            typeof(IServiceProvider).Assembly.Location,                 // System.ComponentModel
            typeof(EscapeType).Assembly.Location,                       // Senko.Common
            typeof(MessageContext).Assembly.Location,                   // Senko.Framework
            typeof(IArgumentReader).Assembly.Location,                  // Senko.Arguments
            typeof(ICommand).Assembly.Location,                         // Senko.Modules
            typeof(ServiceProviderServiceExtensions).Assembly.Location  // Microsoft.Extensions.DependencyInjection
        };

        public RoslynCommandBuilder(IReadOnlyList<ICommandValueProvider> valueProviders)
        {
            _valueProviders = valueProviders;
            _classes = new Dictionary<string, CommandInformation>();
        }

        public void AddModules(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AddModule(type);
            }
        }

        public void AddModule(Type type)
        {
            var assemblyPath = type.Assembly.Location;

            if (!_assemblies.Contains(assemblyPath))
            {
                _assemblies.Add(assemblyPath);
            }

            foreach (var (id, aliases, method) in ModuleUtils.GetMethods(type))
            {
                if (_classes.ContainsKey(id))
                {
                    throw new InvalidOperationException($"The command {id} is already registered.");
                }

                var className = ToCamelCase(id) + "Command";
                var typeName = Namespace + '.' + className;

                _classes.Add(id, new CommandInformation(type, typeName, CreateCommandClass(id, aliases, className, type, method)));
            }
        }

        public IEnumerable<ICommand> Compile()
        {
            var syntaxTree = CSharpSyntaxTree.Create(CreateCompilationUnit());

            var compilation = CSharpCompilation.Create(
                AssemblyName,
                new[] { syntaxTree },
                _assemblies.Select(p => MetadataReference.CreateFromFile(p)).ToArray(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using var dllStream = new MemoryStream();
            using var pdbStream = new MemoryStream();

            var emitResult = compilation.Emit(dllStream, pdbStream);
            if (!emitResult.Success)
            {
                // emitResult.Diagnostics
                throw new InvalidOperationException(string.Join("\n", emitResult.Diagnostics.Select(d => d.GetMessage())));
            }

            var assembly = Assembly.Load(dllStream.ToArray());

            return _classes.Select(kv => (ICommand) Activator.CreateInstance(assembly.GetType(kv.Value.TypeName)));
        }

        /// <summary>
        ///     Create the <see cref="CompilationUnitSyntax"/> of the current <see cref="_classes"/>.
        /// </summary>
        /// <returns>The <see cref="CompilationUnitSyntax"/>.</returns>
        private CompilationUnitSyntax CreateCompilationUnit()
        {
            return S.CompilationUnit()
                .AddUsings(
                    S.UsingDirective(S.ParseName("System")),
                    S.UsingDirective(S.ParseName(typeof(Task).Namespace)),
                    S.UsingDirective(S.ParseName(typeof(MessageContext).Namespace)),
                    S.UsingDirective(S.ParseName(typeof(IArgumentReader).Namespace)),
                    S.UsingDirective(S.ParseName(typeof(ICommand).Namespace)),
                    S.UsingDirective(S.ParseName(typeof(IDiscordUser).Namespace)),
                    S.UsingDirective(S.ParseName(typeof(ServiceProviderServiceExtensions).Namespace))
                )
                .AddMembers(
                    S.NamespaceDeclaration(S.IdentifierName(Namespace))
                        .AddMembers(_classes.Select(kv => kv.Value.DeclarationSyntax).Cast<MemberDeclarationSyntax>().ToArray())
                );
        }

        /// <summary>
        ///     Converts a string to camel case.
        /// </summary>
        /// <example>
        ///     ToCamelCase("foo_bar")  ->  "FooBar"
        ///     ToCamelCase("foo bar")  ->  "FooBar"
        /// </example>
        /// <param name="input">The input string.</param>
        /// <returns>The camel cased string.</returns>
        private static string ToCamelCase(string input)
        {
            var words = new Regex(@"[^A-Za-z]+")
                .Split(input)
                .Select(word => word.First().ToString().ToUpper() + word.Substring(1).ToLower());

            return string.Join("", words);
        }

        /// <summary>
        ///     Create a class that implements <see cref="ICommand"/>.
        ///
        ///     The implementation will execute the given <see cref="method"/>.
        /// </summary>
        /// <param name="id">The unique identifier of the command for <see cref="ICommand.Id"/>.</param>
        /// <param name="className">The new class name.</param>
        /// <param name="moduleType">The type that contains the <see cref="method"/>.</param>
        /// <param name="method">The method that should be executed when <see cref="ICommand.ExecuteAsync"/> is called.</param>
        /// <returns>The <see cref="ClassDeclarationSyntax"/>.</returns>
        private ClassDeclarationSyntax CreateCommandClass(
            string id,
            IReadOnlyList<string> aliases,
            string className,
            Type moduleType,
            MethodInfo method)
        {
            var permission = ModuleUtils.GetPermissionName(moduleType, method);
            var moduleName = ModuleUtils.GetModuleName(moduleType);
            var attr = method.GetCustomAttribute<CommandAttribute>();
            var guildOnly = attr.GuildOnly;
            var permissionGroup = attr.PermissionGroup.ToString();

            return S.ClassDeclaration(className)
                .AddBaseListTypes(S.SimpleBaseType(CommandType))
                .AddMembers(
                    // ICommand.Id
                    S.PropertyDeclaration(StringType, nameof(ICommand.Id))
                        .AddAccessorListAccessors(
                            S.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .AddBodyStatements(S.ReturnStatement(S.LiteralExpression(SyntaxKind.StringLiteralExpression, S.Literal(id))))
                                .WithSemicolonToken(S.Token(SyntaxKind.SemicolonToken))
                        )
                        .AddModifiers(S.Token(SyntaxKind.PublicKeyword)),

                    // ICommand.Aliases
                    S.PropertyDeclaration(ReadOnlyStringList, nameof(ICommand.Aliases))
                        .AddAccessorListAccessors(
                            S.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .AddBodyStatements(S.ReturnStatement(S.ArrayCreationExpression(
                                    S.ArrayType(StringType)
                                        .WithRankSpecifiers(S.SingletonList(S.ArrayRankSpecifier(S.SingletonSeparatedList<ExpressionSyntax>(S.OmittedArraySizeExpression())))),
                                    S.InitializerExpression(SyntaxKind.ArrayInitializerExpression, 
                                        new SeparatedSyntaxList<ExpressionSyntax>()
                                            .AddRange(aliases.Select(s => S.LiteralExpression(SyntaxKind.StringLiteralExpression, S.Literal(s))))))))
                                .WithSemicolonToken(S.Token(SyntaxKind.SemicolonToken))
                        )
                        .AddModifiers(S.Token(SyntaxKind.PublicKeyword)),

                    // ICommand.Module
                    S.PropertyDeclaration(StringType, nameof(ICommand.Module))
                        .AddAccessorListAccessors(
                            S.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .AddBodyStatements(S.ReturnStatement(S.LiteralExpression(SyntaxKind.StringLiteralExpression, S.Literal(moduleName))))
                                .WithSemicolonToken(S.Token(SyntaxKind.SemicolonToken))
                        )
                        .AddModifiers(S.Token(SyntaxKind.PublicKeyword)),

                    // ICommand.Permission
                    S.PropertyDeclaration(StringType, nameof(ICommand.Permission))
                        .AddAccessorListAccessors(
                            S.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .AddBodyStatements(S.ReturnStatement(S.LiteralExpression(SyntaxKind.StringLiteralExpression, S.Literal(permission))))
                                .WithSemicolonToken(S.Token(SyntaxKind.SemicolonToken))
                        )
                        .AddModifiers(S.Token(SyntaxKind.PublicKeyword)),

                    // ICommand.PermissionGroup
                    S.PropertyDeclaration(S.ParseName(nameof(PermissionGroup)), nameof(ICommand.PermissionGroup))
                        .AddAccessorListAccessors(
                            S.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .AddBodyStatements(S.ReturnStatement(S.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, S.IdentifierName(nameof(PermissionGroup)), S.IdentifierName(permissionGroup))))
                                .WithSemicolonToken(S.Token(SyntaxKind.SemicolonToken))
                        )
                        .AddModifiers(S.Token(SyntaxKind.PublicKeyword)),

                    // ICommand.GuildOnly
                    S.PropertyDeclaration(BoolType, nameof(ICommand.GuildOnly))
                        .AddAccessorListAccessors(
                            S.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .AddBodyStatements(S.ReturnStatement(S.LiteralExpression(guildOnly ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)))
                                .WithSemicolonToken(S.Token(SyntaxKind.SemicolonToken))
                        )
                        .AddModifiers(S.Token(SyntaxKind.PublicKeyword)),

                    // ICommand.ExecuteAsync
                    S.MethodDeclaration(TaskType, nameof(ICommand.ExecuteAsync))
                        .AddModifiers(S.Token(SyntaxKind.PublicKeyword), S.Token(SyntaxKind.AsyncKeyword))
                        .AddParameterListParameters(Parameter(ArgumentContext, MessageContextType))
                        .AddBodyStatements(
                            S.ExpressionStatement(InvokeCommand(moduleType, method))
                        )
                );
        }

        /// <summary>
        ///     Create a new <see cref="ExpressionSyntax"/> that invokes the <see cref="method"/>.
        /// </summary>
        /// <param name="moduleType">The module type.</param>
        /// <param name="method">The method to invoke.</param>
        /// <returns>The <see cref="ExpressionSyntax"/>.</returns>
        private ExpressionSyntax InvokeCommand(Type moduleType, MethodInfo method)
        {
            var moduleSyntaxType = S.ParseTypeName(moduleType.FullName);
            var listSyntax = S.ArgumentList();

            var constructor = moduleType.GetConstructors().FirstOrDefault();

            if (constructor != null)
            {
                listSyntax = listSyntax.AddArguments(constructor.GetParameters().Select(p => S.Argument(GetValueFactory(p))).ToArray());
            }

            var moduleInstance = S.ObjectCreationExpression(moduleSyntaxType).WithArgumentList(listSyntax);
            var contextProperty = moduleType
                .GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes<ModuleContextAttribute>().Any());

            if (contextProperty != null)
            {
                var contextInstance = S.ObjectCreationExpression(ModuleContextType)
                        .WithInitializer(
                            S.InitializerExpression(
                                SyntaxKind.ObjectInitializerExpression,
                                S.SingletonSeparatedList<ExpressionSyntax>(
                                    S.AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        S.IdentifierName(nameof(ModuleContext.Context)),
                                        S.IdentifierName(ArgumentContext)
                                    ))));
                
                moduleInstance = moduleInstance.WithInitializer(
                    S.InitializerExpression(
                        SyntaxKind.ObjectInitializerExpression,
                        S.SingletonSeparatedList<ExpressionSyntax>(
                            S.AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                S.IdentifierName(contextProperty.Name),
                                contextInstance
                                ))));
            }

            ExpressionSyntax invoke = S.InvocationExpression(
                S.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    moduleInstance, 
                    S.IdentifierName(method.Name)
                ),
                S.ArgumentList().AddArguments(method.GetParameters().Select(p => S.Argument(GetValueFactory(p))).ToArray())
            );

            if (method.ReturnType == typeof(Task) || method.ReturnType == typeof(ValueTask))
            {
                invoke = S.AwaitExpression(invoke);
            }
            else if (method.ReturnType != typeof(void))
            {
                throw new InvalidOperationException($"The return type {method.ReturnType.Name} is not supported.");
            }

            return invoke;
        }

        /// <summary>
        ///     Get the <see cref="ExpressionSyntax"/> for the given parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The <see cref="ExpressionSyntax"/>.</returns>
        private ExpressionSyntax GetValueFactory(ParameterInfo parameter)
        {
            var type = parameter.ParameterType;
            var context = new RoslynExpressionContext(parameter, ArgumentContext);

            if (ValueFactories.TryGetValue(type, out var value))
            {
                return value(context);
            }

            foreach (var valueProvider in _valueProviders)
            {
                if (!valueProvider.CanProvide(type))
                {
                    continue;
                }

                foreach (var assembly in valueProvider.GetAssemblies(type))
                {
                    if (!_assemblies.Contains(assembly.Location))
                    {
                        _assemblies.Add(assembly.Location);
                    }
                }

                return valueProvider.GetRoslynExpression(context);
            }

            return context.GetService(type);
        }

        private static FieldDeclarationSyntax Field(string name, TypeSyntax type, params SyntaxKind[] modifiers)
        {
            return S.FieldDeclaration(S.VariableDeclaration(type, S.SeparatedList(new[] { S.VariableDeclarator(name) })))
                .AddModifiers(modifiers.Select(S.Token).ToArray());
        }

        private static ParameterSyntax Parameter(string name, TypeSyntax type)
        {
            return S.Parameter(S.Identifier(name))
                .WithType(type);
        }
    }
}
