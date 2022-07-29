using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : GameItem, IsAttackable
{
    public Slider hpSlider;

    public override GameItemType gameItemType { get { return GameItemType.Monster; } }
    public GameObject[] guns = new GameObject[5];
    protected bool isLive = true;
    public float HP;
    protected float maxHP;
    protected float beKnockBackSeconds;
    protected float beKnockBackLength;

    protected override void Awake()
    {
        base.Awake();
        ActiveHPSlider();
    }
    protected virtual void Update()
    {
        UpdateHPSlider();
    }
    public virtual void BeAttacked(float damage, Vector2 direction, float forceMultiple = 1f)
    {
        if (!isLive) { return; }
        HP -= damage;
        direction.Normalize();
        if (HP <= 0)
        {
            hpSlider.gameObject.SetActive(false);
            StartCoroutine(Death());
        }
        else { StartCoroutine(knockBackCoroutine(direction * forceMultiple)); }
    }

    protected virtual IEnumerator Death()
    {

        isLive = false;
        transform.GetComponent<Collider2D>().enabled = false;
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return null;
        Destroy(gameObject);
    }
    protected IEnumerator knockBackCoroutine(Vector2 force)
    {
        float timeleft = beKnockBackSeconds;
        while (timeleft > 0)
        {
            transform.Translate(force * beKnockBackLength * Time.deltaTime / beKnockBackSeconds);
            timeleft -= Time.deltaTime;
            yield return null;
        }
    }
    protected void ActiveHPSlider()
    {
        maxHP = HP;
        hpSlider.gameObject.SetActive(true);
        hpSlider.value = 1;
    }

    protected virtual void UpdateHPSlider()
    {
        hpSlider.value = Mathf.Lerp(hpSlider.value, HP / maxHP, Time.deltaTime * 5);
    }
}