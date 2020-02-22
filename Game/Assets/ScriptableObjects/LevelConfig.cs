
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "New LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Range(3, 10)]
    public int MinimumNumberOfRooms = 3;

    [MinMaxSlider(20f, 50f)]
    [SerializeField]
    private Vector2 widthRange = new Vector2(20f, 25f);

    [MinMaxSlider(20f, 50f)]
    [SerializeField]
    private Vector2 heightRange = new Vector2(20f, 25f);

    [HideInInspector]
    public int Width;
    [HideInInspector]
    public int Height;

    public List<RoomType> BigRooms;
    public List<RoomType> MediumRooms;
    public List<RoomType> SmallRooms;

    public int BigRoomPlacementAttempts = 200;
    public int MediumRoomPlacementAttempts = 200;
    public int SmallRoomPlacementAttempts = 200;

    public void Randomize() {
        Width = Random.Range((int)widthRange.x, (int)widthRange.y);
        Height = Random.Range((int)heightRange.x, (int)heightRange.y);
    }

}

[System.Serializable]
public class RoomType
{
    [Range(7, 35)]
    public int Width = 9;

    [Range(7, 35)]
    public int Height = 9;
}
