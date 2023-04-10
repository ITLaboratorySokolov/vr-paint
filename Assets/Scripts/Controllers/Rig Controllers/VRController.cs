using UnityEngine;
using UnityEngine.XR.Management;

/// <summary>
/// Script used to disable OpenXR
/// </summary>
public class VRController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StopXR();
    }

    /// <summary>
    /// Stop OpenXR
    /// - so the application is displayed and controlled on desktop only
    /// </summary>
    void StopXR()
    {
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            Camera.main.ResetAspect();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }

}
