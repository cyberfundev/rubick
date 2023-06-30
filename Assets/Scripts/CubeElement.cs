using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class CubeElement : MonoBehaviour, IPointerDownHandler
{
    public Vector3Int Position;
    public Vector3Int StartPosition;
    public Transform Transform;
    [SerializeField] private List<ColorElement> _colorElements;
    [SerializeField] private List<ParticleSystem> _winParticles = new List<ParticleSystem>();

    private readonly ISubject<CubeElement> _pointerDown = new Subject<CubeElement>();
    private Vector3 _direction;
    private Vector3 _clickPos;
    private bool _hasDirection;
    private bool _initialized;

    private Dictionary<Direction, int> _colors = new();

    public IObservable<CubeElement> PointerDown => _pointerDown;

    [EasyButtons.Button]
    public void FillColorElements()
    {
        foreach (var spriteRenderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {

            int color = 0;

            if (spriteRenderer.name.Contains("Blue"))
                color = 1;
            if(spriteRenderer.name.Contains("White"))
                color = 2;
            if(spriteRenderer.name.Contains("Pink"))
                color = 3;
            if(spriteRenderer.name.Contains("Green"))
                color = 4;
            if(spriteRenderer.name.Contains("Yellow"))
                color = 5;
            
            _colorElements.Add(new ColorElement(spriteRenderer, color));

        }
    }
    
    [EasyButtons.Button]
    public void FillWinParticles()
    {
        foreach (var particle in transform.GetComponentsInChildren<ParticleSystem>())
        {
            _winParticles.Add(particle);
        }
    }

    public void StartCubeElement()
    {
        StartPosition = CubeHelper.ToVector3Int(transform.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDown.OnNext(this);
        _clickPos = eventData.pointerPressRaycast.worldPosition;
    }

    public (CubeElement, CubeElement) GetNeighbours(Vector3 direction)
    {
        _direction = direction;
        _hasDirection = true;
        var firstNeighbour = GetNeighbour(direction);

        if (firstNeighbour == null)
            firstNeighbour = GetNeighbour(direction * -1);

        var secondNeighbour = GetNeighbour(transform.position - _clickPos);

        return (firstNeighbour, secondNeighbour);
    }

    public void UpdatePosition(Vector3Int pos)
    {
        if (Position != pos || !_initialized)
        {
            _initialized = true;
            Position = pos;
            UpdateColors();
        }
    }

    public bool BelongsToDirection(Direction direction)
    {
        return GetDirections().Contains(direction);
    }

    public int GetColor(Direction direction)
    {
        return _colors[direction];
    }

    public void PlayWin()
    {
        foreach (var winParticle in _winParticles)
        {
            winParticle.Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        var position = transform.position;

        if(_hasDirection)
            Gizmos.DrawRay(position, _direction);
    }

    private CubeElement GetNeighbour(Vector3 direction1)
    {
        bool hitSuccessful = Physics.Raycast(transform.position, direction1, out var hit, 1.3f);

        if (hitSuccessful)
        {
            return hit.transform.GetComponent<CubeElement>();
        }

        return null;
    }

    private void UpdateColors()
    {
        _colors.Clear();

        var directions = GetDirections();

        foreach (var colorElement in _colorElements)
        {
            foreach (var direction in directions)
            {
                var dir = (colorElement.SpriteRenderer.transform.position - transform.position).normalized;

                dir = new Vector3((int)dir.x, (int)dir.y, (int)dir.z);

                if (direction.ToVector().Equals(dir))
                {
                    _colors.Add(direction, colorElement.Color);
                    break;
                }
            }
        }
    }

    private List<Direction> GetDirections()
    {
        var result = new List<Direction>();

        if (Position.x != 0)
        {
            result.Add(Position.x > 0 ? Direction.Right : Direction.Left);
        }
        if (Position.y != 0)
        {
            result.Add(Position.y > 0 ? Direction.Up : Direction.Down);
        }
        if (Position.z != 0)
        {
            result.Add(Position.z > 0 ? Direction.Forward : Direction.Back);
        }

        return result;
    }

    [Serializable]
    public struct ColorElement
    {
        public SpriteRenderer SpriteRenderer;
        public int Color;

        public ColorElement(SpriteRenderer spriteRenderer, int color)
        {
            SpriteRenderer = spriteRenderer;
            Color = color;
        }
    }
}