using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Python : Weapon
{
    protected override void SetID()
    {
        ID = 6;
    }

    protected override void Effect()
    {
        player.SwitchToGun(GunType.Python);
        UpdateWeapon();
    }
}
