﻿using System;
using System.Collections.Generic;
using System.Text;
using Senko.Framework;

namespace Senko.Commands
{
    [Configuration("Modules")]
    public class ModuleOptions
    {
        /// <summary>
        ///     The compiler to use.
        /// </summary>
        public string Compiler { get; set; } = "Reflection";
    }
}
