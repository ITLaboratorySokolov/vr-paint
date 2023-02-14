using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField()]
    Text brushNMTXT;
    [SerializeField()]
    Text widthTXT;

    [Header("Buttons")]
    [SerializeField()]
    Button refreshBT;
    [SerializeField()]
    Button langBT;

    [Header("Language")]
    internal string lang;

    [Header("Strings")]
    string brushCZ = "Štìtec: ";
    string brushEN = "Brush: ";
    string widthCZ = "Velikost";
    string widthEN = "Width";
    string refreshCZ = "Reload brushes";
    string refreshEN = "Naèíst štetce";

    string langCZ = "EN";
    string langEN = "CZ";

    // Start is called before the first frame update
    void Start()
    {
        lang = "CZ";
        SetLabels();
    }

    /// <summary>
    /// Swap languages between CZ and EN
    /// </summary>
    public void SwapLanguage()
    {
        if (lang == "CZ")
            lang = "EN";
        else if (lang == "EN")
            lang = "CZ";

        SetLabels();
    }

    /// <summary>
    /// Set labels to czech or english texts
    /// </summary>
    private void SetLabels()
    {
        if (lang == "CZ")
        {
            brushNMTXT.text = brushCZ;
            widthTXT.text = widthCZ;

            refreshBT.GetComponentInChildren<TMP_Text>().text = refreshCZ;
            langBT.GetComponentInChildren<TMP_Text>().text = langCZ;
        }

        else if (langCZ == "EN")
        {
            brushNMTXT.text = brushEN;
            widthTXT.text = widthEN;

            refreshBT.GetComponentInChildren<TMP_Text>().text = refreshEN;
            langBT.GetComponentInChildren<TMP_Text>().text = langEN;
        }
    }
}
