using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPopulator : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private Player playerPrefab;

    private GameObject startPrefab;
    private GameObject endPrefab;
    public void Initialize(MapGenerator mapGenerator) {
        this.mapGenerator = mapGenerator;
        startPrefab = Resources.Load<GameObject>("StartNode");
        endPrefab = Resources.Load<GameObject>("EndNode");
        MazeRoom startRoom = mapGenerator.GetStartRoom();
        MazeNode startNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(startRoom);
        GameObject start = Instantiate(startPrefab, transform);
        start.transform.localScale = mapGenerator.GetScaled(Vector2.one);
        start.transform.position = mapGenerator.GetScaled(startNode.Rect.position);

        MazeRoom endRoom = mapGenerator.GetEndRoom();
        MazeNode endNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(endRoom);
        GameObject end = Instantiate(endPrefab, transform);
        end.transform.localScale = mapGenerator.GetScaled(Vector2.one);
        end.transform.position = mapGenerator.GetScaled(endNode.Rect.position);

        playerPrefab = Resources.Load<Player>("Player");
        Player player = Instantiate(playerPrefab, transform);
        player.transform.position = start.transform.position;
        GameObject ui = GameObject.FindGameObjectWithTag("UI");
        //ui.SetActive(true);
    }

}
