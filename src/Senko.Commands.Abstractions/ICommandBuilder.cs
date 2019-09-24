using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Senko.Commands
{
    public interface ICommandBuilder
    {
        ICommandBuilder AddModule(Type type);

        ICommandBuilder AddModules(Assembly assembly);
    }
}
