using System;
using Google.Protobuf;
using Packets;

namespace Network.Transport
{
    public class ProtobufPacketCodec : IPacketCodec
    {
        public byte[] Encode(Packet packet)
        {
            return packet.ToByteArray();
        }

        public bool TryDecode(byte[] data, out Packet packet, out string error)
        {
            packet = null;
            error = null;

            try
            {
                packet = Packet.Parser.ParseFrom(data);
                return true;
            }
            catch (InvalidProtocolBufferException e)
            {
                error = $"Packet protobuf parse error. Bytes={data.Length}. Exception: {e.Message}";
            }
            catch (Exception e)
            {
                error = $"Packet decode error. Bytes={data.Length}. Exception: {e.Message}";
            }

            return false;
        }
    }
}
