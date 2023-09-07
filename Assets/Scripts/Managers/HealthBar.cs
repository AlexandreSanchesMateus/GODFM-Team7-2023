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
        _sectionValue = totalHealth / _sections.Count;

        _sections.ForEach(sec => { sec.sprite = _normal; sec.color = _sectionColor; });
        _currentSection = _sections.Count - 1;
    }

    public void UpdateHealthValue(float currentHealth)
    {
        int sectionDamaged = (int)((_toalePV - currentHealth) / _sectionValue);
        float pourcent = currentHealth % _sectionValue * 100 / _toalePV;

        // Remove last section
        if (sectionDamaged != _currentSection)
        {
            StopAllCoroutines();
            transform.DOKill();
            transform.DOShakePosition(0.15f);
            _sections[_currentSection].sprite = _destroy;
            _sections[_currentSection].color = _sectionColor;
            _sections.RemoveAt(_currentSection);
            _currentSection = sectionDamaged;
        }

        if(_pulseRoutine == null)
            _pulseRoutine = StartCoroutine(Pulse());

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
