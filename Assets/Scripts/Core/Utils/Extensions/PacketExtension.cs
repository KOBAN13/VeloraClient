using Packets;

namespace Core.Utils.Extensions
{
    public static class PacketExtension
    {
        public static string DescribePacket(this Packet packet)
        {
            if (packet.Chat != null)
            {
                return $"chat=\"{packet.Chat.Msg}\"";
            }

            if (packet.Id != null)
            {
                return $"id={packet.Id.Id}";
            }

            return "empty";
        }
    }
}
