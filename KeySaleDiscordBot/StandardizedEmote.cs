using System.Text;
using Discord;

namespace KeySaleDiscordBot
{

    public static class BotEmotes
    {
        //https://unicode-table.com/en/search/?q=Regional+Indicator+Symbol+Letter
        public const string Tank = "🇹";
        public const string Healer = "🇭";
        public const string Dps = "🇩";
        public const string Key = "🇰";
        public const string Cancel = "🇨";
        public static IEmote TankEmote = new Emoji(BotEmotes.Tank);
        public static IEmote HealerEmote = new Emoji(BotEmotes.Healer);
        public static IEmote DpsEmote = new Emoji(BotEmotes.Dps);
        //public static IEmote WaitEmote= new StandardizedEmote(":regional_indicator_w:");
        public static IEmote KeyEmote = new Emoji(BotEmotes.Key);
        public static IEmote CancelEmote = new Emoji(BotEmotes.Cancel);

        //public static IEmote GoEmote= new StandardizedEmote(":regional_indicator_g:");
        //
        public static bool IsTank(this string emote) => emote == Tank;
        public static bool IsHealer(this string emote) => emote == Healer;
        public static bool IsDps(this string emote) => emote == Dps;
        public static bool IsKey(this string emote) => emote == Key;

    }


}