using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
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

    [System.Serializable]
    public class PlayerInfo
    {
        public int ID { get; private set; }

        [SerializeField] private bool _buttonInverted;

        [Header("Left Button")]
        [SerializeField] private EPlayerColor _colorLeft;
        [SerializeField] private KeyCode _keyLeft;

        [Header("RIght Button")]
        [SerializeField] private EPlayerColor _colorRight;
        [SerializeField] private KeyCode _keyRighr;

        [Header("Verticale Button")]
        [SerializeField] private EPlayerColor _colorVerticale;
        [SerializeField] private KeyCode _keyVerticale;

        public void SetPlayerID(int newId) => ID = newId;
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

            // Initialisation player's ID
            for (int i = 0; i < _setupPlayers.Count; i++)
            {
                _setupPlayers[i].SetPlayerID(i);
            }
        }
    }
    #endregion
}
