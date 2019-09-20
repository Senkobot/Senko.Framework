using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Senko.Framework;

namespace Senko.Commands
{
    public struct RoslynExpressionContext
    {
        private readonly string _contextName;

        public RoslynExpressionContext(ParameterInfo parameter, string contextName)
        {
            Parameter = parameter;
            _contextName = contextName;
        }

        public Type ExpectedType => Parameter.ParameterType;

        public ParameterInfo Parameter { get; }

        public ArgumentListSyntax GetReadArguments(ArgumentListSyntax extraParameters = null)
        {
            // TODO: Move this to an extension.

            var required = !Parameter.GetCustomAttributes<OptionalAttribute>().Any();
            var result = SyntaxFactory.ArgumentList().AddArguments(
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(Parameter.Name))),
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(required ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression))
            );

            if (extraParameters != null)
            {
                result = result.AddArguments(extraParameters.Arguments.ToArray());
            }

            return result;
        }


        public ExpressionSyntax GetContextProperty(string name)
        {
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(_contextName), SyntaxFactory.IdentifierName(name));
        }

        public ExpressionSyntax GetRequestProperty(string name)
        {
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, GetContextProperty(nameof(MessageContext.Request)), SyntaxFactory.IdentifierName(name));
        }

        public ExpressionSyntax GetService(Type type)
        {
            var typeSyntax = SyntaxFactory.ParseTypeName(type.FullName);
            var services = GetContextProperty(nameof(MessageContext.RequestServices));
            var getName = SyntaxFactory.GenericName(nameof(IServiceProvider.GetService)).WithTypeArgumentList(SyntaxFactory.TypeArgumentList().AddArguments(typeSyntax));
            var getService = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, services, getName);
            return SyntaxFactory.InvocationExpression(getService);
        }
    }
}