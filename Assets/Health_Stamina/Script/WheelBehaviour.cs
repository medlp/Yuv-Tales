using UnityEngine;
using UnityEngine.UI;

public class WheelBehaviour : MonoBehaviour
{

    ////////////////////////////////////////
    //launch recovery when empty (sprint stop good)
    ////////////////////////////////////////
    
    [Header("Health")]
    [SerializeField] private Image HealthImage;
    [SerializeField] private Image HealthEmptyingImage;
    [SerializeField] private Image HealthFullImage;

    [Header("Stamina")]
    [SerializeField] private Image staminaImage;
    [SerializeField] private Image staminaFullImage;

    [Header("Player")]
    [SerializeField] private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
