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
    private static PlayerManager Instance;

    public static Color32 ColorBlue => Instance._blue;
    public static Color32 ColorRed => Instance._red;
    public static Color32 ColorGreen => Instance._green;
    public static Color32 ColorYellow => Instance._yellow;
    public static List<PlayerInfo> PlayerInfos => Instance._setupPlayers;

    [Header("Color Reference")]
    [SerializeField] private Color32 _blue;
    [SerializeField] private Color32 _red;
    [SerializeField] private Color32 _green;
    [SerializeField] private Color32 _yellow;
    [SerializeField] private List<PlayerInfo> _setupPlayers;

    public static float Timer { get; private set; }

    [Serializable]
    public class PlayerInfo
    {
        public int ID { get; private set; }
        public KeyCode LKey => _keyLeft;
        public KeyCode RKey => _keyRight;
        public KeyCode VKey => _keyVerticale;
        public Dictionary<KeyCode, EButtonColor> KeyColorDic { get; private set; }
        public Dictionary<KeyCode, string> KeyPosDic { get; private set; }

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
            KeyColorDic = new() {
                { _keyLeft, _colorLeft},
                {_keyRight, _colorRight},
                {_keyVerticale,_colorVerticale}
            };
        }

        public void SetKeyPosDic()
        {
            KeyPosDic = new() {
                { _keyLeft, "Left"},
                {_keyRight, "Right"},
                {_keyVerticale,"Vertical"}
            };
        }
        
    }
    #endregion

    #region Unity Methode
    private void Awake()
    {
        if(Instance != null)
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
                playerInfo.SetKeyPosDic();
            }
            
        }
    }
    #endregion

    public static Color32 GetInputColor(EButtonColor inputColor)
    {
        switch (inputColor)
        {
            case EButtonColor.RED:
                return PlayerManager.ColorRed;
            case EButtonColor.BLUE:
                return PlayerManager.ColorBlue;
            case EButtonColor.GREEN:
                return PlayerManager.ColorGreen;
            case EButtonColor.YELLOW:
                return PlayerManager.ColorYellow;
        }

        throw new Exception("Wrong Input Color");
    }

    public static void SetEndTimer(float endTimer) => Timer = endTimer;
}
