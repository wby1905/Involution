using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQL : Weapon
{
    protected override void SetID()
    {
        ID = 8;
    }

    protected override void Effect()
    {
        player.SwitchToGun(GunType.SQL);
        UpdateWeapon();
    }
}
