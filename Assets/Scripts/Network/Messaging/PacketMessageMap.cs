using System;
using Packets;

namespace Network.Messaging
{
    public static class PacketMessageMap
    {
        public static bool TryExtract(Packet packet, out Type type, out object payload)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (TryExtract(packet.Chat, out type, out payload) ||
                TryExtract(packet.Id, out type, out payload) ||
                TryExtract(packet.LoginRequest, out type, out payload) ||
                TryExtract(packet.RegisterRequest, out type, out payload) ||
                TryExtract(packet.OkResponse, out type, out payload) ||
                TryExtract(packet.DenyResponse, out type, out payload) ||
                TryExtract(packet.CreateRoomRequest, out type, out payload) ||
                TryExtract(packet.JoinRoomRequest, out type, out payload) ||
                TryExtract(packet.LeaveRoomRequest, out type, out payload) ||
                TryExtract(packet.ReadyRequest, out type, out payload) ||
                TryExtract(packet.RoomStateSnapshot, out type, out payload) ||
                TryExtract(packet.MatchStarted, out type, out payload) ||
                TryExtract(packet.StartGame, out type, out payload) ||
                TryExtract(packet.RoomListRequestMessage, out type, out payload) ||
                TryExtract(packet.RoomSummaryMessage, out type, out payload) ||
                TryExtract(packet.RoomListSnapshot, out type, out payload) ||
                TryExtract(packet.JoinRoomResponseMessage, out type, out payload) ||
                TryExtract(packet.PlayersInRoomRequest, out type, out payload) ||
                TryExtract(packet.PlayersInRoomResponse, out type, out payload) ||
                TryExtract(packet.PlayerKickRoom, out type, out payload) ||
                TryExtract(packet.PlayerRemoveRoom, out type, out payload))
            {
                return true;
            }

            type = null;
            payload = null;
            return false;
        }

        public static Packet CreatePacket<T>(T message) where T : class
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return message switch
            {
                ChatMessage chat => new Packet { Chat = chat },
                LoginRequestMessage loginRequest => new Packet { LoginRequest = loginRequest },
                RegisterRequestMessage registerRequest => new Packet { RegisterRequest = registerRequest },
                RoomListRequestMessage roomListRequest => new Packet { RoomListRequestMessage = roomListRequest },
                CreateRoomRequestMessage createRoomRequest => new Packet { CreateRoomRequest = createRoomRequest },
                JoinRoomRequestMessage joinRoomRequest => new Packet { JoinRoomRequest = joinRoomRequest },
                LeaveRoomRequestMessage leaveRoomRequest => new Packet { LeaveRoomRequest = leaveRoomRequest },
                ReadyRequestMessage readyRequest => new Packet { ReadyRequest = readyRequest },
                StartGameRequestMessage startGameRequest => new Packet { StartGame = startGameRequest },
                PlayerKickRoom playerKickRoom => new Packet { PlayerKickRoom = playerKickRoom },
                PlayersInRoomRequest playersInRoomRequest => new Packet { PlayersInRoomRequest = playersInRoomRequest },
                _ => throw new NotSupportedException($"Message type {typeof(T).Name} is not supported for sending.")
            };
        }

        private static bool TryExtract<T>(T message, out Type type, out object payload) where T : class
        {
            if (message == null)
            {
                type = null;
                payload = null;
                return false;
            }

            type = typeof(T);
            payload = message;
            return true;
        }
    }
}
