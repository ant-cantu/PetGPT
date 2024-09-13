using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatGPT : MonoBehaviour
{
    // Replace this with your OpenAI API key (do not share a build of this with your key placed here)
    private string apiKey = "PLACE_API_KEY_HERE";
    private string apiUrl = "https://api.openai.com/v1/chat/completions";
    private string gptModel = "gpt-3.5-turbo";

    public TextMeshProUGUI GPT_Response;
    public TMP_InputField inputField;
    public Button submitButton;

    private void Start()
    {
        submitButton.onClick.AddListener(submitToGPT);
        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void OnEndEdit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartCoroutine(SendInputToGPT(input));
            inputField.text = ""; // Clear input field
        }
    }

    private void submitToGPT()
    {
        string userInput = inputField.text;
        if (!string.IsNullOrEmpty(userInput))
        {
            StartCoroutine(SendInputToGPT(userInput));
            inputField.text = ""; // Clear input field
        }
    }

    public IEnumerator SendInputToGPT(string userInput)
    {
        // Create the request body with user input
        string requestBody = "{\"model\":\"" + gptModel + "\",\"messages\":[{\"role\":\"user\",\"content\":\"" + userInput + "\"}]}";

        // Create UnityWebRequest and set up headers and body
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Send the request
        request.method = UnityWebRequest.kHttpVerbPOST;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            GPT_Response.text = "Error: " + request.error;
        }
        else
        {
            // Parse the response (adjust according to the OpenAI API's response format)
            string jsonResponse = request.downloadHandler.text;
            // Assuming the response contains a 'choices' array with a 'message' field
            var responseObject = JsonUtility.FromJson<OpenAIResponse>(jsonResponse);

            // Instant response, no type effect
            //GPT_Response.text = responseObject.choices[0].message.content;

            // Display resposne with typing effect
            DisplayGPTResponse(responseObject.choices[0].message.content);

        }
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

    [System.Serializable]
    public class Message
    {
        public string content;
    }

    private void DisplayGPTResponse(string fullText)
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string fullText)
    {
        GPT_Response.text = ""; // Clear text field

        foreach (char letter in fullText.ToCharArray())
        {
            GPT_Response.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
