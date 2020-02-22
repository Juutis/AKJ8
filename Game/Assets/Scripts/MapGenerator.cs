using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

[System.Serializable]
public class RoomType
{
    [MinValue(3), MaxValue(10)]
    public int Width = 3;

    [MinValue(3), MaxValue(10)]
    public int Height = 3;
}

public class MapGenerator : MonoBehaviour
{

    [MinValue(50)]
    [SerializeField]
    private int worldWidth = 2000;
    [MinValue(50)]
    [SerializeField]
    private int worldHeight = 2000;

    [SerializeField]
    private int bigRoomPlacementAttempts = 200;
    [SerializeField]
    private int smallRoomPlacementAttempts = 200;
    [SerializeField]
    private int tinyRoomPlacementAttempts = 200;

    [SerializeField]
    private List<RoomType> bigRooms;
    [SerializeField]
    private List<RoomType> smallRooms;
    [SerializeField]
    private List<RoomType> tinyRooms;

    private SpriteRenderer spritePrefab;
    private MazeCarver mazeCarverPrefab;


    private List<Rect> rooms;
    private Rect world;

    private MazeCarver mazeCarver;
    void Start()
    {
        world = new Rect(0, 0, worldWidth, worldHeight);
        CreateRoomSprite(world, Color.gray);
        rooms = new List<Rect>();
        mazeCarver = InitializeCarver();
        PlaceRooms(bigRooms, bigRoomPlacementAttempts);
        PlaceRooms(smallRooms, smallRoomPlacementAttempts);
        PlaceRooms(tinyRooms, tinyRoomPlacementAttempts);
        mazeCarver.CarveFirstNode();
    }

    private void PlaceRooms(List<RoomType> roomTypes, int attempts)
    {
        for (int attemptNumber = 1; attemptNumber <= attempts; attemptNumber += 1)
        {
            Rect room = GenerateRoom(roomTypes);
            AttemptToPlaceARoom(room);
        }
    }

    private void AttemptToPlaceARoom(Rect room) {
        bool outOfBounds = room.xMax > worldWidth || room.yMax > worldHeight;
        bool overlaps = rooms.Any(existingRoom => room.Overlaps(existingRoom));
        if (!overlaps && !outOfBounds)
        {
            PlaceRoom(room);
        }
    }

    private void PlaceRoom(Rect roomWithPadding)
    {
        Rect actualRoom = new Rect(roomWithPadding.x + 1, roomWithPadding.y + 1, roomWithPadding.width - 2, roomWithPadding.height - 2);
        rooms.Add(roomWithPadding);
        CreateRoomSprite(roomWithPadding, Color.green);
        CreateRoomSprite(actualRoom, Color.blue);
        for (int y = (int)roomWithPadding.y; y < (int)roomWithPadding.yMax; y += 1)
        {
            for (int x = (int)roomWithPadding.x; x < (int)roomWithPadding.xMax; x += 1)
            {
                mazeCarver.AddWall(x, y);
            }
        }
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Created rect");
            rooms.Add(GenerateRoom());
        }*/
    }

    private MazeCarver InitializeCarver()
    {
        if (mazeCarverPrefab == null)
        {
            mazeCarverPrefab = Resources.Load<MazeCarver>("MazeCarver");
        }
        MazeCarver carver = Instantiate(mazeCarverPrefab, Vector2.zero, Quaternion.identity);
        carver.Initialize(world);
        return carver;
    }

    private SpriteRenderer CreateRoomSprite(Rect rect, Color color)
    {
        if (spritePrefab == null)
        {
            spritePrefab = Resources.Load<SpriteRenderer>("RoomSprite");
        }
        SpriteRenderer spriteRenderer = Instantiate(spritePrefab, Vector2.zero, Quaternion.identity);
        spriteRenderer.transform.parent = transform;
        spriteRenderer.transform.localScale = new Vector2(rect.width, rect.height);
        spriteRenderer.transform.position = rect.position;
        spriteRenderer.gameObject.SetActive(true);
        spriteRenderer.color = color;
        return spriteRenderer;
    }

    private int NearestEven(int number) {
        return number += (number % 2);
    }

    private Rect GenerateRoom(List<RoomType> roomTypes)
    {
        RoomType roomType = roomTypes[Random.Range(0, roomTypes.Count)];
        int x = Random.Range((int)world.xMin, (int)world.xMax);
        int y = Random.Range((int)world.yMin, (int)world.yMax);

        Rect room = new Rect(NearestEven(x), NearestEven(y), roomType.Width, roomType.Height);

        return room;
    }
}
