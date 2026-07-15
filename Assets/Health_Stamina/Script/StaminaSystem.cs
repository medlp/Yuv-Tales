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
    private float minStamina = 0f;
    public float currentStamina;
    [SerializeField] private Image staminaImage;
    [SerializeField] private Image staminaFullImage;
    public bool isRecovering = false;
    public bool isEmpty = false;
    [SerializeField] private float decreassingSpeed = 7.0f;
    [SerializeField] private float fillingSpeed = 10.0f;

    [SerializeField] private WheelBehaviour wheelBehaviour;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    public void OnUpdate(bool isSprinting)
    {

        StaminaHandle(isSprinting);

    }

    public void StaminaHandle(bool isSprinting)
    {

        if (transform.localScale !=  Vector3.zero && isSprinting)
            wheelBehaviour.Activation();

        if (isSprinting && !isEmpty)
        {

            currentStamina -= decreassingSpeed * Time.deltaTime;

            if (currentStamina <= minStamina)
            {
                currentStamina = minStamina;
                isEmpty = true;
            }
        }
        
        if (!isSprinting && currentStamina < maxStamina)
        {

            if (isEmpty || isRecovering)
            {
                currentStamina += fillingSpeed * Time.deltaTime;

                if (currentStamina > minStamina)
                    isEmpty = false;

                if (currentStamina >= maxStamina)
                {
                    wheelBehaviour.Deactivation();

                    currentStamina = maxStamina;
                    isRecovering = false;
                }
            }
        }

        staminaFullImage.fillAmount = currentStamina / maxStamina;
    }

}