public class MazeCellEventArgs
{
    public MazeExplorer Explorer { get; }

    public MazeLocation OldLocation { get; }

    public MazeLocation NewLocation { get; }

    public MazeCellEventArgs(MazeExplorer explorer, MazeLocation oldLocation, MazeLocation newLocation)
    {
        OldLocation = oldLocation;
        NewLocation = newLocation;
        Explorer = explorer;
    }
}
