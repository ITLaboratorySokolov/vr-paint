using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigLanguageController : MonoBehaviour
{
    string urlCZ = "URL serveru:";
    string urlEN = "Server URL:";

    string nameCZ = "Jméno klienta:";
    string nameEN = "Client name:";

    string setCZ = "Hrát";
    string setEN = "Play";

    string inputPromptCZ = "Napište text...";
    string inputPromptEN = "Enter text...";

    string langCZ = "EN";
    string langEN = "CZ";

    string lang = "CZ";
    
    [Header("Text")]
    [SerializeField()]
    TMP_Text nameTXT;
    [SerializeField()]
    TMP_Text urlTXT;

    [Header("Buttons")]
    [SerializeField()]
    Button langBT;
    [SerializeField()]
    Button playBT;

    [Header("Controls")]
    [SerializeField()]
    GameObject controlsCZ;
    [SerializeField()]
    GameObject controlsEN;

    [Header("Input fields")]
    [SerializeField()]
    TMP_InputField nameFLD;
    [SerializeField()]
    TMP_InputField urlFLD;


    public void SwapLanguage()
    {
        if (lang == "EN")
            lang = "CZ";
        else
            lang = "EN";

        SwapLabels();
        SetControls();
    }

    public void SwapLabels()
    {
        if (lang == "EN")
        {
            nameTXT.text = nameEN;
            urlTXT.text = urlEN;

            langBT.GetComponentInChildren<TMP_Text>().text = langEN;
            playBT.GetComponentInChildren<TMP_Text>().text = setEN;

            nameFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
            urlFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
        }
        else if (lang == "CZ")
        {
            nameTXT.text = nameCZ;
            urlTXT.text = urlCZ;

            langBT.GetComponentInChildren<TMP_Text>().text = langCZ;
            playBT.GetComponentInChildren<TMP_Text>().text = setCZ;

            nameFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
            urlFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
        }
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
}
