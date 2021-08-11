using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementControllerWithMultiple : MonoBehaviour
{
    [SerializeField]
    private Button arGreenButton;

    [SerializeField]
    private Button arRedButton;

    [SerializeField]
    private Button arBlueButton;

    [SerializeField]
    private Button dismissButton;

    [SerializeField]
    private GameObject welcomePanel;

    [SerializeField]
    private Text selectionText;

    private GameObject placedPrefab;

    private ARRaycastManager arRaycastManager;

    void Awake() 
    {
        arRaycastManager = GetComponent<ARRaycastManager>();

        // set initial prefab, change buttons to dropdown menu later
        ChangePrefabTo("Leo");

        arGreenButton.onClick.AddListener(() => ChangePrefabTo("Leo"));
        arBlueButton.onClick.AddListener(() => ChangePrefabTo("Typhoon"));
        arRedButton.onClick.AddListener(() => ChangePrefabTo("Apache"));
        dismissButton.onClick.AddListener(Dismiss);
    }


    //removes welcome panel so objects can be placed
    private void Dismiss() => welcomePanel.SetActive(false);

    //change the selected model to be displayed on screen
    public void ChangePrefabTo(string prefabName)
    {
        placedPrefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");

        if(placedPrefab == null)
        {
            Debug.LogError($"Prefab with name {prefabName} could not be loaded, make sure you check the naming of your prefabs...");
        }
        
        switch(prefabName)
        {
            case "Typhoon":
                selectionText.text = $"Selected: <color='white'>{prefabName}</color>";
            break;
            case "Apache":
                selectionText.text = $"Selected: <color='white'>{prefabName}</color>";
            break;
            case "Leo":
                selectionText.text = $"Selected: <color='white'>{prefabName}</color>";
            break;
        }
    }

    //get the position where the object will be placed when screen is tapped
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;

        return false;
    }

    void Update()
    {
        if(placedPrefab == null || welcomePanel.gameObject.activeSelf)
        {
            return;
        }

        if(!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        //place object
        if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
        }
    }


    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
}