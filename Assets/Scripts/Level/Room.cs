using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    #region 房间属性
    [Header("房间属性")]

    [HideInInspector]
    public RoomType roomType;//房间类型

    [HideInInspector]
    public Vector2 coordinate;//坐标

    private RoomLayout roomLayout;//布局文件

    public bool isArrived = false;//是否已到达
    public bool isCleared = false;//是否已清理过

    //房间宽高
    public static float RoomWidth = 24f;
    public static float RoomHeight = 14f;

    //单位数量和大小
    public static int HorizomtalUnit { get { return 13; } }
    public static int VerticalUnit { get { return 7; } }
    public static float UnitSize { get { return 0.28f; } }


    #endregion

    #region 房间组成
    [Header("房间组成")]

    public List<GameObject> doorList;//门列表
    public int ActiveDoorCount { get { return activeDoorList.Count; } }
    private List<GameObject> activeDoorList = new List<GameObject>();
    private List<GameObject> neighboringDoorList = new List<GameObject>();

    private List<Room> neighboringRoomList = new List<Room>();//相邻的房间
    private GameObject preRoom;
    #endregion


    #region 房间下属节点
    [Header("房间下属节点")]
    public Transform monsterContainer;
    public Transform itemContainer;
    public Transform defaultContainer;
    #endregion

    #region 其他
    [HideInInspector]
    public Level level;//关卡
    #endregion

    private void Awake()
    {
    }

    public void Initialize()
    {
        roomLayout = level.pools.GetRoomLayout(roomType);
        preRoom = roomLayout.room;
        preRoom = Instantiate(preRoom);
        preRoom.transform.parent = transform;
        preRoom.transform.position = transform.position;
        ChangeDoorOutWard();
    }

    /// <summary>
    /// 激活对应方向的门
    /// </summary>
    /// <param name="directionType"></param>
    public void ActivateDoor(DirectionType directionType, Room neighboringRoom, GameObject neighboringDoor)
    {
        GameObject door = doorList[(int)directionType];

        door.transform.Find("Door").gameObject.SetActive(true);
        activeDoorList.Add(door);
        neighboringRoomList.Add(neighboringRoom);
        neighboringDoorList.Add(neighboringDoor);
    }

    /// <summary>
    /// 打开所有已激活的门
    /// </summary>
    public void OpenActivatedDoor()
    {
        List<GameObject> doors = new List<GameObject>();
        doors.AddRange(activeDoorList);
        // doors.AddRange(neighboringDoorList);
        foreach (var door in doors)
        {
            door.transform.Find("collider").gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            door.transform.Find("Door").GetComponent<Animator>().Play("DoorOpen");
        }
    }

    /// <summary>
    /// 改变该房间和相邻房间门的样式
    /// </summary>
    private void ChangeDoorOutWard()
    {
        if (roomType == RoomType.Normal || roomType == RoomType.Start) { return; }

        List<GameObject> doors = new List<GameObject>();
        doors.AddRange(activeDoorList);
        doors.AddRange(neighboringDoorList);

        foreach (var door in doors)
        {
            SpriteRenderer doorColor = door.transform.Find("Door").GetComponent<SpriteRenderer>();
            switch (roomType)
            {
                case RoomType.Boss:
                    doorColor.color = new Color(144, 0, 0, 255);
                    break;
                case RoomType.Treasure:
                    doorColor.color = new Color(255, 255, 0, 255);
                    break;
                case RoomType.Shop:
                    doorColor.color = new Color(120, 255, 240, 255);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 根据房间布局文件roomLayout 生成内容
    /// </summary>
    public void GenerateRoomContent()
    {

        for (int i = 0; i < roomLayout.itemList.Count; i++)
        {
            GenerateGameObjectWithCoordinate(roomLayout.itemList[i].value1, roomLayout.itemList[i].value2, itemContainer);
        }

        for (int i = 0; i < roomLayout.monsterList.Count; i++)
        {
            GameObject monster;
            if (roomType != RoomType.Boss) monster = level.pools.GetMonster(MonsterType.Minion);
            else monster = level.pools.GetMonster(MonsterType.Boss);
            GenerateGameObjectWithCoordinate(monster, roomLayout.monsterList[i], monsterContainer);
        }

        CheckOpenDoor();
    }

    /// <summary>
    /// 生成清理房间的奖励品
    /// </summary>
    private void GenerateRoomClearingReward()
    {
        GameObject reward = level.pools.GetRoomClearingReward(roomType);
        GenerateGameObjectWithCoordinate(reward, roomLayout.RewardPosition, itemContainer);
        if (roomType == RoomType.Boss)
        {
            transform.Find(roomLayout.name + "(Clone)").transform.Find("LevelUp").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 使用具体位置在房间里生成单个物体
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    public GameObject GenerateGameObjectWithPosition(GameObject prefab, Vector2 position, Transform container)
    {
        if (prefab == null) { return null; }

        GameObject go = Instantiate(prefab, container);
        go.transform.position = position;
        return go;
    }
    /// <summary>
    /// 使用坐标在房间里生成单个物体
    /// </summary>
    public GameObject GenerateGameObjectWithCoordinate(GameObject prefab, Vector2 coordinate, Transform container)
    {
        if (prefab == null) { return null; }

        GameObject go = Instantiate(prefab, container);
        //Unit数量为 x:13，y:7,每单位大小为UnitSize
        //coordinate范围为 x:1-25，y:1-13,每单位大小为UnitSize / 2
        //以房间中心为原点，左上角为coordinate起始点，进行位置计算
        // Vector2 postiton = new Vector2(-(HorizomtalUnit - coordinate.x) * UnitSize / 2, (VerticalUnit - coordinate.y) * UnitSize / 2);
        go.transform.localPosition = coordinate;

        return go;
    }


    /// <summary>
    /// 检查开门
    /// </summary>
    public void CheckOpenDoor()
    {
        StartCoroutine(DelayCheck());
    }
    /// <summary>
    /// 延迟检查
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayCheck()
    {
        yield return null;
        if (monsterContainer.childCount == 0 && !isCleared)
        {
            OpenActivatedDoor();
            if (roomLayout.isGenerateReward)
            {
                GenerateRoomClearingReward();
            }
            isCleared = true;
        }
    }

    private void OnDestroy()
    {
        gameObject.SetActive(false);
    }
}