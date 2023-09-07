using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Projectil : MonoBehaviour
{
    public GameObject Target;
    public float MoveSpeed = 1;

    public float maxOffset = 5;
    private List<int> directions = new() { 1, -1 };
    public float duration;

    [SerializeField] private GameObject explosionPrefab;

    private void Start()
    {
        if (Target == null) return;
        DOTween.Init();
        Vector2 startPos = transform.position;
        Vector2 targetPos = Target.transform.position;

        Vector2 centerPoint = (startPos - targetPos) / 2;

        Vector2 perpDirection = Vector2.Perpendicular(centerPoint).normalized;

        Vector2 midPoint = centerPoint + perpDirection * Random.Range(0.2f, maxOffset) * directions[Random.Range(0, 2)];

        List<Vector3> waypoints = new List<Vector3>() {midPoint};

        List<Vector2> midpointsHandles = new ();
        
        // Handles direction is center point
        Vector2 handlesDirection = centerPoint.normalized;
        foreach (int dir in directions)
        {
            midpointsHandles.Add(midPoint + handlesDirection * Random.Range(0.2f, maxOffset) * dir);
        }
        
        // Starting point handle
        Vector2 direction1 = (midpointsHandles[0] - startPos).normalized;
        Vector2 startHandle = startPos + direction1 * Random.Range(0.2f, maxOffset);
        
        waypoints.Add(startHandle);
        waypoints.Add(midpointsHandles[0]);
        waypoints.Add(targetPos);
        waypoints.Add(midpointsHandles[1]);
        
        // target point handle
        Vector2 direction2 = (midpointsHandles[1] - targetPos).normalized;
        Vector2 targetHandle = targetPos + direction2 * Random.Range(0.2f, maxOffset);
        
        waypoints.Add(targetHandle);

        transform.DOPath(waypoints.ToArray(), duration, PathType.CubicBezier, resolution:5).SetEase(Ease.OutCirc);
        Invoke(nameof(DestroyProjectile), duration*1.1f);
    }

    void DestroyProjectile()
    {
        // An explosion sprite was wandering around so I used it here, totally fine to use another one or remove it
        Instantiate(explosionPrefab, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0), Quaternion.identity);
        Destroy(gameObject);
    }
}
