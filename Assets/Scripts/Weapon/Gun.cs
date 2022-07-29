using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float interval; //射击间隔时间
    public GameObject bulletPrefab; //子弹的预制体
    public GameObject shellPrefab; //弹壳的预制体

    protected Transform muzzlePos; //枪口位置
    protected Transform shellPos; //弹仓位置
    protected Vector2 direction;  //发射方向
    protected float timer = 1.0f; //计时器
    protected float t;
    protected float flipY; //记录LocalScaleY值，用于翻转枪械
    protected Animator animator; //获取动画器
    [HideInInspector]
    public Vector2 shootDirection = new Vector2(1, 0);
    public int NUM;
    [HideInInspector]
    public bool isPlayer = false;

    protected Player player;

    private void Awake()
    {
        player = GameManager.Instance.player;
    }
    protected virtual void OnEnable()
    {
        animator = GetComponent<Animator>(); //获取动画器
        muzzlePos = transform.Find("Muzzle"); //获取子物体位置
        shellPos = transform.Find("BulletShell"); //获取子物体位置
        flipY = transform.localScale.y;
        t = interval;
        if (!isPlayer)
        {
            gameObject.layer = 22;
            muzzlePos.gameObject.layer = 22;
            NUM = Mathf.Clamp(GameManager.Instance.depth + NUM, 1, 5);
        }
        else
        {
            NUM = player.bulletNum;
        }
    }

    protected virtual void Update()
    {
        if (isPlayer)
        {
            if (!player.isControllable) { return; }
            direction = shootDirection;
            transform.right = direction; //让枪口的局部右方向始终等于该方向
            if (player.shotTiming >= interval)
            {
                Shoot();//发射
                player.shotTiming = 0;
            }
        }
        else
        {
            direction = shootDirection;
            transform.right = direction;
            t -= Time.deltaTime;
            if (t < 0)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    t = interval;
                    timer = 1.0f;

                    if (transform.parent.parent.name.Contains("大学生"))
                    {
                        if (transform.parent.parent.GetComponent<underGraduate>().randomWalk)
                        {
                            Shoot();
                            transform.parent.parent.GetComponent<underGraduate>().SwitchGun();
                        }
                    }
                    else if (transform.parent.parent.name.Contains("SB隋智源"))
                    {
                        if (transform.parent.parent.GetComponent<SBsuizhiyuan>().tracking)
                        {
                            Shoot();
                            transform.parent.parent.GetComponent<SBsuizhiyuan>().SwitchGun();
                        }
                    }
                    else if (transform.parent.parent.name.Contains("小学生"))
                    {

                        Shoot();
                    }
                }

            }
        }
    }
    protected virtual void Shoot()
    {
        Fire();
    }

    protected virtual void Fire()
    {
        animator.SetTrigger("Shoot");

        // GameObject bullet = Instantiate(bulletPrefab, muzzlePos.position, Quaternion.identity);
        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab); //从对象池中获取到子弹的预制体
        bullet.transform.position = muzzlePos.position;//

        float angel = Random.Range(-5f, 5f);//产生一个小的旋转
        bullet.GetComponent<Bullet>().SetSpeed(Quaternion.AngleAxis(angel, Vector3.forward) * direction);
        bullet.GetComponent<Bullet>().isPlayerFlag = isPlayer;
        // Instantiate(shellPrefab, shellPos.position, shellPos.rotation);
        GameObject shell = ObjectPool.Instance.GetObject(shellPrefab); //获取弹壳的预制体
        shell.transform.position = shellPos.position; //将位置设置成弹仓位置
        shell.transform.rotation = shellPos.rotation; //旋转设置成弹仓的旋转

    }
}
