using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletController : MonoBehaviour
{
    [Header("Traces the Bullet's Path")]
    [SerializeField]
    private LineRenderer lineRenderer;

    [Header("Hit Effect")]
    [SerializeField]
    private ParticleSystem hitEffect;

    [Header("Animation Values")]
    [SerializeField]
    private float bounceFreezeTime;
    [SerializeField]
    private float fadeTime;

    [SerializeField]
    private PlayerBulletController selfPrefab;

    private Vector3 reflectionDirection;

    private int bounces = 1;

    private float animationTimer;
    private float bounceTimer;

    private bool doneBounce;

    private Vector3 startPos, endPos;

    private void Start()
    {
        animationTimer = 0.0f;
        bounceTimer = 0.0f;
        doneBounce = false;
    }

    private void Update()
    {
        animationTimer += Time.deltaTime;
        bounceTimer += Time.deltaTime;

        if (bounces == 0)
        {
            doneBounce = true;
        }

        lineRenderer.startColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, Mathf.Lerp(1f, 0f, animationTimer / fadeTime));
        lineRenderer.endColor = new Color(lineRenderer.endColor.r, lineRenderer.endColor.g, lineRenderer.endColor.b, Mathf.Lerp(1f, 0f, animationTimer / fadeTime));

        if (!doneBounce && bounceTimer >= bounceFreezeTime)
        {
            doneBounce = true;
            if (bounces > 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(endPos, reflectionDirection, out hit, 100f))
                {
                    PlayerBulletController bullet = GameObject.Instantiate(selfPrefab);
                    bullet.DoBulletShot(endPos, hit.point, bounces - 1, Vector3.Reflect(reflectionDirection, hit.normal), hit.normal);
                }
                else
                {
                    PlayerBulletController bullet = GameObject.Instantiate(selfPrefab);
                    bullet.DoBulletShot(endPos, startPos + (reflectionDirection * 100f));
                }
            }
        }

        if (animationTimer >= fadeTime && bounceTimer >= bounceFreezeTime && doneBounce)
        {
            Destroy(this.gameObject);
        }
    }

    public void DoBulletShot(Vector3 start, Vector3 end, int bounces, Vector3 reflection, Vector3 normal)
    {
        this.bounces = bounces;
        startPos = start;
        endPos = end;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        reflectionDirection = reflection;
        hitEffect.gameObject.transform.position = end;
        hitEffect.gameObject.transform.localRotation = Quaternion.Euler(normal);
    }

    public void DoBulletShot(Vector3 start, Vector3 end)
    {
        DoBulletShot(start, end, 0, Vector3.zero, Vector3.zero);
    }
}
