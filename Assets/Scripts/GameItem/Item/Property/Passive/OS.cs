using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OS : Property
{
    protected override void SetID()
    {
        ID = 1;
    }

    protected override void Effect()
    {
        foreach(GameObject gun in player.guns){
            gun.GetComponent<Gun>().interval /= 2;
        }

    }
}
