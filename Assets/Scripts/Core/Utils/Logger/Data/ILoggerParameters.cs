using UnityEngine;

namespace Core.Utils.Logger.Data
{
    public interface ILoggerParameters
    {
        Color InfoColor { get; }
        Color WarningColor { get; }
        Color ErrorColor { get; }
    }
}