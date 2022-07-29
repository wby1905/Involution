using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IsAttackable
{
    public float SPEED = 3.0f;

    //状态
    public int MAX_HEALTH;
    [HideInInspector]
    public int maxHealth;
    private float health;
    public float Health
    {
        get { return health; }
        private set { health = Mathf.Clamp(value, 0, maxHealth); }
    }
    [HideInInspector]
    public int coins;
    public int COIN;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public bool isAbleSwitch;
    public bool isLive;
    public bool isControllable = true;
    bool isInvincible;
    [HideInInspector]
    public int bulletNum = 1;
    public float shotTiming;
    public GunType initial;

    [Header("自身")]
    float horizontal;
    float vertical;
    Rigidbody2D rigidbody2d;
    public GameObject Head;
    public GameObject Body;
    SpriteRenderer bodyRenderer;
    SpriteRenderer headRenderer;
    private Animator headAnimator;
    private Animator bodyAnimator;
    private Animator wholeAnimator;
    Vector2 lookDirection = new Vector2(1, 0);
    public GameObject[] guns;
    private bool[] isGunAvailable;
    private int gunNum;//当前使用的枪械的下标

    [Header("其他")]
    UIManager UI;
    Level level;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        headAnimator = Head.GetComponent<Animator>();
        bodyAnimator = Body.GetComponent<Animator>();
        wholeAnimator = GetComponent<Animator>();
        headRenderer = Head.GetComponent<SpriteRenderer>();
        bodyRenderer = Body.GetComponent<SpriteRenderer>();
    }

    // 在第一次帧更新之前调用 Start
    void Start()
    {
        level = GameManager.Instance.level;
        UI = UIManager.Instance;

        PlayerInitialize();

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.O))
        {
            PlayerDeath();
        }
        if (Input.GetKey(KeyCode.P))
        {
            PlayerInitialize();
        }
        if (Input.GetKey(KeyCode.H))
        {
            BeAttacked(1, -rigidbody2d.velocity);
        }
        if (!isControllable) { return; }
        UpdateMovement();
        UpdateControl();
        if (isAbleSwitch)
        {
            SwitchGun();
        }
    }

    void UpdateControl()
    {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            LaunchBullet(KeyCode.UpArrow);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            LaunchBullet(KeyCode.DownArrow);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            LaunchBullet(KeyCode.LeftArrow);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            LaunchBullet(KeyCode.RightArrow);
        }
        else
        {
            Head.GetComponent<Animator>().SetBool("IsShooting", false);
        }
    }

    /// <summary>
    /// 按下按键发射子弹
    /// </summary>
    void LaunchBullet(KeyCode key)
    {
        shotTiming += Time.deltaTime;

        headAnimator.SetBool("IsShooting", true);
        Gun Gun = guns[gunNum].GetComponent<Gun>();

        if (key == KeyCode.UpArrow)
        {
            headAnimator.Play("HeadUp");
            Gun.shootDirection = new Vector2(0, 1);
        }
        else if (key == KeyCode.DownArrow)
        {
            headAnimator.Play("HeadDown");
            Gun.shootDirection = new Vector2(0, -1);
        }
        else if (key == KeyCode.LeftArrow)
        {
            headAnimator.Play("HeadLeft");
            Gun.shootDirection = new Vector2(-1, 0);
        }
        else if (key == KeyCode.RightArrow)
        {
            headAnimator.Play("HeadRight");
            Gun.shootDirection = new Vector2(1, 0);
        }
    }

    public void SwitchToGun(GunType gunType)
    {
        isGunAvailable[(int)gunType] = true;
        guns[gunNum].SetActive(false);
        gunNum = (int)gunType;
        guns[gunNum].GetComponent<Gun>().isPlayer = true;
        guns[gunNum].GetComponent<Gun>().NUM = bulletNum;
        guns[gunNum].SetActive(true);
    }

    public Transform GetGunNow()
    {
        return guns[gunNum].transform;
    }
    void SwitchGun()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            guns[gunNum].SetActive(false);
            do
            {
                if (--gunNum < 0)
                {
                    gunNum = guns.Length - 1;
                }
            } while (!isGunAvailable[gunNum]);
            guns[gunNum].GetComponent<Gun>().NUM = bulletNum;
            guns[gunNum].SetActive(true);
            UI.UpdateStatus();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            guns[gunNum].SetActive(false);
            do
            {
                if (++gunNum == guns.Length)
                {
                    gunNum = 0;
                }
            } while (!isGunAvailable[gunNum]);
            guns[gunNum].GetComponent<Gun>().NUM = bulletNum;
            guns[gunNum].SetActive(true);
            UI.UpdateStatus();
        }
    }

    void UpdateMovement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
            headAnimator.SetBool("IsWalking", true);
            bodyAnimator.SetBool("IsWalking", true);
        }
        else
        {
            lookDirection.Set(0, 0);
            bodyAnimator.SetBool("IsWalking", false);
            headAnimator.SetBool("IsWalking", false);
        }
        bodyAnimator.SetFloat("MoveX", lookDirection.x);
        bodyAnimator.SetFloat("MoveY", lookDirection.y);
        headAnimator.SetFloat("MoveX", lookDirection.x);
        headAnimator.SetFloat("MoveY", lookDirection.y);
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void PlayerPause()
    {
        isControllable = false;
        SPEED = speed;
        speed = 0;
        rigidbody2d.velocity = Vector2.zero;
        headAnimator.speed = 0;
        bodyAnimator.speed = 0;
    }
    public void PlayerResume()
    {
        isControllable = true;
        speed = SPEED;
        headAnimator.speed = 1;
        bodyAnimator.speed = 1;
    }

    public void PlayerDeath()
    {
        isLive = false;
        isControllable = false;
        wholeAnimator.SetBool("IsDeath", true);
        PlayerPause();
    }

    /// <summary>
    /// 状态初始化
    /// </summary>
    public void PlayerInitialize()
    {
        maxHealth = MAX_HEALTH;
        health = maxHealth;

        speed = SPEED;
        coins = COIN;
        isGunAvailable = new bool[guns.Length];
        SwitchToGun(initial);
        shotTiming = 0;

        isAbleSwitch = false;
        isLive = true;
        isControllable = true;
        isInvincible = false;

        wholeAnimator.ResetTrigger("IsDeath");
        wholeAnimator.Play("Respawn");
        PlayerResume();

        UI.PlayerUIInitialize();
    }

    /// <summary>
    /// 加血
    /// </summary>
    /// <param name="health"></param>
    /// <param name="type"></param>
    /// <param name="maxHealth"></param>
    public void AddHealth(int health)
    {
        Health += health;
        UI.hp.UpdateHP();
    }
    /// <summary>
    /// 扣血
    /// </summary>
    /// <param name="damage"></param>
    public void ReduceHealth(float damage)
    {
        Health -= damage;
        UI.hp.UpdateHP();
        if (Health == 0) { PlayerDeath(); }
    }

    public void Accelerate(int spd)
    {
        speed += spd;
    }

    /// <summary>
    /// 受到攻击
    /// </summary>
    /// <param name="damage"></param>
    public void BeAttacked(float damage, Vector2 direction, float forceMultiple = 1)
    {
        if (isInvincible || !isLive) { return; }
        ReduceHealth(damage);
        if (isLive)
        {
            StartCoroutine(knockBackCoroutine(direction * forceMultiple));
            StartCoroutine(Invincible());
        }
    }

    /// <summary>
    /// 被击退效果
    /// </summary>
    /// <param name="force"></param>
    /// <returns></returns>
    IEnumerator knockBackCoroutine(Vector2 force)
    {


        float length = 0.3f;
        float overTime = 0.1f;
        float timeleft = overTime;
        while (timeleft > 0)
        {
            //overTime时间内移动direction * length的距离
            transform.Translate(force * length * Time.deltaTime / overTime);
            timeleft -= Time.deltaTime;
            yield return null;
        }


    }

    /// <summary>
    /// 进入无敌状态并闪烁
    /// </summary>
    IEnumerator Invincible()
    {
        isInvincible = true;
        Color red = new Color(1, 0.2f, 0.2f, 1);

        float time = 0;//计时
        float flashCD = 0;//闪烁计时

        while (time < 1f)
        {
            time += Time.deltaTime;
            flashCD += Time.deltaTime;
            if (flashCD > 0)
            {
                if (bodyRenderer.color == Color.white)
                {
                    bodyRenderer.color = red;
                    headRenderer.color = red;
                }
                else if (bodyRenderer.color == red)
                {
                    bodyRenderer.color = Color.white;
                    headRenderer.color = Color.white;
                }
                flashCD -= 0.13f;
            }
            yield return null;
        }
        bodyRenderer.color = Color.white;
        headRenderer.color = Color.white;
        isInvincible = false;
    }

    /// <summary>
    /// 判断碰撞体并移动到下一个房间
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        //移动到下一个房间
        if (isControllable && collider.transform.CompareTag("Door"))
        {
            string flag = collider.transform.parent.gameObject.name;
            if (flag == "DoorUp")
            {
                level.MoveToNextRoom(Vector2.up);
            }
            else if (flag == "DoorDown")
            {
                level.MoveToNextRoom(Vector2.down);
            }
            else if (flag == "DoorLeft")
            {
                level.MoveToNextRoom(Vector2.left);
            }
            else if (flag == "DoorRight")
            {
                level.MoveToNextRoom(Vector2.right);
            }
        }
    }

}
