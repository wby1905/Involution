using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    public float fadeSpeed;
    private LineRenderer line;
    private float alpha;

    public int layerNow;


    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        alpha = line.endColor.a;
    }

    private void OnEnable()
    {
        line.endColor = new Color(line.endColor.r, line.endColor.g, line.endColor.b, alpha);
        gameObject.layer = layerNow;
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        gameObject.layer = layerNow;
        while (line.endColor.a > 0)
        {
            line.endColor = new Color(line.endColor.r, line.endColor.g, line.endColor.b, line.endColor.a - fadeSpeed);
            yield return new WaitForSeconds(0.1f);
        }

        ObjectPool.Instance.PushObject(gameObject);
    }
}
