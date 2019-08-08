using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using Senko.Framework.Discord;
using Senko.Framework.Features;

namespace Senko.Framework
{
    internal class DefaultMessageResponse : MessageResponse
    {
        private static readonly Func<IFeatureCollection, IMessageResponseFeature> EmptyResponseFeature = f => new MessageResponseFeature();

        private FeatureReferences<FeatureInterfaces> _features;
        private MessageContext _context;

        public virtual void Initialize(MessageContext context)
        {
            _context = context;
            _features = new FeatureReferences<FeatureInterfaces>(context.Features);
        }

        public virtual void Uninitialize()
        {
            _context = null;
            _features = default;
        }
        
        private IMessageResponseFeature HttpResponseFeature => _features.Fetch(ref _features.Cache.Response, EmptyResponseFeature);

        public override MessageContext Context => _context;

        public override IList<MessageBuilder> Messages
        {
            get => HttpResponseFeature.Messages;
            set => HttpResponseFeature.Messages = value;
        }

        private struct FeatureInterfaces
        {
            public IMessageResponseFeature Response;
        }
    }
}