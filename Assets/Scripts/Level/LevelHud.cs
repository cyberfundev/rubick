using System;
using DG.Tweening;
using Profiles;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Level
{
    public class LevelHud : MonoBehaviour
    {
        [SerializeField] private Button _shuffleButton;
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private TextMeshProUGUI _bestAllSidesTime;
        [SerializeField] private TextMeshProUGUI _best1SideTime;
        [SerializeField] private TextMeshProUGUI _sidesCompleted;

        private readonly ISubject<Unit> _shuffleListener = new Subject<Unit>();

        private bool _inAnimation;
        private PlayerProfile _playerProfile;
        private Sequence _sequence;

        public IObservable<Unit> OnShuffleClicked => _shuffleListener;

        [Inject]
        private void Construct(PlayerProfile playerProfile)
        {
            _playerProfile = playerProfile;
        }

        private void Start()
        {
            _shuffleButton.onClick
                .AsObservable()
                .Subscribe(_ => Shuffle());

            SubscribeBestTexts();
            SetBestsTexts();
        }

        private void SubscribeBestTexts()
        {
            _playerProfile.Best1.Subscribe(_ => SetBestsTexts());
            _playerProfile.BestFull.Subscribe(_ => SetBestsTexts());
        }

        private void SetBestsTexts()
        {
            _best1SideTime.text = string.Empty;
            _bestAllSidesTime.text = string.Empty;

            if (_playerProfile.Best1.Value > 0)
            {
                FormatTime(_best1SideTime, TimeSpan.FromSeconds(_playerProfile.Best1.Value));
                _best1SideTime.text = BestTemplate(1, _best1SideTime.text);
            }

            if (_playerProfile.BestFull.Value > 0)
            {
                FormatTime(_bestAllSidesTime, TimeSpan.FromSeconds(_playerProfile.BestFull.Value));
                _bestAllSidesTime.text = BestTemplate(6, _bestAllSidesTime.text);
            }
        }

        public void UpdateSidesCount(int sides)
        {
            _sidesCompleted.text = $"{sides}/6";
        }

        public void TimerUpdate(float seconds)
        {
            if (_inAnimation)
                return;
            var time = TimeSpan.FromMilliseconds(seconds * 1000);

            FormatTime(_timer, time);
        }

        private void FormatTime(TextMeshProUGUI textMeshProUGUI, TimeSpan time)
        {
            textMeshProUGUI.text = String.Empty;

            if (time.Hours > 0)
            {
                textMeshProUGUI.text += $"{time.TotalHours:D2}:";
            }

            if (time.Minutes > 0 || time.Hours > 0)
            {
                textMeshProUGUI.text += $"{time.Minutes:D2}:";
            }

            textMeshProUGUI.text += $"{time.Seconds:D2}:";

            if (time.Milliseconds > 9)
            {
                var milliseconds = Int32.Parse(time.Milliseconds.ToString().Substring(0, 2));
                textMeshProUGUI.text += $"{milliseconds:D2}";
            }
            else
            {
                textMeshProUGUI.text += $"{time.Milliseconds:D2}";
            }
        }

        public async void AnimateSideCompleted()
        {
            _inAnimation = true;

            await _timer.DOColor(Color.green, 0.3f).SetLoops(3, LoopType.Restart).AsyncWaitForCompletion();
            await _timer.DOColor(Color.white, 0.3f).AsyncWaitForCompletion();

            _inAnimation = false;
        }

        private string BestTemplate(int amount, string text)
        {
            return $"Best {amount}: {text}";
        }

        private void Shuffle()
        {
            _shuffleListener.OnNext(default);
        }

        [EasyButtons.Button]
        public void StartShuffleBounce()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(_shuffleButton.transform.DOScale(1.2f, 0.2f));
            _sequence.Append(_shuffleButton.transform.DOScale(1.1f, 0.1f));
            _sequence.Append(_shuffleButton.transform.DOScale(1.25f, 0.1f));
            _sequence.Append(_shuffleButton.transform.DOScale(1f, 0.2f));
            _sequence.AppendInterval(1.5f);
            _sequence.SetLoops(-1, LoopType.Restart);
        }

        [EasyButtons.Button]
        public void StopShuffleBounce()
        {
            _sequence?.Kill();
        }
    }
}