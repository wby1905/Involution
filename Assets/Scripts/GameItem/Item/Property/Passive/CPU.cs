using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : Property
{
    protected override void SetID()
    {
        ID = 3;
    }

    protected override void Effect()
    {
        player.isAbleSwitch = true;
    }
}
