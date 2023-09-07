using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private PlayerManager.PlayerInfo _info;

    [SerializeField] private GameObject _shotPrefab;
    [SerializeField] private GameObject _target;

    [SerializeField] private GameObject _projectilPrefab;

    private bool _coolDown = false;
    private float _timeBetweenShots = 0;
    private float _shotTimer;


    public void InitPlayer(PlayerManager.PlayerInfo info)
    {
        _info = info;
        _shotTimer = _timeBetweenShots;
    }

    public void ChangeAttackParameters(float shotCooldown) => _timeBetweenShots = shotCooldown;

    void Update()
    {
        if (_info == null) return;

        if (_coolDown)
        {
            _shotTimer += Time.deltaTime;
            if(_shotTimer >= _timeBetweenShots)
            {
                _coolDown = false;
                _shotTimer = 0;
            }
        }
        else
        {
            foreach(KeyValuePair<KeyCode, EButtonColor> keyValue in _info.KeyColorDic)
            {
                if (Input.GetKeyDown(keyValue.Key))
                    ProcessInput(true, keyValue.Value);
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
                    BossController.OnPlayerInput(_info.ID, color);
                    StartCoroutine(BeamFade());
                }
                break;

            case BossController.EBossState.VULNERABLE:
                
                break;
        }
    }

    private void ShowColorInput(EButtonColor inputColor)
    {
        Color32 color = PlayerManager.GetInputColor(inputColor);

        Vector3 selfPos = gameObject.transform.position;
        Vector3 targetPos = _target.transform.position;
        
        Vector3 direction =  selfPos - targetPos;
        float distance = direction.magnitude;
        direction /= distance;

        Quaternion angle = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);

        Image Beam = Instantiate(_shotPrefab, selfPos, angle, gameObject.transform).GetComponent<Image>();
        Beam.color = color;
        RectTransform rect = Beam.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, distance);
    }

    private IEnumerator BeamFade()
    {
        yield return new WaitForSeconds(0.8f);
        BossController.OnPlayerInput(_info.ID, EButtonColor.NONE);
    }
}
