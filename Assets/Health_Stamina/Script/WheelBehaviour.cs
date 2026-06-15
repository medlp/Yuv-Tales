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

    [Header("DespawnTimer")]
    [SerializeField] private float offTimer = 1f;
    [SerializeField] private float offTime = 0f;

    public enum Stats
    {
        Stamina,
        Health
    };

    private void Start()
    {

        healthImage.transform.localScale = Vector3.zero;

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

        StartCoroutine(TimerDezactition());

        StartCoroutine(Rotate(false));

    }

    public void Activation(Stats stats)
    {
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
        if (isStaminaActive && isHealthActive) return;

        StartCoroutine(Rotate(true));

        healthImage.rectTransform.LeanScale(Vector3.one, 0.5f);

    }

    IEnumerator Rotate(bool clockwise)
    {
        float startRotation = 0f;
        float endRotation = clockwise ? 360.0f : -360.0f;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360;
            healthImage.gameObject.transform.eulerAngles = new Vector3(0, 0, zRotation);
            yield return null;
        }

    }

    IEnumerator TimerDezactition()
    {
        while (offTime < offTimer)
        {
            offTime += Time.deltaTime;
            yield return null;
        }

        offTime = 0f;

        healthImage.rectTransform.LeanScale(Vector3.zero, 0.5f);

    }

}
