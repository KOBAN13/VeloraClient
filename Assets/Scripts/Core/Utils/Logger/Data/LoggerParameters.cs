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
}