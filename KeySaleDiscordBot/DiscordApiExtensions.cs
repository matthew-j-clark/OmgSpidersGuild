using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace KeySaleDiscordBot
{
    public static class DiscordApiExtensions
    {
        public static async Task AddStandardReactions(this RestUserMessage message)
        {
            await message.AddReactionAsync(BotEmotes.TankEmote);
           await message.AddReactionAsync(BotEmotes.HealerEmote);
           await message.AddReactionAsync(BotEmotes.DpsEmote);
           await message.AddReactionAsync(BotEmotes.KeyEmote);
           await message.AddReactionAsync(BotEmotes.CancelEmote);
            //await message.AddReactionsAsync(new[])
            //{
            //    BotEmotes.TankEmote,
            //    BotEmotes.HealerEmote,
            //    BotEmotes.DpsEmote,
            //    BotEmotes.KeyEmote
            //});
        }
    }

}