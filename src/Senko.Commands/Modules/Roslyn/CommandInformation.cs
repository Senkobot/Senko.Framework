using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Senko.Commands.Roslyn
{
    internal class CommandInformation
    {
        public CommandInformation(Type moduleType, string typeName, ClassDeclarationSyntax declarationSyntax)
        {
            ModuleType = moduleType;
            TypeName = typeName;
            DeclarationSyntax = declarationSyntax;
        }

        public Type ModuleType { get; }

        public string TypeName { get; }

        public ClassDeclarationSyntax DeclarationSyntax { get; }
    }
}