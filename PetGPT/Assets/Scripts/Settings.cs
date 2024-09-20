using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    // Settings menu objects
    [Header("Settings Menu Objects")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject settingsText;
    [SerializeField] private GameObject settingsBackground;
    [SerializeField] private Button exitMenuButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_InputField apiKeyField;

    // Error menu objects
    [Header("Error Pop-up Objects")]
    [SerializeField] private GameObject errorMenu;
    [SerializeField] private TextMeshProUGUI errorMsg;
    [SerializeField] private Button exitErrorButton;
    [SerializeField] private Button okButton;

    // Privacy notice objects
    [Header("Privacy Notice Objects")]
    [SerializeField] private GameObject privacyNotice;
    [SerializeField] private Button privacySubmit;
    [SerializeField] private Toggle acceptTerms;
    [SerializeField] private TextMeshProUGUI buttonText;

    // Debug Only
    [ContextMenu("Reset Prefs")]
    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    // External scripts
    private ChatGPT gpt;

    // Variables
    private string apiKey;

    // Start is called before the first frame update
    void Start()
    {
        // Assign external scripts/objects
        gpt = GetComponent<ChatGPT>();
            
        // Assign API Key
        apiKey = PlayerPrefs.GetString("API_KEY");

        if(apiKey != null || apiKey != "")
        {
            apiKeyField.contentType = TMP_InputField.ContentType.Password;
            apiKeyField.text = apiKey;
        }

        // Error pop-up listeners
        exitErrorButton.onClick.AddListener(HideError);
        okButton.onClick.AddListener(HideError);

        // Settings menu listeners
        exitMenuButton.onClick.AddListener(ExitSettingsMenu);
        saveButton.onClick.AddListener(saveSettings);

        // Only display first time running the app
        if (PlayerPrefs.GetInt("FIRST_BOOT") == 0)
        {
            settingsMenu.SetActive(true);
            privacyNotice.SetActive(true);

            privacySubmit.onClick.AddListener(PrivacyButtonClicked);
        }
    }

    void Update()
    {
        if (!acceptTerms.isOn)
            buttonText.text = "Exit Application";
        else
            buttonText.text = "Submit";      
    }

    private void PrivacyButtonClicked()
    {
        if (!acceptTerms.isOn)
            Application.Quit();
        else
        {
            privacyNotice.SetActive(false);
            settingsText.SetActive(true);
            PlayerPrefs.SetInt("FIRST_BOOT", 1);
        }
    }

    private void ExitSettingsMenu()
    {
        settingsMenu.SetActive(false);
    }

    private void saveSettings()
    {
        if(apiKeyField.text == "")
            ShowError("API Key Field is empty! Please insert your API Key.");
        else
        {
            // Save API Key
            PlayerPrefs.SetString("API_KEY", apiKeyField.text);

            // Set API Key in ChatGPT script
            gpt.setAPIKey(apiKeyField.text);

            // Hide settings menu
            settingsMenu.SetActive(false);
        }
    }

    private void ShowError(string message)
    {
        errorMsg.text = "Error: " + message;
        errorMenu.SetActive(true);
        errorMenu.transform.SetAsLastSibling();
    }

    private void HideError()
    {
        errorMenu.SetActive(false);
    }
}


// Encrypt player prefs key
// Give life to the pet
// Change pet skin
// If GPT response is large, open in new panel
// Settings button/typed text?