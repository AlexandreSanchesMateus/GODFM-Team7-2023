using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiskScript : MonoBehaviour
{

    private static DiskScript Instance;

    [SerializeField]
    public List<PlayerManager.EPlayerColor> vulnColors = new (4);

    public int diskLevel = 1;
    public int rotationSpeed = -10;
    public float rotationAngle = 0;
    
    // DEBUG
    private float timer = 1;
    private int index;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitVulnColors();
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayDebugAnimation();
        PlayRotationAnimation(Time.fixedDeltaTime);
    }

    void PlayDebugAnimation()
    {
        if (timer >= 1)
        {
            switch (vulnColors[index])
            {
                case PlayerManager.EPlayerColor.BLUE:
                    _renderer.color = PlayerManager.ColorBlue;
                    break;
                case PlayerManager.EPlayerColor.RED:
                    _renderer.color = PlayerManager.ColorRed;
                    break;
                case PlayerManager.EPlayerColor.GREEN:
                    _renderer.color = PlayerManager.ColorGreen;
                    break;
                case PlayerManager.EPlayerColor.YELLOW:
                    _renderer.color = PlayerManager.ColorYellow;
                    break;
            }
            timer = 0;
            index += index == 3 ? -3 : 1;
        }
        else
        {
            timer += Time.fixedDeltaTime;
        }
    }

    void PlayRotationAnimation(float deltaTime)
    {
        rotationAngle += (deltaTime * rotationSpeed * diskLevel) % 360;
        Quaternion angle = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
        transform.rotation = angle;
    }

    [ContextMenu("Init Colors")]
    void InitVulnColors()
    {
        vulnColors = new (4);
        for (int i = 0; i < 4; i++)
        {
            int rand = Random.Range(0, 4);
            PlayerManager.EPlayerColor selectedColor = (PlayerManager.EPlayerColor)rand;
            while (vulnColors.FindAll(x => x == selectedColor).Count == 3)
            {
                rand = Random.Range(0, 4);
                selectedColor = (PlayerManager.EPlayerColor)rand;
            }
            vulnColors.Add((PlayerManager.EPlayerColor)rand);
        }
    }

    /// <summary>
    /// Returns True if all colors were matched and false otherwise
    /// </summary>
    /// <param name="colors"> List of colors sent by the player </param>
    /// <param name="autoRefresh"> Whether or not to automatically change colors </param>
    /// <returns></returns>
    public static bool OnPlayerShooting(List<PlayerManager.EPlayerColor> colors, bool autoRefresh)
    {
        colors.Sort();
        Instance.vulnColors.Sort();
        if (colors == Instance.vulnColors)
        {
            if (autoRefresh)
            {
                Instance.InitVulnColors();
            }

            Instance.diskLevel++;
            return true;
        }
        return false;
    }
    
}
