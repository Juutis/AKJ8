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

    private LevelDepthConfig depthConfig;

    private Meleeer meleeCreepPrefab;
    private Ranger rangeCreepPrefab;
    private Key keyPrefab;

    private enum CreepType {
        None,
        Melee,
        Ranged
    }
    public void Initialize(MapGenerator mapGenerator, LevelDepthConfig levelDepthConfig) {
        this.mapGenerator = mapGenerator;
        depthConfig = levelDepthConfig;
        meleeCreepPrefab = Resources.Load<Meleeer>("MeleeEnemy");
        rangeCreepPrefab = Resources.Load<Ranger>("RangeEnemy");
        keyPrefab = Resources.Load<Key>("Key");

        mapGenerator.HideWorldSprite();
        SpawnCreeps(depthConfig.MeleeCreepNumber, CreepType.Melee);
        SpawnCreeps(depthConfig.RangedCreepNumber, CreepType.Ranged);
        CreateStartAndEnd();
        SpawnKey();
        SpawnPlayer();
        SetUpCamera();
        mapGenerator.Rooms.ForEach(room => room.HideImage());
        GameObject ui = GameObject.FindGameObjectWithTag("UI");
        //ui.SetActive(true);
    }

    private void SpawnKey() {
        Key key = Instantiate(keyPrefab, transform);
        List<MazeRoom> normalRooms = mapGenerator.Rooms.FindAll(room => !room.IsEnd && !room.IsStart);
        MazeRoom keyRoom = normalRooms[Random.Range(0, normalRooms.Count)];
        MazeNode node = mapGenerator.GetRandomEmptyNodeCloseToCenter(keyRoom);
        key.transform.localScale = mapGenerator.GetScaled(Vector2.one);
        key.transform.position = mapGenerator.GetScaled(node.Rect.position);
    }

    private void SpawnCreeps(int spawnThisMany, CreepType creepType) {
        Debug.Log(string.Format("Spawning {0} creeps of type: {1}", spawnThisMany, creepType));
        if (spawnThisMany <= 0) {
            return;
        }
        List<MazeRoom> normalRooms = mapGenerator.Rooms.FindAll(room => !room.IsEnd && !room.IsStart);
        while(normalRooms.Count > 0) {
            if (spawnThisMany <= 0) {
                break;
            }
            int roomIndex = Random.Range(0, normalRooms.Count);
            MazeRoom room = normalRooms[roomIndex];
            normalRooms.RemoveAt(roomIndex);
            List<MazeNode> nodes = mapGenerator.GetEmptyRoomNodes(room);
            while(nodes.Count > 0) {
                if (spawnThisMany <= 0) {
                    break;
                }
                int nodeIndex = Random.Range(0, nodes.Count);
                MazeNode chosenNode = nodes[nodeIndex];
                nodes.RemoveAt(nodeIndex);
                if (!chosenNode.HasACreep) {
                    bool canSpawnMeleeCreeps = creepType == CreepType.Melee && room.NumberOfMeleeCreeps < depthConfig.MaxMeleeCreepsPerRoom;
                    bool canSpawnRangedCreeps = creepType == CreepType.Ranged && room.NumberOfRangedCreeps < depthConfig.MaxRangedCreepsPerRoom;
                    if (canSpawnMeleeCreeps || canSpawnRangedCreeps) {
                        SpawnCreep(chosenNode, creepType);
                        spawnThisMany -= 1;
                    }
                }
            }
        }
    }

    private void SpawnCreep(MazeNode node, CreepType creepType) {
        node.HasACreep = true;
        Vector3 position = mapGenerator.GetScaled(node.Rect.position);
        if (creepType == CreepType.Melee) {
            Meleeer meleeCreep = Instantiate(meleeCreepPrefab, transform);
            meleeCreep.transform.position = position;
            node.Room.NumberOfMeleeCreeps += 1;
        } else if (creepType == CreepType.Ranged) {
            Ranger rangeCreep = Instantiate(rangeCreepPrefab, transform);
            rangeCreep.transform.position = position;
            node.Room.NumberOfRangedCreeps += 1;
        }
        //Debug.Log(string.Format("Spawned a creep of type: {0} at {1}, {2}", creepType, position.x, position.y));
    }

    private void SetUpCamera() {
        FollowerCamera followerCamera = Camera.main.GetComponent<FollowerCamera>();
        followerCamera.StartFollowing();
    }

    private void SpawnPlayer() {
        playerPrefab = Resources.Load<Player>("Player");
        Player player = Instantiate(playerPrefab, transform);
        player.transform.localScale = mapGenerator.GetScaled(Vector2.one);
        player.transform.position = startObject.transform.position;
    }

    private void CreateStartAndEnd() {
        startPrefab = Resources.Load<GameObject>("StartNode");
        MazeRoom startRoom = mapGenerator.GetStartRoom();
        MazeNode startNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(startRoom);
        startObject = Instantiate(startPrefab, transform);
        startObject.transform.localScale = mapGenerator.GetScaled(Vector2.one);
        startObject.transform.position = mapGenerator.GetScaled(startNode.Rect.position);

        endPrefab = Resources.Load<GameObject>("EndNode");
        MazeRoom endRoom = mapGenerator.GetEndRoom();
        MazeNode endNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(endRoom);
        endObject = Instantiate(endPrefab, transform);
        endObject.transform.localScale = mapGenerator.GetScaled(Vector2.one);
        endObject.transform.position = mapGenerator.GetScaled(endNode.Rect.position);
        Debug.Log(string.Format("Spawned levelEnd at {0}, {1}", endNode.Rect.position.x, endNode.Rect.position.y));
    }

}
