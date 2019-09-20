using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Senko.Commands
{
    internal static class RoslynContextExtensions
    {
        public static ArgumentListSyntax GetReadArguments(this RoslynExpressionContext context, ArgumentListSyntax extraParameters = null)
        {
            var required = !context.Parameter.GetCustomAttributes<OptionalAttribute>().Any();
            var result = SyntaxFactory.ArgumentList().AddArguments(
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(context.Parameter.Name))),
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(required ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression))
            );

            if (extraParameters != null)
            {
                result = result.AddArguments(extraParameters.Arguments.ToArray());
            }

            return result;
        }
    }
}
