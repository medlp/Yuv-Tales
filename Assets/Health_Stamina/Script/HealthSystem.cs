using Unity.Content;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{

    ////////////////////////////////////////
    //SoulsLike HealthSystem (big chunk + decreassing bar)
    //hide yellow image when inactive and refill with red
    //onTime Recovery : like stamina
    ////////////////////////////////////////

    //ORGA

    [Header("Health")]
    public float maxHealth = 100.0f;
    private float minHealth;
    public float currentHealth;
    private float emptyingHealth;
    [SerializeField] private Image healthImage;
    [SerializeField] private Image healthEmptyingImage;
    [SerializeField] private Image healthFullImage;
    private bool isDead = false;
    private bool isFull = false;
    [SerializeField] private float decreassingSpeed = 7.0f;
    [SerializeField] private float fillingSpeed = 5.0f;
    private bool canRecover = false;

    //Invinsibility
    private float invTimer = 1f;
    private float invTime = 0f;
    private bool isTouched = false;

    [SerializeField] private WheelBehaviour wheelBehaviour;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        maxHealth = maxHealth + (maxHealth * 0.136f);
        currentHealth = maxHealth; minHealth = (maxHealth * 0.136f);
        emptyingHealth = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
            }
        }

        HealthHandle(); 
    }

    public void HealthHandle()
    {
        if (isTouched)
            TouchedTimer();

        if (emptyingHealth > currentHealth)
            EmptyingHealth();

        if (!isFull && canRecover)
            Recovering();

        healthEmptyingImage.fillAmount = emptyingHealth / maxHealth;
        healthFullImage.fillAmount = currentHealth / maxHealth;
    }

    private void TouchedTimer()
    {
        invTime += Time.deltaTime;

        if (invTime >= invTimer)
        {
            isTouched = false;
            invTime = 0f;
        }
    }

    private void Recovering()
    {
        currentHealth += fillingSpeed * Time.deltaTime;
        emptyingHealth += fillingSpeed * Time.deltaTime;

        if (currentHealth >= maxHealth)
        {
            wheelBehaviour.Deactivation(true);
            isFull = true;
            canRecover = false;
            currentHealth = maxHealth;
            emptyingHealth = maxHealth;
        }
    }

    private void EmptyingHealth()
    {
        emptyingHealth -= decreassingSpeed * Time.deltaTime;

        if (emptyingHealth <= currentHealth)
            canRecover = true;
    }

    public void TakeDamage(float dmg)
    {
        wheelBehaviour.Activation(true);

        if (currentHealth <= minHealth)
            isDead = true;

        if (!isTouched && currentHealth > minHealth)
        {
            currentHealth -= dmg;
            isTouched = true;
            isFull = false;
            canRecover = false;
        }
    }
}
