using System;
using UnityEngine;

public abstract class MazeExplorer : MonoBehaviour
{
    public MazeGrid Maze { get; set; }

    public abstract UserType ExplorerType { get; }

    public string Name { get; set; }

    public MazeLocation Location { get; set; } = new MazeLocation(1, 0);
    
    public event Action<MazeExplorer, MazeLocation> Moved;

    protected virtual bool CanMove(Direction direction)
    {
        return Maze.CanMove(Location, direction);
    }

    public void Move(Direction direction)
    {
        MazeLocation oldLocation = Location.Clone();

        switch (direction)
        {
            case Direction.Right:
                Location.Column++;
                break;

            case Direction.Down:
                Location.Row++;
                break;

            case Direction.Left:
                Location.Column--;
                break;

            case Direction.Up:
                Location.Row--;
                break;
        }

        SetPosition();

        Moved?.Invoke(this, oldLocation);
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Location.Column, -Location.Row);
    }
}
