using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SBsuizhiyuan : Monster
{



    public float speed;
    public float generateTime = 60.0f;
    public GameObject[] monsters;
    float timer;
    Rigidbody2D rigidBody2D;
    Animator animator;
    public Transform Target;

    int changeCount;

    float findRadius = 4.0f;

    public float damage;
    public float knockback;
    int gunIdx;
    public bool tracking;
    bool lookAround;
    Vector2 direction;
    Vector2 prePosition;
    // Start is called before the first frame update
    void Start()
    {
        hpSlider = UIManager.Instance.bossHp;
        rigidBody2D = GetComponent<Rigidbody2D>();
        direction.x = UnityEngine.Random.Range(0f, 1f);
        direction.y = UnityEngine.Random.Range(0f, 1f);
        direction.Normalize();
        timer = generateTime;
        changeCount = (int)(1 / Time.deltaTime) + 1;
        animator = GetComponent<Animator>();
        prePosition = transform.position;
        tracking = false;
        lookAround = false;
        gunIdx = UnityEngine.Random.Range(0, guns.Length);
        guns[gunIdx].SetActive(true);
    }


    protected override void Update()
    {
        base.Update();
        timer -= Time.deltaTime;
        changeCount -= 1;
        if (tracking) { Shoot(); }
        if (changeCount == 0)
        {
            ChangeDirection(-0.25f, 0.25f);
            changeCount = (int)(1 / Time.deltaTime) + 1;
        }
        if (timer < 0)
        {
            // 召唤小怪
            GameObject instance = (GameObject)Instantiate(monsters[UnityEngine.Random.Range(0, monsters.Length)], transform.parent);
            lookAround = false;
            timer = generateTime;
        }
        else if (timer < 1 && !tracking)
        {
            lookAround = true;
        }


    }
    public void SwitchGun()
    {
        guns[gunIdx].SetActive(false);
        gunIdx = UnityEngine.Random.Range(0, guns.Length);
        guns[gunIdx].SetActive(true);
        Debug.Log("switchgun idx: " + gunIdx);
    }
    void Shoot()
    {
        if (!lookAround)
        {
            Gun Gun = guns[gunIdx].GetComponent<Gun>();
            Gun.shootDirection = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
            Gun.shootDirection.y += 0.5f;
        }
    }
    protected override void Awake()
    {
        hpSlider = UIManager.Instance.bossHp;
        base.Awake();
        beKnockBackSeconds = 0.5f;
        beKnockBackLength = 1.0f;
    }
    void ChangeDirection(float low, float high)
    {
        Vector2 position = rigidBody2D.position;
        Vector2 target = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector2 site = target - position;
        float site_angle = Mathf.Atan2(site.y, site.x);
        // 随机选择一个角度进行旋转
        float angle = site_angle + UnityEngine.Random.Range(low * Mathf.PI, high * Mathf.PI);
        direction.x = Mathf.Cos(angle);
        direction.y = Mathf.Sin(angle);
        direction.Normalize();
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

    }
    void FixedUpdate()
    {
        Track();
        if (lookAround)
        {
            animator.SetBool("lookAround", true);
        }
        else
        {
            animator.SetBool("lookAround", false);
            Vector2 position = rigidBody2D.position;
            position.x = position.x + direction.x * Time.deltaTime * speed;
            position.y = position.y + direction.y * Time.deltaTime * speed;
            rigidBody2D.MovePosition(position);
            if (position == prePosition)
            {
                Debug.Log("reverse");
                ChangeDirection(-0.25f, 0.25f);
                // if (direction.x>0 && direction.y<0){
                //     direction.y = -direction.y;
                //     animator.SetFloat("MoveY",direction.y);
                // }
                // else if(direction.x<0 && direction.y>0){
                //     direction.x = -direction.x;
                //     animator.SetFloat("MoveX",direction.x);
                // }

            }
            prePosition = position;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag != "Player")
        {
            ChangeDirection(-0.25f, 0.25f);
        }
    }

    void Track()
    {
        Vector2 position = rigidBody2D.position;
        Vector2 target = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector2 site = target - position;
        if (site.magnitude <= findRadius)
        {
            tracking = true;
            int lay = LayerMask.NameToLayer("wall");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, site, site.magnitude, 1 << lay);
            if (hit.collider == null)
            {
                site.Normalize();
                direction = site;
                animator.SetFloat("MoveX", direction.x);
                animator.SetFloat("MoveY", direction.y);
            }

        }
        else
        {
            tracking = false;
        }
    }
}
