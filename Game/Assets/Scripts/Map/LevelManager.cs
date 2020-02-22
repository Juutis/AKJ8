using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    void Awake() {
        DontDestroyOnLoad(gameObject);
        if (GameObject.FindGameObjectsWithTag("LevelManager").Length > 1) {
            Destroy(gameObject);
        }
        main = this;
    }

    public static LevelManager main;

    private MapGenerator mapGeneratorPrefab;
    [SerializeField]
    private int levelNumber = 1;
    void Start()
    {
        mapGeneratorPrefab = Resources.Load<MapGenerator>("MapGenerator");
        MapGenerator mapGenerator = Instantiate(mapGeneratorPrefab);
        GeneralLevelConfig generalLevelConfig = Resources.Load<GeneralLevelConfig>("LevelConfigs/GeneralLevelConfig");
        LevelDepthConfig depthConfig = generalLevelConfig.GetLevelDepthConfiguration(levelNumber - 1);
        LevelConfig config = GetLevelConfig();
        config.Randomize();
        mapGenerator.Initialize(config, depthConfig);
    }

    private LevelConfig GetLevelConfig() {
        List<LevelConfig> configs = new List<LevelConfig>(
            Resources.LoadAll<LevelConfig>(string.Format("LevelConfigs/{0}", levelNumber))
        );
        return configs[Random.Range(0, configs.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
