using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private String PlayerName;
    private List<PlayerManager.EPlayerColor> inputPlayer;

    private PlayerManager.PlayerInfo _playerInfo;

    public int ID => _playerInfo.ID;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode keyCode in _playerInfo.KeyColorDic.Keys.ToArray())
        {
            PlayerManager.EPlayerColor inputColor = _playerInfo.KeyColorDic[keyCode];

            if (Input.GetKeyDown(keyCode))
            {
                inputPlayer.Add(inputColor);
                //OnInputDetected(inputColor);
            }

            if (Input.GetKeyUp(keyCode))
            {
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
