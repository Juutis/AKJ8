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

    private MapGenerator mapGenerator;
    private GeneralLevelConfig generalLevelConfig;
    [SerializeField]
    private int levelNumber = 1;

    public int LevelNumber { get { return levelNumber; } }
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

    }
}
