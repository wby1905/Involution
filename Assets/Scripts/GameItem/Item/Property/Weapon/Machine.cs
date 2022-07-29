using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : Weapon
{
    protected override void SetID()
    {
        ID = 5;
    }

    protected override void Effect()
    {
        player.SwitchToGun(GunType.Machine);
        UpdateWeapon();
    }
}
