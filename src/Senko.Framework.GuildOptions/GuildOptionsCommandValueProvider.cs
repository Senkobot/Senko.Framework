using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Senko.Commands;
using Senko.Framework.Managers;
using S = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Senko.Framework
{
    public class GuildOptionsCommandValueProvider : ICommandValueProvider
    {
        private const string GuildIdNotDefined = "Cannot get the guild options from a non-guild message.";

        private static readonly MethodInfo GetMethod = typeof(SettingExtensions)
            .GetMethod(nameof(SettingExtensions.GetOptionsAsync));

        public bool CanProvide(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IGuildOptions<>);
        }

        public IEnumerable<Assembly> GetAssemblies(Type type)
        {
            var optionsType = type.GetGenericArguments()[0];

            yield return typeof(IGuildOptionsManager).Assembly;
            yield return optionsType.Assembly;
        }

        public ExpressionSyntax GetRoslynExpression(RoslynExpressionContext context)
        {
            var manager = context.GetService(typeof(IGuildOptionsManager));
            var optionsType = context.ExpectedType.GetGenericArguments()[0];
            var typeSyntax = S.ParseTypeName(optionsType.FullName);
            var getName = S.GenericName(nameof(SettingExtensions.GetOptionsAsync)).WithTypeArgumentList(S.TypeArgumentList().AddArguments(typeSyntax));
            var get = S.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, manager, getName);
            var guildId = S.BinaryExpression(
                SyntaxKind.CoalesceExpression,
                context.GetRequestProperty(nameof(MessageRequest.GuildId)),
                S.ThrowExpression(S.ObjectCreationExpression(
                    S.IdentifierName("InvalidOperationException"),
                    S.ArgumentList().AddArguments(S.Argument(S.LiteralExpression(SyntaxKind.StringLiteralExpression, S.Literal(GuildIdNotDefined)))),
                    null
                ))
            );
            var invoke = S.InvocationExpression(get, S.ArgumentList().AddArguments(S.Argument(guildId)));

            return S.AwaitExpression(invoke);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public async ValueTask<object> GetValueAsync(ParameterInfo parameter, MessageContext context)
        {
            var optionsManager = context.RequestServices.GetRequiredService<IGuildOptionsManager>();
            var type = parameter.ParameterType;
            var optionsType = type.GetGenericArguments()[0];
            var method = GetMethod.MakeGenericMethod(optionsType);
            var asTask = typeof(ValueTask<>).MakeGenericType(type).GetMethod("AsTask");
            var resultProperty = typeof(Task<>).MakeGenericType(type).GetProperty("Result");

            if (!context.Request.GuildId.HasValue)
            {
                throw new InvalidOperationException(GuildIdNotDefined);
            }

            var task = (Task)asTask.Invoke(method.Invoke(null, new object[]
            {
                optionsManager,
                context.Request.GuildId.Value
            }), Array.Empty<object>());

            await task;

            return resultProperty.GetValue(task);
        }
    }
}
