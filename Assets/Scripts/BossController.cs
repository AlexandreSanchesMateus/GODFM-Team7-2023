using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossController : MonoBehaviour
{
    private static BossController Instance;
    public static EBossState CurrentState => Instance._currentState;

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

    // Attack
    private List<EButtonColor> currentPlayersInput;
    EButtonColor[] colorSelected = new EButtonColor[4];

    private float _currentPV;

    private bool _increaseHypnoLevel = false;
    private float _currentHypnoLevel;

    private bool _timeElapse = false;
    private float _timeInGame = 0.0001F;

    [Header("UI")]
    [SerializeField] private Image _imgHypnoLevel;
    [SerializeField] private TextMeshProUGUI _textTimer;
    [SerializeField] private List<Image> _bossEyes;
    [Header("General Settings")]
    [SerializeField] private int _maxHypnoLevel;
    [SerializeField, Tooltip("Value 1 correspond to the gain of 1 hypnitic level every seconds.")] private float _hypnoLevelSpeed = 1;
    [SerializeField] private int _maxPv;
    [SerializeField] private Image fill;
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

        currentPlayersInput = new List<EButtonColor>(PlayerManager.PlayerInfos.Count);
        fill.fillAmount = 1;
        _currentState = EBossState.NONE;
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

        // STATE MACHINE
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
                InitDisqueAttack();
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
                
                // Animation
                // WARNING NEED DELAI BEFOR LOAD NEXT SCENE //

                _timeElapse = false;
                PlayerManager.SetEndTimer(_timeInGame);
                CustomSceneManager.LoadEndGame(true);
                break;
        }

        _previousState = _currentState;
    }

    #region Animation Related
    public void OnInitializationEnd()
    {
        _timeElapse = true;
        _currentState = EBossState.CHOSE_ATTACK;
    }

    public void OnRecoverEnd()
    {
        StartCoroutine(DelayState(EBossState.CHOSE_ATTACK, 0.5f));
    }
    #endregion

    public static void OnPlayerInput(int playerId, EButtonColor color)
    {
        Instance.currentPlayersInput[1] = color;
        switch (Instance._currentState)
        {
            case EBossState.ATTACK_DISQUE:
                Instance.CheckDestroyDisque();
                break;

            case EBossState.HYPNOTIC_PHASE:
                break;

            case EBossState.VULNERABLE:
                break;

            default:
                break;
        }
    }

    private void CheckHypnoticLevel()
    {
        if(_currentHypnoLevel >= _maxHypnoLevel)
        {
            _currentState = EBossState.HYPNOTIC_PHASE;
            _increaseHypnoLevel = false;
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
        EButtonColor[] colorSelected = new EButtonColor[4];
        List<EButtonColor> colorPossible = new List<EButtonColor>{
            EButtonColor.BLUE, EButtonColor.BLUE,
            EButtonColor.GREEN, EButtonColor.GREEN,
            EButtonColor.RED, EButtonColor.RED,
            EButtonColor.YELLOW, EButtonColor.YELLOW,
        };

        for (int i = 0; i < 4; i++)
        {
            int random = Random.Range(0, colorPossible.Count);
            colorPossible.RemoveAt(random);
            colorSelected[i] = colorPossible[random];
            _bossEyes[i].color = PlayerManager.GetInputColor(colorSelected[i]);
        }
    }

    private void CheckDestroyDisque()
    {
        List<EButtonColor> playedColor = new List<EButtonColor>(colorSelected);
        foreach(EButtonColor color in currentPlayersInput)
        {
            /*if (color == PlayerManager.EPlayerColor.NONE)
                return;*/

            if(playedColor.Exists(p => p == color))
                playedColor.Remove(color);
        }

        if (playedColor.Count == 0)
        {
            // Animation
            _currentState = EBossState.NONE;
            StartCoroutine(DelayState(EBossState.VULNERABLE, 1f));
        }
        else
        {
            // Animation Block
            // Réinitialisation
            InitDisqueAttack();
        }
    }

    private IEnumerator HypnotiqueAttack()
    {
        yield return null;
    }
    #endregion
}
