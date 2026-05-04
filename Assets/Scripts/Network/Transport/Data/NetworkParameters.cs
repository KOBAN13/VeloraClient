using Sirenix.OdinInspector;
using UnityEngine;

namespace Network.Transport.Data
{
    [CreateAssetMenu(fileName = "NetworkParameters", menuName = "Db/NetworkParameters")]
    public class NetworkParameters : SerializedScriptableObject, INetworkParameters
    {
        [field: SerializeField] public ETransportType TransportType { get; private set; }
        [field: SerializeField] public string WebsocketUrl { get; private set; }
        [field: SerializeField] public string TcpHost { get; private set; }
        [field: SerializeField] public int TcpPort { get; private set; }
    }
}
