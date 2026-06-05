using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{

    ////////////////////////////////////////
    //launch recovery when empty (sprint stop good)
    ////////////////////////////////////////

    [Header("Stamina")]
    [SerializeField ] private float maxStamina = 100.0f;
    private float minStamina;
    private float currentStamina;
    [SerializeField] private Image staminaImage;
    [SerializeField] private Image staminaFullImage;
    public bool isRecovering = false;
    public bool isEmpty = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
            currentStamina -= 10.0f * Time.deltaTime;

            if (currentStamina <= minStamina)
            {
                currentStamina = minStamina;
                isEmpty = true;
                isSprinting = false;
            }
        }
        else if (!isSprinting && currentStamina < maxStamina)
        {
            if (isEmpty || isRecovering)
            {
                currentStamina += 10.0f * Time.deltaTime;

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
