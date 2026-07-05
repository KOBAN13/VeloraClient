using System;
using System.Linq;
using Network.Contracts;
using Network.Data;
using Network.Transport.Data;
using R3;

namespace Network.Services.Session
{
    public class SessionManager : ISessionManager
    {
        private readonly PlayerData _user;
        private readonly RoomSummaryData _currentRoom;
        
        private readonly ReactiveProperty<ERoomRole> _myRole = new();
        
        public Observable<ERoomRole> MyRole => _myRole;

        public SessionManager(PlayerData user, RoomSummaryData currentRoom)
        {
            _user = user;
            _currentRoom = currentRoom;
        }

        public T RoomVariables<T>(string keyVariable) where T : new()
        {
            //work in progreess variables
            //var variables = _currentRoom.GetVariable(keyVariable);
           // return (T)variables.Value;

           return new T();
        }
        
        public T UserVariables<T>(string keyVariable) where T : new()
        {
            //work in progreess variables
            //var variables = _user.GetVariable(keyVariable);
           // return (T)variables.Value;
           
           return new T();
        }
        
        public void SetRole(ERoomRole role)
        {
            _myRole.Value = role;
        }
        
        public ERoomRole GetRole()
        {
            return _myRole.Value;
        }
        
        public ulong FindUserIdByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));

            var user = _currentRoom.Players.FirstOrDefault(playerData => playerData.Username == name);
            
            return user.UserId;
        }
    }
}