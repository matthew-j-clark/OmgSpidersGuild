using System.Collections.Generic;

namespace SpiderDiscordBot.ImageCommands
{
    public static class GenericImageList
    {
        public static List<IBotCommand> CommandList => new List<IBotCommand>()
        {
            new GenericImageCommand("!hr", "I NEED HR","https://tenor.com/view/karen-karening-intensifies-done-iam-done-gif-16742218"),
            new GenericImageCommand("!karen", "KAAAAAARRENNNN","https://tenor.com/view/karen-karening-intensifies-done-iam-done-gif-16742218,https://tenor.com/view/snl-hell-naw-no-black-panther-karen-gif-11636970"),
            new GenericImageCommand("!ravioli", "raviolis?","https://tenor.com/view/trailer-gif-7304634")
        };

    }
}
