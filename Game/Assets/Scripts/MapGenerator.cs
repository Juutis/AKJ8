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

public enum OrthogonalDirection
{
    None,
    North,
    East,
    South,
    West
}

public class MapGenerator : MonoBehaviour
{

    [MinValue(10)]
    [SerializeField]
    private int worldWidth = 2000;
    [MinValue(10)]
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


    private List<MazeRoom> rooms;
    private Rect world;

    private MazeCarver mazeCarver;

    private int worldCreateAttempts = 0;
    private int maxWorldCreateAttempts = 5;
    private int minRoomCount = 3;

    void Start()
    {
        world = new Rect(0, 0, worldWidth, worldHeight);
        CreateWorld();
    }

    public void MazeCarverFinished() {
        List<MazeRoom> roomsWithDoors = rooms.FindAll(room => room.HasAtLeastOneDoor);
        if (roomsWithDoors.Count < minRoomCount) {
            Debug.Log(string.Format("Only {0} rooms found. {1} required!", roomsWithDoors.Count, minRoomCount));
            CreateWorld();
        } else {
            SelectStartAndEndRooms(roomsWithDoors);
        }
    }

    private void RemoveOldWorld() {
        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }
        if (mazeCarver != null) {
            Destroy(mazeCarver.gameObject);
        }
        rooms = new List<MazeRoom>();
    }

    private void CreateWorld()
    {
        RemoveOldWorld();
        if (worldCreateAttempts >= maxWorldCreateAttempts) {
            Debug.Log(string.Format(
                "Mapgen couldn't create a world with at least {0} rooms with doors. Tried {1} times. Please re-evaluate your configuration.",
                minRoomCount,
                worldCreateAttempts
            ));
            return;
        }
        worldCreateAttempts += 1;
        CreateRoomSprite(world, Color.gray);
        mazeCarver = InitializeCarver();

        PlaceRooms(bigRooms, bigRoomPlacementAttempts);
        PlaceRooms(smallRooms, smallRoomPlacementAttempts);
        PlaceRooms(tinyRooms, tinyRoomPlacementAttempts);
        mazeCarver.StartCarving();
    }

    private void SelectStartAndEndRooms(List<MazeRoom> roomsWithDoors)
    {
        int startRoomIndex = Random.Range(0, roomsWithDoors.Count);
        MazeRoom startRoom = roomsWithDoors[startRoomIndex];
        startRoom.IsStart = true;
        startRoom.Image.color = Color.green;
        roomsWithDoors.RemoveAt(startRoomIndex);
        MazeRoom endRoom = roomsWithDoors.OrderByDescending(room => room.Distance(startRoom)).First();
        endRoom.IsEnd = true;
        endRoom.Image.color = Color.white;

    }
    private void PlaceRooms(List<RoomType> roomTypes, int attempts)
    {
        for (int attemptNumber = 1; attemptNumber <= attempts; attemptNumber += 1)
        {
            MazeRoom room = GenerateRoom(roomTypes);
            AttemptToPlaceARoom(room);
        }
    }


    private void AttemptToPlaceARoom(MazeRoom room)
    {
        bool outOfBounds = room.Rect.xMax > worldWidth || room.Rect.yMax > worldHeight;
        bool overlaps = rooms.Any(existingRoom => room.Rect.Overlaps(existingRoom.Rect));
        if (!overlaps && !outOfBounds)
        {
            PlaceRoom(room);
        }
    }

    private void PlaceRoom(MazeRoom roomWithPadding)
    {
        Rect actualRoom = new Rect(
            roomWithPadding.Rect.x + 1,
            roomWithPadding.Rect.y + 1,
            roomWithPadding.Rect.width - 2,
            roomWithPadding.Rect.height - 2
        );
        rooms.Add(roomWithPadding);
        roomWithPadding.Image = CreateRoomSprite(roomWithPadding.Rect, Color.gray);
        CreateRoomSprite(actualRoom, Color.blue);
        PlaceRoomNodes(roomWithPadding, actualRoom);
    }

    private void PlaceRoomNodes(MazeRoom room, Rect actualRoom)
    {
        int roomY = (int)actualRoom.y;
        int roomYMax = (int)actualRoom.yMax;
        int roomX = (int)actualRoom.x;
        int roomXMax = (int)actualRoom.xMax;
        for (int y = roomY; y < roomYMax; y += 1)
        {
            for (int x = roomX; x < roomXMax; x += 1)
            {
                bool isWall = (
                    x == roomX ||
                    x == roomXMax - 1 ||
                    y == roomY ||
                    y == roomYMax - 1
                );

                if (isWall)
                {
                    bool isCorner = (
                        (x == roomX && y == roomY) ||
                        (x == roomX && y == roomYMax - 1) ||
                        (x == roomXMax - 1 && y == roomY) ||
                        (x == roomXMax - 1 && y == roomYMax - 1)
                    );
                    mazeCarver.AddWall(x, y, isCorner, room, GetWallPosition(x, y, roomY, roomYMax, roomX, roomXMax));
                }
                else
                {
                    mazeCarver.AddOpenNode(x, y);
                }
            }
        }
    }

    private OrthogonalDirection GetWallPosition(int x, int y, int yMin, int yMax, int xMin, int xMax)
    {
        if (x == xMin)
        {
            return OrthogonalDirection.West;
        }
        if (x == xMax)
        {
            return OrthogonalDirection.East;
        }
        if (y == yMin)
        {
            return OrthogonalDirection.South;
        }
        if (y == yMax)
        {
            return OrthogonalDirection.North;
        }
        return OrthogonalDirection.None;
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
        carver.Initialize(world, this);
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

    private int NearestEven(int number)
    {
        return number += (number % 2);
    }

    private MazeRoom GenerateRoom(List<RoomType> roomTypes)
    {
        RoomType roomType = roomTypes[Random.Range(0, roomTypes.Count)];
        int x = Random.Range((int)world.xMin, (int)world.xMax);
        int y = Random.Range((int)world.yMin, (int)world.yMax);

        MazeRoom room = new MazeRoom(new Rect(NearestEven(x), NearestEven(y), roomType.Width, roomType.Height));
        return room;
    }
}

public class MazeRoom
{
    public MazeRoom(Rect rect)
    {
        Rect = rect;
    }

    public bool IsStart = false;
    public bool IsEnd = false;

    public Rect Rect;

    public SpriteRenderer Image;

    private List<MazeNode> doors;

    private int numberOfDoors = 0;
    private int maxDoors = 3;

    public bool HasAtLeastOneDoor { get { return numberOfDoors > 0; } }

    public float Distance(MazeRoom otherRoom) {
        return Vector2.Distance(Rect.position, otherRoom.Rect.position);
    }

    public bool AttemptToCreateADoor(MazeNode node)
    {
        if (doors == null)
        {
            doors = new List<MazeNode>();
        }
        if (numberOfDoors > maxDoors)
        {
            Debug.Log(string.Format("Too many doors ({0} / {1})!", numberOfDoors, maxDoors));
            return false;
        }
        bool alreadyHaveDoorThere = doors.Any(door => door.WallPosition == node.WallPosition);
        if (!alreadyHaveDoorThere)
        {
            doors.Add(node);
            numberOfDoors += 1;
            return true;
        }
        return false;
    }
}
