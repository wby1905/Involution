using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasergun : Gun
{
    private GameObject Effect;
    private GameObject[] effect = new GameObject[10];
    public GameObject Muzzle;
    private LineRenderer[] laser = new LineRenderer[10]; //用于激活射线
    private int num;
    private bool isShooting;//持续性射击
    public float shootingTime = 10.0f;
    public float stayTime = 1.0f;
    public float time1;
    public float time2;
    private float laserAngle;
    public float damage;
    public float knockback;
    private Color[] colorList = new Color[5] { Color.red, Color.blue, Color.yellow, Color.magenta, Color.green };

    protected override void Update()
    {
        if (isPlayer)
        {
            if (!player.isControllable)
            {
                isShooting = false;
                for (int i = 0; i < num; i++)
                {
                    laser[i].enabled = false;
                    effect[i].SetActive(false);
                }
            }
        }
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
                    time1 -= Time.deltaTime;
                    if (time1 < 0)
                    {
                        time2 -= Time.deltaTime;
                        if (time2 < 0)
                        {
                            time1 = shootingTime;
                            time2 = stayTime;
                            transform.parent.parent.GetComponent<SBsuizhiyuan>().SwitchGun();
                        }
                    }
                }
                else
                {
                    isShooting = false;
                    for (int i = 0; i < num; i++)
                    {
                        laser[i].enabled = false;
                        effect[i].SetActive(false);
                    }
                }
            }
            else if (transform.parent.parent.name == "小学生")
            {

                Shoot();
            }


        }
    }
    private void Start()
    {
        if (!isPlayer)
        {
            Muzzle.gameObject.layer = 22;
            damage = 0.5f;
            knockback = 0;
        }

        num = NUM;
        Effect = transform.Find("Effect").gameObject;
        for (int i = 0; i < 10; i++)
        {
            GameObject temp = Instantiate(Muzzle, transform);
            laser[i] = temp.transform.GetComponent<LineRenderer>();
            laser[i].enabled = false;

            effect[i] = Instantiate(Effect, transform);
        }
    }

    protected override void Shoot()
    {
        direction = shootDirection;
        transform.right = direction;
        if (isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
           Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))//按键按下射击
            {
                isShooting = true;
                for (int i = 0; i < num; i++)
                {
                    laser[i].enabled = true;
                    effect[i].SetActive(true);
                }
            }
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) ||
            Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))//按键抬起结束射击
            {
                isShooting = false;
                for (int i = 0; i < num; i++)
                {
                    laser[i].enabled = false;
                    effect[i].SetActive(false);
                }
            }
            animator.SetBool("Shoot", isShooting);
        }

        else
        {
            isShooting = true;
            for (int i = 0; i < num; i++)
            {
                laser[i].enabled = true;
                effect[i].SetActive(true);
            }
            animator.SetBool("Shoot", isShooting);
        }

        if (isShooting)
        {
            Fire();
        }
    }

    protected override void Fire()
    {
        // Debug.DrawLine(muzzlePos.position, hit2D.point);
        laserAngle = 0.05f * Mathf.PI;
        Vector2 direct = new Vector2(0, 0);

        int medium = num / 2;
        if (num % 2 == 1)
            for (int i = 0; i < num; i++)
            {
                direct.x = (direction.x) * Mathf.Cos((i - medium) * laserAngle) - direction.y * Mathf.Sin((i - medium) * laserAngle);
                direct.y = (direction.x) * Mathf.Sin((i - medium) * laserAngle) + direction.y * Mathf.Cos((i - medium) * laserAngle);
                direct.Normalize();
                laser[i].SetColors(colorList[i], colorList[i]);
                RaycastHit2D hit2D = Physics2D.Raycast(muzzlePos.position, direct, 30);
                laser[i].SetPosition(0, muzzlePos.position);
                laser[i].SetPosition(1, hit2D.point);

                effect[i].transform.position = hit2D.point;
                effect[i].transform.forward = -direct;
                if (isPlayer && hit2D.collider.tag == "Monster")
                {
                    hit2D.collider.GetComponent<Monster>().BeAttacked(damage, new Vector2(muzzlePos.position.x, muzzlePos.position.y), knockback);
                }
                if (!isPlayer && hit2D.collider.tag == "Player")
                {
                    hit2D.collider.GetComponent<Player>().BeAttacked(damage, new Vector2(muzzlePos.position.x, muzzlePos.position.y), knockback);

                }
            }
        else
        {
            for (int i = 0; i < medium; i++)
            {
                if (i == medium - 1) laserAngle = 0.5f * laserAngle;

                direct.x = (direction.x) * Mathf.Cos((i - medium) * laserAngle) - direction.y * Mathf.Sin((i - medium) * laserAngle);
                direct.y = (direction.x) * Mathf.Sin((i - medium) * laserAngle) + direction.y * Mathf.Cos((i - medium) * laserAngle);
                direct.Normalize();
                laser[i].SetColors(colorList[i], colorList[i]);
                RaycastHit2D hit2D = Physics2D.Raycast(muzzlePos.position, direct, 30);
                laser[i].SetPosition(0, muzzlePos.position);
                laser[i].SetPosition(1, hit2D.point);

                effect[i].transform.position = hit2D.point;
                effect[i].transform.forward = -direct;
                if (isPlayer && hit2D.collider.tag == "Monster")
                {
                    hit2D.collider.GetComponent<Monster>().BeAttacked(damage, new Vector2(muzzlePos.position.x, muzzlePos.position.y), knockback);
                }
                if (!isPlayer && hit2D.collider.tag == "Player")
                {
                    Debug.Log("python damage");
                    hit2D.collider.GetComponent<Player>().BeAttacked(damage, new Vector2(muzzlePos.position.x, muzzlePos.position.y), knockback);

                }
            }


            for (int i = medium; i < num; i++)
            {
                direct.x = (direction.x) * Mathf.Cos((i - medium + 1) * laserAngle) - direction.y * Mathf.Sin((i - medium + 1) * laserAngle);
                direct.y = (direction.x) * Mathf.Sin((i - medium + 1) * laserAngle) + direction.y * Mathf.Cos((i - medium + 1) * laserAngle);
                direct.Normalize();
                laser[i].SetColors(colorList[i], colorList[i]);
                RaycastHit2D hit2D = Physics2D.Raycast(muzzlePos.position, direct, 30);
                laser[i].SetPosition(0, muzzlePos.position);
                laser[i].SetPosition(1, hit2D.point);

                effect[i].transform.position = hit2D.point;
                effect[i].transform.forward = -direct;
                if (i == medium) laserAngle = 2f * laserAngle;
                if (isPlayer && hit2D.collider.tag == "Monster")
                {
                    hit2D.collider.GetComponent<Monster>().BeAttacked(damage, new Vector2(muzzlePos.position.x, muzzlePos.position.y), knockback);
                }
                if (!isPlayer && hit2D.collider.tag == "Player")
                {
                    Debug.Log("python damage");
                    hit2D.collider.GetComponent<Player>().BeAttacked(damage, new Vector2(muzzlePos.position.x, muzzlePos.position.y), knockback);

                }
            }

        }
    }
}
