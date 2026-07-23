using System.Collections;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class WheelBehaviour : MonoBehaviour
{
    ////////////////////////////////////////
    ///actually the player dies because no more health
    ///make the image inactive and not the gameObject
    ///rotation && scale good
    ////////////////////////////////////////

    [Header("Rotation")]
    [SerializeField] private float duration = 1.0f;


    private RectTransform rectTransform;

    private bool isHealthActivate = false;
    private bool isStaminaActivate = false;
    private bool isActivate = false;

    private bool isDialogueActive = false;

    private Coroutine currentRotate;
    private LTDescr currentScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        rectTransform.localScale = Vector3.zero; //to hide the wheel

        isDialogueActive = DialogueManager.isActive;
    }

    public void DialogueTriggered()
    {
        isDialogueActive = true;

        isHealthActivate = false;
        Deactivation();
    }

    public void DialogueEnded()
    {
        isDialogueActive = true;

        isHealthActivate = false;
        Deactivation();
    }

    public void Deactivation(bool isHealth = false)
    {
        if (isHealth)
        {
            isHealthActivate = false;
        }            
        else
        {
            isStaminaActivate = false;
        }

        if (!isHealthActivate && !isStaminaActivate && isActivate) //if non one activate the wheel and already activate
        {
            isActivate = false;
            
            StartEffect(false);
        }
    }

    public void Activation(bool isHealth = false)
    {
        if (isHealth)
        {
            isHealthActivate = true;
        }
        else
        {
            isStaminaActivate = true;
        }

        if (!isActivate) //call the rescale only if not already activate
        {
            isActivate = true;
            
            StartEffect(true);
        }
    }

    private void StartEffect(bool isGrowing)
    {
        if (currentRotate != null)
            StopCoroutine(currentRotate);

        if (currentScale != null)
            LeanTween.cancel(currentScale.id); //stop the current rescal

        float offsetTime = (rectTransform.eulerAngles.z / 360f) * duration;        

        if (isGrowing)
            currentScale = rectTransform.LeanScale(Vector3.one, duration - offsetTime);
        else
            currentScale = rectTransform.LeanScale(Vector3.zero, duration - offsetTime);

        currentRotate = StartCoroutine(Rotate(isGrowing, offsetTime));
    }

    private IEnumerator Rotate(bool clockwise, float offsetTime) //offsetTime because the animation can start in the midle of a previous one
    {
        // if (!clockwise)
        //     yield return new WaitForSeconds(duration);

        float startRotation = 0f;
        float endRotation = clockwise ? 360.0f : -360.0f;
        float t = offsetTime;
        while (t < duration)
        {
        //     if (!clockwise && (isStaminaActive || isHealthActive)) yield break;

            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360;
            rectTransform.eulerAngles = new Vector3(0, 0, zRotation);
            yield return null;
        }

        currentRotate = null;

    }

    // IEnumerator TimerDezactition()
    // {
    //     Debug.Log("Timer");

    //     yield return new WaitForSeconds(duration);

    //     healthImage.rectTransform.LeanScale(Vector3.zero, duration);
    //     staminaImage.rectTransform.LeanScale(Vector3.zero, duration);

    //     yield return null;

    // }

}
