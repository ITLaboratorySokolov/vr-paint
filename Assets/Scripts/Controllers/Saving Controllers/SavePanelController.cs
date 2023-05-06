using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SavePanelController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField()]
    Button load1;
    [SerializeField()]
    Button load2;
    [SerializeField()]
    Button load3;

    /// <summary>
    /// Refresh button interactibility when save panel is displayed
    /// - user can click load buttons only if there is any save to be loaded
    /// </summary>
    private void OnEnable()
    {
        ReloadActive();
    }

    /// <summary>
    /// Refresh button interactibility when save panel is displayed
    /// - user can click load buttons only if there is any save to be loaded
    /// </summary>
    public void ReloadActive()
    {
        load1.interactable = true;
        string path = Application.dataPath + "/Saves/" + 1 + "/";
        if (!Directory.Exists(path))
            load1.interactable = false;

        load2.interactable = true;
        path = Application.dataPath + "/Saves/" + 2 + "/";
        if (!Directory.Exists(path))
            load2.interactable = false;

        load3.interactable = true;
        path = Application.dataPath + "/Saves/" + 3 + "/";
        if (!Directory.Exists(path))
            load3.interactable = false;
    }
}
