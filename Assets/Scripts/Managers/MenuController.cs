using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;


public class MenuController : MonoBehaviour
{
    [Header("ButtonDisplays")]
    [SerializeField] private List<ButtonDisplay> buttonDisplays;
    [SerializeField] private List<Transform> _playerPositions;
    [Header("Panels")]
    [SerializeField] private Animator _animator;

    private bool _onReadyPanel;
    private float _timer;

    private void Start()
    {
        _onReadyPanel = false;
        _timer = 0;

        for (int i = 0; i < 4; i++)
        {
            buttonDisplays[i].InitKeys(this, PlayerManager.PlayerInfos[i]);
        }
    }

    public void CheckAllPlayersReady()
    {
        for (int i = 0; i < buttonDisplays.Count; i++)
        {
            if (!buttonDisplays[i].isReady)
                return;
        }

        StartCoroutine(MovePlayers());
    }

    private IEnumerator MovePlayers()
    {
        _animator.SetTrigger("Play");
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < buttonDisplays.Count; i++)
        {
            buttonDisplays[i].MoveShapeTo(_playerPositions[i].position, 1f);
        }

        yield return new WaitForSeconds(1f);
        CustomSceneManager.LoadGame(true);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (_onReadyPanel)
            {
                _timer = 0;
            }
            else
            {
                _animator.SetBool("Open", true);
                _onReadyPanel = true;
            }
        }

        if (_onReadyPanel)
        {
            _timer += Time.deltaTime;

            if(_timer >= 30)
            {
                _animator.SetBool("Open", false);
                _onReadyPanel = false;
                _timer = 0;
            }
        }
    }
}
