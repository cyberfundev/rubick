using System;
using Profiles;
using Tutor;
using UniRx;
using UnityEngine;
using Zenject;

namespace Level
{
    public class LevelLooper : IInitializable
    {
        private readonly Cube _cube;
        private readonly LevelHud _levelHud;

        private bool _startedTimer = false;
        private bool _completedCube = false;
        private float _currentTime = 0;
        private int _maxSides = 0;

        private readonly ISubject<int> _sideCompletedListener = new Subject<int>();
        private readonly ISubject<Unit> _shuffleListener = new Subject<Unit>();
        private readonly PlayerProfile _playerProfile;
        private readonly ProfileService _profileService;
        private readonly InitialTutor _initialTutor;

        public IObservable<int> OnSideCompleted => _sideCompletedListener;
        public IObservable<Unit> OnShuffle => _shuffleListener;

        public LevelLooper(
            GameInput input,
            Cube cube,
            LevelHud levelHud,
            GameLooper gameLooper,
            PlayerProfile playerProfile,
            ProfileService profileService,
            InitialTutor initialTutor)
        {
            _initialTutor = initialTutor;
            _profileService = profileService;
            _playerProfile = playerProfile;
            _cube = cube;
            _levelHud = levelHud;

            input.OnSwiped
                .Where(_ => !_completedCube)
                .Subscribe(OnSwiped);
            input.OnRotate.Subscribe(OnRotate);

            levelHud.OnShuffleClicked.Subscribe(_ => Shuffle());

            cube.OnTurnFinished.Subscribe(_ => TurnFinished());

            Observable
                .EveryUpdate()
                .Where(_ => gameLooper.GameState == GameState.PlayLoop && _startedTimer)
                .Subscribe(_ => UpdateTimer());
        }

        public void Initialize()
        {
            _cube.StartCube();
            Shuffle();


            if (!_playerProfile.PassedTutor)
            {
                _initialTutor.StartTutor();
            }
        }

        private void OnSwiped(Direction direction)
        {
            CubeElement cubeElement = _cube.SelectedElement;
            if (cubeElement == null)
                return;

            var rotated = _cube.RotateAxis(direction);

            if (rotated && !_startedTimer)
            {
                StartTimer();
            }
        }

        private void StartTimer()
        {
            _startedTimer = true;
            _currentTime = 0;
            _levelHud.TimerUpdate((int) _currentTime);
        }

        private void UpdateTimer()
        {
            _currentTime += Time.deltaTime;

            _levelHud.TimerUpdate(_currentTime);
        }

        private void TurnFinished()
        {
            if (_currentTime <= 0)
            {
                return;
            }
            
            var sides = _cube.GetCompletedSidesCount();

            if (sides > _maxSides)
            {
                if (sides == 1 || sides == 6 || _maxSides == 0)
                {
                    _levelHud.AnimateSideCompleted();

                    CheckBestValue(sides != 6 && _maxSides == 0 ? 1 : sides);
                }

                _maxSides = sides;
                _sideCompletedListener.OnNext(_maxSides);
            }

            _levelHud.UpdateSidesCount(sides);

            if (sides == 6)
            {
                _completedCube = true;
                StopTimer();
                PlayWinEffect();
                _levelHud.StartShuffleBounce();
            }
        }

        private void PlayWinEffect()
        {
            _cube.PlayWinEffect();
        }

        private void CheckBestValue(int sides)
        {
            if (sides == 1)
            {
                if (_currentTime < _playerProfile.Best1.Value || _playerProfile.Best1.Value == 0)
                {
                    _playerProfile.Best1.Value = (int) _currentTime;
                    _profileService.SaveAll();
                }
            }

            if (sides == 6)
            {
                if (_currentTime < _playerProfile.BestFull.Value || _playerProfile.BestFull.Value == 0)
                {
                    _playerProfile.BestFull.Value = (int) _currentTime;
                    _profileService.SaveAll();
                }
            }
        }

        private void StopTimer()
        {
            _startedTimer = false;
            _currentTime = 0;
        }

        private async void OnRotate(Direction rotate)
        {
            await _cube.RotateCube(rotate);
        }

        private async void Shuffle()
        {
            if (_cube.Rotating)
            {
                return;
            }
            
            await _cube.Shuffle();
            _maxSides = 0;
            _completedCube = false;

            TurnFinished();
            StopTimer();

            _levelHud.StopShuffleBounce();
            _levelHud.TimerUpdate((int) _currentTime);
            _shuffleListener.OnNext(default);
        }
    }
}