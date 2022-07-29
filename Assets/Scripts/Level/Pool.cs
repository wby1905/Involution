using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [Header("房间池")]
    List<RoomLayout> startRoom = new List<RoomLayout>();
    List<RoomLayout> normalRoom = new List<RoomLayout>();
    List<RoomLayout> bossRoom = new List<RoomLayout>();
    List<RoomLayout> treasureRoom = new List<RoomLayout>();
    List<RoomLayout> shopRoom = new List<RoomLayout>();

    string[] roomLayoutFileFolderPath = new[]
{
        "RoomLayout/StartRoom",
        "RoomLayout/NormalRoom",
        "RoomLayout/BossRoom",
        "RoomLayout/TreasureRoom",
        "RoomLayout/ShopRoom",
    };

    [Header("道具池")]
    [SerializeField]
    private Item defaultItem;//默认道具，道具池为空时返回该道具

    [Space(10)]
    [SerializeField]
    private ItemPool TreasureRoomItemPool;
    private List<Item> TreasureRoomItemList = new List<Item>();
    [SerializeField]
    private ItemPool BossRoomItemPool;
    private List<Item> BossRoomItemList = new List<Item>();
    [SerializeField]
    private ItemPool ShopItemPool;
    private List<Item> ShopItemList = new List<Item>();

    [Header("房间清空奖励")]
    [SerializeField]
    private GameObject bossRoomClearingReward;
    [SerializeField]
    private GameObject normalRoomClearingReward;

    [Header("拾取物商品列表")]
    [SerializeField]
    private RandomGameObjectTable pickupGoodsTable;

    [Header("怪物列表")]
    [SerializeField]
    private RandomGameObjectTable minionTable;
    [SerializeField]
    private RandomGameObjectTable bossTable;

    private void Awake()
    {
        startRoom.AddRange(Resources.LoadAll<RoomLayout>(roomLayoutFileFolderPath[0]));
        normalRoom.AddRange(Resources.LoadAll<RoomLayout>(roomLayoutFileFolderPath[1]));
        bossRoom.AddRange(Resources.LoadAll<RoomLayout>(roomLayoutFileFolderPath[2]));
        treasureRoom.AddRange(Resources.LoadAll<RoomLayout>(roomLayoutFileFolderPath[3]));
        shopRoom.AddRange(Resources.LoadAll<RoomLayout>(roomLayoutFileFolderPath[4]));

        TreasureRoomItemList.AddRange(TreasureRoomItemPool.itemList);
        BossRoomItemList.AddRange(BossRoomItemPool.itemList);
        ShopItemList.AddRange(ShopItemPool.itemList);
    }

    public RoomLayout GetRoomLayout(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start:
                return GetRandomRoomLayout(startRoom, false);
            case RoomType.Normal:
                if (normalRoom.Count == 0)
                {
                    normalRoom.AddRange(Resources.LoadAll<RoomLayout>(roomLayoutFileFolderPath[1]));
                }
                return GetRandomRoomLayout(normalRoom, true);
            case RoomType.Boss:
                return GetRandomRoomLayout(bossRoom, false);
            case RoomType.Treasure:
                return GetRandomRoomLayout(treasureRoom, false);
            case RoomType.Shop:
                return GetRandomRoomLayout(shopRoom, false);
            default:
                return null;
        }
    }
    RoomLayout GetRandomRoomLayout(List<RoomLayout> list, bool isRemove)
    {
        RoomLayout go;
        int index = Random.Range(0, list.Count);
        go = list[index];
        if (isRemove) { list.RemoveAt(index); }
        return go;
    }

    /// <summary>
    /// 从各个道具池中获取道具
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Item GetItem(ItemPoolType type)
    {
        Item Item;
        switch (type)
        {
            case ItemPoolType.TreasureRoom:
                Item = GetRamdomItem(TreasureRoomItemList);
                break;
            case ItemPoolType.BossRoom:
                Item = GetRamdomItem(BossRoomItemList);
                break;
            case ItemPoolType.Shop:
                Item = GetRamdomItem(ShopItemList);
                break;
            default:
                Item = null;
                break;
        }
        return Item;
    }
    private Item GetRamdomItem(List<Item> list)
    {
        if (list.Count == 0)
        {
            return defaultItem;
        }

        int index = Random.Range(0, list.Count);
        Item go = list[index];
        list.RemoveAt(index);
        return go;
    }

    /// <summary>
    /// 获取清空房间的奖励
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetRoomClearingReward(RoomType type)
    {
        GameObject reward = null;
        switch (type)
        {
            case RoomType.Normal:
                reward = normalRoomClearingReward;
                break;
            case RoomType.Boss:
                reward = bossRoomClearingReward;
                break;
            default:
                break;
        }
        return reward;
    }

    /// <summary>
    /// 获取拾取物商品
    /// </summary>
    /// <returns></returns>
    public GameObject GetPickupGoods()
    {
        return pickupGoodsTable.GetGameObject();
    }

    /// <summary>
    /// 获取怪物
    /// </summary>
    /// <returns></returns>
    public GameObject GetMonster(MonsterType monster)
    {
        if (monster == MonsterType.Minion)
            return minionTable.GetGameObject();
        if (monster == MonsterType.Boss)
            return bossTable.GetGameObject();
        else return null;
    }
}
