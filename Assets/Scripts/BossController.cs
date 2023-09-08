using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class BossController : MonoBehaviour
{
    private static BossController Instance;
    public static EBossState CurrentState => Instance._currentState;
    public static List<PlayerScript> Players => Instance._players;

    public enum EBossState
    {
        ATTACK_DISQUE,
        HYPNOTIC_PHASE,
        VULNERABLE,
        DEATH,
        NONE,
    }

    private EBossState _currentState;
    private EBossState _previousState;
    private Animator _bossAnimator;

    // Attack
    private List<EButtonColor> currentPlayersInput;
    List<EButtonColor> colorSelected = new List<EButtonColor>() { EButtonColor.NONE, EButtonColor.NONE, EButtonColor.NONE, EButtonColor.NONE };

    private float _currentPV;

    private bool _increaseHypnoLevel = false;
    private float _currentHypnoLevel;

    private bool _timeElapse = false;
    private float _timeInGame = 0.0001F;

    [Header("UI")]
    [SerializeField] private List<PlayerScript> _players;
    [SerializeField] private Image _imgHypnoLevel;
    [SerializeField] private TextMeshProUGUI _textTimer;
    [SerializeField] private Animator _backgroundAnimator;
    [SerializeField] private List<Image> _bossEyes;
    [SerializeField] private List<Transform> _bossEyesTransform;
    [Header("General Settings")]
    [SerializeField] private float _activationTime;
    [SerializeField] private int _maxHypnoLevel;
    [SerializeField, Tooltip("Value 1 correspond to the gain of 1 hypnitic level every seconds.")] private float _hypnoLevelSpeed = 1;
    [SerializeField] private int _maxPv;
    [SerializeField] private float _playerDamage;
    [SerializeField] private HealthBar _healthBar;
    [Header("Attack Phase")]
    [SerializeField] private float _delayBetweenAttack;

    [SerializeField] private int numberOfDisks = 2;
    private int diskPhase;
    [Header("Vulnerability phase")]
    [SerializeField] private float _vulnerabilityTime;
    private float _vulnerabilityTimer;
    
    private List<Barrier> barriers;
    private int barrierLevel;

    [Header("Shooting Cooldowns")]
    [SerializeField]
    private float diskShotCooldown = 1f;
    [SerializeField]
    private float hypnoShotCooldown = 0.1f;
    [SerializeField]
    private float vulnShotCooldown = 0.1f;
    
    [Header("References")]
    [SerializeField] private Transform bossBody;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _bossAnimator = gameObject.GetComponent<Animator>();

        for (int i = 0; i < PlayerManager.PlayerInfos.Count; i++)
        {
            _players[i].InitPlayer(PlayerManager.PlayerInfos[i]);
        }

        currentPlayersInput = new List<EButtonColor>{ EButtonColor.NONE, EButtonColor.NONE, EButtonColor.NONE, EButtonColor.NONE };
        _healthBar.InitializeHealthBar(_maxPv);
        _currentState = EBossState.NONE;

        _currentPV = _maxPv;

        Invoke(nameof(OnInitializationEnd), _activationTime);
    }

    private void Update()
    {
        // HYPONIQUE LEVEL
        if (_increaseHypnoLevel)
        {
            _currentHypnoLevel += Time.deltaTime * _hypnoLevelSpeed;
            _imgHypnoLevel.fillAmount = _currentHypnoLevel / _maxHypnoLevel;
            CheckHypnoticLevel();
        }

        // TIMER
        if (_timeElapse)
        {
            _timeInGame += Time.deltaTime;

            string minutes = ((int)(_timeInGame % 3600 / 60)).ToString("D2");
            string secound = ((int)_timeInGame % 60).ToString("D2");
            string milisecound = ((int)((_timeInGame - (int)_timeInGame) * 1000)).ToString("D3");

            _textTimer.text = string.Format("{0}.{1}.{2}", minutes, secound, milisecound);
        }

        if (_currentState == EBossState.VULNERABLE)
        {
            _vulnerabilityTimer += Time.deltaTime;
            if (_vulnerabilityTimer >= _vulnerabilityTime)
            {
                _vulnerabilityTimer = 0;
                _currentState = EBossState.NONE;
                StartCoroutine(DelayState(EBossState.ATTACK_DISQUE, 1f));
            }
        }

        // STATE MACHINE
        if (_previousState == _currentState) return;
        
        currentPlayersInput = new List<EButtonColor>{ EButtonColor.NONE, EButtonColor.NONE, EButtonColor.NONE, EButtonColor.NONE };
        
        switch (_currentState)
        {
            // Disque of color
            case EBossState.ATTACK_DISQUE:
                diskPhase = 0;
                _bossAnimator.SetBool("Pendule", true);
                _increaseHypnoLevel = true;
                _players.ForEach(p =>
                {
                    p.ChangeAttackParameters(diskShotCooldown); 
                    p.SetLaserTarget(_bossEyesTransform); 
                });
                InitDisqueAttack();
                break;

            // Minigame when the level of hypnose is at its maximum
            case EBossState.HYPNOTIC_PHASE:
                // TODO: Maybe disable completely the eyes?
                _bossEyes.ForEach(img => img.color = Color.white);
                _players.ForEach(p =>
                {
                    p.DestroyBeamInstant();
                    p.ChangeAttackParameters(hypnoShotCooldown);
                    // p.SetLaserTarget(_bossEyesTransform); 
                });
                _backgroundAnimator.SetBool("Psycho", true);
                _bossAnimator.SetBool("Pendule", false);
                _increaseHypnoLevel = false;
                BarrierScript.InitBarrierVisuals();
                // InitHypnoAttack();
                break;

            // Players can damage the boss
            case EBossState.VULNERABLE:
                _players.ForEach(p =>
                {
                    p.ChangeAttackParameters(vulnShotCooldown); 
                    p.SetLaserTarget(gameObject.transform); 
                });
                _bossEyes.ForEach(img => img.color = Color.white);
                _increaseHypnoLevel = false;
                break;

            // Boss Dead
            case EBossState.DEATH:
                // Animation
                // WARNING NEED DELAI BEFOR LOAD NEXT SCENE //
                _increaseHypnoLevel = false;
                _timeElapse = false;
                _bossAnimator.SetBool("Pendule", false);
                PlayerManager.SetEndTimer(_timeInGame);
                _players.ForEach(p => p.SetActivePlayer(false));
                StartCoroutine(LoadLeaderBoard());
                break;
        }

        _previousState = _currentState;
        _bossAnimator.SetInteger("State", (int)_currentState);
    }

    #region Animation Related
    public void OnInitializationEnd()
    {
        _timeElapse = true;
        _currentState = EBossState.ATTACK_DISQUE;
        _players.ForEach(p => p.SetActivePlayer(true));
        _bossAnimator.SetBool("Pendule", true);
    }

    public void OnRecoverEnd()
    {
        StartCoroutine(DelayState(EBossState.ATTACK_DISQUE, 0.5f));
    }
    #endregion

    public static void OnPlayerInput(int playerId, EButtonColor color)
    {
        Instance.currentPlayersInput[playerId] = color;
        switch (Instance._currentState)
        {
            case EBossState.ATTACK_DISQUE:
                Instance.CheckDestroyDisque();
                break;

            case EBossState.HYPNOTIC_PHASE:
                // Instance.CheckHypnoEnd();
                break;

            case EBossState.VULNERABLE:
                if (color != EButtonColor.NONE)
                    TakeDamage();
                break;
        }
    }

    private void CheckHypnoticLevel()
    {
        if(_currentHypnoLevel >= _maxHypnoLevel)
        {
            _currentState = EBossState.HYPNOTIC_PHASE;
            _increaseHypnoLevel = false;
            _currentHypnoLevel = 0;
        }
    }

    private IEnumerator DelayState(EBossState nextState, float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentState = nextState;
    }

    #region Attack Types
    private void InitDisqueAttack()
    {
        currentPlayersInput.ForEach(input => input = EButtonColor.NONE);

        colorSelected = new List<EButtonColor>() { EButtonColor.NONE, EButtonColor.NONE, EButtonColor.NONE, EButtonColor.NONE };
        List<EButtonColor> colorPossible = new List<EButtonColor>{
            EButtonColor.BLUE, EButtonColor.BLUE,
            EButtonColor.GREEN, EButtonColor.GREEN,
            EButtonColor.RED, EButtonColor.RED,
            EButtonColor.YELLOW, EButtonColor.YELLOW,
        };

        for (int i = 0; i < 4; i++)
        {
            int random = Random.Range(0, colorPossible.Count);
            colorSelected[i] = colorPossible[random];
            colorPossible.RemoveAt(random);
            _bossEyes[i].color = PlayerManager.GetInputColor(colorSelected[i]);
        }
    }

    private void CheckDestroyDisque()
    {
        List<EButtonColor> playedColor = new List<EButtonColor>(colorSelected);
        foreach(EButtonColor color in currentPlayersInput)
        {
            if (color == EButtonColor.NONE)
                return;

            if (playedColor.Exists(p => p == color))
                playedColor.Remove(color);
        }

        if (playedColor.Count == 0)
        {
            diskPhase++;
            if (diskPhase == numberOfDisks)
            {
                // Animation
                _currentState = EBossState.NONE;
                StartCoroutine(DelayState(EBossState.VULNERABLE, 1f));
            }
            else
            {
                InitDisqueAttack();
            }
            
        }
        /*else
        {
            // Animation Block
            // Réinitialisation
            InitDisqueAttack();
        }*/
    }
    
    private void InitHypnoAttack()
    {
        barrierLevel = 0;
        barriers = new List<Barrier>(2) { new(), new() };
        BarrierScript.InitBarrierVisuals(barriers);
        _players.ForEach(p =>
        {
            p.SetLaserTarget(BarrierScript.GetPointsTransforms(barrierLevel));
        });
    }

    private void CheckHypnoEnd()
    {
        Barrier currBarrier = barriers[barrierLevel];
        for (int i = 0; i < 4; i++)
        {
            if (currBarrier.neededColors[i] != currentPlayersInput[i])
            {
                // Not all good colors
                return;
            }
        }

        BarrierScript.RemoveBarrier(barrierLevel);
        barrierLevel++;
        if (barrierLevel == 2)
        {
            //BarrierScript.SetState(false);
            _backgroundAnimator.SetBool("Psycho", false);
            _currentState = EBossState.NONE;
            StartCoroutine(DelayState(EBossState.ATTACK_DISQUE, 1f));
            return;
        }

        _players.ForEach(p =>
        {
            p.SetLaserTarget(BarrierScript.GetPointsTransforms(barrierLevel));
        });
    }

    private static void TakeDamage()
    {
        Instance._currentPV -= Instance._playerDamage;
        Instance._healthBar.UpdateHealthValue(Instance._currentPV);

        // Death
        if (Instance._currentPV <= 0)
            Instance._currentState = EBossState.DEATH;
    }

    public static void DoShake()
    {
        Instance.bossBody.DOShakePosition(0.5f, new Vector3(1, 1, 0), 10, 60);
    }

    public static void QuitPsychoPhase()
    {
        Instance._currentState = EBossState.NONE;
        Instance._backgroundAnimator.SetBool("Psycho", false);
        Instance.StartCoroutine(Instance.DelayState(EBossState.ATTACK_DISQUE, 1f));
    }
    
    #endregion
    
    // TODO: REMOVE
    [ContextMenu("Go Hypno")]
    public void GoHypno()
    {
        _currentState = EBossState.HYPNOTIC_PHASE;
    }
    
    [ContextMenu("Go Vuln")]
    public void GoVuln()
    {
        _currentState = EBossState.VULNERABLE;
    }
    
    [ContextMenu("ResetHypno")]
    public void ResetHypno()
    {
        InitHypnoAttack();
    }
    
    [ContextMenu("DestroyBarrier")]
    public void DestroyBarrier()
    {
        for (int i = 0; i < 4; i++)
        {
            OnPlayerInput(i, barriers[barrierLevel].neededColors[i]);
        }
    }

    private IEnumerator LoadLeaderBoard()
    {
        yield return new WaitForSeconds(2);
        CustomSceneManager.LoadEndGame(true);
    }
}


