using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [Header("房间属性")]
    public const int MAX_SIZE = 20;
    [SerializeField]
    private Room roomPrefab;

    public int roomNum;
    [HideInInspector]
    public Room[,] roomArray = new Room[MAX_SIZE, MAX_SIZE];
    [HideInInspector]
    public Room currentRoom;

    #region 功能类
    [HideInInspector]
    public Pool pools;
    public GameItemManager manager;
    #endregion

    #region 其他
    public Player player;
    private UIManager UI;
    #endregion

    private void Awake()
    {
        pools = GetComponent<Pool>();
        manager = GetComponent<GameItemManager>();
    }
    private void OnEnable()
    {
        player = GameManager.Instance.player;
        UI = UIManager.Instance;
        CreateRooms();
        UI.miniMap.CreateMiniMap();
        MoveToStartRoom();
    }
    private void Start()
    {
        player = GameManager.Instance.player;
        UI = UIManager.Instance;
        CreateRooms();
        UI.miniMap.CreateMiniMap();
        MoveToStartRoom();
    }

    /// <summary>
    /// 创建所有的房间
    /// </summary>
    private void CreateRooms()
    {
        //储存备选生成房间的位置列表
        List<Vector2> alternativeRoomList = new List<Vector2>();
        List<Vector2> hasBeenRemoveRoomList = new List<Vector2>();

        //单门房间列表
        List<Room> singleDoorRoomList = new List<Room>();

        while (singleDoorRoomList.Count < 3)
        {
            //清空已生成的房间
            Array.Clear(roomArray, 0, roomArray.Length);
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            //清空相关数据
            alternativeRoomList.Clear();
            hasBeenRemoveRoomList.Clear();
            singleDoorRoomList.Clear();

            //创建起始房间
            int outsetX = roomArray.GetLength(0) / 2;
            int outsetY = roomArray.GetLength(1) / 2;
            Room lastRoom =
                roomArray[outsetX, outsetY] =
                    CreateRoom(new Vector2(outsetX, outsetY));
            currentRoom = lastRoom;

            //创建其他房间
            Action<int, int> action =
                (newX, newY) =>
                {
                    Vector2 coordinate = new Vector2(newX, newY);
                    if (roomArray[newX, newY] == null)
                    {
                        if (alternativeRoomList.Contains(coordinate))
                        {
                            alternativeRoomList.Remove(coordinate);
                            hasBeenRemoveRoomList.Add(coordinate);
                        }
                        else if (!hasBeenRemoveRoomList.Contains(coordinate))
                        {
                            alternativeRoomList.Add(coordinate);
                        }
                    }
                };

            for (int i = 1; i < roomNum; i++)
            {
                int x = (int)lastRoom.coordinate.x;
                int y = (int)lastRoom.coordinate.y;

                action(x + 1, y);
                action(x - 1, y);
                action(x, y - 1);
                action(x, y + 1);

                Vector2 newRoomCoordinate =
                    alternativeRoomList[UnityEngine
                        .Random
                        .Range(0, alternativeRoomList.Count)];
                lastRoom =
                    roomArray[(int)newRoomCoordinate.x,
                    (int)newRoomCoordinate.y] = CreateRoom(newRoomCoordinate);
                alternativeRoomList.Remove(newRoomCoordinate);
            }
            LinkDoors();
            //计算单门房间数量
            foreach (Room room in roomArray)
            {
                if (
                    room != null &&
                    room.ActiveDoorCount == 1 &&
                    room != currentRoom
                )
                {
                    singleDoorRoomList.Add(room);
                }
            }
        }
        SetRoomsType(singleDoorRoomList);
    }

    private Room CreateRoom(Vector2 coordinate)
    {
        Room newRoom = Instantiate(roomPrefab, transform);
        newRoom.coordinate = coordinate;

        int x = (int)coordinate.x - roomArray.GetLength(0) / 2;
        int y = (int)coordinate.y - roomArray.GetLength(1) / 2;
        newRoom.transform.position =
            new Vector2(y * Room.RoomWidth, x * Room.RoomHeight);
        newRoom.level = gameObject.GetComponent<Level>();
        return newRoom;
    }

    /// <summary>
    /// 打通各个房间相连的门，并记录相连信息
    /// </summary>
    private void LinkDoors()
    {
        foreach (Room room in roomArray)
        {
            if (room != null)
            {
                int x = (int)room.coordinate.x;
                int y = (int)room.coordinate.y;
                if (roomArray[x + 1, y] != null)
                {
                    Room neighboringRoom = roomArray[x + 1, y];
                    GameObject neighboringDoor = neighboringRoom.doorList[1];
                    room
                        .ActivateDoor(DirectionType.Up,
                        neighboringRoom,
                        neighboringDoor);
                }
                if (roomArray[x - 1, y] != null)
                {
                    room
                        .ActivateDoor(DirectionType.Down,
                        roomArray[x - 1, y],
                        (roomArray[x - 1, y].doorList[0]));
                }
                if (roomArray[x, y - 1] != null)
                {
                    room
                        .ActivateDoor(DirectionType.Left,
                        roomArray[x, y - 1],
                        roomArray[x, y - 1].doorList[3]);
                }
                if (roomArray[x, y + 1] != null)
                {
                    room
                        .ActivateDoor(DirectionType.Right,
                        roomArray[x, y + 1],
                        roomArray[x, y + 1].doorList[2]);
                }
            }
        }
    }

    /// <summary>
    /// 设置各个房间的类型
    /// </summary>
    private void SetRoomsType(List<Room> singleDoorRoomList)
    {
        //先全部设为普通
        foreach (Room room in roomArray)
        {
            if (room != null)
            {
                room.roomType = RoomType.Normal;
            }
        }

        //宝藏
        singleDoorRoomList[singleDoorRoomList.Count - 3].roomType =
            RoomType.Treasure;


        //Boss
        singleDoorRoomList[singleDoorRoomList.Count - 1].roomType =
            RoomType.Boss;

        //商店
        singleDoorRoomList[singleDoorRoomList.Count - 2].roomType =
            RoomType.Shop;

        //起始
        currentRoom.roomType = RoomType.Start;

        //初始化
        foreach (Room room in roomArray)
        {
            if (room != null)
            {
                room.Initialize();
            }
        }
    }

    /// <summary>
    /// 进入初始房间
    /// </summary>
    private void MoveToStartRoom()
    {
        StartCoroutine(MoveToDesignateRoom(Vector2.zero));
    }


    /// <summary>
    /// 移动到下一个房间
    /// </summary>
    /// <param name="MoveDirection"></param>
    public void MoveToNextRoom(Vector2 MoveDirection)
    {
        if (currentRoom.isCleared)
        {
            StartCoroutine(MoveToDesignateRoom(MoveDirection));
        }
    }

    private IEnumerator MoveToDesignateRoom(Vector2 MoveDirection)
    {
        Camera mainCamera = GameManager.Instance.myCamera;
        float delaySeconds = 0.3f;

        int x = (int)currentRoom.coordinate.x + (int)MoveDirection.y;
        int y = (int)currentRoom.coordinate.y + (int)MoveDirection.x;
        currentRoom = roomArray[x, y];

        //如果没去过该房间便生成房间内容
        if (!currentRoom.isArrived)
        {
            currentRoom.GenerateRoomContent();
            currentRoom.isArrived = true;
        }

        //更新小地图
        UI.miniMap.UpdateMiniMap(MoveDirection);
        // UpdateGridGraph();

        //暂停并移动玩家
        player.PlayerPause();
        player.transform.position += (Vector3)MoveDirection * 6.5f;

        //移动镜头
        Vector3 originPos = mainCamera.transform.position;
        Vector3 targetPos = currentRoom.transform.position;
        targetPos.y += 1;
        targetPos.z += mainCamera.transform.position.z;
        float time = 0;
        while (time <= delaySeconds)
        {
            mainCamera.transform.position =
                Vector3
                    .Lerp(originPos,
                    targetPos,
                    (1 / delaySeconds) * (time += Time.deltaTime));
            yield return null;
        }

        //恢复玩家暂停
        player.PlayerResume();
    }

}