using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private PlayerManager.PlayerInfo _info;

    [SerializeField] private GameObject _beamPrefab;
    [SerializeField] private GameObject _target;

    [SerializeField] private GameObject _projectilPrefab;

    private bool _coolDown = false;
    private float _timeBetweenShots = 0;
    private float _shotTimer;

    private Image _beam;


    public void InitPlayer(PlayerManager.PlayerInfo info)
    {
        _info = info;
        _shotTimer = Mathf.Infinity;
    }

    public void ChangeAttackParameters(float shotCooldown)
    {
        _timeBetweenShots = shotCooldown;
        _shotTimer = _timeBetweenShots;
    }

    public void SetActivePlayer(bool state)
    {
        isActive = state;
    }

    private bool isActive { get; set; }

    void Update()
    {
        if (_info == null || !isActive) return;

        if (_coolDown)
        {
            _shotTimer += Time.deltaTime;
            if(_shotTimer >= _timeBetweenShots)
            {
                _coolDown = false;
                _shotTimer = 0;
            }
            else
            {
                return;
            }
        }

        foreach(KeyValuePair<KeyCode, EButtonColor> keyValue in _info.KeyColorDic)
        {
            if (Input.GetKeyDown(keyValue.Key))
            {
                ProcessInput(true, keyValue.Value);
                _coolDown = true;
            }
            else if (Input.GetKeyUp(keyValue.Key))
                ProcessInput(false, keyValue.Value);
        }
        
    }

    private void ProcessInput(bool isPressed, EButtonColor color)
    {
        switch (BossController.CurrentState)
        {
            case BossController.EBossState.ATTACK_DISQUE:
                if (isPressed)
                {
                    InstanciateBeam(color);
                    BossController.OnPlayerInput(_info.ID, color);
                    StartCoroutine(BeamFade());
                }
                break;

            case BossController.EBossState.VULNERABLE:
                
                break;
        }

    }

    private void InstanciateBeam(EButtonColor inputColor)
    {
        Color32 color = PlayerManager.GetInputColor(inputColor);

        Vector3 selfPos = gameObject.transform.position;
        Vector3 targetPos = _target.transform.position;
        
        Vector3 direction =  selfPos - targetPos;
        float distance = direction.magnitude;
        direction /= distance;
        

        Quaternion angle = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);

        _beam = Instantiate(_beamPrefab, selfPos, angle, gameObject.transform).GetComponent<Image>();
        _beam.color = color;
        RectTransform rect = _beam.GetComponent<RectTransform>();
        // distance = (int)((distance - 624) / 160) * 160 + 624;
        distance = (int)((distance - 39) / 10) * 10 + 39;
        rect.sizeDelta = new Vector2(distance, rect.sizeDelta.y);
    }

    private IEnumerator BeamFade()
    {
        yield return new WaitForSeconds(_timeBetweenShots*0.9f);
        Destroy(_beam.gameObject);
        BossController.OnPlayerInput(_info.ID, EButtonColor.NONE);
    }
}
