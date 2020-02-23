using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (GameObject.FindGameObjectsWithTag("LevelManager").Length > 1)
        {
            Destroy(gameObject);
        }
        main = this;
    }

    public static LevelManager main;

    private FullscreenFade fullscreenFade;
    private GameMenu gameMenu;

    public MapGenerator MapGenerator;
    private GeneralLevelConfig generalLevelConfig;
    [SerializeField]
    private int levelNumber = 1;

    public int LevelNumber { get { return levelNumber; } }

    private bool dead = false;
    private bool paused = false;
    private bool loading = false;
    private bool inLevelScreen = false;
    MapPopulator mapPopulator;
    MagicWand wandPrefab;

    GameObject HUD;

    Player player;

    void Start()
    {
        FullscreenFade fsPrefab = Resources.Load<FullscreenFade>("FullscreenFade");
        fullscreenFade = Instantiate(fsPrefab, transform);
        fullscreenFade.Initialize();
        MapGenerator mapGeneratorPrefab = Resources.Load<MapGenerator>("MapGenerator");
        MapGenerator = Instantiate(mapGeneratorPrefab);
        generalLevelConfig = Resources.Load<GeneralLevelConfig>("LevelConfigs/GeneralLevelConfig");
        HUD = GameObject.FindGameObjectWithTag("Respawn");
        HUD.SetActive(false);
        LoadBaseLevel();
        GetMenu().ShowStart("\"Zarguuf! Come down into the cellar. We have prepared a challenge for you!\"");
        //LoadNextLevel();
    }

    public void StartGameForReal() {
        fullscreenFade.FadeOut(ReadyWithBase);
    }

    public void LoadBaseLevel()
    {
        loading = true;
        MazeRoom mainRoom = new MazeRoom(new Rect(5, 5, 11, 11));
        LevelDepthConfig depthConfig = generalLevelConfig.GetLevelDepthConfiguration(-1);
        LevelConfig levelConfig = Resources.Load<LevelConfig>("LevelConfigs/base");
        levelConfig.Randomize();

        MapGenerator.Initialize(levelConfig, depthConfig, ReadyWithBase);
        //MazeCarver carver = mapGenerator.InitializeCarver(worldRect, mapGenerator, levelConfig, depthConfig);
        MazeCarver carver = MapGenerator.InitializeCarver();
        MapGenerator.PlaceRoom(mainRoom);
        carver.FillWithHallway();
        //carver.RemoveFalseWalls();
        carver.Create3DWalls();
        carver.CreateNavMeshes();
        mapPopulator = MapGenerator.InitializeMapPopulator();
        //mapPopulator.SpawnKeyAt(mapGenerator.GetScaled(new Vector3(10, 10, 0)));

        SpawnWandAt(new Vector3(9, 9, 0), depthConfig.PowerLevel);
        SpawnWandAt(new Vector3(7, 9, 0), depthConfig.PowerLevel);
        SpawnWandAt(new Vector3(9, 7, 0), depthConfig.PowerLevel);
        Dummy dummyPrefab = Resources.Load<Dummy>("Dummy");

        Dummy dummy = Instantiate(dummyPrefab, MapGenerator.transform);
        dummy.transform.position = MapGenerator.GetScaled(new Vector3(13, 13, 0));

        mapPopulator.SpawnEndAt(MapGenerator.GetScaled(new Vector3(10, 10, 0)));
        player = mapPopulator.SpawnPlayerAt(MapGenerator.GetScaled(new Vector3(7, 7, 0)));
        mapPopulator.SetUpCamera(player);
        //fullscreenFade.FadeOut(ReadyWithBase);
    }

    private void SpawnWandAt(Vector3 position, float powerLevel)
    {
        if (wandPrefab == null)
        {
            wandPrefab = Resources.Load<MagicWand>("MagicWand");
        }
        MagicWand newWand = Instantiate(wandPrefab, MapGenerator.transform);
        newWand.SetOptions(MagicWand.GetOptions(powerLevel));
        newWand.transform.position = MapGenerator.GetScaled(position);
    }

    public void SpawnKeyAt(Vector3 position)
    {
        mapPopulator.SpawnKeyAt(position);
    }

    public void ReadyWithBase()
    {
        loading = false;
        HUD.SetActive(true);
        //player.LoadCursor();
        GetMenu().Hide();
    }

    public void LoadNextLevel()
    {
        loading = true;
        Camera.main.GetComponent<FollowerCamera>().StopFollowing();
        fullscreenFade.FadeIn(LoadLevel);
    }

    public void LoadLevel()
    {
        LevelConfig config = GetLevelConfig();
        if (config == null)
        {
            GetMenu().ShowEnd(
                "The end.\nZarguuf defeated all his apprentices and continued on with his Saturday morning."
            );
            return;
        }
        LevelDepthConfig depthConfig = generalLevelConfig.GetLevelDepthConfiguration(levelNumber - 1);
        config.Randomize();
        LootManager.main.SetConfig(depthConfig);
        MapGenerator.Initialize(config, depthConfig, AfterLevelIsLoaded);
        MapGenerator.CreateWorld();
        levelNumber += 1;
    }

    public void PlayerDie()
    {
        FadeIn(ShowDeathMenu);
        dead = true;
    }

    private GameMenu GetMenu()
    {
        if (gameMenu == null)
        {
            GameMenu gameMenuPrefab = Resources.Load<GameMenu>("GameMenu");
            gameMenu = Instantiate(gameMenuPrefab);
        }
        return gameMenu;
    }

    public void ShowDeathMenu()
    {
        Time.timeScale = 0f;
        GameMenu menu = GetMenu();
        Debug.Log("Died!");
        menu.ShowDeath(
            string.Format(
                "Zarguuf was defeated {0} floors deep into his cellar.",
                LevelManager.main.LevelNumber
            )
        );
    }

    public void Pause()
    {
        if (!loading && !paused && !dead && !inLevelScreen)
        {
            paused = true;
            FadeIn(ShowPauseMenu);
        }
        else if (paused)
        {
            UnPause();
        }
    }
    public void ShowPauseMenu()
    {
        GameMenu menu = GetMenu();
        menu.ShowPause("Paused.");
    }
    public void UnPause()
    {
        if (paused)
        {
            FadeOut(HidePauseMenu);
        }
    }

    public void HidePauseMenu()
    {
        paused = false;
        GameMenu menu = GetMenu();
        menu.Hide();
    }

    public void AfterLevelIsLoaded()
    {
        fullscreenFade.FadeOut(Unfreeze);
    }

    public void FadeOut(FadeComplete callback)
    {
        fullscreenFade.FadeOut(callback);
    }

    public void FadeIn(FadeComplete callback)
    {
        fullscreenFade.FadeIn(callback);
    }
    public void Unfreeze()
    {
        Debug.Log("All ready!");
        loading = false;
    }

    private LevelConfig GetLevelConfig()
    {
        List<LevelConfig> configs = new List<LevelConfig>(
            Resources.LoadAll<LevelConfig>(string.Format("LevelConfigs/{0}", levelNumber))
        );
        if (configs.Count == 0)
        {
            return null;
        }
        return configs[Random.Range(0, configs.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
}
