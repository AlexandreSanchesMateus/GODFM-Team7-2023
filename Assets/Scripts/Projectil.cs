using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Projectil : MonoBehaviour
{
    public GameObject Target;
    public float MoveSpeed = 1;

    private List<int> directions = new() { 1, -1 };
    public float duration;
    [Header("Offsets"), Tooltip("Max offset For generating Bezier handles")]
    public float maxOffset = 5;
    [Tooltip("Min offset For generating Bezier handles")]
    public float minOffset = 0.2f;
    
    [Tooltip("Max offset for generating explosions (from point of impact)")]
    public float maxOffsetEx = 0.75f;
    [Tooltip("Min offset for generating explosions (from point of impact)")]
    public float minOffsetEx = -0.75f;

    [SerializeField] private GameObject explosionPrefab;
    public Transform Parent;

    private void Start()
    {
        if (Target == null) return;
        DOTween.Init();
        Vector2 startPos = transform.position;
        Vector2 targetPos = Target.transform.position;

        Vector2 centerPoint = (startPos - targetPos) / 2;

        Vector2 perpDirection = Vector2.Perpendicular(centerPoint).normalized;

        Vector2 midPoint = centerPoint + perpDirection * Random.Range(minOffset, maxOffset) * directions[Random.Range(0, 2)];

        List<Vector3> waypoints = new List<Vector3>() {midPoint};

        List<Vector2> midpointsHandles = new ();
        
        // Handles direction is center point
        Vector2 handlesDirection = centerPoint.normalized;
        foreach (int dir in directions)
        {
            midpointsHandles.Add(midPoint + handlesDirection * Random.Range(minOffset, maxOffset) * dir);
        }
        
        // Starting point handle
        Vector2 direction1 = (midpointsHandles[0] - startPos).normalized;
        Vector2 startHandle = startPos + direction1 * Random.Range(minOffset, maxOffset);
        
        waypoints.Add(startHandle);
        waypoints.Add(midpointsHandles[0]);
        waypoints.Add(targetPos);
        waypoints.Add(midpointsHandles[1]);
        
        // target point handle
        Vector2 direction2 = (midpointsHandles[1] - targetPos).normalized;
        Vector2 targetHandle = targetPos + direction2 * Random.Range(minOffset, maxOffset);
        
        waypoints.Add(targetHandle);

        transform.DOPath(waypoints.ToArray(), duration, PathType.CubicBezier, resolution:5).SetEase(Ease.OutCirc);
        Invoke(nameof(DestroyProjectile), duration*1.02f);
    }

    void DestroyProjectile()
    {
        // An explosion sprite was wandering around so I used it here, totally fine to use another one or remove it
        GameObject proj = Instantiate(explosionPrefab, transform.position + new Vector3(Random.Range(minOffsetEx, maxOffsetEx), 
            Random.Range(minOffsetEx, maxOffsetEx), 0), Quaternion.identity, Parent);
        var localScale = proj.transform.localScale;
        float rand = Random.Range(-0.4f, 0.1f);
        localScale = new Vector3(rand + localScale.x, rand + localScale.y, localScale.z);
        proj.transform.localScale = localScale;
        Image img = proj.GetComponent<Image>();
        img.color = GetComponent<Image>().color;
        
        BossController.DoShake();
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        if (maxOffsetEx < minOffsetEx)
        {
            (maxOffsetEx, minOffsetEx) = (minOffsetEx, maxOffsetEx);
        }
    }
}
