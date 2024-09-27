using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatGPT : MonoBehaviour
{
    private string apiKey;
    private string apiUrl = "https://api.openai.com/v1/chat/completions";
    private string gptModel = "gpt-3.5-turbo";
    private string gptPersonality;// = "You are a helpful desktop assistant. My name is Anthony.";

    // List to store the conversation history
    private List<Message> convoHistory = new List<Message>();

    // Struct to hold individual messages
    [System.Serializable]
    public class Message
    {
        public string role;     // "user", "assistant", or "system"
        public string content;  // The message content
    }

    // Object variables
    public TextMeshProUGUI GPT_Response;
    public TMP_InputField inputField;

    // External script variables
    private Settings settings;
    [SerializeField] private GameObject petObject;
    private PetScript petScript;

    private void Start()
    {
        // Assign external scripts
        settings = GetComponent<Settings>();
        petObject = GameObject.FindWithTag("Player");
        petScript = petObject.GetComponent<PetScript>();

        // Set the saved API Key
        apiKey = PlayerPrefs.GetString("API_KEY");

        // Set AI persona
        if(PlayerPrefs.GetString("AI_PERSONA") != "")
            gptPersonality = PlayerPrefs.GetString("AI_PERSONA");

        if (PlayerPrefs.GetString("USERNAME") != "")
            gptPersonality += ". My name is " + PlayerPrefs.GetString("USERNAME");

        AddMessage("system", gptPersonality);

        inputField.onEndEdit.AddListener(OnEndEdit);
        inputField.onSelect.AddListener(OnInputSelect);
    }

    public void SetAPIKey(string key)
    {
        apiKey = key;
    }  
    
    public void SetAPIValues(string persona, string name)
    {
        gptPersonality = persona + ". My name is " + name;
    }

    public void SetAPIValues(string persona)
    {
        gptPersonality = persona;
    }

    private void OnEndEdit(string userInput)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (userInput == "Settings" || userInput == "settings")
            {
                petScript.CloseChatBox();
                settings.ShowSettings();
                return;
            }

            AddMessage("user", userInput);
            StartCoroutine(SendInputToGPT());
            inputField.text = ""; // Clear input field
        }
    }

    private void OnInputSelect(string userInput)
    {
        //Debug.Log("Entered field");
        inputField.text = "";
    }

    public void InitializeChat()
    {
        AddMessage("user", "Hello");
        StartCoroutine(SendInputToGPT());
    }

    public IEnumerator SendInputToGPT()
    {
        // Start constructing the request string
        string requestBody = "{\"model\":\"" + gptModel + "\",\"messages\":[{\"role\":\"system\",\"content\":\"" + gptPersonality + "\"}";

        // Add each message from the conversation history
        foreach (var message in convoHistory)
        {
            requestBody += ",{\"role\":\"" + message.role + "\",\"content\":\"" + message.content + "\"}";
        }

        // Close the request array and object
        requestBody += "]}";

        //string jsonBody = JsonUtility.ToJson(requestBody);
        Debug.Log("Request Body: " + requestBody);

        // Create UnityWebRequest and set up headers and body
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            GPT_Response.text = "Error: " + request.error;
            Debug.Log("Error: " + request.error);
        }
        else
        {
            // Parse the response (adjust according to the OpenAI API's response format)
            string jsonResponse = request.downloadHandler.text;

            // Assuming the response contains a 'choices' array with a 'message' field
            var responseObject = JsonUtility.FromJson<OpenAIResponse>(jsonResponse);

            // chatGPT response
            string aiResponse = responseObject.choices[0].message.content;

            // Add the response to the conversation history
            AddMessage("assistant", aiResponse);

            // Instant response, no type effect
            //GPT_Response.text = responseObject.choices[0].message.content;

            // Display resposne with typing effect
            DisplayGPTResponse(responseObject.choices[0].message.content);

        }
    }

    // Adds a message to the conversation history
    private void AddMessage(string role, string content)
    {
        Message message = new ChatGPT.Message { role = role, content = content };
        convoHistory.Add(message);
    }

    public void ClearConvoHistory()
    {
        // Clear the conversation history, saves tokens
        convoHistory.Clear();

        // re-add the system message
        AddMessage("system", gptPersonality);

        // Clear the UI
        GPT_Response.text = "";
    }

    // Helper classes for deserializing the OpenAI API response
    [System.Serializable]
    public class OpenAIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    private void DisplayGPTResponse(string fullText)
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string fullText)
    {
        GPT_Response.text = ""; // Clear text field

        // Foreach letter in the response, add one letter at a time.
        foreach (char letter in fullText.ToCharArray())
        {
            GPT_Response.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
