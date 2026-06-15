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

    [SerializeField] private float duration = 1.0f;

    [Header("Health")]
    [SerializeField] private Image healthImage;

    [Header("DespawnTimer")]
    private float invTimer = 1f;
    private float invTime = 0f;
    private bool isTouched = false;

    private void Start()
    {

        healthImage.transform.localScale = Vector3.zero;

    }

    public void Deactivation()
    {

        StartCoroutine(Rotate(false));

        healthImage.rectTransform.LeanScale(Vector3.zero, 0.5f);
    }

    public void Activation()
    {

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

}
