using UnityEngine;

public enum Direction
{
    Right = 0,

    Down = 1,

    Left = 2,

    Up = 3,

    None = 5,
};

public static class DirectionExtensions
{
    public static bool IsOpppositeDirection(this Direction direction, Direction other)
    {
        return ((int)direction + (int)other) % 2 == 0;
    }

    public static Direction GetRandomDirection()
    {
        return (Direction)Random.Range(0, 4);
    }
}
