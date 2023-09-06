using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public class MenuController : MonoBehaviour
    {
        [FormerlySerializedAs("ButtonDisplays")] [SerializeField]
        private List<ButtonDisplay> buttonDisplays;

        private readonly List<PlayerButton> _playerButtons = new(4);
        private readonly List<PlayerButton> _playersReady = new(4);

        [SerializeField]
        private float readyTime = 2f;
    

        private void Start()
        {
            for (int i = 0; i < 4; i++)
            {
                PlayerManager.PlayerInfo playerInfo = PlayerManager.PlayerInfos[i];
                ButtonDisplay buttonDisplay = buttonDisplays[i];
            
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


    internal class PlayerButton
    {
        //public int PlayerID { get; private set; }
        private readonly PlayerManager.PlayerInfo _info;
        private readonly ButtonDisplay _buttonDisplay;
        private readonly List<EButtonColor> _pressedColors = new(3);
        private readonly Dictionary<KeyCode, Coroutine> _coroutines;

        private float _readyTimer;
        private readonly float _readyTime;

        public PlayerButton(PlayerManager.PlayerInfo pInfo, ButtonDisplay display, float readyTime)
        {
            _info = pInfo;
            _buttonDisplay = display;
            _readyTime = readyTime;
            _coroutines = new Dictionary<KeyCode, Coroutine>();
        }

        public void RefreshInputs()
        {
            foreach (var keyCode in _info.KeyColorDic.Keys)
            {            
                EButtonColor keycodeColor = _info.KeyColorDic[keyCode];

                if (Input.GetKeyDown(keyCode))
                {
                    SetButtonState(_info.KeyPosDic[keyCode], true);
                
                    _pressedColors.Add(keycodeColor);
                    Color32 color = PlayerManager.GetInputColor(keycodeColor);
                    StartButtonColouring(keyCode, color); // Fade effect of the colour of a button
                }
            
                if (Input.GetKeyUp(keyCode))
                {
                    SetButtonState(_info.KeyPosDic[keyCode], false);
                
                    _pressedColors.Remove(keycodeColor);
                    StartButtonColouring(keyCode, Color.white);
                }
            }
        }
    
        public bool VerifyInputs(float deltaTime)
        {
            if (_pressedColors.Count >= 3)
            {
                _readyTimer += deltaTime;
                if (_readyTimer >= _readyTime)
                {
                    return true;
                }

                return false;
            }

            _readyTimer = 0;
            return false;
        }

        private void SetButtonState(string position, bool state)
        {
            switch (position)
            {
                case "Left":
                    _buttonDisplay.SetLeftButton(state);
                    break;
                case "Right":
                    _buttonDisplay.SetRightButton(state);
                    break;
                case "Vertical":
                    _buttonDisplay.SetVerticalButton(state);
                    break;
            }
        }

        private void StartButtonColouring(KeyCode keyCode, Color targetColor)
        {
            Image img = _buttonDisplay.posImageDic[_info.KeyPosDic[keyCode]];

            if (_coroutines.TryGetValue(keyCode, out Coroutine co))
            {
                _buttonDisplay.StopCoroutine(co);
            }
            _coroutines[keyCode] = _buttonDisplay.StartCoroutine(ColorButton(img, img.color, targetColor, _readyTime, _readyTime / (_readyTime*10)));
        }
 
        // time in seconds
        IEnumerator ColorButton(Image img,Color startColor, Color targetColor, float time, float t)
        {
            Color lerpedColor = Color.Lerp(startColor, targetColor, t);
            img.color = lerpedColor;
            yield return new WaitForSeconds(_readyTime/ (_readyTime*10));
            if (t >= time)
            {
                yield return null;
            }
            else
            {
                // TODO: Coroutines overlapping if released before one ends, Well... actually it could be a feature
                _buttonDisplay.StartCoroutine(ColorButton(img, startColor, targetColor, time, t + time / (_readyTime*10)));
            }
        }
    
    }
}