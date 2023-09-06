using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private String PlayerName;
    private List<PlayerManager.EPlayerColor> _pressedColors = new(3);
    
    private PlayerManager.PlayerInfo _playerInfo;

    [SerializeField] private int playerId;

    // To be able to put cooldown on presses
    // Should be handled by gamemanager imo to be able to easely use game state
    public bool useFrames = false;
    public int FramesBetweenShots;
    public int TimeBetweenShots;

    public float TimeBeforeHold = 0.2f; // Time before input is detected as held

    private float shotTimer;
    private float holdTimer;
    private int frameCount;

    public GameObject shotPrefab;
    public Transform target;


    // Start is called before the first frame update
    void Start()
    {
         _playerInfo = PlayerManager.PlayerInfos[playerId];   //doesn't seem to work
        //_playerInfo.SetKeyColorDic();
        shotTimer = TimeBetweenShots;
        frameCount = FramesBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        bool canPress = false;
        if (useFrames)
        {
            frameCount++;
            if (frameCount >= FramesBetweenShots)
            {
                canPress = true;
            }
        }
        else
        {
            shotTimer += Time.deltaTime;
            if (shotTimer >= TimeBetweenShots)
            {
                canPress = true;
            }
        }

        foreach (KeyCode keyCode in _playerInfo.KeyColorDic.Keys.ToArray())
        {
            if (!_playerInfo.KeyColorDic.Keys.Contains(keyCode))
            {
                return;
            }
            
            PlayerManager.EPlayerColor keycodeColor = _playerInfo.KeyColorDic[keyCode];
            
            if (canPress && Input.GetKeyDown(keyCode))
            {
                _pressedColors.Add(keycodeColor);
                if (useFrames)
                {
                    frameCount = 0;
                }
                else
                {
                    shotTimer = 0;
                }
                    
                // Debug.Log($"Pressed color '{inputColor}' of player '{PlayerName}'");
                OnInputDetected(keycodeColor);
            }

            if (Input.GetKeyUp(keyCode))
            {
                // Debug.Log($"unPressed color '{keycodeColor}' of player '{PlayerName}'");
                _pressedColors.Remove(keycodeColor);
                holdTimer = 0;
            }
        }
        
        if (_pressedColors.Count != 0)
        {
            if (holdTimer < TimeBeforeHold)
            {
                holdTimer += Time.deltaTime;
                return;
            }
            
            PlayerManager.EPlayerColor lastPressedInput = _pressedColors[0];
            Debug.Log($"Button still pressed color '{lastPressedInput}' of player '{PlayerName}'");
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

    void OnInputDetected(PlayerManager.EPlayerColor inputColor)
    {
        Color32 color = GetInputColor(inputColor);
        

        Vector3 PivotPoint = (transform.position - target.position)/2;
        
        Quaternion angle = Quaternion.AngleAxis(Mathf.Atan2(PivotPoint.y, PivotPoint.x) * Mathf.Rad2Deg, Vector3.forward);
        
        GameObject go = Instantiate(shotPrefab, PivotPoint, angle);
        go.GetComponent<SpriteRenderer>().color = color;
        StartCoroutine(DestroyShot(go));
    }

    IEnumerator DestroyShot(GameObject go)
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(go);
    }

}
