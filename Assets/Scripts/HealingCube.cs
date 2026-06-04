using UnityEngine;

public class HealingCube : MonoBehaviour
{
    private float healingPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        switch (gameObject.tag)
        {
            case "healing_cube_10":
                healingPoint = 10;
                break;
            case "healing_cube_20":
                healingPoint = 20;
                break;
            case "healing_cube_30":
                healingPoint = 30;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerControllerTPS>().Heal(healingPoint);
        }
    }
}
