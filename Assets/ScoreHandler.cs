using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    private static ScoreHandler _instance;
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private Transform scoreParent;
    [FormerlySerializedAs("playerScoreText")] [SerializeField] private TMP_Text playerYourScoreText;
    private TMP_Text playerScoreboardText;

    [SerializeField] private Color playerScoreColor;
    [SerializeField] private Color otherScoreColor;

    private float sizeIncrement = 0.02f;
    private int sizeMultiplier = 5;
    private float baseFontSize;


    private void Awake()
    {
        _instance = this;
        baseFontSize = scorePrefab.GetComponent<TMP_Text>().fontSize;
    }

    private void Start()
    {
        PlayerManager.SetEndTimer(600f);
        float endTimer = PlayerManager.Timer;
        
        List<float> scoresTemp = new(6);
        for (int i = 0; i < 5; i++)
        {
            scoresTemp.Add(PlayerPrefs.GetFloat($"Score{i}", 0f));
        }
        //scoresTemp.AddRange(PlayerManager.Instance.scores);
        scoresTemp.RemoveAll(x => Math.Abs(x - endTimer) < 0.0001f);
        scoresTemp.Add(endTimer);
        scoresTemp.RemoveAll(x => x == 0f);
        scoresTemp.Sort();
        
        List<float> scoresDisplayed = scoresTemp.GetRange(0, Math.Min(5, scoresTemp.Count));
        
        bool found = false;
        int index = 0;
        foreach (float time in scoresDisplayed)
        {
            PlayerPrefs.SetFloat($"Score{index}", time);
            if (!found && Math.Abs(endTimer - time) < 0.0001f)
            {
                AddScore(time, index+1, true);
                found = true;
            }
            else
            {
                AddScore(time, index+1);
            }

            index++;
        }
    }

    private void FixedUpdate()
    {
        if (playerScoreboardText == null) return;

        if (playerScoreboardText.fontSize >= 1.5f*baseFontSize)
        {
            sizeMultiplier = -5;
        }
        else if (playerScoreboardText.fontSize <= 0.9f*baseFontSize)
        {
            sizeMultiplier = 5;
        }

        playerScoreboardText.fontSize += sizeIncrement * sizeMultiplier;
    }

    public static void SetPlayerScore(float time)
    {
        _instance.playerYourScoreText.text = $"Your Score: {FormatTime(time)}";
        _instance.playerYourScoreText.color = _instance.playerScoreColor;
    }

    public static void AddScore(float time, int rank, bool isPlayer = false)
    {
        TMP_Text scoreText = Instantiate(_instance.scorePrefab, _instance.scoreParent).GetComponent<TMP_Text>();
        scoreText.color = isPlayer ? _instance.playerScoreColor : _instance.otherScoreColor;
        if (isPlayer) _instance.playerScoreboardText = scoreText;
        //scoreText.fontSize *= isPlayer ? 1.5f : 1f;

        scoreText.text = $"{rank}.  {FormatTime(time)}";
    }

    private static string FormatTime(float time)
    {
        string minutes = ((int)(time % 3600 / 60)).ToString("D2");
        string second = ((int)time % 60).ToString("D2");
        string millisecond = ((int)((time - (int)time) * 1000)).ToString();

        return $"{minutes}:{second}.{millisecond}";
    }
}
