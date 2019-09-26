using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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
