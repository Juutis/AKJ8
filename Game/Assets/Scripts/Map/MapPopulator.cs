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
        mapGenerator.Rooms.ForEach(room => room.HideImage());
        GameObject ui = GameObject.FindGameObjectWithTag("UI");
        //ui.SetActive(true);
    }

    public void Populate() {
        SpawnBoss();
        SpawnCreeps(depthConfig.MeleeCreepNumber, CreepType.Melee);
        SpawnCreeps(depthConfig.RangedCreepNumber, CreepType.Ranged);
        CreateStartAndEnd();
        //SpawnKey();
        Player player = SpawnPlayer();
        SetUpCamera(player);
    }

    private void SpawnBoss()
    {
        Boss bossPrefab = Resources.Load<Boss>("Boss");
        Boss boss = Instantiate(bossPrefab, transform);
        List<MazeRoom> rooms = mapGenerator.Rooms.FindAll(room => !room.IsEnd && !room.IsStart);
        MazeRoom bossRoom = rooms[Random.Range(0, rooms.Count)];
        bossRoom.HasBoss = true;
        MazeNode bossNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(bossRoom);
        boss.transform.position = mapGenerator.GetScaled(bossNode.Rect.position);
        boss.Initialize(LevelManager.main.LevelNumber);
    }

    /*private void SpawnKey()
    {
        List<MazeRoom> normalRooms = mapGenerator.Rooms.FindAll(room => !room.IsEnd && !room.IsStart);
        MazeRoom keyRoom = normalRooms[Random.Range(0, normalRooms.Count)];
        MazeNode node = mapGenerator.GetRandomEmptyNodeCloseToCenter(keyRoom);
        SpawnKeyAt(mapGenerator.GetScaled(node.Rect.position));
    }*/

    public void SpawnKeyAt(Vector3 position) {
        if (keyPrefab == null) {
            keyPrefab = Resources.Load<Key>("Key");
        }
        Key key = Instantiate(keyPrefab, transform);
        key.transform.localScale = mapGenerator.GetScaled(Vector2.one);
        key.transform.position = position;
    }

    private void SpawnCreeps(int spawnThisMany, CreepType creepType)
    {
        if (spawnThisMany <= 0)
        {
            return;
        }
        List<MazeRoom> normalRooms = mapGenerator.Rooms.FindAll(room => !room.IsStart && !room.HasBoss);
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

    public void SetUpCamera(Player player)
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
        return SpawnPlayerAt(startObject.transform.position);
    }

    public Player SpawnPlayerAt(Vector3 position) {
        playerPrefab = Resources.Load<Player>("Player");
        Player player;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)  {
            player = Instantiate(playerPrefab, null);
        } else {
            player = playerObject.GetComponent<Player>();
        }
//        Player player = Instantiate(playerPrefab, transform);
        player.transform.localScale = mapGenerator.GetScaled(Vector3.one);
        player.transform.position = position;
        return player;
    }

    public void SpawnEndAt(Vector3 position) {
        endPrefab = Resources.Load<GameObject>("EndNode");
        endObject = Instantiate(endPrefab, transform);
        endObject.transform.localScale = mapGenerator.GetScaled(Vector3.one);
        endObject.transform.position = position;
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

        endRoom = mapGenerator.GetEndRoom();
        MazeNode endNode = mapGenerator.GetRandomEmptyNodeCloseToCenter(endRoom);
        mapGenerator.GetEmptyRoomNodes(endRoom).ForEach(node =>
        {
            node.Image.sprite = depthConfig.EndRoomSprite;
            node.Image.color = depthConfig.EndRoomSpriteColor;
        });
        SpawnEndAt(mapGenerator.GetScaled(endNode.Rect.position));
    }

}
