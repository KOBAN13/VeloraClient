using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Utils.Logger.Data
{
    [CreateAssetMenu(fileName = "LoggerParameters", menuName = "Db/LoggerParameters")]
    public class LoggerParameters : SerializedScriptableObject, ILoggerParameters
    {
        [field: SerializeField] public Color InfoColor { get; private set; }
        [field: SerializeField] public Color WarningColor { get; private set; }
        [field: SerializeField] public Color ErrorColor { get; private set; }
    }

    public sealed class DefaultLoggerParameters : ILoggerParameters
    {
        public Color InfoColor => Color.white;
        public Color WarningColor => Color.yellow;
        public Color ErrorColor => Color.red;
    }
}
