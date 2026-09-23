using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironmentVisualAsset : MonoBehaviour
{
    /*[SerializeField] private SpriteRenderer _environmentSprite;
    [SerializeField] private Tilemap _tileMapSprite;
    [SerializeField] private float _colorChangeDuration;
    
    private Color _targetColor;
    private float _timeElapsed = 0f;
    private bool _isChangingColor = false;

    void Start()
    {
        SetTargetSprite();
        CheckIfSpriteRegisteredInManager();
    }

    void Update()
    {
        HandleColorSmoothChange();
    }

    private void HandleColorSmoothChange()
    {
        if (!_isChangingColor) { return; }
        if (_colorChangeDuration <= 0f) { return; }

        _timeElapsed += Time.deltaTime;
        float t = _timeElapsed / _colorChangeDuration;

        if(_environmentSprite != null)
        {
            _environmentSprite.color = Color.Lerp(_environmentSprite.color, _targetColor, t);
        }

        if(_tileMapSprite != null)
        {
            _tileMapSprite.color = Color.Lerp(_tileMapSprite.color, _targetColor, t);
        }

        if (t >= 1f)
        {
            _isChangingColor = false;
        }
    }

    private void CheckIfSpriteRegisteredInManager()
    {
        if(_environmentSprite == null && _tileMapSprite == null) { return; }
        if(EnvironmentVisualManager.Instance == null) { return; }

        if (!EnvironmentVisualManager.Instance.EnvironmentSprites.Contains(this))
        {
            EnvironmentVisualManager.Instance.EnvironmentSprites.Add(this);
        }
    }

    public void ChangeSpriteColor(Color color, float duration)
    {
        _targetColor = color;
        _colorChangeDuration = duration;
        _timeElapsed = 0f;
        _isChangingColor = true;
    }

    private void SetTargetSprite()
    {
        if (_environmentSprite != null)
        {
            _targetColor = _environmentSprite.color;
        }

        if (_tileMapSprite != null)
        {
            _targetColor = _tileMapSprite.color;
        }
    }*/
}
