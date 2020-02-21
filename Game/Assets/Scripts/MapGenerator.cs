using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

[System.Serializable]
public class RoomType {
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
    private int numberOfRounds = 200;

    [SerializeField]
    private List<RoomType> roomTypes;

    private SpriteRenderer spritePrefab;
    private MazeCarver mazeCarverPrefab;


    private List<Rect> rooms;
    private Rect world;
    void Start()
    {
        world = new Rect(0, 0, worldWidth, worldHeight);
        CreateRoomSprite(world, Color.gray);
        rooms = new List<Rect>();
        for(int index = 0; index <= numberOfRounds; index += 1) {
            Rect room = GenerateRoom();
            bool overlaps = rooms.Any(existingRoom => room.Overlaps(existingRoom));
            if (!overlaps) {
                Rect minRoom = new Rect(room.x + 1, room.y + 1, room.width - 2, room.height - 2);
                rooms.Add(room);
                CreateRoomSprite(room, Color.green);
                CreateRoomSprite(minRoom, Color.blue);
            }
        }
        Carve();
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Created rect");
            rooms.Add(GenerateRoom());
        }*/
    }

    private void Carve() {
        if (mazeCarverPrefab == null) {
            mazeCarverPrefab = Resources.Load<MazeCarver>("MazeCarver");
        }
        MazeCarver mazeCarver = Instantiate(mazeCarverPrefab, Vector2.zero, Quaternion.identity);
        mazeCarver.Initialize(world);
    }

    private SpriteRenderer CreateRoomSprite(Rect rect, Color color) {
        if (spritePrefab == null) {
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

    private Rect GenerateRoom() {
        RoomType roomType = roomTypes[Random.Range(0, roomTypes.Count)];
        int x = Random.Range((int)world.xMin, (int)world.xMax);
        int y = Random.Range((int)world.yMin, (int)world.yMax);
        /*Debug.Log(string.Format("{0}, {1}, {2}, {3}, {4}, {5}",
            world.xMin,
            world.xMax,
            world.yMin,
            world.yMax,
            x,
            y
        ));*/

        Rect room = new Rect(x, y, roomType.Width, roomType.Height);

        return room;
    }
}
