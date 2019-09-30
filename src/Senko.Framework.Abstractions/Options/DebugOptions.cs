using System;
using System.Collections.Generic;
using System.Text;

namespace Senko.Framework.Options
{
    public class DebugOptions
    {
        /// <summary>
        ///     True if the pipeline should be rethrow the exception.
        ///     This is mainly used in unit tests.
        /// </summary>
        public bool ThrowOnMessageException { get; set; }
    }
}
