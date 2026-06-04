using UnityEngine;
using UnityEngine.Rendering;

public class DamageBushTest : MonoBehaviour
{
    public float dealedDamage = 5.0f;

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
            other.GetComponent<PlayerControllerTPS>().TakeDamage(dealedDamage);
        }
    }
}
