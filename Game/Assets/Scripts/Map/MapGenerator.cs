using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;


public enum OrthogonalDirection
{
    None,
    North,
    East,
    South,
    West
}

public delegate void GenerationComplete();

public class MapGenerator : MonoBehaviour
{

    private SpriteRenderer spritePrefab;
    private SpriteRenderer worldSprite;
    private MazeCarver mazeCarverPrefab;
    private MapPopulator mapPopulatorPrefab;

    private List<MazeRoom> rooms;
    public List<MazeRoom> Rooms { get { return rooms; } }
    private Rect world;

    private MazeCarver mazeCarver;

    private int worldCreateAttempts = 0;
    private int maxWorldCreateAttempts = 5;

    private int scale = 2;

    private LevelConfig config;

    private LevelDepthConfig depthConfig;
    private GenerationComplete completeCallback;
    private MapPopulator mapPopulator;

    public void Initialize(LevelConfig levelConfig, LevelDepthConfig levelDepthConfig, GenerationComplete callback)
    {
        completeCallback = callback;
        depthConfig = levelDepthConfig;
        config = levelConfig;
        mapPopulatorPrefab = Resources.Load<MapPopulator>("MapPopulator");
        world = new Rect(0, 0, config.Width, config.Height);
        CreateWorld();
    }

    public void MazeCarverFinished()
    {
        List<MazeRoom> roomsWithDoors = rooms.FindAll(room => room.HasAtLeastOneDoor);
        if (roomsWithDoors.Count < config.MinimumNumberOfRooms)
        {
            Debug.Log(string.Format("Only {0} rooms found. {1} required!", roomsWithDoors.Count, config.MinimumNumberOfRooms));
            CreateWorld();
        }
        else
        {
            SelectStartAndEndRooms(roomsWithDoors);
            mapPopulator = Instantiate(mapPopulatorPrefab);
            mapPopulator.Initialize(this, depthConfig);
            completeCallback();
        }
    }

    public Vector3 GetScaled(Vector3 coordinate)
    {
        return coordinate * scale;
    }

    public MazeRoom GetStartRoom()
    {
        return rooms.Find(room => room.IsStart);
    }
    public MazeRoom GetEndRoom()
    {
        return rooms.Find(room => room.IsEnd);
    }

    public List<MazeNode> GetEmptyRoomNodes(MazeRoom room)
    {
        List<MazeNode> emptyNodes = mazeCarver
            .GetAllNodes()
            .FindAll(node => node != null && !node.IsWall && node.IsOpen && node.IsRoom && node.Room != null && node.Room.Equals(room));
        return emptyNodes;
    }

    public MazeNode GetRandomEmptyNode(MazeRoom room)
    {
        List<MazeNode> emptyNodes = GetEmptyRoomNodes(room);
        int randomIndex = Random.Range(0, emptyNodes.Count);

        return emptyNodes[randomIndex];
    }

    public MazeNode GetRandomEmptyNodeCloseToCenter(MazeRoom room)
    {
        List<MazeNode> emptyNodes = GetEmptyRoomNodes(room);
        int min = 0;
        int max = emptyNodes.Count;
        if (emptyNodes.Count > 2)
        {
            min += emptyNodes.Count / 2;
            max -= emptyNodes.Count / 2;
        }
        int randomIndex = Random.Range(min, max);

        return emptyNodes[randomIndex];
    }

    public void HideWorldSprite()
    {
        if (worldSprite != null)
        {
            worldSprite.enabled = false;
        }
    }

    private void RemoveOldWorld()
    {

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if (mazeCarver != null)
        {
            if (mazeCarver.MeshCombiner != null) {
                Destroy(mazeCarver.MeshCombiner.gameObject);
            }
            if (mazeCarver.NavMeshBaker != null) {
                Destroy(mazeCarver.NavMeshBaker.gameObject);
            }
            Destroy(mazeCarver.gameObject);
        }
        if (mapPopulator != null) {
            Destroy(mapPopulator.gameObject);
        }
        rooms = new List<MazeRoom>();
        if (worldSprite != null)
        {
            Destroy(worldSprite.gameObject);
        }
    }

    private void CreateWorld()
    {
        RemoveOldWorld();
        if (worldCreateAttempts >= maxWorldCreateAttempts)
        {
            Debug.Log(string.Format(
                "Mapgen couldn't create a world with at least {0} rooms with doors. Tried {1} times. Please re-evaluate your configuration.",
                config.MinimumNumberOfRooms,
                worldCreateAttempts
            ));
            return;
        }
        worldCreateAttempts += 1;
        worldSprite = CreateRoomSprite(world, Color.gray);

        mazeCarver = InitializeCarver();

        PlaceRooms(config.BigRooms, config.BigRoomPlacementAttempts);
        PlaceRooms(config.MediumRooms, config.MediumRoomPlacementAttempts);
        PlaceRooms(config.SmallRooms, config.SmallRoomPlacementAttempts);
        if (rooms.Count < config.MinimumNumberOfRooms) {
            CreateWorld();
        } else {
            mazeCarver.StartCarving();
        }
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
        if (roomTypes.Count > 0) {
            for (int attemptNumber = 1; attemptNumber <= attempts; attemptNumber += 1)
            {
                MazeRoom room = GenerateRoom(roomTypes);
                AttemptToPlaceARoom(room);
            }
        }
    }


    private void AttemptToPlaceARoom(MazeRoom room)
    {
        bool outOfBounds = room.Rect.xMax > config.Width - 1 || room.Rect.yMax > config.Height - 1;
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
        roomWithPadding.ActualImage = CreateRoomSprite(actualRoom, Color.blue);
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
                    mazeCarver.AddWallNode(x, y, isCorner, room, GetWallPosition(x, y, roomY, roomYMax, roomX, roomXMax));
                }
                else
                {
                    mazeCarver.AddOpenNode(x, y, room);
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
        carver.Initialize(world, this, config, depthConfig);
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
        spriteRenderer.transform.localScale = GetScaled(new Vector2(rect.width, rect.height));
        spriteRenderer.transform.position = GetScaled(rect.center) - (Vector3)Vector2.one;
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
        int x = Random.Range((int)world.xMin + 1, (int)world.xMax);
        int y = Random.Range((int)world.yMin + 1, (int)world.yMax);

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
    public SpriteRenderer ActualImage;

    private List<MazeNode> doors;

    private int numberOfDoors = 0;
    private int maxDoors = 3;

    public int NumberOfMeleeCreeps = 0;
    public int NumberOfRangedCreeps = 0;

    public bool HasAtLeastOneDoor { get { return numberOfDoors > 0; } }

    public void HideImage()
    {
        Image.enabled = false;
        ActualImage.enabled = false;
    }

    public float Distance(MazeRoom otherRoom)
    {
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
