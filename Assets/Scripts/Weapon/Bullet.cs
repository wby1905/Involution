using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public float knockback;
    public GameObject explosionPrefab; //传入爆炸特效的预制体
    new private Rigidbody2D rigidbody; //传入刚体
    public bool isPlayerFlag;
    void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        if (!isPlayerFlag)
        {
            gameObject.layer = 22;
            damage = 0.5f;
            knockback = 0.3f;
            speed = 5;

        }
        else
        {
            gameObject.layer = 17;
            speed = 15;
        }
    }

    public void SetSpeed(Vector2 direction)
    {
        if (!isPlayerFlag)
        {
            speed = 5;
            gameObject.layer = 22;
            transform.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        else
        {
            speed = 15;
            gameObject.layer = 17;
        }
        direction.Normalize();
        rigidbody.velocity = direction * speed;
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab); //生成一个爆炸特效预制体
        exp.transform.position = transform.position;
        if (!isPlayerFlag && other.tag == "Player")
        {
            other.GetComponent<Player>().BeAttacked(damage, rigidbody.velocity / speed, knockback);
        }
        if (isPlayerFlag && other.tag == "Monster")
        {
            other.GetComponent<Monster>().BeAttacked(damage, rigidbody.velocity / speed, knockback);
        }
        // Destroy(gameObject);
        ObjectPool.Instance.PushObject(gameObject);
    }
}
