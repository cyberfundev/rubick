using Extensions;
using Monetization;
using Profiles;
using Services;
using Zenject;

namespace Level
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<MonetizationConfig>()
                .AsCached()
                .NonLazy();

            Container.BindService<AdsService>();
            Container.BindService<AdsInitializer>();
            Container.BindService<FBInitializer>();
            Container.BindService<ProfileService>();
            
            Container.BindProfile<PlayerProfile>();
        }
    }
}