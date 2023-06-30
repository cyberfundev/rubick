using Profiles;
using Services;
using Zenject;

namespace Extensions
{
    public static class ContainerExtensions
    {
        public static void BindProfile<T>(this DiContainer container) where T : Profile
        {
            container
                .Bind<T>()
                .FromInstance(ProfileService.LoadService<T>())
                .AsCached();
        }
        public static void BindService<T>(this DiContainer container) where T : IService
        {
            container
                .BindInterfacesAndSelfTo<T>()
                .AsCached()
                .NonLazy();
        }
    }
}
