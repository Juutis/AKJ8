
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "GeneralLevelConfig", menuName = "New GeneralLevelConfig")]
public class GeneralLevelConfig : ScriptableObject
{
    [ShowAssetPreview]
    public Sprite DefaultHallwaySprite;

    [ShowAssetPreview]
    public Sprite DefaultRoomSprite;

    public List<LevelDepthConfiguration> LevelDepthConfigs;
    public LevelDepthConfig GetLevelDepthConfiguration(int depth) {
        LevelDepthConfiguration depthConfig = LevelDepthConfigs[depth];
        return new LevelDepthConfig {
            HallwaySprite = depthConfig.OverrideHallwaySprite != null ? depthConfig.OverrideHallwaySprite : DefaultHallwaySprite,
            RoomSprite = depthConfig.OverrideRoomSprite != null ? depthConfig.OverrideRoomSprite : DefaultRoomSprite,
            HallwaySpriteColor = depthConfig.HallwaySpriteColor,
            RoomSpriteColor = depthConfig.RoomSpriteColor
        };
    }
}

public struct LevelDepthConfig {
    public Sprite HallwaySprite;
    public Sprite RoomSprite;
    public Color HallwaySpriteColor;
    public Color RoomSpriteColor;
}

[System.Serializable]
public class LevelDepthConfiguration {
    [ShowAssetPreview]
    public Sprite OverrideHallwaySprite;
    public Color HallwaySpriteColor = Color.white;

    [ShowAssetPreview]
    public Sprite OverrideRoomSprite;
    public Color RoomSpriteColor = Color.white;
}
