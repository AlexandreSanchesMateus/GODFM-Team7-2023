using System;
using System.Collections.Generic;
using UnityEngine;



public enum EButtonColor
{
    BLUE,
    RED,
    GREEN,
    YELLOW,
    NONE,
}

public class PlayerManager : MonoBehaviour
{
    #region Variables

    // TODO: Change back to public later
    public static PlayerManager Instance;

    public static Color32 ColorBlue => Instance._blue;
    public static Color32 ColorRed => Instance._red;
    public static Color32 ColorGreen => Instance._green;
    public static Color32 ColorYellow => Instance._yellow;
    public static List<PlayerInfo> PlayerInfos => Instance._setupPlayers;

    [Header("Color Reference")] [SerializeField]
    private Color32 _blue;

    [SerializeField] private Color32 _red;
    [SerializeField] private Color32 _green;
    [SerializeField] private Color32 _yellow;
    [SerializeField] private List<PlayerInfo> _setupPlayers;

    public static float Timer { get; private set; }
    [SerializeField] public List<float> scores = new(5);

    public GameObject Projectile;



    [Serializable]
    public class PlayerInfo
    {
        public int ID { get; private set; }
        public EButtonColor PlayerColor { get; private set; }
        public Dictionary<KeyCode, EButtonColor> KeyColorDic { get; private set; }



        [SerializeField] private bool _buttonInverted;

        [Header("Left Button")]
        [SerializeField] private EButtonColor _colorLeft;
        [SerializeField] private KeyCode _keyLeft;
        [Header("Right Button")]
        [SerializeField] private EButtonColor _colorRight;
        [SerializeField] private KeyCode _keyRight;
        [Header("Vertical Button")]
        [SerializeField] private EButtonColor _colorVerticale;
        [SerializeField] private KeyCode _keyVerticale;

        public void SetPlayerID(int newId) => ID = newId;

        public void SetKeyColorDic()
        {
            KeyColorDic = new()
            {
                { _keyLeft, _colorLeft },
                { _keyRight, _colorRight },
                { _keyVerticale, _colorVerticale }
            };

            for (int i = 0; i < 4; i++)
            {
                if (!KeyColorDic.ContainsValue((EButtonColor)i))
                    PlayerColor = (EButtonColor)i;
            }
        }
    }

    #endregion

    #region Unity Methode

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialisation player's ID and keyColorDic
            for (int i = 0; i < _setupPlayers.Count; i++)
            {
                PlayerInfo playerInfo = _setupPlayers[i];
                playerInfo.SetPlayerID(i);
                playerInfo.SetKeyColorDic();
            }

        }
    }

    #endregion

    public static Color32 GetInputColor(EButtonColor inputColor)
    {
        switch (inputColor)
        {
            case EButtonColor.RED:
                return ColorRed;
            case EButtonColor.BLUE:
                return ColorBlue;
            case EButtonColor.GREEN:
                return ColorGreen;
            case EButtonColor.YELLOW:
                return ColorYellow;
            default:
                return Color.white;
        }

        throw new Exception("Wrong Input Color");
    }

    public static void SetEndTimer(float endTimer)
    {
        Timer = endTimer;
    }

    [ContextMenu("Simulate end run (800)")]
    public void testFunc()
    {
        SetEndTimer(800f);
    }
}
