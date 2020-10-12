namespace SharedModels
{
    public class MessageIdentifier
    {
        public string GuildId { get; set; }
        public string MessageId { get; set; }
        public string ChannelId { get; set; }

        public override bool Equals(object obj)
        {
            var otherMessage = ((MessageIdentifier)obj);
            return otherMessage.MessageId == this.MessageId 
                && otherMessage.ChannelId == this.ChannelId
                && otherMessage.GuildId == this.GuildId;
        }

        public override int GetHashCode()
        {
            return this.MessageId.GetHashCode() + this.GuildId.GetHashCode() + this.ChannelId.GetHashCode();
        }
    }
}
