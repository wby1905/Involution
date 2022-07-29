using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    public float speed; //弹壳抛出的速度
    public float stopTime = .5f; //弹壳停止的时间
    public float fadeSpeed = .01f; //弹壳消失的时间
    new private Rigidbody2D rigidbody; 
    private SpriteRenderer sprite;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();


    }

    private void OnEnable()//对象池激活弹壳时调用
    {
        float angel = Random.Range(-30f, 30f);
        rigidbody.velocity = Quaternion.AngleAxis(angel, Vector3.forward) * Vector3.up * speed;
        //实现一个弹壳向上抛出的效果
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
        rigidbody.gravityScale = 3;

        StartCoroutine(Stop());
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(stopTime); //等待设定好的时间
        rigidbody.velocity = Vector2.zero; //速度都设置成零
        rigidbody.gravityScale = 0; //重力设置成0

        while (sprite.color.a > 0)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.g, sprite.color.a - fadeSpeed);
            yield return new WaitForFixedUpdate(); //等待固定帧
        }
        // Destroy(gameObject);
        ObjectPool.Instance.PushObject(gameObject);
    }
}
