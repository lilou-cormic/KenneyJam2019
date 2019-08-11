using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MazeGrid
{
    private static bool _isLeft = false;

    private MazeCell[,] MazeCells { get; }

    public int RowCount { get; private set; }

    public int ColumnCount { get; private set; }

    private HumanMazeExplorer _Player;
    public HumanMazeExplorer Player
    {
        get { return _Player; }

        set
        {
            if (_Player != null)
                _Player.Moved -= Player_Moved;

            _Player = value;

            if (_Player != null)
                _Player.Moved += Player_Moved;
        }
    }

    /// <summary>Les intercepteurs</summary>
    public List<NPCMazeExplorer> Interceptors { get; } = new List<NPCMazeExplorer>();

#if DEBUG
    /// <summary>Le labyrithe en string (pour debuggage)</summary>
    public string MazeString { get; private set; }
#endif

    public event Action<MazeCellEventArgs> CaseCouranteChanged;

    public event Action EndReached;

    public event Action GameOver;

    private MazeGrid(Options options, bool[,] verticalWalls, bool[,] horizontalWalls)
    {
        int width = options.Width;
        int height = options.Height;

        RowCount = (height * 2) + 1;
        ColumnCount = (width * 2) + 1;

        MazeCells = new MazeCell[ColumnCount, RowCount];

        AddBorders();
        AddIntersections();
        AddVerticalWalls(verticalWalls);
        AddHorizontalWalls(horizontalWalls);
        AddSpaces();

#if UNITY_EDITOR
        #region MazeString
        StringBuilder mazeString = new StringBuilder();

        mazeString.Append("█ ");
        for (int column = 0; column < width * 2 - 1; column++)
        {
            mazeString.Append("█");
        }
        mazeString.AppendLine();

        for (int row = 0; row < height; row++)
        {
            mazeString.Append("█ ");
            for (int column = 0; column < width - 1; column++)
            {
                mazeString.Append((verticalWalls[column, row] ? "█" : " ") + " ");
            }
            mazeString.AppendLine("█");

            if (row != height - 1)
            {
                mazeString.Append("█");
                for (int column = 0; column < width; column++)
                {
                    mazeString.Append((horizontalWalls[column, row] ? "█" : " ") + "█");
                }
                mazeString.AppendLine();
            }
        }

        mazeString.Append("█");
        for (int column = 1; column < width * 2 - 1; column++)
            mazeString.Append("█");
        mazeString.Append(" █");

        Debug.Log(mazeString.ToString());
        #endregion
#endif
    }

    public static MazeGrid CreateMaze(Options options)
    {
        int width = options.Width;
        int height = options.Height;

        bool[,] verticalWalls = CreateVerticalWalls(width, height);
        bool[,] horizontalWalls = CreateHorizontalWalls(width, height);
        int[] sets = CreateSets(width, height);

        Direction direction = 0;
        int cellIndex = 0;

        while (!AreAllSameSet(sets))
        {
            while (true)
            {
                cellIndex = UnityEngine.Random.Range(0, sets.Length);
                direction = DirectionExtensions.GetRandomDirection();

                if (!IsValidDirection(direction, cellIndex, width, height))
                    continue;

                GetSets(direction, sets, cellIndex, width, out int classe1, out int classe2);

                if (classe1 == classe2)
                    continue;

                RemoveWall(direction, verticalWalls, horizontalWalls, cellIndex, width);

                UnionSets(sets, classe1, classe2);

                break;
            }
        }

        return new MazeGrid(options, verticalWalls, horizontalWalls);
    }

    public void AddInterceptor(NPCMazeExplorer interceptor)
    {
        interceptor.Moved += Interceptor_Moved;

        Interceptors.Add(interceptor);
    }

    private void AddBorders()
    {
        _isLeft = !_isLeft;

        int minRow = 0;
        int maxRow = RowCount - 1;
        int minColumn = 0;
        int maxColumn = ColumnCount - 1;

        for (int row = minRow; row <= maxRow; row++)
        {
            MazeCells[minColumn, row] = new MazeCell(CellType.LeftBorder);
            MazeCells[maxColumn, row] = new MazeCell(CellType.RightBorder);
        }

        for (int column = minColumn; column <= maxColumn; column++)
        {
            MazeCells[column, minRow] = new MazeCell((column != (_isLeft ? minColumn + 1 : maxColumn - 1) ? CellType.TopBorder : CellType.Space));
            MazeCells[column, maxRow] = new MazeCell((column != (_isLeft ? maxColumn - 1 : minColumn + 1) ? CellType.BottomBorder : CellType.Space));
        }

        MazeCells[minColumn, minRow] = new MazeCell(CellType.CornerBorder);
        MazeCells[minColumn, maxRow] = new MazeCell(CellType.CornerBorder);
        MazeCells[maxColumn, minRow] = new MazeCell(CellType.CornerBorder);
        MazeCells[maxColumn, maxRow] = new MazeCell(CellType.CornerBorder);
    }

    private void AddIntersections()
    {
        for (int column = 2; column < ColumnCount; column += 2)
        {
            for (int row = 2; row < RowCount; row += 2)
            {
                MazeCells[column, row] = new MazeCell(CellType.Intersection);
            }
        }
    }

    private void AddVerticalWalls(bool[,] verticalWalls)
    {
        for (int wallColumn = 0, column = 2; wallColumn < verticalWalls.GetLength(0); wallColumn++, column += 2)
        {
            for (int wallRow = 0, row = 1; wallRow < verticalWalls.GetLength(1); wallRow++, row += 2)
            {
                MazeCells[column, row] = new MazeCell((verticalWalls[wallColumn, wallRow] ? CellType.VerticalWall : CellType.Space));
            }
        }
    }

    private void AddHorizontalWalls(bool[,] horizontalWalls)
    {
        for (int wallColumn = 0, column = 1; wallColumn < horizontalWalls.GetLength(0); wallColumn++, column += 2)
        {
            for (int wallRow = 0, row = 2; wallRow < horizontalWalls.GetLength(1); wallRow++, row += 2)
            {
                MazeCells[column, row] = new MazeCell((horizontalWalls[wallColumn, wallRow] ? CellType.HorizontalWall : CellType.Space));
            }
        }
    }

    private void AddSpaces()
    {
        for (int column = 1; column < ColumnCount; column += 2)
        {
            for (int row = 1; row < RowCount; row += 2)
            {
                MazeCells[column, row] = new MazeCell(CellType.Space);
            }
        }
    }

    public MazeCell GetCell(int column, int row)
    {
        return MazeCells[column, row];
    }

    public int GetStartColumn()
    {
        return _isLeft ? 1 : ColumnCount - 2;
    }

    public bool CanMove(MazeLocation currentLocation, Direction direction)
    {
        switch (direction)
        {
            case Direction.Right:
                return currentLocation.Column < ColumnCount - 1
                    && GetCell(currentLocation.Column + 1, currentLocation.Row).IsWalkable;

            case Direction.Down:
                return currentLocation.Row < RowCount - 1
                    && GetCell(currentLocation.Column, currentLocation.Row + 1).IsWalkable;

            case Direction.Left:
                return currentLocation.Column > 0
                    && GetCell(currentLocation.Column - 1, currentLocation.Row).IsWalkable;

            case Direction.Up:
                return currentLocation.Row > 0
                    && GetCell(currentLocation.Column, currentLocation.Row - 1).IsWalkable;

            case Direction.None:
            default:
                return false;
        }
    }

    public void StartMovement()
    {
        foreach (NPCMazeExplorer interceptor in Interceptors)
        {
            interceptor.MustMove = true;
        }
    }

    public void StopMovement()
    {
        foreach (NPCMazeExplorer interceptor in Interceptors)
        {
            interceptor.MustMove = false;
        }
    }

    protected virtual void OnCaseCouranteChanged(MazeCellEventArgs e)
    {
        CaseCouranteChanged?.Invoke(e);
    }

    protected virtual void OnEndReached()
    {
        StopMovement();

        EndReached?.Invoke();
    }

    protected virtual void OnGameOver()
    {
        StopMovement();

        GameOver?.Invoke();
    }

    private static bool[,] CreateVerticalWalls(int width, int height)
    {
        bool[,] outVerticalWalls = new bool[width - 1, height];

        for (int column = 0; column < outVerticalWalls.GetLength(0); column++)
        {
            for (int row = 0; row < outVerticalWalls.GetLength(1); row++)
            {
                outVerticalWalls[column, row] = true;
            }
        }

        return outVerticalWalls;
    }


    private static bool[,] CreateHorizontalWalls(int width, int height)
    {
        bool[,] outHorizontalWalls = new bool[width, height - 1];

        for (int column = 0; column < outHorizontalWalls.GetLength(0); column++)
        {
            for (int row = 0; row < outHorizontalWalls.GetLength(1); row++)
            {
                outHorizontalWalls[column, row] = true;
            }
        }

        return outHorizontalWalls;
    }

    private static int[] CreateSets(int width, int height)
    {
        int[] outSets = new int[width * height];

        for (int index = 0; index < outSets.Length; index++)
        {
            outSets[index] = -1;
        }

        return outSets;
    }

    private static bool IsValidDirection(Direction direction, int cellIndex, int width, int height)
    {
        switch (direction)
        {
            case Direction.Right:
                return (cellIndex + 1) % width > 0;

            case Direction.Down:
                return cellIndex + width <= (width * height) - 1;

            case Direction.Left:
                return cellIndex % width > 0;

            case Direction.Up:
                return cellIndex - width >= 0;
        }

        return false;
    }

    private static bool AreAllSameSet(int[] sets)
    {
        for (int index = 1; index < sets.Length; index++)
        {
            if (FindSet(sets, index) != FindSet(sets, index - 1))
                return false;
        }

        return true;
    }

    private static void GetSets(Direction direction, int[] sets, int cellIndex, int width, out int set1, out int set2)
    {
        set1 = FindSet(sets, cellIndex);

        switch (direction)
        {
            case Direction.Right:
                set2 = FindSet(sets, cellIndex + 1);
                break;

            case Direction.Down:
                set2 = FindSet(sets, cellIndex + width);
                break;

            case Direction.Left:
                set2 = FindSet(sets, cellIndex - 1);
                break;

            case Direction.Up:
                set2 = FindSet(sets, cellIndex - width);
                break;

            default:
                set2 = FindSet(sets, cellIndex);
                break;
        }
    }

    private static void RemoveWall(Direction direction, bool[,] verticalWalls, bool[,] horizontalWalls, int cellIndex, int width)
    {
        int row = cellIndex / width;
        int column = cellIndex % width;

        switch (direction)
        {
            case Direction.Right:
                verticalWalls[column, row] = false;
                break;

            case Direction.Down:
                horizontalWalls[column, row] = false;
                break;

            case Direction.Left:
                verticalWalls[column - 1, row] = false;
                break;

            case Direction.Up:
                horizontalWalls[column, row - 1] = false;
                break;
        }
    }

    private static int FindSet(int[] sets, int cellIndex)
    {
        if (sets[cellIndex] < 0)
            return cellIndex;
        else
            return FindSet(sets, sets[cellIndex]);
    }

    private static void UnionSets(int[] sets, int set1, int set2)
    {
        if (sets[set1] > sets[set2])
        {
            sets[set2] += sets[set1];
            sets[set1] = set2;
        }
        else
        {
            sets[set1] += sets[set2];
            sets[set2] = set1;
        }
    }

    private void Player_Moved(MazeExplorer interceptor, MazeLocation location)
    {
        if (GetCell(Player.Location.Column, Player.Location.Row).Explorers.Any(x => x.ExplorerType == UserType.NPC))
        {
            OnGameOver();
            return;
        }

        OnCaseCouranteChanged(new MazeCellEventArgs(Player, location, Player.Location));

        if (Player.Location.Row == RowCount - 1 && Player.Location.Column == (_isLeft ? ColumnCount - 2 : 1))
        {
            OnEndReached();
            return;
        }
    }

    private void Interceptor_Moved(MazeExplorer interceptor, MazeLocation location)
    {
        if (interceptor.Location == Player.Location)
        {
            OnGameOver();
            return;
        }

        GetCell(location.Column, location.Row).Explorers.Remove(interceptor);
        GetCell(interceptor.Location.Column, interceptor.Location.Row).Explorers.Add(interceptor);

        OnCaseCouranteChanged(new MazeCellEventArgs(interceptor, location, interceptor.Location));
    }
}
