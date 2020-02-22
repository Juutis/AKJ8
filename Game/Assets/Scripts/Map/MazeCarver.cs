using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeCarver : MonoBehaviour
{
    // Start is called before the first frame update
    private MazeNode[][] nodes;

    private List<MazeNode> carvedNodes = new List<MazeNode>();

    private Rect worldRect;

    private int width;
    private int height;

    private bool animate = true;
    private float animateInterval = 0f;

    private int maxDeadEndSearches = 100;
    private int deadEndSearches = 0;

    private MeshCombiner meshCombinerPrefab;

    private int scale = 2;

    private enum FloorType {
        None,
        Path,
        Room
    }

    private Dictionary<MazeNode, List<MazeNode>> neighborLists = new Dictionary<MazeNode, List<MazeNode>>();

    private List<Vector2> positions = new List<Vector2>()
    {
        new Vector2(0, -2),
        new Vector2(2, 0),
        new Vector2(0, 2),
        new Vector2(-2, 0)
    };

    private List<Vector2> finePositions = new List<Vector2>()
    {
        new Vector2(0, -1),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0)
    };

    private SpriteRenderer spritePrefab;

    private MapGenerator mapGenerator;


    private NavMeshBaker navMeshBakerPrefab;
    private GameObject navMeshPlanePrefab;

    private GameObject wallPrefab;
    public void Initialize(Rect world, MapGenerator mapGen)
    {
        if (wallPrefab == null)
        {
            wallPrefab = (GameObject)Resources.Load("Wall");
        }
        mapGenerator = mapGen;
        height = (int)world.height;
        width = (int)world.width;
        nodes = new MazeNode[height][];
        for (int y = 0; y < height; y += 1)
        {
            nodes[y] = new MazeNode[width];
            for (int x = 0; x < width; x += 1)
            {
                nodes[y][x] = null;
            }
        }
        worldRect = world;
    }

    public List<MazeNode> GetAllNodes () {
        return nodes.SelectMany(T => T).ToList();
    }

    public void CarveFirstNode()
    {
        MazeNode firstNode = GetRandomNodeThatIsAvailable();
        Carve(firstNode);
    }

    public void AddWallNode(int x, int y, bool isCorner, MazeRoom room, OrthogonalDirection wallPosition)
    {
        MazeNode node = GetOrCreateNode(x, y);
        node.IsWall = true;
        node.IsCornerWall = isCorner;
        node.Room = room;
        node.WallPosition = wallPosition;
        node.Image = CreateRectSprite(node.Rect, Color.white, FloorType.Room);
    }

    public void AddOpenNode(int x, int y, MazeRoom room)
    {
        MazeNode node = GetOrCreateNode(x, y);
        node.IsOpen = true;
        node.Image = CreateRectSprite(node.Rect, Color.yellow, FloorType.Room);
        node.IsRoom = true;
        node.Room = room;
    }

    public void StartCarving()
    {
        CarveFirstNode();
        Traverse();
    }
    public void FindDeadEnds()
    {

        bool foundDeadEnd = false;
        for (int y = 0; y < height; y += 1)
        {
            for (int x = 0; x < width; x += 1)
            {
                MazeNode node = nodes[y][x];
                if (node != null && node.IsOpen && !node.IsRoom && !node.IsWall && !node.IsDeadEnd)
                {
                    List<MazeNode> neighbors = GetFineNeighbors(node);
                    int openNeighborCount = neighbors.FindAll(neighbor => !neighbor.IsDeadEnd && neighbor.IsOpen).Count;

                    bool isDeadEnd = openNeighborCount < 2;
                    if (isDeadEnd)
                    {
                        node.Image.color = Color.magenta;
                        node.IsDeadEnd = true;
                        foundDeadEnd = true;
                    }

                }
            }
        }
        deadEndSearches += 1;
        if (foundDeadEnd && deadEndSearches < maxDeadEndSearches)
        {
            if (animate)
            {
                Invoke("FindDeadEnds", animateInterval);
            }
            else
            {
                FindDeadEnds();
            }
        }
        else
        {
            Debug.Log(string.Format("Finished dead end searching at {0}", deadEndSearches));
            RemoveDeadEnds();
        }
    }

    private MazeNode currentDeadEndNode;
    private void RemoveDeadEnds()
    {
        bool deadEndsFound = false;
        for (int y = 0; y < height; y += 1)
        {
            for (int x = 0; x < width; x += 1)
            {
                MazeNode node = nodes[y][x];
                if (node != null && node.IsDeadEnd && !node.IsHidden)
                {
                    deadEndsFound = true;
                    currentDeadEndNode = node;
                    if (animate)
                    {
                        Invoke("RemoveCurrentDeadEnd", animateInterval);
                    }
                    else
                    {
                        RemoveCurrentDeadEnd();
                    }
                    return;
                }
            }
        }
        if (!deadEndsFound)
        {
            RemoveFalseWalls();
            Create3DWalls();
            CreateNavMeshes();

            mapGenerator.MazeCarverFinished();
        }
    }
    private NavMeshBaker CreateNavMeshBaker()
    {
        if (navMeshBakerPrefab == null)
        {
            navMeshBakerPrefab = Resources.Load<NavMeshBaker>("NavGrid");
        }
        return Instantiate(navMeshBakerPrefab);
    }

    private GameObject CreateNavMeshPlane(Transform parent)
    {
        if (navMeshPlanePrefab == null)
        {
            navMeshPlanePrefab = Resources.Load<GameObject>("NavMeshPlane");
        }
        return Instantiate(navMeshPlanePrefab, parent);
    }
    private void RemoveFalseWalls()
    {
        for (int y = 0; y <= height; y += 1)
        {
            for (int x = 0; x <= width; x += 1)
            {
                MazeNode node = GetOrCreateNode(x, y);
                if (node != null && node.IsWall && !node.IsOpen)
                {
                    //node.Image.color = Color.yellow;
                    node.IsWall = false;
                    node.IsOpen = true;
                    node.IsRoom = true;
                    if (node.Image != null)
                    {
                        node.Image.color = mapGenerator.RoomFloorSpriteColor;
                        node.Image.sprite = mapGenerator.RoomFloorSprite;
                    }
                }
            }
        }
    }
    private NavMeshBaker navMeshBaker;
    private void CreateNavMeshes()
    {
        navMeshBaker = CreateNavMeshBaker();
        for (int y = -1; y <= height; y += 1)
        {
            for (int x = -1; x <= width; x += 1)
            {
                MazeNode node = GetOrCreateNode(x, y);
                if (node != null && node.IsOpen && !node.IsWall && !node.IsDeadEnd && !node.IsHidden)
                {
                    GameObject navMeshPlane = CreateNavMeshPlane(navMeshBaker.transform);
                    Vector2 newPos = mapGenerator.GetScaled(node.Rect.position);
                    navMeshPlane.transform.position = newPos;
                }
            }
        }
        //Invoke("Bake", 0.5f);
        Bake();
    }

    private void Bake()
    {
        navMeshBaker.Bake();
    }

    private void Create3DWalls()
    {
        if (meshCombinerPrefab == null) {
            meshCombinerPrefab = Resources.Load<MeshCombiner>("MeshCombiner");
        }
        MeshCombiner meshCombiner = Instantiate(meshCombinerPrefab);
        for (int y = -1; y <= height; y += 1)
        {
            for (int x = -1; x <= width; x += 1)
            {
                MazeNode node = GetOrCreateNode(x, y);
                GameObject wall = Instantiate(wallPrefab, meshCombiner.transform);
                
                if (node == null)
                {
                    Vector2 newPos = mapGenerator.GetScaled(new Vector2(x, y));
                    wall.transform.position = newPos;
                }
                else if (node.IsWall || !node.IsOpen || node.IsDeadEnd)
                {
                    Vector2 newPos = mapGenerator.GetScaled(node.Rect.position);
                    wall.transform.position = newPos;
                }
            }
        }
        meshCombiner.Combine();
    }

    private void RemoveCurrentDeadEnd()
    {
        if (currentDeadEndNode != null)
        {
            currentDeadEndNode.Image.color = Color.clear;
            currentDeadEndNode.IsHidden = true;
            currentDeadEndNode = null;
        }
        RemoveDeadEnds();
    }

    private MazeNode GetRandomNodeThatIsAvailable()
    {
        List<Vector2> positions = new List<Vector2>();
        for (int y = 0; y < height; y += 1)
        {
            for (int x = 0; x < width; x += 1)
            {
                int evenX = NearestOdd(x);
                int evenY = NearestOdd(y);
                MazeNode node = nodes[evenY][evenX];
                if (node == null || !node.IsOpen && !node.IsWall)
                {
                    positions.Add(new Vector2(evenX, evenY));
                }
            }
        }
        if (positions.Count > 0)
        {
            Vector2 randomPosition = positions[Random.Range(0, positions.Count)];
            return GetOrCreateNode(
                (int)randomPosition.x,
                (int)randomPosition.y
            );
        }
        return null;
    }

    private void Carve(MazeNode node)
    {
        node.IsOpen = true;
        carvedNodes.Add(node);
        node.Image = CreateRectSprite(node.Rect, Color.red, FloorType.Path);
    }

    private void CarveBetween(MazeNode dirNode, MazeNode node)
    {
        int dirX = (int)dirNode.Rect.x;
        int dirY = (int)dirNode.Rect.y;
        int targetX = (int)node.Rect.x;
        int targetY = (int)node.Rect.y;

        int newX = dirX;
        int newY = dirY;
        if (dirX < targetX)
        {
            newX = dirX + 1;
        }
        if (dirX > targetX)
        {
            newX = dirX - 1;
        }
        if (dirY < targetY)
        {
            newY = dirY + 1;
        }
        if (dirY > targetY)
        {
            newY = dirY - 1;
        }
        MazeNode betweenNode = GetOrCreateNode(newX, newY);
        FloorType floorType = (node.IsWall || node.IsRoom) ? FloorType.Room : FloorType.Path;
        betweenNode.Image = CreateRectSprite(betweenNode.Rect, Color.white, floorType);
        betweenNode.IsOpen = true;

        node.IsOpen = true;
        carvedNodes.Add(node);
        node.Image = CreateRectSprite(node.Rect, Color.white, floorType);
    }

    private void Traverse()
    {
        if (carvedNodes.Count < 1)
        {
            Debug.Log("No more!");
            FindDeadEnds();
            return;
        }
        MazeNode node = carvedNodes[Random.Range(0, carvedNodes.Count)];
        List<MazeNode> neighbors = GetNeighbors(node);

        List<MazeNode> cleanNeighbors = neighbors.FindAll(neighbor => !neighbor.IsOpen && !neighbor.IsWall && !neighbor.IsRoom);
        if (cleanNeighbors.Count == 0)
        {
            List<MazeNode> walls = neighbors.FindAll(neighbor => neighbor.IsWall && !neighbor.IsCornerWall);
            if (!CreateDoors(node, walls))
            {
            }
            carvedNodes.Remove(node);
        }
        else
        {
            CarveBetween(node, cleanNeighbors[Random.Range(0, cleanNeighbors.Count)]);
        }
        if (animate)
        {
            Invoke("Traverse", animateInterval);
        }
        else
        {
            Traverse();
        }
    }

    private bool CreateDoors(MazeNode dirNode, List<MazeNode> nodes)
    {
        bool someDoorsWereCreated = false;
        while (nodes.Count > 0)
        {
            int nodeIndex = Random.Range(0, nodes.Count);
            MazeNode node = nodes[nodeIndex];
            nodes.RemoveAt(nodeIndex);
            if (node.Room.AttemptToCreateADoor(node))
            {
                node.IsOpen = true;
                node.IsWall = false;
                //node.Image.color = Color.white;
                CarveBetween(dirNode, node);
                someDoorsWereCreated = true;
            }
        }
        return someDoorsWereCreated;
    }

    private int NearestOdd(int number)
    {
        return (number % 2 == 0) ? number + 1 : number;
    }

    private void Update()
    {
        /*if (Input.GetKey(KeyCode.Space))
        {
            Traverse();
        }*/
    }

    private SpriteRenderer CreateRectSprite(Rect rect, Color color, FloorType floorType)
    {
        if (spritePrefab == null)
        {
            spritePrefab = Resources.Load<SpriteRenderer>("NodeSprite");
        }
        SpriteRenderer spriteRenderer = Instantiate(spritePrefab, Vector2.zero, Quaternion.identity);
        spriteRenderer.transform.parent = transform;
        Vector3 scale = mapGenerator.GetScaled(new Vector2(rect.width, rect.height));
        scale.z = 1f;
        spriteRenderer.transform.localScale = scale;
        spriteRenderer.transform.position = mapGenerator.GetScaled(rect.position);
        spriteRenderer.gameObject.SetActive(true);
        spriteRenderer.color = color;
        spriteRenderer.name = string.Format("x:{0} y:{1}", rect.x, rect.y);
        if (floorType == FloorType.Path) {
            spriteRenderer.color = mapGenerator.FloorSpriteColor;
            spriteRenderer.sprite = mapGenerator.FloorSprite;
            spriteRenderer.sortingOrder = 20;
        }
        if (floorType == FloorType.Room) {
            spriteRenderer.color = mapGenerator.RoomFloorSpriteColor;
            spriteRenderer.sprite = mapGenerator.RoomFloorSprite;
            spriteRenderer.sortingOrder = 30;
        }
        return spriteRenderer;
    }
    private MazeNode GetOrCreateNode(int x, int y)
    {
        MazeNode node = null;
        if ((nodes.Length > y && y >= 0) && (nodes[y].Length > x && x >= 0))
        {
            node = nodes[y][x];
            if (node == null)
            {
                node = new MazeNode(x, y);
                nodes[y][x] = node;
            }
        }
        return node;
    }

    private List<MazeNode> GetFineNeighbors(MazeNode node)
    {
        return FindNeighbors(node, finePositions);
    }
    private List<MazeNode> GetNeighbors(MazeNode node)
    {
        if (neighborLists.ContainsKey(node))
        {
            return neighborLists[node];
        }
        List<MazeNode> neighbors = FindNeighbors(node, positions);
        neighborLists[node] = neighbors;
        return neighbors;
    }

    private List<MazeNode> FindNeighbors(MazeNode node, List<Vector2> neighborPositions)
    {
        List<MazeNode> neighbors = new List<MazeNode>();
        foreach (Vector2 pos in neighborPositions)
        {
            int x = (int)node.Rect.x + (int)pos.x;
            int y = (int)node.Rect.y + (int)pos.y;
            bool isWithinBounds = (y >= 0 && nodes.Length > y) && (x >= 0 && nodes[y].Length > x);
            if (isWithinBounds)
            {
                MazeNode foundNode = GetOrCreateNode(x, y);
                neighbors.Add(foundNode);
            }
        }
        return neighbors;
    }

}

public class MazeNode
{
    public MazeNode(int x, int y)
    {
        Rect = new Rect(x, y, 1, 1);
    }

    public Rect Rect;

    public bool IsOpen = false;

    public bool IsWall = false;

    public bool IsRoom = false;

    public bool IsCornerWall = false;

    public bool IsDeadEnd = false;
    public bool IsHidden = false;

    public OrthogonalDirection WallPosition;

    public MazeRoom Room = null;

    public SpriteRenderer Image;


    public override bool Equals(System.Object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            MazeNode node = (MazeNode)obj;
            return node.Rect == this.Rect;
        }
    }

    public override int GetHashCode()
    {
        return ((int)Rect.x << 2) ^ (int)Rect.y;
    }

}