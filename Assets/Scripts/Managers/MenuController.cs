using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private List<ButtonDisplay> ButtonDisplays;

    private List<PlayerButton> _playerButtons = new(4);
    private List<PlayerButton> _playersReady = new(4);

    [SerializeField]
    private float readyTime = 2f;
    

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerManager.PlayerInfo playerInfo = PlayerManager.PlayerInfos[i];
            
            ButtonDisplay buttonDisplay = ButtonDisplays[i];
            
            _playerButtons.Add(new PlayerButton(playerInfo, buttonDisplay, readyTime));
        }
    }


    private void Update()
    {
        foreach (PlayerButton playerButton in _playerButtons)
        {
            // For tests =================================
            if (_playersReady.Contains(playerButton))
            {
                continue;
            }
            // ===========================================
            playerButton.RefreshInputs();
            if (playerButton.VerifyInputs(Time.deltaTime) && !_playersReady.Contains(playerButton))
            {
                _playersReady.Add(playerButton);
            }
        }

        if (_playersReady.Count >= 4)
        {
            // Can Start
            CustomSceneManager.LoadGame(true);
        }
    }
}


class PlayerButton
{
    public int PlayerID { get; private set; }
    public PlayerManager.PlayerInfo info;
    public ButtonDisplay ButtonDisplay;
    private List<EButtonColor> _pressedColors = new(3);
    private Dictionary<KeyCode, Coroutine> coroutines;

    private float readyTimer = 0;
    private float readyTime;

    public PlayerButton(PlayerManager.PlayerInfo pInfo, ButtonDisplay display, float readyTime)
    {
        info = pInfo;
        ButtonDisplay = display;
        this.readyTime = readyTime;

        coroutines = new Dictionary<KeyCode, Coroutine>();
    }

    public void RefreshInputs()
    {
        foreach (var keyCode in info.KeyColorDic.Keys)
        {            
            EButtonColor keycodeColor = info.KeyColorDic[keyCode];
            Image img = ButtonDisplay.posImageDic[info.KeyPosDic[keyCode]];

            if (Input.GetKeyDown(keyCode))
            {
                switch (info.KeyPosDic[keyCode])
                {
                    case "Left":
                        ButtonDisplay.SetLeftButton(true);
                        break;
                    case "Right":
                        ButtonDisplay.SetRightButton(true);
                        break;
                    case "Vertical":
                        ButtonDisplay.SetVerticalButton(true);
                        break;
                }
                
                _pressedColors.Add(keycodeColor);
                Color32 color = PlayerManager.GetInputColor(keycodeColor);
                if (coroutines.TryGetValue(keyCode, out Coroutine co))
                {
                    ButtonDisplay.StopCoroutine(co);
                }
                coroutines[keyCode] = ButtonDisplay.StartCoroutine(ColorButton(img, img.color, color, 2, 0));
            }
            
            if (Input.GetKeyUp(keyCode))
            {
                switch (info.KeyPosDic[keyCode])
                {
                    case "Left":
                        ButtonDisplay.SetLeftButton(false);
                        break;
                    case "Right":
                        ButtonDisplay.SetRightButton(false);
                        break;
                    case "Vertical":
                        ButtonDisplay.SetVerticalButton(false);
                        break;
                }
                
                _pressedColors.Remove(keycodeColor);
                if (coroutines.TryGetValue(keyCode, out Coroutine co))
                {
                    ButtonDisplay.StopCoroutine(co);
                }
                coroutines[keyCode] = ButtonDisplay.StartCoroutine(ColorButton(img, img.color, Color.white, 2, 0));
            }
            
        }
    }

    public bool VerifyInputs(float deltaTime)
    {
        if (_pressedColors.Count >= 3)
        {
            readyTimer += deltaTime;
            if (readyTimer >= readyTime)
            {
                return true;
            }

            return false;
        }

        readyTimer = 0;
        return false;
    }

    // time in seconds
    IEnumerator ColorButton(Image img,Color startColor, Color targetColor, int time, float t)
    {
        Color lerpedColor = Color.Lerp(startColor, targetColor, t);
        img.color = lerpedColor;
        yield return new WaitForSeconds(0.1f);
        if (t >= time)
        {
            yield return null;
        }
        else
        {
            // TODO: Coroutines overlaping if released before one ends, Well... actually it could be a feature
            ButtonDisplay.StartCoroutine(ColorButton(img, startColor, targetColor, time, t + time / 10f));
        }
    }
    
}
