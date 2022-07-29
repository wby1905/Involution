using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBrokenHeart : Property
{
    protected override void SetID()
    {
        ID = 99;
    }

    protected override void Effect()
    {
        player.MAX_HEALTH += 1;
        player.maxHealth += 1;
        player.AddHealth(1);
    }
}
