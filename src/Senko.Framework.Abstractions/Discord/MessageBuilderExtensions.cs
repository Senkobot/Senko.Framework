using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Framework.Discord;

namespace Senko.Framework
{
    public static class MessageBuilderExtensions
    {
        public static MessageBuilder AddMessage(this MessageResponse response, string content = null, ulong? channelId = null)
        {
            var builder = new MessageBuilder
            {
                ChannelId = channelId ?? response.Context.Request.ChannelId,
                Content = content
            };

            response.Messages.Add(builder);

            return builder;
        }

        public static MessageBuilder AddEmbed(this MessageResponse response, string title = null, string description = null, ulong? channelId = null)
        {
            var builder = new MessageBuilder
            {
                ChannelId = channelId ?? response.Context.Request.ChannelId,
                EmbedBuilder =
                {
                    Title = title,
                    Description = description
                }
            };

            response.Messages.Add(builder);

            return builder;
        }

        public static MessageBuilder ConfigureEmbed(this MessageBuilder builder, Action<EmbedBuilder> func)
        {
            if (builder.HasCallback)
            {
                builder.Then(e =>
                {
                    func(e.Builder.EmbedBuilder);
                    e.Builder.IsChanged = true;
                    return Task.CompletedTask;
                }, true);
            }
            else
            {
                func(builder.EmbedBuilder);
            }

            return builder;
        }

        public static MessageBuilder SetContent(this MessageBuilder builder, string content)
        {
            if (builder.HasCallback)
            {
                builder.Then(e =>
                {
                    e.Builder.Content = content;
                    e.Builder.IsChanged = true;
                    return Task.CompletedTask;
                }, true);
            }
            else
            {
                builder.Content = content;
            }

            return builder;
        }

        public static MessageBuilder SetTTS(this MessageBuilder builder, bool tts)
        {
            if (builder.HasCallback)
            {
                throw new InvalidOperationException("The TTS cannot be changed after the message has been sent.");
            }

            builder.IsTTS = tts;
            return builder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder AddEmbedField(this MessageBuilder builder, string title, object content, bool isInline = false)
        {
            return builder.ConfigureEmbed(embed => embed.AddField(title, content, isInline));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder SetEmbedAuthor(this MessageBuilder builder, string name, string iconUrl = null, string url = null)
        {
            return builder.ConfigureEmbed(embed => embed.SetAuthor(name, iconUrl, url));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder SetEmbedColor(this MessageBuilder builder, int r, int g, int b)
        {
            return builder.ConfigureEmbed(embed => embed.SetColor(new Color(r, g, b)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder SetEmbedColor(this MessageBuilder builder, float r, float g, float b)
        {
            return builder.ConfigureEmbed(embed => embed.SetColor(new Color(r, g, b)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder SetEmbedColor(this MessageBuilder builder, Color color)
        {
            return builder.ConfigureEmbed(embed => embed.SetColor(color));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder SetEmbedDescription(this MessageBuilder builder, string description)
        {
            return builder.ConfigureEmbed(embed => embed.SetDescription(description));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder SetEmbedFooter(this MessageBuilder builder, string text, string url = "")
        {
            return builder.ConfigureEmbed(embed => embed.SetFooter(text, url));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder SetEmbedTitle(this MessageBuilder builder, string title)
        {
            return builder.ConfigureEmbed(embed => embed.SetTitle(title));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder SetEmbedImage(this MessageBuilder builder, string url)
        {
            return builder.ConfigureEmbed(embed => embed.SetImage(url));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder SetEmbedThumbnail(this MessageBuilder builder, string url)
        {
            return builder.ConfigureEmbed(embed => embed.SetThumbnail(url));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder Then(this MessageBuilder builder, Action<ResponseMessageSuccessArguments> action)
        {
            return builder.Then(args =>
            {
                action(args);
                return Task.CompletedTask;
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder Delay(this MessageBuilder builder, int milliseconds)
        {
            return builder.Then(_ => Task.Delay(milliseconds));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder Delay(this MessageBuilder builder, TimeSpan ts)
        {
            return builder.Then(_ => Task.Delay(ts));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MessageBuilder Catch(this MessageBuilder builder, Action<ResponseMessageErrorArguments> action)
        {
            return builder.Catch(args =>
            {
                action(args);
                return Task.CompletedTask;
            });
        }
    }
}