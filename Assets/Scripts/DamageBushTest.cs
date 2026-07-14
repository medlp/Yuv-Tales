using UnityEngine;
using UnityEngine.Rendering;

public class DamageBushTest : MonoBehaviour
{
    public int dealedDamage = 10;

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Gameplay)
            return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthSystem>().TakeDamage(dealedDamage);
        }
    }
}
