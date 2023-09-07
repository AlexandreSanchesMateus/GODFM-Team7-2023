using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BarrierScript : MonoBehaviour
{
    private static BarrierScript _instance;
    
    private List<List<Image>> Balls = new();

    [FormerlySerializedAs("barrier1")] [SerializeField]
    private List<Image> barrier1Imgs;
    [FormerlySerializedAs("barrier2")] [SerializeField]
    private List<Image> barrier2Imgs;

    [SerializeField] private List<GameObject> barriers;


    private void Awake()
    {
        _instance = this;
        Balls = new List<List<Image>>(2) { barrier1Imgs, barrier2Imgs };
        SetState(false);
    }

    public static void InitBarrierVisuals(List<Barrier> barriers)
    {
        SetState(true);
        for (int i = 0; i < barriers.Count; i++)
        {
            for (int j = 0; j < _instance.Balls[i].Count; j++)
            {

                _instance.Balls[i][j].color = PlayerManager.GetInputColor(barriers[i].neededColors[j]);

            }
        }
    }

    public static List<Transform> GetPointsTransforms(int barrierLevel)
    {
        List<Transform> temp = new List<Transform>(4);
        
        temp.AddRange(_instance.Balls[barrierLevel].Select(image => image.gameObject.transform));

        return temp;
    }

    public static void RemoveBarrier(int barrierLevel)
    {
        foreach (var images in _instance.Balls[barrierLevel])
        {
            images.color = PlayerManager.GetInputColor(EButtonColor.NONE);
        }
        _instance.barriers[barrierLevel].SetActive(false);
    }

    public static void SetState(bool state)
    {
        foreach (var barrier in _instance.barriers)
        {
            barrier.SetActive(state);
        }
    }
}

public class Barrier
{
    public List<EButtonColor> neededColors { get; private set; }

    public Barrier()
    {
        neededColors = new(4);
        List<List<EButtonColor>> possibleColors = new List<List<EButtonColor>>()
        {
            new() { EButtonColor.YELLOW, EButtonColor.BLUE, EButtonColor.RED },
            new() { EButtonColor.BLUE, EButtonColor.GREEN, EButtonColor.RED },
            new() { EButtonColor.YELLOW, EButtonColor.BLUE, EButtonColor.GREEN },
            new() { EButtonColor.YELLOW, EButtonColor.RED, EButtonColor.GREEN }
        };

        for (int i = 0; i < 2; i++)
        {
            foreach (List<EButtonColor> possibleColorPerPlayer in possibleColors)
            {
                possibleColorPerPlayer.RemoveAt(Random.Range(0, possibleColorPerPlayer.Count));
            }
        }

        foreach (var color in possibleColors)        
        {
            neededColors.Add(color[0]);
        }

        neededColors[Random.Range(0, neededColors.Count)] = EButtonColor.NONE;
    }
}
