using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Gun
{
    //private GameObject[] bulletList = new GameObject[5];
    protected Transform[] muzzleList = new Transform[5];
    public GameObject Effect;
    private GameObject[] effect = new GameObject[5];
    private int num = 3;

    public float damage;
    public float knockback;

    protected void Start()
    {
        num = NUM;
        muzzleList[0] = transform.Find("Muzzle");
        muzzleList[1] = transform.Find("Muzzle2"); //获取子物体位置
        muzzleList[2] = transform.Find("Muzzle3");
        muzzleList[3] = transform.Find("Muzzle4");
        muzzleList[4] = transform.Find("Muzzle5");

        for (int i = 0; i < 5; i++)
        {
            GameObject temp_effect = Instantiate(Effect, transform);
            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
            effect[i] = temp_effect;
            effect[i].SetActive(false);
            //bulletList[i]=temp_bullet;
        }
        if (!isPlayer)
        {
            gameObject.layer = 22;
            damage = 0.5f;
            knockback = 0.03f;
        }

    }


    protected override void Fire()
    {
        animator.SetTrigger("Shoot");

        float Angle = 0.05f * Mathf.PI;
        Vector2 direct = new Vector2(0, 0);
        int medium = num / 2;

        for (int i = 0; i < num; i++)
        {
            effect[i].SetActive(true);

            if (num % 2 == 1)
            {
                direct.x = (direction.x) * Mathf.Cos((i - medium) * Angle) - direction.y * Mathf.Sin((i - medium) * Angle);
                direct.y = (direction.x) * Mathf.Sin((i - medium) * Angle) + direction.y * Mathf.Cos((i - medium) * Angle);
                direct.Normalize();

            }
            else
            {
                direct.x = (direction.x) * Mathf.Cos((i - medium) * Angle + 0.5f * Angle) - direction.y * Mathf.Sin((i - medium) * Angle + 0.5f * Angle);
                direct.y = (direction.x) * Mathf.Sin((i - medium) * Angle + 0.5f * Angle) + direction.y * Mathf.Cos((i - medium) * Angle + 0.5f * Angle);
                direct.Normalize();
            }

            RaycastHit2D hit2D = Physics2D.Raycast(muzzleList[i].position, direct, 30);

            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
            LineRenderer tracer = bullet.GetComponent<LineRenderer>();
            tracer.SetPosition(0, muzzleList[i].position);
            tracer.SetPosition(1, hit2D.point);
            bullet.gameObject.layer = gameObject.layer;
            bullet.gameObject.GetComponent<BulletTracer>().layerNow = gameObject.layer;

            GameObject shell = ObjectPool.Instance.GetObject(shellPrefab);
            shell.transform.position = shellPos.position;
            shell.transform.rotation = shellPos.rotation;
            if (isPlayer && hit2D.collider.tag == "Monster")
            {
                hit2D.collider.GetComponent<Monster>().BeAttacked(damage, new Vector2(muzzlePos.position.x, muzzlePos.position.y), knockback);
            }
            if (!isPlayer && hit2D.collider.tag == "Player")
            {
                hit2D.collider.GetComponent<Player>().BeAttacked(damage, new Vector2(muzzlePos.position.x, muzzlePos.position.y), knockback);
            }
            effect[i].transform.position = hit2D.point;
            effect[i].transform.forward = -direction;
            effect[i].SetActive(false);


        }



    }

    IEnumerator Wait(int i)
    {
        yield return new WaitForSeconds(0.1f);

    }


}
