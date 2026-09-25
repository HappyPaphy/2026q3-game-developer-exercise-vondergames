using UnityEngine;

public class GameStatusMessage : MonoBehaviour
{
    public static GameStatusMessage Instance;

    [SerializeField] private GameObject _statusMessagePrefab;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateMessage(string message, Color color)
    {
        GameObject obj = Instantiate(_statusMessagePrefab, transform);
        StatusMessageUI messageUI = obj.GetComponent<StatusMessageUI>();
        StartCoroutine(messageUI.ShowStatusMessage(message, color));
    }
}
