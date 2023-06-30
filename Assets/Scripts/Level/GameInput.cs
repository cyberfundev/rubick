using System;
using UniRx;
using UnityEngine;

namespace Level
{
    public class GameInput
    {
        private const float MINSwipeDistance = 50f;

        private Vector2 _dragPosStart;
        private bool _drag;
        private bool _doubleDrag;
        private readonly ISubject<Direction> _swipeListener = new Subject<Direction>();
        private readonly ISubject<Direction> _rotateDragListener = new Subject<Direction>();
        private readonly ISubject<Unit> _stoppedDragListener = new Subject<Unit>();

        public IObservable<Direction> OnSwiped => _swipeListener;
        public IObservable<Direction> OnRotate => _rotateDragListener;
        public IObservable<Unit> OnStoppedDrag => _stoppedDragListener;

        public GameInput(GameLooper gameLooper)
        {
            Observable
                .EveryUpdate()
                .Where(_ => gameLooper.GameState == GameState.PlayLoop)
                .Subscribe(_ => CheckInput());
        }

        private void CheckInput()
        {
            bool dragRotate = Input.GetMouseButton(1) || Input.touchCount > 1 || Input.GetKey(KeyCode.LeftAlt);

            if (dragRotate)
            {
                if (!_doubleDrag)
                {
                    StopDrag();
                    _doubleDrag = true;
                    _dragPosStart = Input.mousePosition;
                }
            }
            else
            {
                if (_doubleDrag)
                {
                    StopRotate();
                }
            }

            if (Input.GetMouseButtonDown(0) && !dragRotate)
            {
                if (!_drag)
                {
                    _drag = true;
                    _dragPosStart = Input.mousePosition;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (_doubleDrag)
                {
                    if (Vector2.Distance(Input.mousePosition, _dragPosStart) > MINSwipeDistance)
                    {
                        _rotateDragListener.OnNext(CheckDirection());
                        StopDrag();
                    }
                }

                if (_drag)
                {
                    if (Vector2.Distance(Input.mousePosition, _dragPosStart) > MINSwipeDistance)
                    {
                        _swipeListener.OnNext(CheckDirection());
                        StopDrag();
                    }
                }
            }
            else
            {
                StopDrag();
                StopRotate();
                _stoppedDragListener.OnNext(default);
            }

#if UNITY_EDITOR

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            if (horizontal != 0)
            {
                _rotateDragListener.OnNext(horizontal < 0 ? Direction.Left : Direction.Right);
                StopDrag();
            }
            if (vertical != 0)
            {
                _rotateDragListener.OnNext(vertical < 0 ? Direction.Down : Direction.Up);
                StopDrag();
            }
#endif
        }

        private Direction CheckDirection()
        {
            float xDist = Input.mousePosition.x - _dragPosStart.x;
            float yDist = Input.mousePosition.y - _dragPosStart.y;

            if (Mathf.Abs(xDist) > Mathf.Abs(yDist))
            {
                if (xDist > 0)
                    return Direction.Left;
                return Direction.Right;
            }

            if (yDist < 0)
                return Direction.Down;
            return Direction.Up;
        }

        private void StopDrag()
        {
            _drag = false;
            _dragPosStart = Vector2.zero;
        }
        private void StopRotate()
        {
            _doubleDrag = false;
            _dragPosStart = Vector2.zero;
        }
    }
}