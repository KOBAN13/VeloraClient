namespace Core.Utils.Logger
{
    public interface ILoggerService
    {
        void Log(string message, string source = "System");
        void Warning(string message, string source = "System");
        void Error(string message, string source = "System");
    }
}
