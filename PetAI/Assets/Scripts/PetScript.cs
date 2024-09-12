using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Timeline.Actions;
using UnityEngine;
//using UnityEngine.Networking;

public class PetScript : MonoBehaviour
{
    // Game Objects
    public RectTransform canvasRectTransform;
    public GameObject chatBox;
    Camera mainCamera;

    // Components
    private SpriteRenderer sr;

    private bool isDragging = false;
    private Vector3 offset;
    private Vector2 chatBoxOffset;
    private Vector3 lastMousePosition;
    

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        sr = GetComponent<SpriteRenderer>();

        //Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        Debug.Log("Current Resolution: " + Screen.width + "x" + Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            // Get current mouse positon
            Vector3 currentMousePosition = GetMouseWorldPosition();

            // Move the object to follow the mouse
            transform.position = currentMousePosition + offset;

            // Keep object upright
            transform.rotation = Quaternion.identity;

            // Check direction of mouse movement
            if (currentMousePosition.x < lastMousePosition.x)
                sr.flipX = true; // Mouse moving left, flipX true
            else if(currentMousePosition.x > lastMousePosition.x)
                sr.flipX = false; // Mouse moving right, flipX false

            // Update the last mouse position
            lastMousePosition = currentMousePosition;
        }

        // If right click
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("RIGHT CLICK");
            if (!chatBox.activeSelf)
            {
                if (transform.position.x < -1.013525f)
                    sr.flipX = false;
                else
                    sr.flipX = true;

                chatBox.SetActive(true);
                PositionChatBox();
            }
            else
                chatBox.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        // When clicked on, start dragging
        isDragging = true;

        // Calculate the offset between the objects positon and the mouse position
        offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseUp()
    {
        // When unclicked, stop dragging
        isDragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePoint.z = 0;

        return mousePoint;
    }

    void PositionChatBox()
    {
        // Get the world position of the clicked object
        Vector3 worldPosition = transform.position;
        Debug.Log("World Pos: " + worldPosition);

        // Convert the world position to screen coordinates
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        // Convert the screen position to a local position in the canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, // The RectTransform of the parent canvas
            screenPosition,      // The screen position of the object
            mainCamera,          // The camera rendering the screen space
            out Vector2 localPoint // The output local position within the canvas
        );

        // Apply an offset
        if (transform.position.x < -1.013525f)
        {
            chatBoxOffset = new Vector2(210f, 140f);
        }
        else
        {
            chatBoxOffset = new Vector2(-210f, 140f);
        }

        localPoint += chatBoxOffset;

        // Set the position of the chat panel to the calculated position
        chatBox.GetComponent<RectTransform>().localPosition = localPoint;
    }
}
