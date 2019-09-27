using System;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Discord
{
    public class DiscordSelfUser : DiscordUser, IDiscordSelfUser
    {
        public ValueTask<IDiscordChannel> GetDMChannelsAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask ModifyAsync(Action<UserModifyArgs> modifyArgs)
        {
            var args = new UserModifyArgs();
            modifyArgs(args);
            Username = args.Username;

            if (args.Avatar != null)
            {
                throw new NotImplementedException();
            }

            return default;
        }
    }
}
