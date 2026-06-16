using System.Collections;
using Unity.Hierarchy;
using UnityEditor.Experimental.GraphView;
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

    [Header("Wheel")]
    [SerializeField] private Image healthImage;
    private bool isHealthActive = false;
    [SerializeField] private Image staminaImage;
    private bool isStaminaActive = false;

    public enum Stats
    {
        Stamina,
        Health
    };

    private void Start()
    {

        healthImage.transform.localScale = Vector3.zero;
        staminaImage.transform.localScale = Vector3.zero;
    }

    public void Deactivation(Stats stats)
    {

        switch (stats)
        {
            case Stats.Stamina:
                isStaminaActive = false;
                break;
            case Stats.Health:
                isHealthActive = false;
                break;
            default:
                break;
        }

        if (isHealthActive || isStaminaActive) return;

        Debug.Log("Desactivation");

        StartCoroutine(Rotate(false));

        StartCoroutine(TimerDezactition());

    }

    public void Activation(Stats stats)
    {

        if (isStaminaActive || isHealthActive) return;

        Debug.Log("Activation");

        switch (stats)
        {
            case Stats.Stamina:
                isStaminaActive = true;
                break;
            case Stats.Health:
                isHealthActive = true;
                break;
            default:
                break;
        }

        StartCoroutine(Rotate(true));

        healthImage.rectTransform.LeanScale(Vector3.one, duration);
        staminaImage.rectTransform.LeanScale(Vector3.one, duration);

    }

    IEnumerator Rotate(bool clockwise)
    {
        Debug.Log("Rotate");

        if (!clockwise)
            yield return new WaitForSeconds(duration);

        float startRotation = 0f;
        float endRotation = clockwise ? 360.0f : -360.0f;
        float t = 0;
        while (t < duration)
        {
            if (!clockwise && (isStaminaActive || isHealthActive)) yield break;

            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360;
            healthImage.gameObject.transform.eulerAngles = new Vector3(0, 0, zRotation);
            staminaImage.gameObject.transform.eulerAngles = new Vector3(0, 0, zRotation);
            yield return null;
        }

    }

    IEnumerator TimerDezactition()
    {
        Debug.Log("Timer");

        yield return new WaitForSeconds(duration);

        healthImage.rectTransform.LeanScale(Vector3.zero, duration);
        staminaImage.rectTransform.LeanScale(Vector3.zero, duration);

        yield return null;

    }

}
