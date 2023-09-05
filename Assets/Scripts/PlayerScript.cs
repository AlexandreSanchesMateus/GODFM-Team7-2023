using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private String PlayerName;
    private List<PlayerManager.EPlayerColor> inputPlayer = new(3);

    [SerializeField]
    private PlayerManager.PlayerInfo _playerInfo;

    [SerializeField] private int playerId;

    
    
    // Start is called before the first frame update
    void Start()
    {
        // _playerInfo = PlayerManager.PlayerInfos[playerId];   doesn't seem to work
        _playerInfo.SetKeyColorDic();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode keyCode in _playerInfo.KeyColorDic.Keys.ToArray())
        {
            if (!_playerInfo.KeyColorDic.Keys.Contains(keyCode))
            {
                return;
            }
            
            PlayerManager.EPlayerColor inputColor = _playerInfo.KeyColorDic[keyCode];

            if (Input.GetKeyDown(keyCode))
            {
                inputPlayer.Add(inputColor);
                // Debug.Log($"Pressed color '{inputColor}' of player '{PlayerName}'");
                //OnInputDetected(inputColor);
            }

            if (Input.GetKeyUp(keyCode))
            {
                // Debug.Log($"unPressed color '{inputColor}' of player '{PlayerName}'");
                inputPlayer.Remove(inputColor);
            }
            
        }
    }

    Color32 GetInputColor(PlayerManager.EPlayerColor inputColor)
    {
        switch (inputColor)
        {
            case PlayerManager.EPlayerColor.RED:
                return PlayerManager.ColorRed;
            case PlayerManager.EPlayerColor.BLUE:
                return PlayerManager.ColorBlue;
            case PlayerManager.EPlayerColor.GREEN:
                return PlayerManager.ColorGreen;
            case PlayerManager.EPlayerColor.YELLOW:
                return PlayerManager.ColorYellow;
        }

        throw new Exception("Wrong Input Color");
    }

    void OnInputDetected(PlayerManager.EPlayerColor color)
    {
        
    }
}
