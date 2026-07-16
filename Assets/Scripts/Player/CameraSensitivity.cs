using Unity.Cinemachine;
using UnityEngine;

public class CameraSensitivity: MonoBehaviour
{
    [SerializeField] private CinemachineInputAxisController inputAxisController;

    private float baseGainX;
    private float baseGainY;
    private bool initialized;

    void Start()
    {
        if (inputAxisController == null)
        {
            inputAxisController = FindFirstObjectByType<CinemachineInputAxisController>();
        }

        if (inputAxisController != null && inputAxisController.Controllers.Count >= 2)
        {
            baseGainX = inputAxisController.Controllers[0].Input.Gain;
            baseGainY = inputAxisController.Controllers[1].Input.Gain;
            initialized = true;
        }
    }

    void Update()
    {
        if (!initialized) return;

        var controllers = inputAxisController.Controllers;

        var ctrlX = controllers[0];
        var inputX = ctrlX.Input;
        inputX.Gain = baseGainX * SettingsManager.MainCamSensitivityX;
        ctrlX.Input = inputX;
        controllers[0] = ctrlX;

        var ctrlY = controllers[1];
        var inputY = ctrlY.Input;
        inputY.Gain = baseGainY * SettingsManager.MainCamSensitivityY;
        ctrlY.Input = inputY;
        controllers[1] = ctrlY;
    }
}