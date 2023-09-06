using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    private static BossController Instance;

    public enum EBossState
    {
        ATTACK_DISQUE,
        HYPNOTIC_PHASE,
        CHOSE_ATTACK,
        VULNERABLE,
        RECOVER,
        DEATH,
        NONE,
    }

    private EBossState _currentState;
    private EBossState _previousState;
    private Animator _bossAnimator;

    private bool _increaseHypnoLevel = false;
    private float _currentHypnoLevel;
    private float _currentPV;

    [Header("UI")]
    [SerializeField] private Image _imgHypnoLevel;

    [Header("General Settings")]
    [SerializeField] private int _maxHypnoLevel;
    [SerializeField, Tooltip("Value 1 correspond to the gain of 1 hypnitic level every seconds.")] private float _hypnoLevelSpeed = 1;
    [SerializeField] private int _maxPv;
    [Header("Attack Phase")]
    [SerializeField] private float _delayBetweenAttack;
    [Header("Vulnerability phase")]
    [SerializeField] private float _vulnerabilityTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _bossAnimator = gameObject.GetComponent<Animator>();
        _currentState = EBossState.NONE;
    }

    private void Update()
    {
        if (_increaseHypnoLevel)
        {
            _currentHypnoLevel += Time.deltaTime * _hypnoLevelSpeed;
            _imgHypnoLevel.fillAmount = _currentHypnoLevel / _maxHypnoLevel;
            CheckHypnoticLevel();
        }

        if (_previousState == _currentState) return;

        switch (_currentState)
        {
            // Chose the next attack randomly
            case EBossState.CHOSE_ATTACK:
                StartCoroutine(DelayState((EBossState)Random.Range(0, 5), _delayBetweenAttack));
                break;

            // Disque of color
            case EBossState.ATTACK_DISQUE:
                _increaseHypnoLevel = true;
                StartCoroutine(DisqueAttack());
                break;

            // Minigame when the level of hypnose is at its maximum
            case EBossState.HYPNOTIC_PHASE:
                _increaseHypnoLevel = true;
                StartCoroutine(HypnotiqueAttack());
                break;

            // Players can damage the boss
            case EBossState.VULNERABLE:
                _increaseHypnoLevel = false;
                break;

            // Exit of vulnerable state
            case EBossState.RECOVER:
                
                break;

            // Boss Dead
            case EBossState.DEATH:
                break;
        }

        _previousState = _currentState;
    }

    #region Animation Related
    public void OnInitializationEnd()
    {

    }

    public void OnRecoverEnd()
    {
        StartCoroutine(DelayState(EBossState.CHOSE_ATTACK, 0.5f));
    }
    #endregion

    public static void OnPlayerInput(string player, bool isPressed, PlayerManager.EPlayerColor color)
    {

    }

    private void CheckHypnoticLevel()
    {
        if(_currentHypnoLevel >= _maxHypnoLevel)
        {
            // Stop current attack
            StopAllCoroutines();
            // Change state
            _increaseHypnoLevel = false;
            _currentState = EBossState.HYPNOTIC_PHASE;
        }
    }

    private IEnumerator DelayState(EBossState nextState, float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentState = nextState;
    }

    #region Attack Types
    private IEnumerator DisqueAttack()
    {
        yield return null;
    }

    private IEnumerator HypnotiqueAttack()
    {
        yield return null;
    }
    #endregion
}
