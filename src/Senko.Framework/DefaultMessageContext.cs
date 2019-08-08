using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using Senko.Discord;
using Senko.Framework.Features;

namespace Senko.Framework
{
    public class DefaultMessageContext : MessageContext
    {
        private static readonly Func<IFeatureCollection, IUserFeature> _newUserFeature = f => new UserFeature();
        private static readonly Func<IFeatureCollection, ISelfFeature> _newSelfFeature = f => new SelfFeature();
        private static readonly Func<IFeatureCollection, IItemsFeature> _newItemsFeature = f => new ItemsFeature();
        private static readonly Func<IFeatureCollection, IServiceProvidersFeature> _newServiceProvidersFeature = f => new ServiceProvidersFeature();
        
        private FeatureReferences<FeatureInterfaces> _features;
        private readonly DefaultMessageRequest _request;
        private readonly DefaultMessageResponse _response;

        public DefaultMessageContext()
        {
            _request = new DefaultMessageRequest();
            _response = new DefaultMessageResponse();
            Initialize(new FeatureCollection());
        }

        public virtual void Initialize(IFeatureCollection features)
        {
            _features = new FeatureReferences<FeatureInterfaces>(features);
            _request.Initialize(this);
            _response.Initialize(this);
        }

        public virtual void Uninitialize()
        {
            _features = default;
            _request.Uninitialize();
            _response.Uninitialize();
        }

        private IServiceProvidersFeature ServiceProvidersFeature => _features.Fetch(ref _features.Cache.ServiceProviders, _newServiceProvidersFeature);
        
        private IUserFeature UserFeature => _features.Fetch(ref _features.Cache.User, _newUserFeature);
        
        private ISelfFeature SelfFeature => _features.Fetch(ref _features.Cache.Self, _newSelfFeature);

        private IItemsFeature ItemsFeature => _features.Fetch(ref _features.Cache.Items, _newItemsFeature);

        public override IServiceProvider RequestServices
        {
            get => ServiceProvidersFeature.RequestServices;
            set => ServiceProvidersFeature.RequestServices = value;
        }

        public override IDiscordUser User
        {
            get => UserFeature.User;
            set => UserFeature.User = value;
        }

        public override IDiscordUser Self
        {
            get => SelfFeature.User;
            set => SelfFeature.User = value;
        }

        public override IFeatureCollection Features => _features.Collection;

        public override MessageRequest Request => _request;

        public override MessageResponse Response => _response;

        public override IDictionary<object, object> Items
        {
            get => ItemsFeature.Items;
            set => ItemsFeature.Items = value;
        }

        private struct FeatureInterfaces
        {
            public IServiceProvidersFeature ServiceProviders;
            public IItemsFeature Items;
            public IUserFeature User;
            public ISelfFeature Self;
        }
    }
}
