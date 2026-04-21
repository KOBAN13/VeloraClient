using Packets;

namespace Extensions
{
    public static class PacketExtension
    {
        public static string DescribePacket(this Packet packet)
        {
            return packet.MsgCase switch
            {
                Packet.MsgOneofCase.Chat => $"chat=\"{packet.Chat?.Msg}\"",
                Packet.MsgOneofCase.Id => $"id={packet.Id?.Id}",
                _ => "empty"
            };
        }
    }
}