using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class StatusMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textStatusMessage;
    [SerializeField] private CanvasGroup _canvasGroupMessage;

    public IEnumerator ShowStatusMessage(string message)
    {
        _textStatusMessage.text = message;
        _canvasGroupMessage.alpha = 1f;
        yield return new WaitForSeconds(2f);

        _canvasGroupMessage.DOFade(0f, 0.5f).SetUpdate(false);
        yield return new WaitForSeconds(0.7f);

        Destroy(gameObject);
    }
}
