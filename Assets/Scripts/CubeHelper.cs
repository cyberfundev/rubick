using System;
using UnityEngine;

public static class CubeHelper
{
    public static Vector3Int ToVector3Int(Vector3 transformPosition)
    {
        return new(Mathf.RoundToInt(transformPosition.x), Mathf.RoundToInt(transformPosition.y), Mathf.RoundToInt(transformPosition.z));
    }

    public static bool IsInAxis(Vector3Int pos, Axis axis)
    {
        switch (axis)
        {
            case Axis.HorizontalDown:
                return pos.y == -1;
            case Axis.HorizontalCenter:
                return pos.y == 0;
            case Axis.HorizontalUp:
                return pos.y == 1;
            case Axis.VerticalLeft:
                return pos.x == -1;
            case Axis.VerticalCenter:
                return pos.x == 0;
            case Axis.VerticalRight:
                return pos.x == 1;
            case Axis.ForwardBack:
                return pos.z == -1;
            case Axis.ForwardCenter:
                return pos.z == 0;
            default:
                return pos.z == 1;
        }
    }

    public static Vector3 GetRotateVector(Axis axis)
    {
        switch (axis)
        {
            case Axis.HorizontalDown:
            case Axis.HorizontalCenter:
            case Axis.HorizontalUp:
                return Vector3.up;
            case Axis.ForwardBack:
            case Axis.ForwardCenter:
            case Axis.ForwardForward:
                return Vector3.forward;
            default:
                return Vector3.right;
        }
    }

    public static Axis GetAxis(Vector3 direction, CubeElement selectedElement)
    {
        (CubeElement element1, CubeElement additional) cubeElements = selectedElement.GetNeighbours(direction);
        if (cubeElements.element1 != null || cubeElements.additional != null)
        {
            foreach (var axisName in Enum.GetNames(typeof(Axis)))
            {
                Axis axis = (Axis) Enum.Parse(typeof(Axis), axisName);
                if(axis == Axis.None)
                    continue;

                try
                {
                    if (IsInAxis(cubeElements.element1.Position, axis) && IsInAxis(selectedElement.Position, axis) && (cubeElements.additional == null || IsInAxis(cubeElements.additional.Position, axis)))
                        return axis;
                }
                catch (Exception e)
                {
                    return Axis.None;
                }
            }
        }

        return Axis.None;
    }
}