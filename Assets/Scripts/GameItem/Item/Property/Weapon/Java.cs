using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Java : Weapon
{
    protected override void SetID()
    {
        ID = 7;
    }

    protected override void Effect()
    {
        player.SwitchToGun(GunType.Java);
        UpdateWeapon();
    }
}
