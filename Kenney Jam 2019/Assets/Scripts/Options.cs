public class Options
{
    public int Width { get; set; }

    public int Height { get; set; }

    public int InterceptorCount { get; set; }

    public Options()
    {
        Height = 4;
        Width = 4;
        InterceptorCount = 1;
    }
}