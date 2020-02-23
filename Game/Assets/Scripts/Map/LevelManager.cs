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

    private MapGenerator mapGenerator;
    private GeneralLevelConfig generalLevelConfig;
    [SerializeField]
    private int levelNumber = 1;

    public int LevelNumber { get { return levelNumber; } }

    private bool dead = false;
    private bool paused = false;
    private bool loading = false;
    private bool inLevelScreen = false;

    void Start()
    {
        FullscreenFade fsPrefab = Resources.Load<FullscreenFade>("FullscreenFade");
        fullscreenFade = Instantiate(fsPrefab, transform);
        fullscreenFade.Initialize();
        MapGenerator mapGeneratorPrefab = Resources.Load<MapGenerator>("MapGenerator");
        mapGenerator = Instantiate(mapGeneratorPrefab);
        generalLevelConfig = Resources.Load<GeneralLevelConfig>("LevelConfigs/GeneralLevelConfig");
        LoadNextLevel();
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
            Debug.Log("The End!");
            return;
        }
        LevelDepthConfig depthConfig = generalLevelConfig.GetLevelDepthConfiguration(levelNumber - 1);
        config.Randomize();
        LootManager.main.SetConfig(depthConfig);
        mapGenerator.Initialize(config, depthConfig, AfterLevelIsLoaded);
        levelNumber += 1;
    }

    public void PlayerDie() {
        FadeIn(ShowDeathMenu);
        dead = true;
    }

    private GameMenu GetMenu() {
        if (gameMenu == null) {
            GameMenu gameMenuPrefab = Resources.Load<GameMenu>("GameMenu");
            gameMenu = Instantiate(gameMenuPrefab);
        }
        return gameMenu;
    }

    public void ShowDeathMenu() {
        Time.timeScale = 0f;
        GameMenu menu = GetMenu();
        Debug.Log("Died!");
        menu.ShowDeath("You have died. Do you wish to restart?");
    }

    public void Pause() {
        if (!loading && !paused && !dead && !inLevelScreen) {
            paused = true;
            FadeIn(ShowPauseMenu);
        } else if (paused) {
            UnPause();
        }
    }
    public void ShowPauseMenu () {
        GameMenu menu = GetMenu();
        menu.ShowPause("Paused.");
    }
    public void UnPause () {
        if (paused) {
            FadeOut(HidePauseMenu);
        }
    }

    public void HidePauseMenu () {
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
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Pause();
        }
    }
}
