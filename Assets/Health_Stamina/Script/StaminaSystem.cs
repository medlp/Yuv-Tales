using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{

    ////////////////////////////////////////
    //launch recovery when empty (sprint stop good)
    ////////////////////////////////////////

    [Header("Stamina")]
    public float maxStamina = 100.0f;
    private float minStamina;
    public float currentStamina;
    [SerializeField] private Image staminaImage;
    [SerializeField] private Image staminaFullImage;
    public bool isRecovering = false;
    public bool isEmpty = false;
    public bool canSprint = true;//??
    [SerializeField] private float decreassingSpeed = 7.0f;
    [SerializeField] private float fillingSpeed = 10.0f;

    private WheelBehaviour wheelBehaviour;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        wheelBehaviour = GetComponent<WheelBehaviour>();

        maxStamina = maxStamina + (maxStamina * 0.16f);
        currentStamina = maxStamina; minStamina = (maxStamina * 0.16f);
        
    }

    // Update is called once per frame
    public void OnUpdate(bool isSprinting)
    {

        StaminaHandle(isSprinting);  
        
    }

    public void StaminaHandle(bool isSprinting)
    {
        if (isSprinting && !isEmpty)
        {
            wheelBehaviour.Activation();

            currentStamina -= decreassingSpeed * Time.deltaTime;

            if (currentStamina <= minStamina)
            {
                currentStamina = minStamina;
                isEmpty = true;
            }
        }
        
        if (!isSprinting && currentStamina < maxStamina)
        {
            wheelBehaviour.Deactivation();

            if (isEmpty || isRecovering)
            {
                currentStamina += fillingSpeed * Time.deltaTime;

                if (currentStamina > minStamina)
                    isEmpty = false;

                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                    isRecovering = false;
                }
            }
        }

        staminaFullImage.fillAmount = currentStamina / maxStamina;
    }

}
