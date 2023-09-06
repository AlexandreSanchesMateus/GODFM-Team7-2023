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
    private Animator _buttonsAnimator;

    private (bool, bool, bool) _activeButton;
    private bool hasBeenHold = false;
    private Coroutine _fillCoroutine;
    
    private List<String> posNames = new(){"Left", "Right", "Vertical"};
    [SerializeField]
    private List<Image> images;
    public Dictionary<String, Image> posImageDic = new();

    private void Start()
    {
        _buttonsAnimator = gameObject.GetComponent<Animator>();
        _activeButton.Item1 = false;
        _activeButton.Item1 = false;
        _activeButton.Item1 = false;

        for (int i = 0; i < posNames.Count; i++)
        {
            posImageDic[posNames[i]] = images[i];
        }
    }

    [ContextMenu("Activate All Buttons")]
    public void ActivateAllButtons()
    {
        _buttonsAnimator.SetBool("LButton", true);
        _buttonsAnimator.SetBool("RButton", true);
        _buttonsAnimator.SetBool("VButton", true);
    }

    public void SetLeftButton(bool setActive)
    {
        _activeButton.Item1 = setActive;
        _buttonsAnimator.SetBool("LButton", setActive);
    }

    public void SetRightButton(bool setActive)
    {
        _activeButton.Item2 = setActive;
        _buttonsAnimator.SetBool("RButton", setActive);
    }

    public void SetVerticalButton(bool setActive)
    {
        _activeButton.Item3 = setActive;
        _buttonsAnimator.SetBool("VButton", setActive);
    }

    public void MoveShapeTo(Vector2 wordPos) => gameObject.transform.DOMove(wordPos, _travelDuration, true).SetEase(Ease.OutQuad);

    private void CheckAllActive()
    {
        bool allActive = _activeButton.Item1 && _activeButton.Item2 && _activeButton.Item3;
        if (allActive)
        {

        }
        else
        {

        }
    }

    private IEnumerator FillRadian()
    {
        yield return null;
    }
}
