using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "新的道具池文件")]
public class ItemPool : ScriptableObject
{
    public List<Item> itemList;
}
