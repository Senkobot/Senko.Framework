using System;
using System.Reflection;
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