using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntity : CharacterEntity
{
    public StaminaComponent CharacterStaminaComponent;
    public UltimateComponent CharacterUltimateComponent;

    [Header("Component")]
    [SerializeField] protected Slider _slider_Stamina;
    [SerializeField] protected Slider _slider_Ultimate;
    [SerializeField] protected TextMeshProUGUI _text_Health;
    [SerializeField] protected TextMeshProUGUI _text_Stamina;
    [SerializeField] protected TextMeshProUGUI _text_Ultimate;

    public float curMaxHPLevelIndex;
    public float curMaxStaminaLevelIndex;

    protected bool isDiedOnce = false;
    private float takeDamage_DifficultyModifier;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        CharacterStaminaComponent.SetStamina(CharacterStaminaComponent.MaxStamina);
        CharacterHealthComponent.SetHP(CharacterHealthComponent.MaxHP);

        if(slider_HP != null)
            slider_HP.minValue = 0;

        if (_slider_Stamina != null)
            _slider_Stamina.minValue = 0;

        if (_slider_Ultimate != null)
            _slider_Ultimate.minValue = 0;
    }

    protected override void Update()
    {
        if (_slider_Stamina != null)
        {
            _slider_Stamina.maxValue = CharacterStaminaComponent.MaxStamina;
            _slider_Stamina.value = CharacterStaminaComponent.CurrentStamina;
        }

        if(_slider_Ultimate != null)
        {
            _slider_Ultimate.maxValue = CharacterUltimateComponent.MaxUltimate;
            _slider_Ultimate.value = CharacterUltimateComponent.CurrentUltimate;
        }

        if(_text_Health != null)
            _text_Health.text = $"{(int)CharacterHealthComponent.CurrentHP}";

        if(_text_Ultimate != null)
            _text_Ultimate.text = $"{(int)CharacterUltimateComponent.CurrentUltimate}";

        if (CharacterStaminaComponent.CurrentStamina < 0)
        {
            if(_text_Stamina != null)
                _text_Stamina.text = $"{0f}";
        }
        else
        {
            if (_text_Stamina != null)
                _text_Stamina.text = $"{(int)CharacterStaminaComponent.CurrentStamina}";
        }
            
        if (CharacterHealthComponent.CurrentHP < 0)
        {
            CharacterHealthComponent.SetHP(0);
        }

        base.Update();
    }

    public override void Die()
    {
        PlayerController.Instance.CharacterHealthComponent.SetHP(100);

        base.Die();
    }

    public virtual void SetNewMaxHP(float maxHPMultiplier)
    {
        CharacterHealthComponent.SetMaxHP(CharacterHealthComponent.MaxHP * maxHPMultiplier);
    }

    public virtual void TakeDamage(float damageValue)
    {
        float damage = damageValue;
        float hp = CharacterHealthComponent.CurrentHP;

        if (hp > 1 && (hp - damage <= 1))
        {
            CharacterHealthComponent.SetHP(1f);
        }
        else
        {
            CharacterHealthComponent.TakeDamage(damage);
        }
    }

    protected void SetActiveSlider(bool isTrue)
    {
        _slider_Ultimate.gameObject.SetActive(isTrue);
    }

    public IEnumerator GainSliderValueFeedback(GameObject slider, bool isPositiveFeedback, Vector3 originalScale)
    {
        Vector3 targetScale = new Vector3 (1f, 1f, 1f);
        float pulseDuration = 0.2f;

        if(isPositiveFeedback)
        {
            targetScale = originalScale * 1.3f;
        }
        else
        {
            targetScale = originalScale * 0.7f;
        }

        float halfDuration = pulseDuration / 2f;

        float timeElapsed = 0f;
        while (timeElapsed < halfDuration)
        {
            float t = timeElapsed / halfDuration;

            slider.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        slider.transform.localScale = targetScale;

        timeElapsed = 0f;

        while (timeElapsed < halfDuration)
        {
            float t = timeElapsed / halfDuration;

            slider.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            timeElapsed += Time.deltaTime;
            yield return null; 
        }

        slider.transform.localScale = originalScale;
    }
}

[Serializable]
public class StaminaComponent
{
    public float MaxStamina => _maxStamina;
    [SerializeField] private float _maxStamina;

    public float CurrentStamina => _currentStamina;
    [SerializeField] private float _currentStamina;

    public Action OnDamageTaken;

    public void SetStamina(float hpValue)
    {
        _currentStamina = hpValue;
    }

    public void Recover(float recoverValue)
    {
        _currentStamina = Mathf.Clamp(_currentStamina += recoverValue, 0, _maxStamina);
    }

    public void DepleteStamina(float depleteValue/*, int defense = 0*/)
    {
        _currentStamina -= depleteValue;
    }

    public void SetMaxStamina(float value)
    {
        _maxStamina = value;
    }
}

[Serializable]
public class UltimateComponent
{
    public float MaxUltimate => _maxUltimate;
    [SerializeField] private float _maxUltimate;

    public float CurrentUltimate => _currentUltimate;
    [SerializeField] private float _currentUltimate;

    public void SetUltimate(float ultimateValue)
    {
        _currentUltimate = ultimateValue;
    }

    public void GainUltimate(float gainValue)
    {
        _currentUltimate = Mathf.Clamp(_currentUltimate += gainValue, 0, _maxUltimate);
    }

    public void DepleteUltimate(float depleteValue)
    {
        _currentUltimate -= depleteValue;
    }

    public void SetMaxUltimate(float value)
    {
        _maxUltimate = value;
    }
}
