using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZCU.TechnologyLab.Common.Connections.Client.Session;

public class LanguageController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField()]
    Text brushNMTXT;
    [SerializeField()]
    Text widthTXT;
    [SerializeField()]
    TMP_Text quitTXT;

    [Header("Buttons")]
    [SerializeField()]
    Button refreshBT;
    [SerializeField()]
    Button langBT;
    [SerializeField()]
    Button yesBT;
    [SerializeField()]
    Button noBT;

    [Header("Controls")]
    [SerializeField()]
    GameObject controlsCZ;
    [SerializeField()]
    GameObject controlsEN;
    [SerializeField()]
    Button controlsBT;

    [Header("Language")]
    internal string lang;

    [Header("Strings")]
    string brushCZ = "Štìtec: ";
    string brushEN = "Brush: ";
    string widthCZ = "Velikost";
    string widthEN = "Width";
    string refreshCZ = "Reload brushes";
    string refreshEN = "Naèíst štetce";
    string contCZ = "Ovládání";
    string contEN = "Controls";

    string quitCZ = "Ukonèit aplikaci?";
    string quitEN = "Do you want to quit?";
    string yesCZ = "Ano";
    string yesEN = "Yes";
    string noCZ = "Ne";
    string noEN = "No";

    string langCZ = "EN";
    string langEN = "CZ";

    // Start is called before the first frame update
    void Start()
    {
        lang = "CZ";
        SetLabels();
        SetControls();
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
        SetControls();
    }

    private void SetControls()
    {
        if (lang == "CZ")
        {
            controlsCZ.SetActive(true);
            controlsEN.SetActive(false);
        }
        else if (lang == "EN")
        {
            controlsEN.SetActive(true);
            controlsCZ.SetActive(false);
        }
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
            quitTXT.text = quitCZ;

            refreshBT.GetComponentInChildren<TMP_Text>().text = refreshCZ;
            langBT.GetComponentInChildren<TMP_Text>().text = langCZ;
            controlsBT.GetComponentInChildren<TMP_Text>().text = contCZ;
            yesBT.GetComponentInChildren<TMP_Text>().text = yesCZ;
            noBT.GetComponentInChildren<TMP_Text>().text = noCZ;
        }

        else if (langCZ == "EN")
        {
            brushNMTXT.text = brushEN;
            widthTXT.text = widthEN;
            quitTXT.text = quitEN;

            refreshBT.GetComponentInChildren<TMP_Text>().text = refreshEN;
            langBT.GetComponentInChildren<TMP_Text>().text = langEN;
            controlsBT.GetComponentInChildren<TMP_Text>().text = contEN;
            yesBT.GetComponentInChildren<TMP_Text>().text = yesEN;
            noBT.GetComponentInChildren<TMP_Text>().text = noEN;
        }
    }

    internal string GetSessionStateString(SessionState state)
    {
        if (lang == "CZ")
        {
            if (state == SessionState.Closed)
                return "Odpojeno";
            if (state == SessionState.Connected)
                return "Pøipojeno";
            if (state == SessionState.Reconnecting)
                return "Pøipojování";
            if (state == SessionState.Closing)
                return "Odpojování";
            if (state == SessionState.Starting)
                return "Zaèíná";
        }

        return state.ToString();
    }
}
