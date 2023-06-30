using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Level;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class Cube : MonoBehaviour
{
    [SerializeField] private Transform _rotateParent;
    [SerializeField] private Transform _cubeParent;
    [SerializeField] private Transform _parent;
    [SerializeField] private List<CubeElement> _cubes;
    [SerializeField] private Transform _cubesParent;

    private float _rotateDuration = 0.2f;
    private CubeElement _selectedElement;

    private bool _rotating = false;

    private readonly ISubject<Unit> _endRotateListener = new Subject<Unit>();
    private readonly ISubject<Unit> _startTurn = new Subject<Unit>();
    private readonly ISubject<Unit> _startRotate = new Subject<Unit>();
    private Vector3 _currentCubeRotation;

    public IObservable<Unit> OnTurnFinished => _endRotateListener;
    public IObservable<Unit> OnTurnStarted => _startTurn;
    public IObservable<Unit> OnRotateStarted => _startRotate;

    public CubeElement SelectedElement => _selectedElement;
    public bool Rotating => _rotating;

    [Inject]
    private void Construct(GameInput gameInput)
    {
        gameInput.OnStoppedDrag.Subscribe(_ => OnStoppedDrag());
    }

    public void StartCube()
    {
        for (var index = 0; index < _cubes.Count; index++)
        {
            CubeElement cubeElement = _cubes[index];
            cubeElement.PointerDown.Subscribe(OnSelectedElement);
            cubeElement.StartCubeElement();
            cubeElement.Transform.name = "cube " + index;
        }
    }

    public bool RotateAxis(Direction direction)
    {
        if (_rotating)
            return false;

        _rotating = true;
        
        _startTurn.OnNext(default);

        Axis axis = CubeHelper.GetAxis(direction.ToVector(), _selectedElement);

        if (axis == Axis.None)
        {
            OnEndRotate();
            return false;
        }

        int dirMultiplier = direction == Direction.Up || direction == Direction.Right ? 1 : -1;

        PrepareCube(axis);

        Debug.Log(axis);
        _parent
            .DOLocalRotate(CubeHelper.GetRotateVector(axis) * 90 * dirMultiplier, _rotateDuration)
            .OnComplete(OnEndRotate);
        return true;
    }

    public async UniTask Shuffle()
    {
        int loops = Random.Range(10, 20);
        
        _cubeParent.localRotation = Quaternion.identity;

        ResetCube();
        ResetElements();

        for (int i = 0; i < loops; i++)
        {
            Axis axis = (Axis) Random.Range(1, 10);
            int multiplier = Random.Range(-1, 1);
            multiplier = multiplier == 0 ? 1 : multiplier;

            PrepareCube(axis);
            _parent.localRotation = Quaternion.Euler(CubeHelper.GetRotateVector(axis) * 90 * multiplier);
            await UniTask.Yield();
            ResetElements();
        }
    }

    private void ResetCube()
    {
        foreach (CubeElement cubeElement in _cubes)
        {
            cubeElement.transform.position = cubeElement.StartPosition;
            cubeElement.transform.localRotation = Quaternion.identity;
        }
    }

    public async UniTask RotateCube(Direction direction)
    {
        if(_rotating)
            return;
        
        _rotating = true;
        _startRotate.OnNext(default);
        
        _rotateParent.eulerAngles = Vector3.zero;
        _cubeParent.SetParent(_rotateParent);
        int dirMultiplier = direction == Direction.Up || direction == Direction.Right ? 1 : -1;

        await _rotateParent
            .DOLocalRotate(90 * dirMultiplier * (!direction.Horizontal() ? Vector3.right : Vector3.up), _rotateDuration)
            .AsyncWaitForCompletion();

        _cubeParent.SetParent(transform);

        ResetElements();
        
        _rotating = false;
    }

    public int GetCompletedSidesCount()
    {
        int completedSides = 0;
        foreach (var dirValue in Enum.GetNames(typeof(Direction)))
        {
            Direction direction = (Direction) Enum.Parse(typeof(Direction), dirValue);
            if(direction == Direction.None)
                continue;

            int sideColor = -1;
            bool unCompletedSide = false;
            foreach (CubeElement cubeElement in _cubes)
            {
                if(!cubeElement.BelongsToDirection(direction))
                    continue;
                
                if (sideColor < 0)
                {
                    sideColor = cubeElement.GetColor(direction);
                }
                else
                {
                    if (sideColor != cubeElement.GetColor(direction))
                    {
                        unCompletedSide = true;
                        break;
                    }
                }
            }

            if (unCompletedSide)
            {
                continue;
            }
            completedSides++;
        }

        return completedSides;
    }

    public void PlayWinEffect()
    {
        foreach (var cubeElement in _cubes)
        {
            cubeElement.PlayWin();
        }
    }

    private void OnSelectedElement(CubeElement cubeElement)
    {
        _selectedElement = cubeElement;
    }

    private void OnStoppedDrag()
    {
        _selectedElement = null;
    }

    private void PrepareCube(Axis axis)
    {
        foreach (var cube in _cubes.FindAll(x => CubeHelper.IsInAxis(x.Position, axis)))
        {
            cube.Transform.SetParent(_parent);
        }
    }

    private void OnEndRotate()
    {
        ResetElements();

        _rotating = false;

        _endRotateListener.OnNext(default);
    }

    private void ResetElements()
    {
        foreach (var cube in _cubes)
        {
            cube.Transform.SetParent(_cubesParent);
            cube.UpdatePosition(CubeHelper.ToVector3Int(cube.Transform.position));
        }
        
        _parent.localRotation = Quaternion.identity;
    }
}

public enum Direction
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4,
    Forward = 5,
    Back = 6,
}