using System;
using Microsoft.AspNetCore.Http.Features;
using Senko.Framework.Features;

namespace Senko.Framework
{
    internal class DefaultMessageRequest : MessageRequest
    {
        private static readonly Func<IFeatureCollection, IMessageRequestFeature> EmptyRequestFeature = f => new MessageRequestFeature();
        private MessageContext _context;
        private FeatureReferences<FeatureInterfaces> _features;

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

        private IMessageRequestFeature HttpRequestFeature => _features.Fetch(ref _features.Cache.Request, EmptyRequestFeature);

        public override MessageContext Context => _context;

        public override string Message
        {
            get => HttpRequestFeature.Message;
            set => HttpRequestFeature.Message = value;
        }

        public override ulong MessageId
        {
            get => HttpRequestFeature.MessageId;
            set => HttpRequestFeature.MessageId = value;
        }

        public override ulong ChannelId
        {
            get => HttpRequestFeature.ChannelId;
            set => HttpRequestFeature.ChannelId = value;
        }

        public override ulong? GuildId 
        {
            get => HttpRequestFeature.GuildId;
            set => HttpRequestFeature.GuildId = value;
        }

        private struct FeatureInterfaces
        {
            public IMessageRequestFeature Request;
        }
    }
}