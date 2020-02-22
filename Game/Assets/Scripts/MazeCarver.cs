using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeNode
{
    public MazeNode(int x, int y)
    {
        Rect = new Rect(x, y, 1, 1);
    }

    public Rect Rect;

    public bool IsOpen = false;

    public bool IsWall = false;

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

public class MazeCarver : MonoBehaviour
{
    // Start is called before the first frame update
    private MazeNode[][] nodes;

    private List<MazeNode> carvedNodes = new List<MazeNode>();

    private Rect worldRect;

    private int width;
    private int height;

    private int newAttempts = 3;

    private Dictionary<MazeNode, List<MazeNode>> neighborLists = new Dictionary<MazeNode, List<MazeNode>>();

    private List<Vector2> positions = new List<Vector2>()
    {
        new Vector2(0, -2),
        new Vector2(2, 0),
        new Vector2(0, 2),
        new Vector2(-2, 0)
    };

    private SpriteRenderer spritePrefab;
    public void Initialize(Rect world)
    {
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

    public void CarveFirstNode()
    {
        MazeNode firstNode = GetRandomNodeThatIsAvailable();
        Carve(firstNode);
    }

    public void AddWall(int x, int y) {
        MazeNode node = GetOrCreateNode(x, y);
        node.IsWall = true;
        CreateRectSprite(node.Rect, Color.yellow);
    }

    private MazeNode GetRandomNodeThatIsAvailable() {
        List<Vector2> positions = new List<Vector2>();
        for (int y = 0; y < height; y += 1) {
            for (int x = 0; x < width; x += 1) {
                int evenX = NearestOdd(x);
                int evenY = NearestOdd(y);
                MazeNode node = nodes[evenY][evenX];
                    if (node == null || !node.IsOpen && !node.IsWall) {
                    positions.Add(new Vector2(evenX, evenY));
                }
            }
        }
        if (positions.Count > 0) {
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
        CreateRectSprite(node.Rect, Color.red);
    }

    private void CarveBetween(MazeNode dirNode, MazeNode node)
    {
        int dirX = (int)dirNode.Rect.x;
        int dirY = (int)dirNode.Rect.y;
        int targetX = (int)node.Rect.x;
        int targetY = (int)node.Rect.y;

        int newX = dirX;
        int newY = dirY;
        if (dirX < targetX) {
            newX = dirX + 1;
        }
        if (dirX > targetX) {
            newX = dirX - 1;
        }
        if (dirY < targetY) {
            newY = dirY + 1;
        }
        if (dirY > targetY) {
            newY = dirY - 1;
        }
        MazeNode betweenNode = GetOrCreateNode(newX, newY);
        CreateRectSprite(betweenNode.Rect, Color.yellow);

        node.IsOpen = true;
        carvedNodes.Add(node);
        CreateRectSprite(node.Rect, Color.yellow);
    }

    private void Traverse()
    {
        if (carvedNodes.Count < 1)
        {
            /*newAttempts -= 1;
            if (newAttempts > 0) {
                CarveFirstNode();
            } else {
            }*/
            Debug.Log("No more!");
            return;
        }
        MazeNode node = carvedNodes[Random.Range(0, carvedNodes.Count)];
        List<MazeNode> neighbors = GetNeighbors(node);

        List<MazeNode> cleanNeighbors = neighbors.FindAll(neighbor => !neighbor.IsOpen && !neighbor.IsWall);
        if (cleanNeighbors.Count == 0) {
            carvedNodes.Remove(node);
        } else {
            CarveBetween(node, cleanNeighbors[Random.Range(0, cleanNeighbors.Count)]);
        }
        //Traverse();
    }
    private int NearestOdd(int number) {
        return (number % 2 == 0) ? number + 1 : number;
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            Traverse();
        }
    }

    private SpriteRenderer CreateRectSprite(Rect rect, Color color)
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
    private MazeNode GetOrCreateNode(int x, int y)
    {
        MazeNode node = nodes[y][x];
        if (node == null) {
            node = new MazeNode(x, y);
            nodes[y][x] = node;
        }
        return node;
    }

    private List<MazeNode> GetNeighbors(MazeNode node)
    {
        if (neighborLists.ContainsKey(node)) {
            return neighborLists[node];
        }
        List<MazeNode> neighbors = new List<MazeNode>();
        foreach (Vector2 pos in positions)
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
        neighborLists[node] = neighbors;
        return neighbors;
    }
}
