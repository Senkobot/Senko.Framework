using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Senko.Framework;

namespace Senko.Commands
{
    public interface ICommandValueProvider
    {
        /// <summary>
        ///     True if the current provider can provide the given <see cref="type"/>.
        /// </summary>
        /// <param name="type">The type to provide.</param>
        /// <returns>True if the provider can provide the type.</returns>
        bool CanProvide(Type type);

        /// <summary>
        ///     Get the assemblies being used by the value provider.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The assemblies.</returns>
        IEnumerable<Assembly> GetAssemblies(Type type);

        /// <summary>
        ///     Get the Roslyn expression.
        /// </summary>
        /// <returns>The expression.</returns>
        ExpressionSyntax GetRoslynExpression(RoslynExpressionContext context);

        /// <summary>
        ///     Get the value from the context.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="context">The context.</param>
        /// <returns>The value.</returns>
        Task<object> GetValueAsync(ParameterInfo parameter, MessageContext context);
    }
}
