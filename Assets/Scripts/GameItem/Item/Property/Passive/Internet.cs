using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Internet : Property
{
    protected override void SetID()
    {
        ID = 4;
    }

    protected override void Effect()
    {
        player.Accelerate(3);
    }
}
