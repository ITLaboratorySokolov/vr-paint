using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// Class used to swap the skybox in painting scene
/// </summary>
public class SkyboxController : MonoBehaviour
{
    [SerializeField]
    Material[] availibleSkyboxes;

    [SerializeField]
    Color[] groundColors;

    [SerializeField]
    Material groundMat;


    [SerializeField]
    InputActionReference skyToggleAction;

    int activeScene = 0;

    /// <summary>
    /// Adds reaction to sky toggle action
    /// </summary>
    private void Start()
    {
        skyToggleAction.action.performed += SwapToNextSkybox;
    }

    /// <summary>
    /// Swap to next skybox in array availibleSkyboxes
    /// </summary>
    private void SwapToNextSkybox(CallbackContext txt)
    {
        activeScene++;
        activeScene = activeScene % availibleSkyboxes.Length;

        RenderSettings.skybox = availibleSkyboxes[activeScene];
        groundMat.color = groundColors[activeScene];

        DynamicGI.UpdateEnvironment();
    }
}
