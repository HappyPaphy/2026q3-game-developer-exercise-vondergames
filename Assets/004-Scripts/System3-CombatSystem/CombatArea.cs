using UnityEngine;

public class CombatArea : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.CharacterHealthComponent.SetHP(100);
        }
    }
}
