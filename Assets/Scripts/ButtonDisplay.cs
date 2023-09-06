using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class ButtonDisplay : MonoBehaviour
{
    public bool isReady { get; private set; }

    [SerializeField] private float _travelDuration;
    private Animator _buttonsAnimator;

    private (bool, bool, bool) _activeBotton;
    private bool hasBeenHold = false;
    private Coroutine _fillCoroutine;

    private void Start()
    {
        _buttonsAnimator = gameObject.GetComponent<Animator>();
        _activeBotton.Item1 = false;
        _activeBotton.Item1 = false;
        _activeBotton.Item1 = false;
    }

    [ContextMenu("Activate All Buttons")]
    public void ActivateAllButtons()
    {
        _buttonsAnimator.SetBool("LButton", true);
        _buttonsAnimator.SetBool("RButton", true);
        _buttonsAnimator.SetBool("VButton", true);
    }

    public void SetLeftButton(bool setAvtive)
    {
        _activeBotton.Item1 = setAvtive;
        _buttonsAnimator.SetBool("LButton", setAvtive);
    }

    public void SetRightButton(bool setAvtive)
    {
        _activeBotton.Item2 = setAvtive;
        _buttonsAnimator.SetBool("RButton", setAvtive);
    }

    public void SetVerticalButton(bool setAvtive)
    {
        _activeBotton.Item3 = setAvtive;
        _buttonsAnimator.SetBool("VButton", setAvtive);
    }

    public void MoveShapeTo(Vector2 wordPos) => gameObject.transform.DOMove(wordPos, _travelDuration, true).SetEase(Ease.OutQuad);

    private void CheckAllActive()
    {
        bool allActive = _activeBotton.Item1 && _activeBotton.Item2 && _activeBotton.Item3;
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
