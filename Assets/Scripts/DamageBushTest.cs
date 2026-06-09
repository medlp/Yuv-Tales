using UnityEngine;
using UnityEngine.Rendering;

public class DamageBushTest : MonoBehaviour
{
    public int dealedDamage = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthSystem>().TakeDamage(dealedDamage);
        }
    }
}
