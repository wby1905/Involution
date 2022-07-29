using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float lerp; //转向的速度
    public float speed = 15;
    public GameObject explosionPrefab;
    new private Rigidbody2D rigidbody;
    private Vector3 targetPos;//目标位置
    private Vector3 direction;//目标方向
    private bool arrived;//用于表示到达
    public float damage;
    public float knockback;
    public bool isPlayerFlag;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        if (!isPlayerFlag)
        {
            damage = 1;
            knockback = 0.5f;
            gameObject.layer = 22;
        }
        else
        {
            gameObject.layer = 17;
        }

    }
    public void SetTarget(Vector2 _target)
    {
        arrived = false;
        targetPos = _target;
    }

    private void FixedUpdate()
    {
        direction = (targetPos - transform.position).normalized;//持续获得火箭方向

        if (!arrived)//在火箭弹未到达目标位置时，每一帧都修改火箭弹的方向
        {
            transform.right = Vector3.Slerp(transform.right, direction, lerp / Vector2.Distance(transform.position, targetPos));
            //Slerp函数用于返回值，返回值取决于第三个参数，0返回第一个，1返回第二个，0.5返回中间值
            rigidbody.velocity = transform.right * speed;
        }
        if (Vector2.Distance(transform.position, targetPos) < 1f && !arrived)
        {
            arrived = true;//虽然还没有到，但足够近时就认为它到了
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
        exp.transform.position = transform.position;
        if (isPlayerFlag && other.tag == "Monster")
        {
            other.GetComponent<Monster>().BeAttacked(damage, rigidbody.velocity / speed, knockback);
        }
        if (!isPlayerFlag && other.tag == "Player")
        {
            other.GetComponent<Player>().BeAttacked(damage, rigidbody.velocity / speed, knockback);
        }
        rigidbody.velocity = Vector2.zero;
        StartCoroutine(Push(gameObject, 0f));//让火箭过一段时间再消失。
    }

    IEnumerator Push(GameObject _object, float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPool.Instance.PushObject(_object);
    }
}
