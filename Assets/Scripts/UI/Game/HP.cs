using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    public GameObject heartFull;
    public GameObject heartHalf;
    public GameObject heartVoid;

    public Player player;

    void Start()
    {
        player = GameManager.Instance.player;

    }

    public void UpdateHP()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < (int)player.Health; i++)
        {
            Instantiate(heartFull, transform);
        }
        for (int i = 0; i < (int)(player.Health * 2) % 2; i++)
        {
            Instantiate(heartHalf, transform);
        }
        for (int i = 0; i < (int)(player.maxHealth - player.Health); i++)
        {
            Instantiate(heartVoid, transform);
        }
    }

}