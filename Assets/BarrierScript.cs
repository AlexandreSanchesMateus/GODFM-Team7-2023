using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

public class BarrierScript : MonoBehaviour
{
    private static BarrierScript _instance;

    // ---------------------- New Version -------------------------- //

    [System.Serializable]
    public class WallInfo
    {
        public Transform PosWall1;
        public Transform PosWall2;

        [HideInInspector] public (Image, EButtonColor, bool) Wall1;
        [HideInInspector] public (Image, EButtonColor, bool) Wall2;
        [HideInInspector] public List<Image> BeamsHit = new List<Image>();
    }

    [SerializeField] private GameObject _outsideWallPrefab;
    [SerializeField] private GameObject _neutralOutsideWallPrefab;
    [SerializeField] private GameObject _insideWallPrefab;
    [SerializeField] private GameObject _neutralInsideWallPrefab;
    [SerializeField] private List<WallInfo> _wallInfos;

    int _remainingWall;

    public static List<Transform> GetBarriereTargetPos(bool outsideWall)
    {
        List<Transform> position = new List<Transform>();
        for (int i = 0; i < _instance._wallInfos.Count; i++)
        {
            if (outsideWall)
                position.Add(_instance._wallInfos[i].PosWall1);
            else
                position.Add(_instance._wallInfos[i].PosWall2);
        }
        return position;
    }

    public static void InitBarrierVisuals()
    {
        _instance._remainingWall = 2;
        _instance._wallInfos.ForEach(b => { b.Wall1.Item2 = EButtonColor.NONE; b.Wall2.Item2 = EButtonColor.NONE; });

        List<int> playerWall = new List<int> { 0, 1, 2, 3 };
        playerWall.RemoveAt(Random.Range(0, 4));

        for (int i = 0; i < 4; i++)
        {
            bool exist = playerWall.Exists(p => p == i);
            GameObject wall = Instantiate<GameObject>( exist ? _instance._outsideWallPrefab : _instance._neutralOutsideWallPrefab, _instance.gameObject.transform);

            switch (i)
            {
                case 1:
                    wall.transform.localRotation = Quaternion.Euler(180, 0, 0);
                    break;
                case 2:
                    wall.transform.localRotation = Quaternion.Euler(180, 180, 0);
                    break;
                case 3:
                    wall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    break;
            }

            _instance._wallInfos[i].Wall1.Item1 = wall.GetComponent<Image>();
            _instance._wallInfos[i].Wall1.Item3 = exist;
        }

        playerWall = new List<int> { 0, 1, 2, 3 };
        playerWall.RemoveAt(Random.Range(0, 4));

        for (int i = 0; i < 4; i++)
        {
            bool exist = playerWall.Exists(p => p == i);
            GameObject wall = Instantiate<GameObject>(exist ? _instance._insideWallPrefab : _instance._neutralInsideWallPrefab, _instance.gameObject.transform);
            
            switch (i)
            {
                case 1:
                    wall.transform.localRotation = Quaternion.Euler(180, 0, 0);
                    break;
                case 2:
                    wall.transform.localRotation = Quaternion.Euler(180, 180, 0);
                    break;
                case 3:
                    wall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    break;
            }

            _instance._wallInfos[i].Wall2.Item1 = wall.GetComponent<Image>();
            _instance._wallInfos[i].Wall2.Item3 = exist;
        }

        List<Transform> targets = GetBarriereTargetPos(true);
        for (int i = 0; i < BossController.Players.Count; i++)
        {
            BossController.Players[i].SetLaserTarget(targets[i]);
        }
    }

    public static void OnBeamHitWall(int playerID, EButtonColor color, Image Beam)
    {
        if (_instance._remainingWall == 0) return;

        if(!_instance._wallInfos[playerID].BeamsHit.Contains(Beam))
            _instance._wallInfos[playerID].BeamsHit.Add(Beam);

        EButtonColor sameColor = EButtonColor.NONE;
        bool breakDisque = true;
        bool allHit = true;

        if (_instance._remainingWall == 2)
        {
            _instance._wallInfos[playerID].Wall1.Item1.DOKill();
            _instance._wallInfos[playerID].Wall1.Item1.DOColor(PlayerManager.GetInputColor(color), 0.3f);
            _instance._wallInfos[playerID].Wall1.Item2 = color;

            _instance._wallInfos.ForEach(wall =>
            {
                if (wall.Wall1.Item3)
                {
                    if (wall.Wall1.Item2 == EButtonColor.NONE)
                    {
                        allHit = false;
                    }
                    else if (sameColor == EButtonColor.NONE)
                    {
                        sameColor = wall.Wall1.Item2;
                    }
                    else if (sameColor != wall.Wall1.Item2)
                    {
                        breakDisque = false;
                    }
                }
                else
                {
                    allHit = true;
                    breakDisque = false;
                }
            });

            if (allHit)
            {
                _instance.StartCoroutine(_instance.WaitBeamAction(true, breakDisque));

                // Reinitialisation
                /*_instance.ReinitialiseWall(true);

                if (breakDisque)
                {
                    _instance._wallInfos.ForEach(wall => Destroy(wall.Wall1.Item1.gameObject));

                    List<Transform> targets = GetBarriereTargetPos(false);
                    for (int i = 0; i < BossController.Players.Count; i++)
                    {
                        BossController.Players[i].SetLaserTarget(targets[i]);
                    }
                }*/
            }
        }
        else
        {
            _instance._wallInfos[playerID].Wall2.Item1.DOKill();
            _instance._wallInfos[playerID].Wall2.Item1.DOColor(PlayerManager.GetInputColor(color), 0.3f);
            _instance._wallInfos[playerID].Wall2.Item2 = color;

            _instance._wallInfos.ForEach(wall =>
            {
                if (wall.Wall2.Item3)
                {
                    if (wall.Wall2.Item2 == EButtonColor.NONE)
                    {
                        allHit = false;
                    }
                    else if (sameColor == EButtonColor.NONE)
                    {
                        sameColor = wall.Wall2.Item2;
                    }
                    else if (sameColor != wall.Wall2.Item2)
                    {
                        breakDisque = false;
                    }
                }
                else
                {
                    allHit = true;
                    breakDisque = false;
                }
            });

            if (allHit)
            {
                _instance.StartCoroutine(_instance.WaitBeamAction(false, breakDisque));

                // Reinitialisation
                /*_instance.ReinitialiseWall(false);

                if (breakDisque)
                {
                    _instance._wallInfos.ForEach(wall => Destroy(wall.Wall2.Item1.gameObject));

                    // Exit Mode
                    BossController.QuitPsychoPhase();
                }*/
            }
        }
    }

    private void ReinitialiseWall(bool outsidWall)
    {
        _instance._wallInfos.ForEach(Wall =>
        {
            Wall.BeamsHit.ForEach(beam => Destroy(beam.gameObject));

            if (outsidWall)
            {
                Wall.Wall1.Item1.DOKill();
                Wall.Wall1.Item1.DOColor(Color.white, 0.2f);
                Wall.Wall1.Item2 = EButtonColor.NONE;
            }
            else
            {
                Wall.Wall2.Item1.DOKill();
                Wall.Wall2.Item1.DOColor(Color.white, 0.2f);
                Wall.Wall2.Item2 = EButtonColor.NONE;
            }
        });
    }

    private IEnumerator WaitBeamAction(bool outsideWall, bool breaking)
    {
        BossController.Players.ForEach(p => p.SetActivePlayer(false));
        yield return new WaitForSeconds(0.4f);

        if (outsideWall)
        {
            _instance.ReinitialiseWall(true);

            if (breaking)
            {
                _instance._wallInfos.ForEach(wall => Destroy(wall.Wall1.Item1.gameObject));

                List<Transform> targets = GetBarriereTargetPos(false);
                for (int i = 0; i < BossController.Players.Count; i++)
                {
                    BossController.Players[i].SetLaserTarget(targets[i]);
                }
            }
        }
        else
        {
            // Reinitialisation
            _instance.ReinitialiseWall(false);

            if (breaking)
            {
                _instance._wallInfos.ForEach(wall => Destroy(wall.Wall2.Item1.gameObject));

                // Exit Mode
                BossController.QuitPsychoPhase();
            }
        }

        BossController.Players.ForEach(p => p.SetActivePlayer(true));
    }

    // ---------------------- Last Version ------------------------- //

    private List<List<Image>> Balls = new();

    [FormerlySerializedAs("barrier1"), SerializeField] private List<Image> barrier1Imgs;
    [FormerlySerializedAs("barrier2"), SerializeField] private List<Image> barrier2Imgs;

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
        foreach (GameObject barrier in _instance.barriers)
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

        foreach (List<EButtonColor> color in possibleColors)        
        {
            neededColors.Add(color[0]);
        }

        neededColors[Random.Range(0, neededColors.Count)] = EButtonColor.NONE;
    }
}
