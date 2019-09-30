using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Senko.Commands
{
    public interface ICommandBuilder
    {
        IServiceCollection Services { get; }

        ICommandBuilder AddModule(Type type);

        ICommandBuilder AddModules(Assembly assembly);
    }
}
