using System;
using System.Collections.Generic;
using System.Text;
using Senko.Discord;
using Senko.Framework;

namespace Senko.Commands
{
    public class ModuleBase
    {
        private ModuleContext _moduleContext;

        /// <summary>
        /// Gets or sets the <see cref="ModuleContext"/>.
        /// </summary>
        [ModuleContext]
        public ModuleContext ModuleContext
        {
            get => _moduleContext ??= new ModuleContext();
            set => _moduleContext = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the <see cref="MessageContext"/> for the executing action.
        /// </summary>
        public MessageContext Context => ModuleContext.Context;

        /// <summary>
        /// Gets the <see cref="MessageRequest"/> for the executing action.
        /// </summary>
        public MessageRequest Request => Context?.Request;

        /// <summary>
        /// Gets the <see cref="MessageResponse"/> for the executing action.
        /// </summary>
        public MessageResponse Response => Context?.Response;
    }
}
