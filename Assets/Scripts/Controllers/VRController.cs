using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class VRController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        // StartCoroutine(StopCardboard());
    }

    public IEnumerator StopCardboard()
    {
        XRSettings.LoadDeviceByName("");
        yield return null;
        XRSettings.enabled = false;
        ResetCameras();
        Screen.orientation = ScreenOrientation.Portrait;
    }

    void ResetCameras()
    {
        // Camera looping logic copied from GvrEditorEmulator.cs
        for (int i = 0; i < Camera.allCameras.Length; i++)
        {
            Camera cam = Camera.allCameras[i];
            if (cam.enabled && cam.stereoTargetEye != StereoTargetEyeMask.None)
            {
                // Reset local position.
                // Only required if you change the camera's local position while in 2D mode.
                cam.transform.localPosition = Vector3.zero;

                // Reset local rotation.
                // Only required if you change the camera's local rotation while in 2D mode.
                cam.transform.localRotation = Quaternion.identity;

                // No longer needed, see issue github.com/googlevr/gvr-unity-sdk/issues/628.
                // cam.ResetAspect();

                // No need to reset `fieldOfView`, since it's reset automatically.
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        XRGeneralSettings.Instance.Manager.InitializeLoader();
        // XRSettings.enabled = true;
        // XRGeneralSettings.Instance.Manager.StartSubsystems();
    }
}
