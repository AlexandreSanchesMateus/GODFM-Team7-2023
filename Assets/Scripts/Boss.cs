using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public enum EBossState
    {
        ATTACK_ONE,
        ATTACK_TWO,
        ATTACK_THREE,
        ATTACK_FOUR,
        CHOSE_ATTACK,
        VULNERABLE,
        RECOVER,
        DEATH,
        NONE,
    }

    private EBossState _currentState;
    private EBossState _previousState;
    private float _attackSpeed;

    [SerializeField] private float _delayBetweenAttack;

    void Start()
    {
        _currentState = EBossState.NONE;
    }

    void Update()
    {
        if (_previousState == _currentState) return;

        switch (_currentState)
        {
            case EBossState.CHOSE_ATTACK:
                StartCoroutine(DelayState((EBossState)Random.Range(0, 5), _delayBetweenAttack));
                break;

            case EBossState.ATTACK_ONE:
                break;

            case EBossState.ATTACK_TWO:
                break;

            case EBossState.ATTACK_THREE:
                break;

            case EBossState.ATTACK_FOUR:
                break;

            case EBossState.VULNERABLE:
                break;

            case EBossState.DEATH:
                break;
        }

        _previousState = _currentState;
    }

    public void OnInitializationEnd()
    {

    }

    public void OnRecoverEnd()
    {
        StartCoroutine(DelayState(EBossState.CHOSE_ATTACK, 0.5f));
    }

    private IEnumerator DelayState(EBossState nextState, float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentState = nextState;
    }
}
