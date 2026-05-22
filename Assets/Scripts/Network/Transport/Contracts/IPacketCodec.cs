using Packets;

namespace Network.Transport.Contracts
{
    public interface IPacketCodec
    {
        byte[] Encode(Packet packet);
        bool TryDecode(byte[] data, out Packet packet, out string error);
    }
}
