using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private float TimeDestroy = -1;
    private int ID;

    void Start()
    {
        if(TimeDestroy != -1)
        {
            Invoke(nameof(DestroyLazer), TimeDestroy);
        }
    }

    public void SetLazer(float time, int id)
    {
        TimeDestroy = time;
        ID = id;
    }

    private void DestroyLazer()
    {
        BossController.OnPlayerInput(ID, EButtonColor.NONE);
        Destroy(gameObject);
    }
}
