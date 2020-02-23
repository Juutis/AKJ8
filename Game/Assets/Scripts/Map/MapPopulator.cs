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

    private MazeRoom startRoom;
    private MazeRoom endRoom;

    private enum CreepType
    {
        None,
        Melee,
        Ranged
    }
    public void Initialize(MapGenerator mapGenerator, LevelDepthConfig levelDepthConfig)
    {
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
        Player player = SpawnPlayer();
        SpawnBoss();
        SetUpCamera(player);
        mapGenerator.Rooms.ForEach(room => room.HideImage());
        GameObject ui = GameObject.FindGameObjectWithTag("UI");
        //ui.SetActive(true);
    }

    private void SpawnBoss()
    {
        Boss bossPrefab = Resources.Load<Boss>("Boss");
        Boss boss = Instantiate(bossPrefab, transform);
        MazeNode bossNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(endRoom);
        boss.transform.position = mapGenerator.GetScaled(bossNode.Rect.position);
        boss.Initialize(LevelManager.main.LevelNumber);
    }

    private void SpawnKey()
    {
        Key key = Instantiate(keyPrefab, transform);
        List<MazeRoom> normalRooms = mapGenerator.Rooms.FindAll(room => !room.IsEnd && !room.IsStart);
        MazeRoom keyRoom = normalRooms[Random.Range(0, normalRooms.Count)];
        MazeNode node = mapGenerator.GetRandomEmptyNodeCloseToCenter(keyRoom);
        key.transform.localScale = mapGenerator.GetScaled(Vector2.one);
        key.transform.position = mapGenerator.GetScaled(node.Rect.position);
    }

    private void SpawnCreeps(int spawnThisMany, CreepType creepType)
    {
        if (spawnThisMany <= 0)
        {
            return;
        }
        List<MazeRoom> normalRooms = mapGenerator.Rooms.FindAll(room => !room.IsEnd && !room.IsStart);
        while (normalRooms.Count > 0)
        {
            if (spawnThisMany <= 0)
            {
                break;
            }
            int roomIndex = Random.Range(0, normalRooms.Count);
            MazeRoom room = normalRooms[roomIndex];
            normalRooms.RemoveAt(roomIndex);
            List<MazeNode> nodes = mapGenerator.GetEmptyRoomNodes(room);
            while (nodes.Count > 0)
            {
                if (spawnThisMany <= 0)
                {
                    break;
                }
                int nodeIndex = Random.Range(0, nodes.Count);
                MazeNode chosenNode = nodes[nodeIndex];
                nodes.RemoveAt(nodeIndex);
                if (!chosenNode.HasACreep)
                {
                    bool canSpawnMeleeCreeps = creepType == CreepType.Melee && room.NumberOfMeleeCreeps < depthConfig.MaxMeleeCreepsPerRoom;
                    bool canSpawnRangedCreeps = creepType == CreepType.Ranged && room.NumberOfRangedCreeps < depthConfig.MaxRangedCreepsPerRoom;
                    if (canSpawnMeleeCreeps || canSpawnRangedCreeps)
                    {
                        SpawnCreep(chosenNode, creepType);
                        spawnThisMany -= 1;
                    }
                }
            }
        }
    }

    private void SpawnCreep(MazeNode node, CreepType creepType)
    {
        node.HasACreep = true;
        Vector3 position = mapGenerator.GetScaled(node.Rect.position);
        if (creepType == CreepType.Melee)
        {
            Meleeer meleeCreep = Instantiate(meleeCreepPrefab, transform);
            meleeCreep.transform.position = position;
            node.Room.NumberOfMeleeCreeps += 1;
        }
        else if (creepType == CreepType.Ranged)
        {
            Ranger rangeCreep = Instantiate(rangeCreepPrefab, transform);
            rangeCreep.transform.position = position;
            node.Room.NumberOfRangedCreeps += 1;
        }
    }

    private void SetUpCamera(Player player)
    {
        FollowerCamera followerCamera = Camera.main.GetComponent<FollowerCamera>();
        Vector3 position = followerCamera.transform.position;
        position.x = player.transform.position.x;
        position.y = player.transform.position.y;
        followerCamera.transform.position = position;
        followerCamera.StartFollowing();
    }

    private Player SpawnPlayer()
    {
        playerPrefab = Resources.Load<Player>("Player");
        Player player = Instantiate(playerPrefab, transform);
        player.transform.localScale = mapGenerator.GetScaled(Vector3.one);
        player.transform.position = startObject.transform.position;
        return player;
    }

    private void CreateStartAndEnd()
    {
        startPrefab = Resources.Load<GameObject>("StartNode");
        startRoom = mapGenerator.GetStartRoom();
        MazeNode startNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(startRoom);
        mapGenerator.GetEmptyRoomNodes(startRoom).ForEach(node =>
        {
            node.Image.sprite = depthConfig.StartRoomSprite;
            node.Image.color = depthConfig.StartRoomSpriteColor;
        });
        startObject = Instantiate(startPrefab, transform);
        startObject.transform.localScale = mapGenerator.GetScaled(Vector3.one);
        startObject.transform.position = mapGenerator.GetScaled(startNode.Rect.position);

        endPrefab = Resources.Load<GameObject>("EndNode");
        endRoom = mapGenerator.GetEndRoom();
        MazeNode endNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(endRoom);
        mapGenerator.GetEmptyRoomNodes(endRoom).ForEach(node =>
        {
            node.Image.sprite = depthConfig.EndRoomSprite;
            node.Image.color = depthConfig.EndRoomSpriteColor;
        });
        endObject = Instantiate(endPrefab, transform);
        endObject.transform.localScale = mapGenerator.GetScaled(Vector3.one);
        endObject.transform.position = mapGenerator.GetScaled(endNode.Rect.position);
    }

}
