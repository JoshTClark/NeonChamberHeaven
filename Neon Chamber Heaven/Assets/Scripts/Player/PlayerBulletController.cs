using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletController : MonoBehaviour
{
    [Header("Traces the Bullet's Path")]
    [SerializeField]
    private LineRenderer lineRenderer;

    [Header("Hit Effects")]
    [SerializeField]
    private ParticleSystem wallHitEffect;
    [SerializeField]
    private ParticleSystem enemyBloodEffect;

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
                PlayerBulletController bullet = GameObject.Instantiate(selfPrefab);
                bullet.DoBulletShot(endPos, reflectionDirection, bounces - 1);
            }
        }

        if (animationTimer >= fadeTime && bounceTimer >= bounceFreezeTime && doneBounce)
        {
            Destroy(this.gameObject);
        }
    }

    public void DoBulletShot(Vector3 start, Vector3 direction, int bounces)
    {
        startPos = start;
        this.bounces = bounces;

        RaycastHit hit;
        if (Physics.Raycast(startPos, direction, out hit, 100f, LayerMask.GetMask("Wall", "Enemy")))
        {
            endPos = hit.point;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, endPos);
            reflectionDirection = Vector3.Reflect(direction, hit.normal);
            if (hit.transform.gameObject.GetComponent<EnemyController>())
            {
                this.bounces = 0;
                wallHitEffect.gameObject.SetActive(false);
                enemyBloodEffect.gameObject.SetActive(true);
                enemyBloodEffect.gameObject.transform.position = endPos;
                enemyBloodEffect.gameObject.transform.localRotation = Quaternion.Euler(hit.normal);
            }
            else
            {
                wallHitEffect.gameObject.SetActive(true);
                enemyBloodEffect.gameObject.SetActive(false);
                wallHitEffect.gameObject.transform.position = endPos;
                wallHitEffect.gameObject.transform.localRotation = Quaternion.Euler(hit.normal);
            }
        }
        else
        {
            endPos = start + (direction * 100f);
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, endPos);
            reflectionDirection = new Vector3(0, 0, 0);
            wallHitEffect.gameObject.transform.position = endPos;
            wallHitEffect.gameObject.transform.localRotation = Quaternion.Euler(hit.normal);
            this.bounces = 0;
        }
    }

    public void DoBulletShot(Vector3 start, Vector3 direction, int bounces, Vector3 visualStart)
    {
        DoBulletShot(start, direction, bounces);
        lineRenderer.SetPosition(0, visualStart);
    }
}
