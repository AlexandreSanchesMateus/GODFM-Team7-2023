using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private List<ButtonDisplay> ButtonDisplays;



    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {

            PlayerManager.PlayerInfo playerInfo = PlayerManager.PlayerInfos[i];
            
            
            ButtonDisplay buttonDisplay = ButtonDisplays[i];

        }
    }


    private void Update()
    {
        foreach (PlayerManager.PlayerInfo info in PlayerManager.PlayerInfos)
        {
            foreach (var keyCode in info.KeyColorDic.Keys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    
                }
            }
        }
    }
}


class PlayerButton
{
    public int PlayerID { get; private set; }
    public PlayerManager.PlayerInfo info;
    public ButtonDisplay ButtonDisplay;
    private List<EButtonColor> _pressedColors = new(3);

    public void RefreshInputs()
    {
        foreach (var keyCode in info.KeyColorDic.Keys)
        {            
            EButtonColor keycodeColor = info.KeyColorDic[keyCode];
            
            if (Input.GetKeyDown(keyCode))
            {
                _pressedColors.Add(keycodeColor);
                
            }
            
            if (Input.GetKeyUp(keyCode))
            {
                _pressedColors.Remove(keycodeColor);
            }
            
        }
    }

    public int VerifyInputs()
    {
        return _pressedColors.Count;
    }
    
}
