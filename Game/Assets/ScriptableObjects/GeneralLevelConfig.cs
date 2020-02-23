
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

    [ShowAssetPreview]
    public Sprite DefaultStartRoomSprite;

    public Color DefaultStartRoomSpriteColor;

    [ShowAssetPreview]
    public Sprite DefaultEndRoomSprite;

    public Color DefaultEndRoomSpriteColor;
    public LevelDepthConfiguration baseDepthConfig;
    public List<LevelDepthConfiguration> LevelDepthConfigs;
    public LevelDepthConfig GetLevelDepthConfiguration(int depth)
    {
        LevelDepthConfiguration depthConfig;
        if (depth < 0)
        {
            depthConfig = baseDepthConfig;
        }
        else
        {
            depthConfig = LevelDepthConfigs[depth];
        }
        return new LevelDepthConfig
        {
            PowerLevel = depthConfig.PowerLevel,
            HallwaySprite = depthConfig.OverrideHallwaySprite != null ? depthConfig.OverrideHallwaySprite : DefaultHallwaySprite,
            RoomSprite = depthConfig.OverrideRoomSprite != null ? depthConfig.OverrideRoomSprite : DefaultRoomSprite,
            StartRoomSprite = depthConfig.OverrideStartRoomSprite != null ? depthConfig.OverrideStartRoomSprite : DefaultStartRoomSprite,
            StartRoomSpriteColor = depthConfig.OverrideStartRoomSpriteColor != Color.clear ? depthConfig.OverrideStartRoomSpriteColor : DefaultStartRoomSpriteColor,
            EndRoomSprite = depthConfig.OverrideEndRoomSprite != null ? depthConfig.OverrideEndRoomSprite : DefaultEndRoomSprite,
            EndRoomSpriteColor = depthConfig.OverrideEndRoomSpriteColor != Color.clear ? depthConfig.OverrideEndRoomSpriteColor : DefaultEndRoomSpriteColor,
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
    public Sprite StartRoomSprite;
    public Color StartRoomSpriteColor;
    public Sprite EndRoomSprite;
    public Color EndRoomSpriteColor;
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
    public Sprite OverrideStartRoomSprite;
    public Color OverrideStartRoomSpriteColor;

    [ShowAssetPreview]
    public Sprite OverrideEndRoomSprite;
    public Color OverrideEndRoomSpriteColor;
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
