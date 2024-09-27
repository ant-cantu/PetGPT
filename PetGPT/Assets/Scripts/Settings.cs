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
    [SerializeField] private Button resetButton;
    [SerializeField] private Button privacyButton;
    [SerializeField] private TMP_InputField apiKeyField;
    [SerializeField] private TMP_InputField aiPersonaField;
    [SerializeField] private TMP_InputField nameField;

    // Notice menu objects
    [Header("Notice Pop-up Objects")]
    [SerializeField] private GameObject noticeMenu;
    [SerializeField] private TextMeshProUGUI noticeMsg;
    [SerializeField] private Button exitNoticeButton;
    [SerializeField] private Button okButton;

    // Privacy notice objects
    [Header("Privacy Notice Objects")]
    [SerializeField] private GameObject privacyNotice;
    [SerializeField] private Button privacySubmit;
    [SerializeField] private Toggle acceptTerms;
    [SerializeField] private TextMeshProUGUI buttonText;

    // External scripts
    private ChatGPT gpt;

    // Variables
    private string apiKey;
    private string aiPersona;
    private string username;

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

        // Fill in input fields with saved data
        aiPersona = PlayerPrefs.GetString("AI_PERSONA");
        username = PlayerPrefs.GetString("USERNAME");

        if (aiPersona != null && aiPersona != "")
            aiPersonaField.text = aiPersona;

        if (username != null && username != "")
            nameField.text = username;

        // Error pop-up listeners
        exitNoticeButton.onClick.AddListener(HideNotice);
        okButton.onClick.AddListener(HideNotice);

        // Settings menu listeners
        exitMenuButton.onClick.AddListener(ExitSettingsMenu);
        saveButton.onClick.AddListener(SaveSettings);
        resetButton.onClick.AddListener(ResetSettings);
        privacyButton.onClick.AddListener(DisplayPolicy);
        privacySubmit.onClick.AddListener(PrivacyButtonClicked);

        // Only display first time running the app
        if (PlayerPrefs.GetInt("FIRST_BOOT") == 0)
        {
            DisplayPolicy();
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

    private void SaveSettings()
    {
        if(apiKeyField.text == "")
            ShowNotice("API Key Field is empty! Please insert your API Key.");
        else
        {
            // Save API Key
            PlayerPrefs.SetString("API_KEY", apiKeyField.text);

            // Set API Key in ChatGPT script
            gpt.SetAPIKey(apiKeyField.text);

            // Set API persona, if fields have data in them.
            if (aiPersonaField.text != "" && nameField.text == "")
            {
                gpt.SetAPIValues(aiPersonaField.text);

                // Save settings to player prefs
                PlayerPrefs.SetString("AI_PERSONA", aiPersonaField.text);
            }
            else if (aiPersonaField.text != "" && nameField.text != "")
            {
                gpt.SetAPIValues(aiPersonaField.text, nameField.text);

                // Save settings to player prefs
                PlayerPrefs.SetString("USERNAME", nameField.text);
                PlayerPrefs.SetString("AI_PERSONA", aiPersonaField.text);
            }
            else if (aiPersonaField.text == "" && nameField.text != "")
            {
                // Default AI persona + name
                gpt.SetAPIValues("You are a helpful desktop assistant.", nameField.text);
            }
            else
            {
                // Default AI persona
                gpt.SetAPIValues("You are a helpful desktop assistant.");
            }

            // Hide settings menu
            settingsMenu.SetActive(false);
        }
    }

    private void ShowNotice(string message)
    {
        noticeMsg.text = "Notice: " + message;
        noticeMenu.SetActive(true);
        noticeMenu.transform.SetAsLastSibling();
    }

    private void HideNotice()
    {
        noticeMenu.SetActive(false);
    }

    public void ShowSettings()
    {
        settingsMenu.SetActive(true);
        settingsText.SetActive(true);
    }

    [ContextMenu("Reset Player Prefs")]
    private void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
        apiKeyField.text = "";
        nameField.text = "";
        aiPersonaField.text = "";
    }

    private void DisplayPolicy()
    {
        settingsMenu.SetActive(true);
        settingsText.SetActive(false);
        privacyNotice.SetActive(true);
    }
}


// Encrypt player prefs key
// Give life to the pet
// Change pet skin
// If GPT response is large, open in new panel
// Settings button/typed text?