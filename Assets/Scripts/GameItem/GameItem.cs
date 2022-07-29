using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameItem : MonoBehaviour
{
    public abstract GameItemType gameItemType { get; }

    protected Player player;
    protected Level level;
    protected virtual void Awake()
    {
        player = GameManager.Instance.player;
        level = GameManager.Instance.level;
    }

    protected virtual void OnDestroy()
    {
        level.manager.DestroyGameObject(gameObject);
    }
}
