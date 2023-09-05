using System;
using System.Collections.Generic;
using UnityEngine;

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

    public enum EPlayerColor
    {
        BLUE,
        RED,
        GREEN,
        YELLOW
    }

    [Serializable]
    public class PlayerInfo
    {
        public int ID { get; private set; }
        public Dictionary<KeyCode, EPlayerColor> KeyColorDic { get; private set; }


        [SerializeField] private bool _buttonInverted;

        [Header("Left Button")]
        [SerializeField] private EPlayerColor _colorLeft;
        [SerializeField] private KeyCode _keyLeft;

        [Header("RIght Button")]
        [SerializeField] private EPlayerColor _colorRight;
        [SerializeField] private KeyCode _keyRight;

        [Header("Vertical Button")]
        [SerializeField] private EPlayerColor _colorVerticale;
        [SerializeField] private KeyCode _keyVerticale;


        public void SetPlayerID(int newId) => ID = newId;

        public void SetKeyColorDic()
        {
            KeyColorDic = new() {
                { _keyLeft, _colorLeft}, {_keyRight, _colorRight}, {_keyVerticale, _colorVerticale}};

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
            }
            
        }
    }
    #endregion
}
