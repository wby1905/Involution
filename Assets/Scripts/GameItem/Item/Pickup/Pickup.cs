using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : Item
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && IsTrigger() && canTriggerWithPlayer)
        {
            Effect();
            // UI.attributes.UpDateAttributes();
            After();
        }
    }
}
