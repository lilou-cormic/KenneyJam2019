using System.Collections.Generic;

public class MazeCell
{
    public CellType CellType { get; }

    public bool IsWalkable { get { return CellType == CellType.Space; } }

    public List<MazeExplorer> Explorers { get; } = new List<MazeExplorer>();

    public MazeCell(CellType cellType)
    {
        CellType = cellType;
    }
}