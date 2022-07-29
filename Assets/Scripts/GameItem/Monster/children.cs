using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class children : Monster
{
    public float speed;
    public float changeTime = 3.0f;
    int gunIdx;
    float timer;
    Rigidbody2D rigidBody2D;
    Animator animator;
    public Transform target;

    float findRadius = 10.0f;
    float minRadius = 3.0f;

    float randomWalkTime = 1.0f;
    public bool randomWalk;

    Vector2 direction;
    Vector2 prePosition;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        direction.x = UnityEngine.Random.Range(0f, 1f);
        direction.y = UnityEngine.Random.Range(0f, 1f);
        direction.Normalize();
        animator = GetComponent<Animator>();
        prePosition = transform.position;
        randomWalk = true;

        gunIdx = UnityEngine.Random.Range(0, guns.Length);
        guns[gunIdx].SetActive(true);
    }

    protected override void Awake()
    {
        base.Awake();
        beKnockBackSeconds = 1.0f;
        beKnockBackLength = 1.0f;
    }
    protected override void Update()
    {
        base.Update();
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            ChangeDirection(-0.5f, 0.5f);
            timer = changeTime;
        }

        if (!randomWalk)
        {
            randomWalkTime -= Time.deltaTime;
            if (randomWalkTime < 0)
            {
                randomWalk = true;
            }
            Shoot();
        }

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
        Track();
        Vector2 position = rigidBody2D.position;
        position.x = position.x + direction.x * Time.deltaTime * speed;
        position.y = position.y + direction.y * Time.deltaTime * speed;
        rigidBody2D.MovePosition(position);
        if (position == prePosition)
        {
            Debug.Log("reverse");
            ChangeDirection(-1.25f, -0.75f);
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
    void OnCollisionEnter2D(Collision2D other)
    {
        // if (other.collider.tag!="Player"){
        ChangeDirection(-0.5f, 0.5f);
        //}
    }

    void Track()
    {
        Vector2 position = rigidBody2D.position;
        Vector2 target = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector2 site = target - position;
        if (site.magnitude <= findRadius && site.magnitude >= minRadius && randomWalk)
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
            randomWalk = false;
        }
    }

}
