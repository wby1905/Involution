using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Property : Item
{
    public static int ID { get; set; }
    public PropInformation propInformation;

    private void Start()
    {
        SetID();
        propInformation.Name = transform.name;
        propInformation.Sprite = GetComponent<SpriteRenderer>().sprite;
    }
    protected override bool IsTrigger()
    {
        return true;
    }

    protected override void After()
    {
        // player.AddItemInformation(this);
        Destroy(gameObject);
    }
    protected abstract void SetID();

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && IsTrigger() && canTriggerWithPlayer)
        {
            Effect();
            UI.UpdateStatus();
            After();
        }
    }
}
