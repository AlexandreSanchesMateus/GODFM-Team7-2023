using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Variables
    private static PlayerManager Instance;

    private static List<PlayerInfo> PlayerInfos => Instance._setupPlayers;
    [SerializeField] private List<PlayerInfo> _setupPlayers;

    [System.Serializable]
    public class PlayerInfo
    {
        public int ID { get; private set; }

        public Color32 Color1 => _firstColor;
        public Color32 Color2 => _secondColor;
        public Color32 Color3 => _thirdColor;

        [SerializeField] private Color32 _firstColor;
        [SerializeField] private Color32 _secondColor;
        [SerializeField] private Color32 _thirdColor;

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
