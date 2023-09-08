using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerScript : MonoBehaviour
{
    private PlayerManager.PlayerInfo _info;

    [SerializeField] private GameObject _beamPrefab;
    [SerializeField] private GameObject _target;

    [SerializeField] private GameObject _projectilPrefab;
    [SerializeField] private Transform laserParent;
    [FormerlySerializedAs("laserStart")] [SerializeField] private Transform shootStart;
    [FormerlySerializedAs("laserTarget")] [SerializeField] private Transform target;

    private bool _coolDown = false;
    private float _timeBetweenShots = 1.4f;
    private float _shotTimer;

    private GameObject _beam;


    public void InitPlayer(PlayerManager.PlayerInfo info)
    {
        _info = info;
    }

    public void ChangeAttackParameters(float shotCooldown)
    {
        _timeBetweenShots = shotCooldown;
    }

    public void SetLaserTarget(List<Transform> transforms)
    {
        target = transforms[_info.ID];
    }
    
    public void SetLaserTarget(Transform gameObjectTransform)
    {
        target = gameObjectTransform;
    }

    public void SetActivePlayer(bool state)
    {
        isActive = state;

        if (state)
        {
            _coolDown = false;
            _shotTimer = 0;
        }
    }

    private bool isActive { get; set; }

    void Update()
    {
        if (_info == null || !isActive) return;

        if (_coolDown)
        {
            _shotTimer += Time.deltaTime;
            if (_shotTimer >= _timeBetweenShots)
            {
                _coolDown = false;
                _shotTimer = 0;
            }
            else
            {
                return;
            }
        }
        else
        {
            foreach (KeyValuePair<KeyCode, EButtonColor> keyValue in _info.KeyColorDic)
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
    }

    private void ProcessInput(bool isPressed, EButtonColor color)
    {
        switch (BossController.CurrentState)
        {
            case BossController.EBossState.ATTACK_DISQUE:
                if (isPressed)
                {
                    InstanciateBeam(color).GetComponent<Laser>().SetLazer(_timeBetweenShots * 0.8f, _info.ID);
                    BossController.OnPlayerInput(_info.ID, color);
                }
                break;
            
            case BossController.EBossState.HYPNOTIC_PHASE:
                if (isPressed)
                {
                    if (_beam != null)
                    {
                        _beam.GetComponent<Image>().DOKill();
                        _beam.GetComponent<Image>().DOColor(PlayerManager.GetInputColor(color), 0.4f);
                    }
                    else
                    {
                        InstanciateBeam(color);
                    }

                    BarrierScript.OnBeamHitWall(_info.ID, color, _beam);
                }
                break;

            case BossController.EBossState.VULNERABLE:
                if (isPressed)
                {
                    InstantiateProjectile(color);
                    BossController.OnPlayerInput(_info.ID, color);
                }
                break;
        }

    }

    private void InstantiateProjectile(EButtonColor color)
    {
        Projectil proj = Instantiate(_projectilPrefab, shootStart.position, Quaternion.identity, laserParent).GetComponent<Projectil>();
        proj.Target = _target;
        proj.GetComponent<Image>().color = PlayerManager.GetInputColor(color);
    }

    private GameObject InstanciateBeam(EButtonColor inputColor)
    {
        Color32 color = PlayerManager.GetInputColor(inputColor);

        Vector3 startPos = shootStart.position;
        Vector3 targetPos = target.position;
        
        Vector3 direction =  startPos - targetPos;
        float distance = direction.magnitude;
        // Debug.Log($"{startPos.x} - {targetPos.x} || {startPos.y} - {targetPos.y} || {startPos.z} - {targetPos.z}");
        distance *= 28.5f; // I have no idea why but this is the value that works
        Debug.Log(distance);
        direction /= distance;
        

        Quaternion angle = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);

        _beam = Instantiate(_beamPrefab, startPos, angle, laserParent);
        _beam.GetComponent<Image>() .color = color;
        RectTransform rect = _beam.GetComponent<RectTransform>();
        // distance = (int)((distance - 624) / 160) * 160 + 624;
        //distance = (int)((distance - 39) / 10) * 10 + 39;
        rect.sizeDelta = new Vector2(distance, rect.sizeDelta.y);
        return _beam;
    }

    private IEnumerator BeamFade()
    {
        yield return new WaitForSeconds(_timeBetweenShots * 0.8f);
        BossController.OnPlayerInput(_info.ID, EButtonColor.NONE);
        yield return null;
        Destroy(_beam);
        _beam = null;
    }

    public void DestroyBeamInstant()
    {
        Destroy(_beam);
        _beam = null;
    }
}
