using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teenager : Monster
{
    public float speed;
    public float changeTime = 3.0f;
    float timer;
    Rigidbody2D rigidBody2D;
    Animator animator;
    public Transform target;

    float findRadius = 5.0f;
    float minRadius = 3.0f;

    float randomWalkTime = 1.0f;
    bool randomWalk;
    public float damage;
    public float knockback;

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
    }

    protected override void Awake()
    {
        base.Awake();
        beKnockBackSeconds = 0.5f;
        beKnockBackLength = 1.0f;
    }

    protected override void Update()
    {
        UpdateHPSlider();
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
        if (other.collider.tag == "Player")
        {
            other.collider.GetComponent<Player>().BeAttacked(damage, other.collider.transform.position - transform.position, knockback);
        }
        //}
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.tag == "Player")
        {
            other.collider.GetComponent<Player>().BeAttacked(damage, other.collider.transform.position - transform.position, knockback);
        }
    }

    void Track()
    {
        Vector2 position = rigidBody2D.position;
        Vector2 target = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector2 site = target - position;
        if (site.magnitude <= findRadius)
        {
            direction.x = site.x;
            direction.y = site.y;
            direction.Normalize();
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);


        }
    }

}
