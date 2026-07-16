using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueDisplay : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private string numberFormat = "F1"; // F1 = 1 décimale, F0 = entier

    private void Start()
    {
        UpdateInputFromSlider(slider.value);

        slider.onValueChanged.AddListener(UpdateInputFromSlider);
        inputField.onEndEdit.AddListener(UpdateSliderFromInput);
    }

    public void RefreshDisplay()
    {
        if (slider != null)
        {
            UpdateInputFromSlider(slider.value);
        }
    }

    private void UpdateInputFromSlider(float value)
    {
        inputField.SetTextWithoutNotify(value.ToString(numberFormat));
    }

    private void UpdateSliderFromInput(string text)
    {
        if (float.TryParse(text, out float value))
        {
            value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
            slider.value = value; 
        }
        else
        {
            inputField.SetTextWithoutNotify(slider.value.ToString(numberFormat));
        }
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(UpdateInputFromSlider);
        inputField.onEndEdit.RemoveListener(UpdateSliderFromInput);
    }
}