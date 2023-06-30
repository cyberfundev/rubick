using Facebook.Unity;

namespace Services
{
    public class FBInitializer : Service
    {
        public override void Initialize()
        {
            FB.Init();
        }
    }
}