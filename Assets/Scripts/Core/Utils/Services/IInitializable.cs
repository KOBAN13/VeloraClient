namespace Core.Utils.Services
{
    public interface IInitializable
    {
        public bool IsInitialized { get; set; }

        void Initialize();
    }
}