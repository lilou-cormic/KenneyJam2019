using UnityEngine;

public class NPCMazeExplorer : MazeExplorer
{
    public override UserType ExplorerType => UserType.NPC;

    private bool _MustMove;
    public bool MustMove
    {
        get { return _MustMove; }

        set
        {
            if (_MustMove != value)
            {
                _MustMove = value;

                if (_MustMove)
                    Explore();
            }
        }
    }

    protected override bool CanMove(Direction direction)
    {
        if (direction == Direction.Up && Location.Row == 1)
            return false;

        if (direction == Direction.Down && Location.Row == Maze.RowCount - 2)
            return false;

        return base.CanMove(direction);
    }

    private Direction _currentDirection = Direction.None;

    private float _moveCoolDown = 0.5f;
    private float _moveTimeLeft = 0f;

    public void SetRandomLocation()
    {
        var randomLocation = Maze.GetRandomLocation();

        Location.Row = randomLocation.Row;
        Location.Column = randomLocation.Column;

        _currentDirection = DirectionExtensions.GetRandomDirection();
    }

    private void Update()
    {
        if (!MustMove)
            return;

        if (_moveTimeLeft > 0)
            _moveTimeLeft -= Time.deltaTime;

        if (_moveTimeLeft <= 0)
            Explore();
    }

    public void Explore()
    {
        _moveTimeLeft = _moveCoolDown;

        Direction direction = _currentDirection;
        Direction direction2;

        if (CanMove(direction))
        {
            direction2 = DirectionExtensions.GetRandomDirection();

            if (CanMove(direction2) && !direction2.IsOpppositeDirection(direction))
                direction = direction2;
        }
        else
        {
            bool turnRight = Random.Range(0, 2) == 1;

            direction2 = (Direction)(((int)direction + (turnRight ? 1 : 3)) % 4);

            if (CanMove(direction2))
            {
                direction = direction2;
            }
            else
            {
                direction2 = (Direction)(((int)direction2 + 2) % 4);

                if (CanMove(direction2))
                    direction = direction2;
                else
                    direction = (Direction)(((int)direction + 2) % 4);
            }
        }

        if (CanMove(direction))
        {
            _currentDirection = direction;
            Move();
        }
    }

    private void Move()
    {
        Move(_currentDirection);
    }
}
