using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private int _sectionNumber;
    [SerializeField] private HorizontalLayoutGroup _layoutGroup;
    [SerializeField] private Transform _lifeParent;
    [SerializeField] private GameObject _lifeSectionPrefab;

    private List<Animator> _sections;
    private float _toalePV;
    private float _sectionValue;

    public void InitializeHealthBar(int totalHealth)
    {
        _layoutGroup.enabled = true;
        _toalePV = totalHealth;
        _sectionValue = totalHealth / _sectionNumber;

        for (int i = 0; i < _sectionNumber; i++)
        {
            GameObject instance = Instantiate<GameObject>(_lifeSectionPrefab, _lifeParent);
            _sections.Add(instance.GetComponent<Animator>());
        }
        _layoutGroup.enabled = false;
    }

    public void UpdateHealthValue(int currentHealth)
    {
        int currentSection = (int)((_toalePV - currentHealth) / _sectionValue);
        float pourcent = currentHealth % _sectionValue * 100 / _toalePV;

        // Remove last section
        if (_sections.Count >= currentSection)
        {
            _sections[currentSection + 1].SetTrigger("Destroy");
            _sections.RemoveAt(currentSection + 1);
        }

        _sections[currentSection].SetTrigger("Damage");

        if (pourcent < 25)
        {
            _sections[currentSection].SetInteger("DammageType", 3);
        }
        else if(pourcent < 50)
        {
            _sections[currentSection].SetInteger("DammageType", 2);
        }
        else if (pourcent < 75)
        {
            _sections[currentSection].SetInteger("DammageType", 1);
        }
    }
}
