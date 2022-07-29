using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    public GameObject bulletPrefab2; //子弹的预制体
    private int[] choose = new int[5] { 1, 1, 1, 1, 1 };
    private Color[] colorList = new Color[5] { Color.white, Color.blue, Color.green, Color.magenta, Color.grey };
    private int num = 1;
    protected Transform[] muzzleList = new Transform[5];

    protected override void OnEnable()
    {
        base.OnEnable();
        num = NUM;
        muzzleList[0] = transform.Find("Muzzle");
        muzzleList[1] = transform.Find("Muzzle2"); //获取子物体位置
        muzzleList[2] = transform.Find("Muzzle3");
        muzzleList[3] = transform.Find("Muzzle4");
        muzzleList[4] = transform.Find("Muzzle5");
    }

    protected override void Fire()
    {
        animator.SetTrigger("Shoot");

        // GameObject bullet = Instantiate(bulletPrefab, muzzlePos.position, Quaternion.identity);
        float angel = 10f;
        int medium = num / 2;

        for (int i = 0; i < num; i++)
        {

            if (choose[i] == 1)
            {
                GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab); //获取到子弹的预制体

                bullet.GetComponent<SpriteRenderer>().color = colorList[i];
                bullet.GetComponent<Bullet>().isPlayerFlag = isPlayer;

                bullet.transform.position = muzzleList[i].position;//
                if (num % 2 == 1)
                    bullet.GetComponent<Bullet>().SetSpeed(Quaternion.AngleAxis((i - medium) * angel, Vector3.forward) * direction);
                else
                    bullet.GetComponent<Bullet>().SetSpeed(Quaternion.AngleAxis((i - medium) * angel + 0.5f * angel, Vector3.forward) * direction);
                choose[i] = 0;

            }
            else
            {
                GameObject bullet2 = ObjectPool.Instance.GetObject(bulletPrefab2);

                bullet2.GetComponent<SpriteRenderer>().color = colorList[i];
                bullet2.GetComponent<Bullet>().isPlayerFlag = isPlayer;

                bullet2.transform.position = muzzleList[i].position;
                if (num % 2 == 1)
                    bullet2.GetComponent<Bullet>().SetSpeed(Quaternion.AngleAxis((i - medium) * angel, Vector3.forward) * direction);
                else
                    bullet2.GetComponent<Bullet>().SetSpeed(Quaternion.AngleAxis((i - medium) * angel + 0.5f * angel, Vector3.forward) * direction);
                choose[i] = 1;

            }
        }
        // Instantiate(shellPrefab, shellPos.position, shellPos.rotation);
        GameObject shell = ObjectPool.Instance.GetObject(shellPrefab); //获取弹壳的预制体
        shell.transform.position = shellPos.position; //将位置设置成弹仓位置
        shell.transform.rotation = shellPos.rotation; //旋转设置成弹仓的旋转
        GameObject shell1 = ObjectPool.Instance.GetObject(shellPrefab); //获取弹壳的预制体
        shell1.transform.position = shellPos.position; //将位置设置成弹仓位置
        shell1.transform.rotation = shellPos.rotation; //旋转设置成弹仓的旋转
    }
}
