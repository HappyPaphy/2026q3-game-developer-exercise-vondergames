using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public enum DayPeriod
{
    None = 999,
    Morning = 0,
    Afternoon = 1,
    Evening = 2
}

public enum DayIndex
{
    Monday = 0,
    Tuesday = 1,
    Wednesday = 2,
    Thursday = 3,
    Friday = 4,
    Saturday = 5,
    Sunday = 6
}

[System.Serializable]
public class DayPeriodModifier
{
    public DayPeriod period;
    public Color lightColor;
    public float lightValue;
}

public class EnvironmentVisualManager : MonoBehaviour
{
    public static EnvironmentVisualManager Instance;

    [SerializeField] private List<DayPeriodModifier> dayPeriods;
    [SerializeField] private float _dayPeriodChangeDuration = 1f;
    [SerializeField] private Light2D _globalLight2D;

    [SerializeField] private TextMeshProUGUI _text_timePeriod;
    [SerializeField] private TextMeshProUGUI _text_DayPeriod;
    [SerializeField] private TextMeshProUGUI _text_DayName;
    [SerializeField] private TextMeshProUGUI _text_DayIndex;

    [SerializeField] private float _timeSpeedMultiplier = 60f;

    private float _currentTimeInMinutes;

    private int _dayNameIndex;
    private int _dayCountIndex;

    private float _startLightValue;
    private float _targetLightValue;

    private Color _startLightColor;
    private Color _targetLightColor;

    private DayPeriod _currentDayPeriod = DayPeriod.None;
    private float _timeElapsed = 0f;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        _dayNameIndex = 0;
        _dayCountIndex = 0;
        _currentTimeInMinutes = 360f;
    }

    void Update()
    {
        HandleDayPeriodChange();
        HandleTimePeriod();
        UpdateUI();
    }

    private void HandleDayPeriodChange()
    {
        if (_globalLight2D == null) { return; }

        _timeElapsed += Time.deltaTime;
        float t = _timeElapsed / _dayPeriodChangeDuration;

        if (_globalLight2D != null)
        {
            _globalLight2D.intensity = Mathf.Lerp(_startLightValue, _targetLightValue, t);
            _globalLight2D.color = Color.Lerp(_startLightColor, _targetLightColor, t);
        }
    }

    public void ChangeDayPeriod(int index)
    {
        if (dayPeriods[index].period == _currentDayPeriod) { return; }

        _currentDayPeriod = dayPeriods[index].period;

        if (_globalLight2D != null)
        {
            _startLightValue = _globalLight2D.intensity;
            _startLightColor = _globalLight2D.color;
        }

        _targetLightValue = dayPeriods[index].lightValue;
        _targetLightColor = dayPeriods[index].lightColor;

        _timeElapsed = 0f;
    }

    private void HandleTimePeriod()
    {
        _currentTimeInMinutes += Time.deltaTime * _timeSpeedMultiplier;

        // Reset clock at Midnight and trigger the next day
        if (_currentTimeInMinutes >= 1440f)
        {
            _currentTimeInMinutes = 0f; // Reset to 00:00
            TriggerNextDay();
        }

        if (_currentTimeInMinutes >= 360f && _currentTimeInMinutes < 600f && _currentDayPeriod != DayPeriod.Morning)
        {
            ChangeDayPeriod(0);
        }
        // 12:00 to 17:59 is Afternoon
        else if (_currentTimeInMinutes >= 600f && _currentTimeInMinutes < 1080f && _currentDayPeriod != DayPeriod.Afternoon)
        {
            ChangeDayPeriod(1);
        }
        // 18:00+ is Evening
        else if (_currentTimeInMinutes >= 1080f && _currentDayPeriod != DayPeriod.Evening)
        {
            ChangeDayPeriod(2);
        }
    }

    public void TriggerNextPeriod()
    {
        switch(_currentDayPeriod)
        {
            case DayPeriod.Morning:
                {
                    _currentTimeInMinutes = 600f;
                }
                break;

            case DayPeriod.Afternoon:
                {
                    _currentTimeInMinutes = 1080f;
                }
                break;

            case DayPeriod.Evening:
                {
                    if (_currentTimeInMinutes >= 1080f)
                        TriggerNextDay();

                    _currentTimeInMinutes = 360f;
                }
                break;
        }
    }

    private void TriggerNextDay()
    {
        _dayCountIndex++;
        _dayNameIndex++;

        if(_dayNameIndex > 6)
        {
            _dayNameIndex = 0;
        }
    }    

    private void UpdateUI()
    {
        switch(_dayNameIndex)
        {
            case 0: _text_DayName.text = "Monday"; break;
            case 1: _text_DayName.text = "Tuesday"; break;
            case 2: _text_DayName.text = "Wednesday"; break;
            case 3: _text_DayName.text = "Thursday"; break;
            case 4: _text_DayName.text = "Friday"; break;
            case 5: _text_DayName.text = "Saturday"; break;
            case 6: _text_DayName.text = "Sunday"; break;
        }

        switch (_currentDayPeriod)
        {
            case DayPeriod.Morning: _text_DayPeriod.text = "Morning"; break;
            case DayPeriod.Afternoon: _text_DayPeriod.text = "Afternoon"; break;
            case DayPeriod.Evening: _text_DayPeriod.text = "Evening"; break;
        }

        _text_DayIndex.text = $"Day {_dayCountIndex}";

        // Calculate standard hours and minutes from total minutes
        int hours = Mathf.FloorToInt(_currentTimeInMinutes / 60f);
        int minutes = Mathf.FloorToInt(_currentTimeInMinutes % 60f);

        // Format to 00:00 
        _text_timePeriod.text = $"{hours:00}:{minutes:00}";
    }
}
