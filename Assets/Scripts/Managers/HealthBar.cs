using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private List<Image> _sections;
    [SerializeField] private Color32 _sectionColor;
    [SerializeField] private Sprite _normal;
    [SerializeField] private Sprite _damaged;
    [SerializeField] private Sprite _destroy;

    private float _toalePV;
    private float _sectionValue;

    private int _currentSection;
    private Coroutine _pulseRoutine;

    public void InitializeHealthBar(int totalHealth)
    {
        _toalePV = totalHealth;
        _sectionValue = totalHealth / 10f;

        _sections.ForEach(sec => { sec.sprite = _normal; sec.color = _sectionColor; });
        _currentSection = 9;
    }

    public void UpdateHealthValue(float currentHealth)
    {
        float percentCurrHealth = currentHealth / _toalePV * 100; 
        
        int sectionDamaged = (int)(percentCurrHealth/10);
        if (sectionDamaged == 10)
        {
            sectionDamaged = 9;
        }
        
        Debug.Log($"Current HEatlth {percentCurrHealth}%");

        float pourcent = (percentCurrHealth - sectionDamaged * 10) * 10;

        // Remove last section
        if (sectionDamaged != _currentSection)
        {
            Debug.Log("Changing Section");
            StopAllCoroutines();
            transform.DOKill();
            transform.DOShakePosition(0.15f);
            _sections[_currentSection].sprite = _destroy;
            // _sections[_currentSection].color = _sectionColor;
            _sections.RemoveAt(_currentSection);
            _currentSection = sectionDamaged;
        }

        // if (_pulseRoutine == null)
        //     _pulseRoutine = StartCoroutine(Pulse());

        if (pourcent < 50)
            _sections[sectionDamaged].sprite = _damaged;
    }

    private IEnumerator Pulse()
    {
        _sections[_currentSection].color = Color.white;
        yield return new WaitForSeconds(0.05f);
        _sections[_currentSection].color = _sectionColor;
        yield return new WaitForSeconds(0.05f);
        _sections[_currentSection].color = Color.white;
        yield return new WaitForSeconds(0.05f);
        _sections[_currentSection].color = _sectionColor;
        yield return new WaitForSeconds(0.05f);
    }
}
