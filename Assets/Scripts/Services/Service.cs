namespace Services
{
    public abstract class Service : IService
    {
        public bool Initialized { get; set; }
        public abstract void Initialize();
    }
}
