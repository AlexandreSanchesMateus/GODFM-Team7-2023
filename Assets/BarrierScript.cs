using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BarrierScript : MonoBehaviour
{
    private static BarrierScript _instance;
    
    private List<List<Image>> Balls = new();

    [SerializeField]
    private List<Image> barrier1;
    [SerializeField]
    private List<Image> barrier2;


    private void Awake()
    {
        _instance = this;
        Balls = new List<List<Image>>(2) { barrier1, barrier2 };
    }

    public static void InitBarrierVisuals(List<Barrier> barriers)
    {
        for (int i = 0; i < barriers.Count; i++)
        {
            for (int j = 0; j < _instance.Balls[i].Count; j++)
            {

                _instance.Balls[i][j].color = PlayerManager.GetInputColor(barriers[i].neededColors[j]);

            }
        }
    }

    public static void RemoveBarrier(int barrierLevel)
    {
        foreach (var images in _instance.Balls[barrierLevel])
        {
            images.color = PlayerManager.GetInputColor(EButtonColor.NONE);
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
