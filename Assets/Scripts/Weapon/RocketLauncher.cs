using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RocketLauncher : Gun
{
    private int num = 3;
    public float rocketAngle = 15;

    protected override void OnEnable()
    {
        base.OnEnable();
        num = NUM;
    }
    protected override void Fire()
    {
        animator.SetTrigger("Shoot");
        StartCoroutine(DelayFire(0.2f));//协程等待0.2s
    }

    IEnumerator DelayFire(float delay)
    {

        int median = num / 2;
        for (int i = 0; i < num; i++)
        {
            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.position = muzzlePos.position;

            if (num % 2 == 1)
            {
                bullet.transform.right = Quaternion.AngleAxis(rocketAngle * (i - median), Vector3.forward) * direction;
            }
            else
            {
                bullet.transform.right = Quaternion.AngleAxis(rocketAngle * (i - median) + rocketAngle / 2, Vector3.forward) * direction;
            }
            int n = Random.Range(4, 9);
            bullet.GetComponent<Rocket>().SetTarget(new Vector2(transform.position.x + shootDirection.x * n, transform.position.y + shootDirection.y * n));
            bullet.GetComponent<Rocket>().isPlayerFlag = isPlayer;
        }
        yield return new WaitForSeconds(delay);

    }
}
