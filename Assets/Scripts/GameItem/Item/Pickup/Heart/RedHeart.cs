using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedHeart : Heart
{
    public int Hp;

    protected override bool IsTrigger()
    {
        return player.Health != player.maxHealth;
    }
    protected override void Effect()
    {
        player.AddHealth(Hp);
    }
}
