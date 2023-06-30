using Zenject;

namespace Services
{
    public interface IService : IInitializable
    {
        bool Initialized { get; set; }
    }
}