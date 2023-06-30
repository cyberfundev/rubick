using System;
using Cysharp.Threading.Tasks;
using Profiles;
using UniRx;

namespace Tutor
{
    public class InitialTutor
    {
        private enum State
        {
            waitingSwipe,
            waitingTurn,
            turned
        }

        private Cube _cube;
        private readonly TutorVisualization _visualization;
        private State _state;
        private PlayerProfile _playerProfile;
        private ProfileService _profileService;


        public InitialTutor(Cube cube, TutorVisualization visualization, PlayerProfile playerProfile, ProfileService profileService)
        {
            _profileService = profileService;
            _playerProfile = playerProfile;
            _visualization = visualization;
            _cube = cube;
            _cube.OnTurnStarted.Subscribe(_ => Swiped());
            _cube.OnRotateStarted.Subscribe(_ => Rotate());
        }

        public async void StartTutor()
        {
            _state = State.waitingSwipe;
            
            _visualization.ShowSwipeTutor();

            await UniTask.WaitUntil(() => _state == State.waitingTurn);

            _visualization.HideTutor();

            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            _visualization.ShowRotateTutor();

            await UniTask.WaitUntil(() => _state == State.turned);

            _visualization.HideTutor();
            
            _playerProfile.PassedTutor = true;
            _profileService.SaveAll();
        }

        private void Rotate()
        {
            _state = State.turned;
        }

        private void Swiped()
        {
            _state = State.waitingTurn;
        }
    }
}