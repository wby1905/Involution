using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemFromPool : MonoBehaviour, IsRandomGameObject
{
    public ItemPoolType type;

    void Start()
    {
        Generate();
    }

    public GameObject Generate()
    {
        Level level = GameManager.Instance.level;
        Item item = level.pools.GetItem(type);
        GameObject go = level.manager.GenerateGameObjectInCurrentRoom(item.gameObject, transform.position);
        Destroy(gameObject);
        return go.gameObject;
    }
}
