using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cpp : Weapon
{
    protected override void SetID()
    {
        ID = 9;
    }

    protected override void Effect()
    {
        player.SwitchToGun(GunType.Cpp);
        UpdateWeapon();
    }
}
