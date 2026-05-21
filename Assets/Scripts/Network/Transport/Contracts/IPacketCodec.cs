using Packets;

namespace Network.Transport
{
    public interface IPacketCodec
    {
        byte[] Encode(Packet packet);
        bool TryDecode(byte[] data, out Packet packet, out string error);
    }
}
