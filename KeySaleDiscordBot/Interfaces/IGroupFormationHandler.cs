using System;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace KeySaleDiscordBot.Interfaces
{
    public interface IGroupFormationHandler
    {
        RestUserMessage MessageToTrack
        {
            get; set;
        }
        GroupFormationStatus Status
        {
            get; set;
        }
        GroupMembers GroupMembers
        {
            get; set;
        }
        Guid RunId
        {
            get;
        }
        ulong AdvertiserId
        {
            get;
        }
        string Details
        {
            get; set;
        }

        Task ReactionRemoved(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction);

        Task ReactionAdded(Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction);
    }

    public enum GroupFormationStatus
    {
        ReceivedSale=0,
        SalePosted=1,
        GroupFormed=2,
        Canceled=int.MaxValue,
    }

   
}