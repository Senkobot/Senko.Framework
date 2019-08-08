using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Senko.Commands.Roslyn
{
    internal class CommandInformation
    {
        public CommandInformation(object module, string typeName, ClassDeclarationSyntax declarationSyntax)
        {
            Module = module;
            TypeName = typeName;
            DeclarationSyntax = declarationSyntax;
        }

        public object Module { get; }

        public string TypeName { get; }

        public ClassDeclarationSyntax DeclarationSyntax { get; }
    }
}