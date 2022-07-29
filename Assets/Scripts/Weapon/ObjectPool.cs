using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private static ObjectPool instance; //单例的静态实例
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    // 字典，使用了队列作为存储类型
    private GameObject pool;//所有物体的父物体
    public static ObjectPool Instance 
    // 共用的静态实例用于访问instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ObjectPool(); // 如果没有静态实例则生成一个静态实例
            }
            return instance;
        }
    }
    public GameObject GetObject(GameObject prefab) //获取对应的游戏对象
    {
        GameObject _object;
        if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
        //判断字典中是否有预制体，和待分配物体数
        {
            _object = GameObject.Instantiate(prefab);
            PushObject(_object);//生成新物体并将其放入池中
            if (pool == null) //判断场景中是否存在对象池父物体，不存在则创建一个物体名为ObjectPool
                pool = new GameObject("ObjectPool");
            GameObject childPool = GameObject.Find(prefab.name + "Pool");
            //创建子对象的对象池
            if (!childPool)
            {
                childPool = new GameObject(prefab.name + "Pool");
                childPool.transform.SetParent(pool.transform);
            }
            _object.transform.SetParent(childPool.transform);
        }
        _object = objectPool[prefab.name].Dequeue();
        _object.SetActive(true); //将对象激活返回调用者使用
        return _object;
    }

    public void PushObject(GameObject prefab) //用于将用完的物体放回池中
    {
        string _name = prefab.name.Replace("(Clone)", string.Empty);//将Clone后缀去掉后查找
        if (!objectPool.ContainsKey(_name))
            objectPool.Add(_name, new Queue<GameObject>());//对象池不存在生成一个
        objectPool[_name].Enqueue(prefab);
        prefab.SetActive(false);
    }
}
