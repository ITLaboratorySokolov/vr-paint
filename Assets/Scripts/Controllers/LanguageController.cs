using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZCU.TechnologyLab.Common.Connections.Client.Session;

/// <summary>
/// Script used to switch between languages
/// - application supports czech and english
/// </summary>
public class LanguageController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField()]
    Text brushNMTXT;
    [SerializeField()]
    Text widthTXT;
    [SerializeField()]
    TMP_Text quitTXT;
    [SerializeField()]
    TMP_Text saveTitleTXT;

    [Header("Buttons")]
    [SerializeField()]
    Button refreshBT;
    [SerializeField()]
    Button langBT;
    [SerializeField()]
    Button yesBT;
    [SerializeField()]
    Button noBT;
    [SerializeField()]
    Button scenesBT;
    [SerializeField()]
    Button load1;
    [SerializeField()]
    Button load2;
    [SerializeField()]
    Button load3;
    [SerializeField()]
    Button save1;
    [SerializeField()]
    Button save2;
    [SerializeField()]
    Button save3;

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
    string brushCZ = "�t�tec: ";
    string brushEN = "Brush: ";
    string widthCZ = "Velikost";
    string widthEN = "Width";
    string refreshCZ = "Na��st �tetce";
    string refreshEN = "Reload brushes";
    string contCZ = "Ovl�d�n�";
    string contEN = "Controls";

    string sceneCZ = "Sc�ny";
    string sceneEN = "Scenes";
    string sceneContCZ = "Spr�va ulo�en�ch sc�n";
    string sceneContEN = "Saved scenes management";
    string saveCZ = "Ulo�it na sc�nu ";
    string saveEN = "Save as scene ";
    string loadCZ = "Na��st sc�nu ";
    string loadEN = "Load scene ";
    
    string quitCZ = "Ukon�it aplikaci?";
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

    /// <summary>
    /// Set controls panel
    /// </summary>
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

            saveTitleTXT.text = sceneContCZ;
            scenesBT.GetComponentInChildren<TMP_Text>().text = sceneCZ;
            load1.GetComponentInChildren<TMP_Text>().text = loadCZ + 1;
            load2.GetComponentInChildren<TMP_Text>().text = loadCZ + 2;
            load3.GetComponentInChildren<TMP_Text>().text = loadCZ + 3;
            save1.GetComponentInChildren<TMP_Text>().text = saveCZ + 1;
            save2.GetComponentInChildren<TMP_Text>().text = saveCZ + 2;
            save3.GetComponentInChildren<TMP_Text>().text = saveCZ + 3;

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

            saveTitleTXT.text = sceneContEN;
            scenesBT.GetComponentInChildren<TMP_Text>().text = sceneEN;
            load1.GetComponentInChildren<TMP_Text>().text = loadEN + 1;
            load2.GetComponentInChildren<TMP_Text>().text = loadEN + 2;
            load3.GetComponentInChildren<TMP_Text>().text = loadEN + 3;
            save1.GetComponentInChildren<TMP_Text>().text = saveEN + 1;
            save2.GetComponentInChildren<TMP_Text>().text = saveEN + 2;
            save3.GetComponentInChildren<TMP_Text>().text = saveEN + 3;

            refreshBT.GetComponentInChildren<TMP_Text>().text = refreshEN;
            langBT.GetComponentInChildren<TMP_Text>().text = langEN;
            controlsBT.GetComponentInChildren<TMP_Text>().text = contEN;
            yesBT.GetComponentInChildren<TMP_Text>().text = yesEN;
            noBT.GetComponentInChildren<TMP_Text>().text = noEN;
        }
    }

    /// <summary>
    /// Translate session state to string that is displayed
    /// </summary>
    /// <param name="state"> Session state </param>
    /// <returns> String to display </returns>
    internal string GetSessionStateString(SessionState state)
    {
        if (lang == "CZ")
        {
            if (state == SessionState.Closed)
                return "Odpojeno";
            if (state == SessionState.Connected)
                return "P�ipojeno";
            if (state == SessionState.Reconnecting)
                return "P�ipojov�n�";
            if (state == SessionState.Closing)
                return "Odpojov�n�";
            if (state == SessionState.Starting)
                return "Za��n�";
        }

        return state.ToString();
    }
}
