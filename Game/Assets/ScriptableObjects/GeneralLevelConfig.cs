
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
    public LevelDepthConfig GetLevelDepthConfiguration(int depth)
    {
        LevelDepthConfiguration depthConfig = LevelDepthConfigs[depth];
        return new LevelDepthConfig
        {
            PowerLevel = depthConfig.PowerLevel,
            HallwaySprite = depthConfig.OverrideHallwaySprite != null ? depthConfig.OverrideHallwaySprite : DefaultHallwaySprite,
            RoomSprite = depthConfig.OverrideRoomSprite != null ? depthConfig.OverrideRoomSprite : DefaultRoomSprite,
            HallwaySpriteColor = depthConfig.HallwaySpriteColor,
            RoomSpriteColor = depthConfig.RoomSpriteColor,
            MeleeCreepNumber = depthConfig.DesiredNumberOfMeleeCreeps,
            RangedCreepNumber = depthConfig.DesiredNumberOfRangedCreeps,
            MaxRangedCreepsPerRoom = depthConfig.MaxRangedCreepsPerRoom,
            MaxMeleeCreepsPerRoom = depthConfig.MaxMeleeCreepsPerRoom
        };
    }
}

public struct LevelDepthConfig
{
    public float PowerLevel;
    public Sprite HallwaySprite;
    public Sprite RoomSprite;
    public Color HallwaySpriteColor;
    public Color RoomSpriteColor;
    public int MeleeCreepNumber;
    public int RangedCreepNumber;
    public int MaxMeleeCreepsPerRoom;
    public int MaxRangedCreepsPerRoom;
}

[System.Serializable]
public class LevelDepthConfiguration
{
    [HeaderAttribute("Loot")]
    [Range(0, 1)]
    public float PowerLevel = 0;

    [HeaderAttribute("Map")]
    [ShowAssetPreview]
    public Sprite OverrideHallwaySprite;
    public Color HallwaySpriteColor = Color.white;

    [ShowAssetPreview]
    public Sprite OverrideRoomSprite;
    public Color RoomSpriteColor = Color.white;

    [HeaderAttribute("Enemies")]
    public int DesiredNumberOfMeleeCreeps = 0;
    public int MaxMeleeCreepsPerRoom = 5;
    public int DesiredNumberOfRangedCreeps = 0;
    public int MaxRangedCreepsPerRoom = 5;
}
