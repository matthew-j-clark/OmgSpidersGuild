﻿using Discord.WebSocket;

using SpidersGoogleSheetsIntegration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmgSpiders.DiscordBot.SignupCommands
{
    public class RevokeSignupCommand : IBotCommand
    {
        public string StartsWithKey => "!revokesignup";
        public string Description => "Revokes your signup for a specific run for a specific character.\n" +
            "ex: !revokesignup heroic thwackdaddy";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var arguments = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (arguments.Length != 3)
            {
                await message.Channel.SendMessageAsync("Invalid usage of !revokesignup. Use like this: " + this.Description);
                return;
            }

            var run = arguments[1];
            var character = arguments[2];
            if (!SignupCommand.RunTypes.Contains(run, StringComparer.OrdinalIgnoreCase))
            {
                await message.Channel.SendMessageAsync("Invalid usage of !signup. Invalid 'runType/id' value.");
                return;
            }

            var sheetsClient = new SheetsClient();
            await sheetsClient.Initialize();

            try
            {
                await sheetsClient.RevokeSignupAsync(character, message.Author.Username);
                await message.Channel.SendMessageAsync($"Successfully Revoked: {character} for {message.Author.Mention} for {run}!");
            }
            catch (Exception ex)
            {
                await message.Channel.SendMessageAsync($"Revoke attempt resulted in error, @sealslicer please check into it.");
                throw;
            }

        }
    }
}
