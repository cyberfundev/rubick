using Monetization;
using Tutor;
using UnityEngine;
using Zenject;

namespace Level
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private Cube _cube;
        [SerializeField] private LevelHud _hud;
        [SerializeField] private TutorVisualization _tutorVisualization;
        
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<LevelLooper>()
                .AsCached();
            Container
                .BindInterfacesAndSelfTo<MonetizationObserver>()
                .AsCached()
                .NonLazy();

            Container
                .Bind<GameLooper>()
                .AsCached();
            Container
                .Bind<GameInput>()
                .AsCached();
            Container
                .Bind<InitialTutor>()
                .AsCached();


            Container
                .BindInstance(_cube);
            Container
                .BindInstance(_hud);
            Container
                .BindInstance(_tutorVisualization);
        }
    }
}