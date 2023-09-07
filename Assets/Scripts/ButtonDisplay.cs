using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


[RequireComponent(typeof(Animator))]
public class ButtonDisplay : MonoBehaviour
{
    public bool isReady { get; private set; }

    [SerializeField] private float _travelDuration;
    [SerializeField] private List<Image> images;

    private bool _init = false;
    private Animator _buttonsAnimator;

    private (bool, bool, bool) _activeButton;
    private bool allActive = false;

    private Dictionary<KeyCode, EButtonColor> keys;
    private MenuController _manager;
    
    /*private List<String> posNames = new(){"Left", "Right", "Vertical"};
    [SerializeField]
    private List<Image> images;
    public Dictionary<String, Image> posImageDic = new();*/

    private void Start()
    {
        _buttonsAnimator = gameObject.GetComponent<Animator>();
        _activeButton.Item1 = false;
        _activeButton.Item1 = false;
        _activeButton.Item1 = false;

        /*for (int i = 0; i < posNames.Count; i++)
        {
            posImageDic[posNames[i]] = images[i];
        }*/
    }

    private void Update()
    {
        if (!_init) return;

        int index = 0;
        foreach(KeyValuePair<KeyCode, EButtonColor> keyValue in keys)
        {
            if (Input.GetKeyDown(keyValue.Key))
                SetKey(index, keyValue.Value, true);
            else if (Input.GetKeyUp(keyValue.Key))
                SetKey(index, keyValue.Value, false);

            index++;
        }
    }

    public void InitKeys(MenuController manager, PlayerManager.PlayerInfo infos)
    {
        _manager = manager;
        keys = infos.KeyColorDic;
        _init = true;
    }

    private void SetKey(int index, EButtonColor color, bool isPressed)
    {
        switch (index)
        {
            case 0:
                _activeButton.Item1 = isPressed;
                _buttonsAnimator.SetBool("LButton", isPressed);
                break;
            case 1:
                _activeButton.Item2 = isPressed;
                _buttonsAnimator.SetBool("RButton", isPressed);
                break;
            case 2:
                _activeButton.Item3 = isPressed;
                _buttonsAnimator.SetBool("VButton", isPressed);
                break;
        }

        if (isPressed)
            images[index].DOColor(PlayerManager.GetInputColor(color), 0.4f);
        else
            images[index].DOColor(Color.white, 0.4f);

        CheckAllActive();
    }

    public void MoveShapeTo(Vector2 wordPos) => gameObject.transform.DOMove(wordPos, _travelDuration, true).SetEase(Ease.OutQuad);

    private void CheckAllActive()
    {
        allActive = _activeButton.Item1 && _activeButton.Item2 && _activeButton.Item3;

        if (isReady)
        {
            if (allActive)
            {
                StopAllCoroutines();
            }
            else
            {
                StartCoroutine(NotReadyDelai());
            }
        }
        else
        {
            if (allActive)
            {
                StartCoroutine(HoldButtons());
            }
            else
            {
                StopAllCoroutines();
            }
        }
    }

    private IEnumerator HoldButtons()
    {
        yield return new WaitForSeconds(2f);
        isReady = true;
        _manager.CheckAllPlayersReady();
    }

    private IEnumerator NotReadyDelai()
    {
        yield return new WaitForSeconds(2f);
        isReady = false;
    }
}
