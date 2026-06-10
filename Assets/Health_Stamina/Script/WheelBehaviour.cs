using System.Collections;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WheelBehaviour : MonoBehaviour
{

    ////////////////////////////////////////
    ///actually the player dies because no more health
    ///make the image inactive and not the gameObject
    ///rotation && scale good
    ////////////////////////////////////////

    [SerializeField] private float duration = 1.0f;

    public void Deactivation()
    {
        if (!this.gameObject.activeInHierarchy) return;

        this.gameObject.transform.localScale = Vector3.one;
        this.gameObject.transform.eulerAngles = Vector3.zero;

        StartCoroutine(Rotate(false));
        StartCoroutine(Scale(false));
        StartCoroutine(TurnOff());
    }

    public void OnEnable()
    {
        this.gameObject.transform.localScale = Vector3.one;
        this.gameObject.transform.eulerAngles = Vector3.zero;

        StartCoroutine(Rotate(false));
        StartCoroutine(Scale(false));
        StartCoroutine(TurnOff());
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
            this.gameObject.transform.eulerAngles = new Vector3(0, 0, zRotation);
            yield return null;
        }

    }

    IEnumerator Scale(bool scaleUp)
    {
        float startScale = scaleUp ? 0.0f : 1.0f;
        float endScale = scaleUp ? 1.0f : 0.0f;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, endScale, t / duration);
            this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }
    
    IEnumerator TurnOff()
    {
        yield return new WaitForSeconds(duration);
        this.gameObject.SetActive(false);
    }
}
