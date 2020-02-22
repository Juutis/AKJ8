using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPopulator : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private Player playerPrefab;

    private GameObject startPrefab;
    private GameObject endPrefab;

    private GameObject startObject;
    private GameObject endObject;
    public void Initialize(MapGenerator mapGenerator) {
        this.mapGenerator = mapGenerator;

        CreateStartAndEnd();
        SpawnPlayer();
        GameObject ui = GameObject.FindGameObjectWithTag("UI");
        //ui.SetActive(true);
    }
    
    private void SpawnPlayer() {
        playerPrefab = Resources.Load<Player>("Player");
        Player player = Instantiate(playerPrefab, transform);
        player.transform.position = startObject.transform.position;
    }

    private void CreateStartAndEnd() {
        startPrefab = Resources.Load<GameObject>("StartNode");
        endPrefab = Resources.Load<GameObject>("EndNode");
        MazeRoom startRoom = mapGenerator.GetStartRoom();
        MazeNode startNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(startRoom);
        startObject = Instantiate(startPrefab, transform);
        startObject.transform.localScale = mapGenerator.GetScaled(Vector2.one);
        startObject.transform.position = mapGenerator.GetScaled(startNode.Rect.position);

        MazeRoom endRoom = mapGenerator.GetEndRoom();
        MazeNode endNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(endRoom);
        endObject = Instantiate(endPrefab, transform);
        endObject.transform.localScale = mapGenerator.GetScaled(Vector2.one);
        endObject.transform.position = mapGenerator.GetScaled(endNode.Rect.position);
    }

}
