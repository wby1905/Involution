using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "新的房间布局文件")]
public class RoomLayout : ScriptableObject
{
    public GameObject room;
    public bool isGenerateReward;
    public Vector2 RewardPosition;

    public List<Vector2> monsterList = new List<Vector2>();
    public List<SimplePairWithGameObjectVector2> itemList = new List<SimplePairWithGameObjectVector2>();
}
