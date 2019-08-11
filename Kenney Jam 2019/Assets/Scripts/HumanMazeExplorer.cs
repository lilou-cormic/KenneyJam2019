using UnityEngine;

public class HumanMazeExplorer : MazeExplorer
{
    public override UserType ExplorerType => UserType.Human;

    private float _moveCoolDown = 0.12f;
    private float _moveTimeLeft = 0f;

    private void Update()
    {
        if (_moveTimeLeft > 0)
            _moveTimeLeft -= Time.deltaTime;

        if (_moveTimeLeft <= 0)
            TryMove(GetDirectionFromInput());
    }

    private Direction GetDirectionFromInput()
    {
        switch (Input.GetAxisRaw("Horizontal"))
        {
            case 1:
                if (CanMove(Direction.Right))
                    return Direction.Right;
                break;

            case -1:
                if (CanMove(Direction.Left))
                    return Direction.Left;
                break;
        }

        switch (Input.GetAxisRaw("Vertical"))
        {
            case 1:
                return Direction.Up;

            case -1:
                return Direction.Down;
        }

        return Direction.None;
    }

    private void TryMove(Direction direction)
    {
        if (direction == Direction.None)
            return;

        _moveTimeLeft = _moveCoolDown;

        if (CanMove(direction))
            Move(direction);
    }
}
