using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private int playerId;

    private PlayerManager.PlayerInfo _playerInfo;
    private List<EButtonColor> _pressedColors = new(3);

    // To be able to put cooldown on presses
    // Should be handled by gamemanager imo to be able to easely use game state
    [SerializeField] private int DisqueAttackDeltaTime;
    private int TimeBetweenShots = 0;

    [SerializeField] private float TimeBeforeHold = 0.2f; // Time before input is detected as held

    private float shotTimer;
    private float holdTimer;

    [SerializeField] private GameObject shotPrefab;
    [SerializeField] private GameObject target;

    private Image LineRenderer;

    // Start is called before the first frame update
    void Start()
    {
         _playerInfo = PlayerManager.PlayerInfos[playerId];   //doesn't seem to work
        shotTimer = TimeBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        bool canPress = false;
        shotTimer += Time.deltaTime;
        if (shotTimer >= TimeBetweenShots)
        {
            canPress = true;
        }

        if (!canPress) return;

        foreach (KeyCode keyCode in _playerInfo.KeyColorDic.Keys.ToArray())
        {
            EButtonColor keycodeColor = _playerInfo.KeyColorDic[keyCode];

            if (canPress && Input.GetKeyDown(keyCode))
            {
                shotTimer = 0;
                _pressedColors.Add(keycodeColor);

                ProcessInput(true, keycodeColor);
            }

            if (Input.GetKeyUp(keyCode))
            {
                holdTimer = 0;
                _pressedColors.Remove(keycodeColor);

                ProcessInput(false, keycodeColor);
            }
        }

        if (_pressedColors.Count != 0)
        {
            if (holdTimer < TimeBeforeHold)
            {
                holdTimer += Time.deltaTime;
                return;
            }

            EButtonColor lastPressedInput = _pressedColors[0];
        }
    }

    private void ProcessInput(bool isPressed, EButtonColor color)
    {
        if(isPressed)
            ShowColorInput(color);

        switch (BossController.CurrentState)
        {
            case BossController.EBossState.ATTACK_DISQUE:
                TimeBetweenShots = DisqueAttackDeltaTime;
                if(isPressed)
                    BossController.OnPlayerInput(playerId, color);
                else
                {
                    BossController.OnPlayerInput(playerId, EButtonColor.NONE);
                    DestroyColorInput();
                }
                break;

            case BossController.EBossState.VULNERABLE:
                TimeBetweenShots = 0;
                break;

            default:
                if (_pressedColors.Count == 0)
                    DestroyColorInput();
                else if(LineRenderer != null)
                    LineRenderer.color = PlayerManager.GetInputColor(_pressedColors[_pressedColors.Count - 1]);
                break;
        }
    }

    private void DestroyColorInput()
    {
        if (LineRenderer == null) return;

        // Animation
        Destroy(LineRenderer.gameObject);
    }

    private void ShowColorInput(EButtonColor inputColor)
    {
        // Color32 color = PlayerManager.GetInputColor(inputColor);
        //
        // Vector3 selfPos = gameObject.transform.position;
        // Vector3 targetPos = target.transform.position;
        //
        // Debug.Log($"SelfPos:{selfPos} ||targetPos:{targetPos}");
        //
        // Vector3 direction =  selfPos - targetPos;
        // float distance = direction.magnitude;
        // direction /= distance;
        // Debug.Log($"Distance:{distance}");
        //
        // Quaternion angle = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        //
        // LineRenderer = Instantiate(shotPrefab, selfPos, angle, gameObject.transform).GetComponent<Image>();
        // LineRenderer.color = color;
        // RectTransform rect = LineRenderer.GetComponent<RectTransform>();
        // rect.sizeDelta = new Vector2(rect.sizeDelta.x, distance);

        Projectil proj = Instantiate(PlayerManager.Instance.Projectile, transform.position, Quaternion.identity)
            .GetComponent<Projectil>();
        proj.Target = target;

    }
}
