using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏过程中生成物体的统一入口
/// </summary>
public class GameItemManager : MonoBehaviour
{
    private Level level;
    private Room currentRoom { get { return level.currentRoom; } }

    private void Awake()
    {
        level = GetComponent<Level>();
    }

    /// <summary>
    /// 使用具体位置在当前房间生成游戏物体
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject GenerateGameObjectInCurrentRoom(GameObject prefab, Vector2 position)
    {
        //物体生成后所归属的默认节点
        Transform container = currentRoom.defaultContainer;

        //尝试获取物体类别，不为空便设置到对应节点
        GameItem gameItem = prefab.GetComponent<GameItem>();
        if (gameItem != null)
        {
            switch (gameItem.gameItemType)
            {
                case GameItemType.Item:
                    container = currentRoom.itemContainer;
                    break;
                case GameItemType.Monster:
                    container = currentRoom.monsterContainer;
                    break;
                default:
                    break;
            }
        }

        return currentRoom.GenerateGameObjectWithPosition(prefab, position, container);
    }
    /// <summary>
    /// 使用具体位置在当前房间生成游戏物体
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public T GenerateGameObjectInCurrentRoom<T>(T prefab, Vector2 position) where T : MonoBehaviour
    {
        //物体生成后所归属的默认节点
        Transform container = currentRoom.defaultContainer;

        //尝试获取物体类别，不为空便设置到对应节点
        GameItem gameItem = prefab.GetComponent<GameItem>();
        if (gameItem != null)
        {
            switch (gameItem.gameItemType)
            {
                case GameItemType.Item:
                    container = currentRoom.itemContainer;
                    break;
                case GameItemType.Monster:
                    container = currentRoom.monsterContainer;
                    break;
                default:
                    break;
            }
        }
        return currentRoom.GenerateGameObjectWithPosition(prefab.gameObject, position, container).GetComponent<T>();
    }
    public void DestroyGameObject(GameObject gameObject)
    {
        GameItem gameItem = gameObject.GetComponent<GameItem>();
        if (gameItem != null)
        {
            switch (gameItem.gameItemType)
            {
                case GameItemType.Monster:
                    if (currentRoom.gameObject.activeSelf)
                    {
                        //配合Room类在OnDestroy()内关闭自身，避免关闭游戏时无效调用
                        currentRoom.CheckOpenDoor();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}