using UnityEngine;
using UnityEngine.XR.Management;

public class VRController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StopXR();
    }


    void StopXR()
    {
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            Camera.main.ResetAspect();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {

    }
}
