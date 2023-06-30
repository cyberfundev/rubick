using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Tutor
{
    public class TutorVisualization : MonoBehaviour
    {
        [SerializeField] private Image _hand;
        [Space] [SerializeField] private Sprite _commonHand;
        [SerializeField] private Sprite _clickedCommonHand;
        [Space] [SerializeField] private Sprite _doubleHand;
        [SerializeField] private Sprite _clickedDoubleHand;

        private Sequence _handSequence;

        private readonly Vector2 _startSwipeTutor = new Vector2(1f, -520f);
        private const int _swipeDist = 500;
        private const float SwipeTime = 1f;

        public void ShowSwipeTutor()
        {
            ShowSwipeTutor(_commonHand, _clickedCommonHand);
        }

        public void ShowRotateTutor()
        {
            ShowSwipeTutor(_doubleHand, _clickedDoubleHand);
        }

        public void HideTutor()
        {
            _hand.gameObject.SetActive(false);
            _handSequence.Kill();
        }

        private void ShowSwipeTutor(Sprite commonHand, Sprite clickedHand)
        {
            _hand.transform.localPosition = _startSwipeTutor;
            _hand.gameObject.SetActive(true);

            _handSequence = DOTween.Sequence();

            _handSequence.AppendCallback(() => { _hand.sprite = commonHand; });
            _handSequence.AppendInterval(0.5f);
            _handSequence.AppendCallback(() => { _hand.sprite = clickedHand; });
            _handSequence.AppendInterval(0.5f);
            _handSequence.Append(_hand.transform.DOLocalMoveY(_startSwipeTutor.y + _swipeDist, SwipeTime));
            _handSequence.AppendCallback(() => { _hand.sprite = commonHand; });
            _handSequence.AppendInterval(0.5f);

            _handSequence.SetLoops(-1, LoopType.Restart);
        }
    }
}