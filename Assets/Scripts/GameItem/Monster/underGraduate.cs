using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class underGraduate : Monster
{
    public float speed;
    Rigidbody2D rigidBody2D;
    Animator animator;
    public Transform Target;

    float findRadius = 7.0f;
    float minRadius = 2.0f;
    public bool randomWalk = false;
    public float damage;
    public float knockback;
    int gunIdx;

    float changeTime = 1.0f;
    float timer;
    Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        direction = new Vector2(0, 0);
        animator = GetComponent<Animator>();
        timer = changeTime;
        gunIdx = UnityEngine.Random.Range(0, guns.Length);
        guns[gunIdx].SetActive(true);
    }

    protected override void Update()
    {
        base.Update();
        if (randomWalk)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = changeTime;
                Track();
            }
            Shoot();

        }
        else
        {
            RandomWalk();
        }

    }
    public void SwitchGun()
    {
        guns[gunIdx].SetActive(false);
        gunIdx = UnityEngine.Random.Range(0, guns.Length);
        guns[gunIdx].SetActive(true);
    }
    void Shoot()
    {
        if (randomWalk)
        {
            Gun Gun = guns[gunIdx].GetComponent<Gun>();
            Gun.shootDirection = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
            Gun.shootDirection.y += 0.5f;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        beKnockBackSeconds = 1.0f;
        beKnockBackLength = 1.0f;
    }
    void ChangeDirection(float low, float high)
    {
        // 随机选择一个角度进行旋转
        float angle = UnityEngine.Random.Range(low * Mathf.PI, high * Mathf.PI);
        direction.x = (direction.x) * Mathf.Cos(angle) - direction.y * Mathf.Sin(angle);
        direction.y = (direction.x) * Mathf.Sin(angle) + direction.y * Mathf.Cos(angle);
        direction.Normalize();
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

    }
    void FixedUpdate()
    {
        Vector2 position = rigidBody2D.position;
        position.x = position.x + direction.x * Time.deltaTime * speed;
        position.y = position.y + direction.y * Time.deltaTime * speed;
        rigidBody2D.MovePosition(position);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        ChangeDirection(-0.5f, 0.5f);
        if (other.collider.tag == "Player")
        {
            other.collider.GetComponent<Player>().BeAttacked(damage, other.collider.transform.position - transform.position, knockback);
        }
    }


    void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.tag == "Player")
        {
            other.collider.GetComponent<Player>().BeAttacked(damage, other.collider.transform.position - transform.position, knockback);
        }
    }
    void RandomWalk()
    {
        Vector2 target = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector2 position = rigidBody2D.position;
        Vector2 site = target - position;
        if (site.magnitude < findRadius || HP < maxHP)
        {
            if (!randomWalk)
            {
                randomWalk = true;
                animator.SetBool("RandomWalk", true);
            }
        }

    }
    void Track()
    {
        Vector2 position = rigidBody2D.position;
        Vector2 target = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector2 site = target - position;
        if (site.magnitude <= findRadius && site.magnitude >= minRadius)
        {

            int lay = LayerMask.NameToLayer("wall");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, site, site.magnitude, 1 << lay);
            if (hit.collider == null)
            {
                float site_angle = Mathf.Atan2(site.y, site.x);
                // 随机选择一个角度进行旋转
                float angle = site_angle + UnityEngine.Random.Range(-0.25f * Mathf.PI, 0.25f * Mathf.PI);
                direction.x = Mathf.Cos(angle);
                direction.y = Mathf.Sin(angle);
                direction.Normalize();
                animator.SetFloat("MoveX", direction.x);
                animator.SetFloat("MoveY", direction.y);
            }

        }
        else if (site.magnitude < minRadius)
        {
            direction.x = UnityEngine.Random.Range(-1.0f, 1.0f);
            direction.y = UnityEngine.Random.Range(-1.0f, 1.0f);
            direction.Normalize();
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
        }
        else
        {
            randomWalk = false;
            direction.x = UnityEngine.Random.Range(-1.0f, 1.0f);
            direction.y = UnityEngine.Random.Range(-1.0f, 1.0f);
            direction.Normalize();
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);

        }
    }
}
