using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{

    ////////////////////////////////////////
    //SoulsLike HealthSystem (big chunk + decreassing bar)
    //hide yellow image when inactive and refill with red
    //onTime Recovery : like stamina
    ////////////////////////////////////////

    [Header("Health")]
    [SerializeField] private float maxHealth = 100.0f;
    private float minHealth;
    private float currentHealth;
    private float emptyingHealth;
    [SerializeField] private Image HealthImage;
    [SerializeField] private Image HealthEmptyingImage;
    [SerializeField] private Image HealthFullImage;
    private bool isDead = false;
    private bool isFull = false;

    //Invinsibility
    private float invTimer = 1f;
    private float invTime = 0f;
    private bool isTouched = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        maxHealth = maxHealth + (maxHealth * 0.136f);
        currentHealth = maxHealth; minHealth = (maxHealth * 0.136f);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            Destroy(gameObject);
        
        HealthHandle(); 
    }

    public void HealthHandle()
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (isTouched)
        {
            invTime += Time.deltaTime;

            if (invTime >= invTimer)
            {
                isTouched = false;
                invTime = 0f;
            }
        }

        if (currentHealth <= minHealth)
        {
            isDead = true;
            return;
        }

        HealthFullImage.fillAmount = currentHealth / maxHealth;
    }
    public void TakeDamage(float dmg)
    {
        if (!isTouched)
        {
            currentHealth -= dmg;
            isTouched = true;
            HealthHandle();
        }
    }
}
