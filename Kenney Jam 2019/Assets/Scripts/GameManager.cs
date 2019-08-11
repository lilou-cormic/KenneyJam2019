using System;
using System.Xml.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private HumanMazeExplorer PlayerPrefab = null;

    [SerializeField]
    private NPCMazeExplorer EnemyPrefab = null;

    [SerializeField]
    private GameObject WallPrefab = null;

    [SerializeField]
    private CombinableItem ItemPrefab = null;

    [SerializeField]
    private AudioClip EndReachedSound = null;

    [SerializeField]
    private AudioClip GameOverSound = null;

    private MazeGrid _Maze;
    public MazeGrid Maze
    {
        get { return _Maze; }

        private set
        {
            if (_Maze != null)
            {
                _Maze.CaseCouranteChanged -= Maze_CaseCouranteChanged;
                _Maze.EndReached -= Maze_EndReached;
                _Maze.GameOver -= Maze_GameOver;

                _Maze.StopMovement();
            }

            _Maze = value;

            if (_Maze != null)
            {
                _Maze.CaseCouranteChanged += Maze_CaseCouranteChanged;
                _Maze.EndReached += Maze_EndReached;
                _Maze.GameOver += Maze_GameOver;
            }
        }
    }

    public Options Options { get; } = new Options();

    public event Action<MazeCellEventArgs> CaseCouranteChanged;

    private void Start()
    {
        NewMaze();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
    }

    private void NewGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    private void NewMaze()
    {
        LoadInventory();

        Maze = MazeGrid.CreateMaze(Options);

        Maze.Player = Instantiate(PlayerPrefab, transform);
        Maze.Player.Maze = Maze;
        Maze.Player.Name = "Player";
        Maze.Player.Location.Column = Maze.GetStartColumn();
        Maze.Player.SetPosition();

        int interceptorCount = Options.InterceptorCount + ((ScoreManager.ScoreMultiplier -1) / 10);

        for (int i = 0; i < interceptorCount; i++)
        {
            NPCMazeExplorer interceptor = Instantiate(EnemyPrefab, transform);
            interceptor.Maze = Maze;
            interceptor.Name = $"Interceptor{i + 1}";
            interceptor.SetRandomLocation();
            interceptor.SetPosition();

            Maze.AddInterceptor(interceptor);
        }

        for (int i = 0; i < UnityEngine.Random.Range(0, 4); i++)
        {
            var itemLocation = Maze.GetRandomLocation();

            CombinableItem item = Instantiate(ItemPrefab, new Vector3(itemLocation.Column, -itemLocation.Row), Quaternion.identity, transform);
            item.ItemDef = Inventory.GetRandomItemDef();
        }

        RenderWalls();

        Maze.StartMovement();
    }

    private void RenderWalls()
    {
        for (int column = 0; column < Maze.ColumnCount; column++)
        {
            for (int row = 0; row < Maze.RowCount; row++)
            {
                if (Maze.GetCell(column, row)?.IsWalkable == false)
                    InstanciateWall(column, row);
            }
        }
    }

    private void InstanciateWall(int column, int row)
    {
        Instantiate(WallPrefab, new Vector3(column, -row), Quaternion.identity, transform);
    }

    private void OnCaseCouranteChanged(MazeCellEventArgs e)
    {
        CaseCouranteChanged?.Invoke(e);
    }

    private void Maze_CaseCouranteChanged(MazeCellEventArgs e)
    {
        OnCaseCouranteChanged(e);
    }

    private void Maze_EndReached()
    {
        ScoreManager.IncrementMultiplier();

        SoundPlayer.Play(EndReachedSound);

        NewGame();
    }

    private void Maze_GameOver()
    {
        ScoreManager.SetHighScore();

        SoundPlayer.Play(GameOverSound);

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }

    public static void SaveInventory()
    {
        //PlayerPrefs.SetString("Inventory", Inventory.ToXElement("Inventory").ToString());
    }

    private static void LoadInventory()
    {
        /*
        string inventoryString = PlayerPrefs.GetString("Inventory");
        if (string.IsNullOrWhiteSpace(inventoryString))
            return;

        Inventory.FromXElement(XElement.Parse(inventoryString));
        */
    }

    public static void QuitGame()
    {
        ScoreManager.SetHighScore();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

public enum CellType
{
    Unknown = 0,

    LeftBorder = 1,

    TopBorder = 2,

    RightBorder = 3,

    BottomBorder = 4,

    CornerBorder = 5,

    HorizontalWall = 6,

    VerticalWall = 7,

    Intersection = 8,

    Space = 9,
}

public enum UserType
{
    Human = 0,

    NPC = 1,
}
