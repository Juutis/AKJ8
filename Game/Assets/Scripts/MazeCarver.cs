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
        nodes = new MazeNode[(int)world.height][];
        for (int verticalIndex = 0; verticalIndex < nodes.Length; verticalIndex += 1)
        {
            nodes[verticalIndex] = new MazeNode[(int)world.width];
            /*for (int horizontalIndex = 0; horizontalIndex < nodes[verticalIndex].Length; horizontalIndex += 1)
            {
                nodes[verticalIndex][horizontalIndex] = CreateNode(horizontalIndex, verticalIndex);
            }*/
        }
        MazeNode firstNode = GetOrCreateNode(
            (int)Random.Range(world.xMin, world.xMax),
            (int)Random.Range(world.yMin, world.yMax)
        );
        Carve(firstNode);
        Traverse();
    }

    private void Carve(MazeNode node)
    {
        node.IsOpen = true;
        carvedNodes.Add(node);
        CreateRectSprite(node.Rect, Color.yellow);
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
        Debug.Log(dirNode.Rect);
        Debug.Log(node.Rect);
        Debug.Log(newX + " - " + newY);
        MazeNode betweenNode = GetOrCreateNode(newX, newY);
        CreateRectSprite(betweenNode.Rect, Color.magenta);

        node.IsOpen = true;
        carvedNodes.Add(node);
        CreateRectSprite(node.Rect, Color.yellow);
    }


    private bool AttemptToCarve(MazeNode dirNode, MazeNode targetNode)
    {
        if (targetNode.IsOpen) {
            return false;
        }
        List<MazeNode> neighbors = GetNeighbors(targetNode);
        bool carvingIsPossible = neighbors
            .Where(neighbor => !neighbor.Equals(dirNode))
            .All(neighbor => !neighbor.IsOpen);

        /*Debug.Log("\nneighbors: ");
        foreach(MazeNode neighbor in neighbors) {
            Debug.Log(string.Format("({2}, {3})Open: {0}, equals: {1}" ,
                neighbor.IsOpen,
                neighbor.Equals(dirNode),
                neighbor.Rect.x,
                neighbor.Rect.y
            ));
        }
        Debug.Log("end neighbors \n");*/

        /*Debug.Log(string.Format("Carving from {0}, {1} to {2}, {3} is: {4}",
            dirNode.Rect.x,
            dirNode.Rect.y,
            targetNode.Rect.x,
            targetNode.Rect.y,
            carvingIsPossible ? "Possible" : "NOT POSSIBLE"
        ));*/

        if (carvingIsPossible)
        {
            Carve(targetNode);
        }
        return carvingIsPossible;
    }

    private void Traverse()
    {
        if (carvedNodes.Count < 1)
        {
            Debug.Log("No more!");
            return;
        }
        MazeNode node = carvedNodes[Random.Range(0, carvedNodes.Count)];
        List<MazeNode> neighbors = GetNeighbors(node);
        //MazeNode neighbor = neighbors[Random.Range(0, neighbors.Count)];
        //if (neighbor)
        //Debug.Log(string.Format("Finding neighbors for {0}, {1}", node.Rect.x, node.Rect.y));
        bool carveSuccesful = false;
        List<MazeNode> cleanNeighbors = neighbors.FindAll(neighbor => !neighbor.IsOpen);
//        Debug.Log(cleanNeighbors.Count);
        if (cleanNeighbors.Count == 0) {
            carvedNodes.Remove(node);
        } else {
            //Carve(cleanNeighbors[Random.Range(0, cleanNeighbors.Count)]);
            CarveBetween(node, cleanNeighbors[Random.Range(0, cleanNeighbors.Count)]);
        }
        /*foreach (MazeNode neighbor in neighbors)
        {
            //Debug.Log(string.Format("Neighbor {0}, {1}", neighbor.Rect.x, neighbor.Rect.y));
            if (AttemptToCarve(node, neighbor))
            {
                //Debug.Log(string.Format("Succesfully carved {0}, {1}", neighbor.Rect.x, neighbor.Rect.y));
                carveSuccesful = true;
                break;
            }
            //Random.Range()
        }*/
        /*if (!carveSuccesful)
        {
            carvedNodes.Remove(node);
        }*/
        //Traverse();
    }

    void Update() {
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
    public MazeNode GetOrCreateNode(int x, int y)
    {
        MazeNode node = nodes[y][x];
        if (node == null) {
            node = new MazeNode(x, y);
            nodes[y][x] = node;
        }
        return node;
    }

    List<MazeNode> GetNeighbors(MazeNode node)
    {
        List<MazeNode> neighbors = new List<MazeNode>();
        /*for (int verticalIndex = -1; verticalIndex <= 1; verticalIndex += 1)
        {
            for (int horizontalIndex = -1; horizontalIndex <= 1; horizontalIndex += 1)
            {*/
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
        /*}
    }*/
        return neighbors;
    }
}
