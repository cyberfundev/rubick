using UnityEngine;

public enum Axis
{
    None = 0,
    
    HorizontalDown = 1,
    HorizontalCenter = 2,
    HorizontalUp = 3,

    VerticalLeft = 4,
    VerticalCenter = 5,
    VerticalRight = 6,

    ForwardBack = 7,
    ForwardCenter = 8,
    ForwardForward = 9,
}

public static class DirectionsExtensions
{
    public static Vector3 ToVector(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector3.up;
            case Direction.Down:
                return Vector3.down;
            case Direction.Left:
                return Vector3.left;
            case Direction.Right:
                return Vector3.right;
            case Direction.Forward:
                return Vector3.forward;
            case Direction.Back:
                return Vector3.back;
            default:
                return Vector3.zero;
        }
    }

    public static Direction ToDirection(this Vector3 vector)
    {
        for (int i = 1; i < Direction.Back.GetHashCode() + 1; i++)
        {
            if (vector.Equals(((Direction) i).ToVector()))
                return (Direction) i;
        }

        return Direction.None;
    }

    public static Direction GetOpposite(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Forward:
                return Direction.Back;
            default:
                return Direction.Forward;
        }
    } 
    public static bool Horizontal(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
            case Direction.Right:
                return true;
            default:
                return false;
        }
    }
    
}