using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStructure : Property
{
    protected override void SetID()
    {
        ID = 2;
    }

    protected override void Effect()
    {
        if (player.bulletNum <= 5)
        {
            player.bulletNum += 2;
            Transform gun = player.GetGunNow();
            gun.gameObject.SetActive(false);
            gun.gameObject.SetActive(true);
        }
    }
}
