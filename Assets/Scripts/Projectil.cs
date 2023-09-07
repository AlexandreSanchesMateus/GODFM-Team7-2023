using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectil : MonoBehaviour
{
    public GameObject Target;
    public float MoveSpeed = 1;

    private void Update()
    {
        if(Target != null)
        {
            float distance = (Target.transform.position - transform.position).magnitude;
            if(distance >= 0.1f)
            {
                transform.position = Vector2.Lerp(transform.position, Target.transform.position, MoveSpeed * Time.deltaTime);
            }
            else
            {
                BossController.TakeDamage();
                Destroy(gameObject);
            }
        }
    }
}
