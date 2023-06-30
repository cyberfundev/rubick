using JetBrains.Annotations;
using UniRx;

namespace Profiles
{
    [UsedImplicitly]
    public class PlayerProfile : Profile
    {
        public ReactiveProperty<float> Best1 { get; set; } = new(0);
        public ReactiveProperty<float> BestFull { get; set; } = new(0);
        public bool PassedTutor { get; set; }
    }
}