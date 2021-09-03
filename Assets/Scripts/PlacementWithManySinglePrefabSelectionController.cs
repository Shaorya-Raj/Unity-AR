using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using TMPro;


[RequireComponent(typeof(ARRaycastManager))]
public class PlacementWithManySinglePrefabSelectionController : MonoBehaviour
{

//new additions
    [SerializeField]
    private Button arGreenButton;

    [SerializeField]
    private Button arRedButton;

    [SerializeField]
    private Button arBlueButton;

    [SerializeField]
    private Text selectionText;
//new additions

    [SerializeField]
    private GameObject placedPrefab;

    [SerializeField]
    private GameObject welcomePanel;

    [SerializeField]
    private Button dismissButton;

    //remove object
    [SerializeField]
    private Button removeObject;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    public Dropdown myDrop;

    //to scale the object
    //[SerializeField]
    //private bool applyScalingPerObject = false;

    //[SerializeField]
    //private Slider scaleSlider;

    //[SerializeField]
    //private Text scaleTextValue;


    private GameObject placedObject;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private ARSessionOrigin aRSessionOrigin;

    private bool onTouchHold = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    //private PlacementObject lastSelectedObject;

    private GameObject PlacedPrefab 
    {
        get 
        {
            return placedPrefab;
        }
        set 
        {
            placedPrefab = value;
        }
    }

    public void Awake() 
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
//new additions
        //ChangePrefabTo("Leo");

        myDrop.onValueChanged.AddListener(delegate{PrefabChange(myDrop);});

        //arGreenButton.onClick.AddListener(() => ChangePrefabTo("Leo"));
        //arBlueButton.onClick.AddListener(() => ChangePrefabTo("Typhoon"));
        //arRedButton.onClick.AddListener(() => ChangePrefabTo("Apache"));
//new additions
        dismissButton.onClick.AddListener(Dismiss);
        //scaleSlider.onValueChanged.AddListener(ScaleChanged);
        removeObject.onClick.AddListener(RemoveObject);
    }

    public void PrefabChange(Dropdown myDrop)
    {
        //placedPrefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");

        if(placedPrefab == null)
        {
            Debug.LogError($"Prefab could not be loaded, make sure you check the naming of your prefabs...");
        }
        
        switch(myDrop.value)
        {
            case 0:
                selectionText.text = $"Selected: <color='red'>None</color>";
                placedPrefab = null;
            break;
            case 1:
                selectionText.text = $"Selected: <color='white'>Apache</color>";
                placedPrefab = Resources.Load<GameObject>("Prefabs/Apache");
            break;
            case 2:
                selectionText.text = $"Selected: <color='white'>Rafale</color>";
                placedPrefab = Resources.Load<GameObject>("Prefabs/Dassault Rafale");
            break;
            case 3:
                selectionText.text = $"Selected: <color='white'>Leopard</color>";
                placedPrefab = Resources.Load<GameObject>("Prefabs/Leo");
            break;
            case 4:
                selectionText.text = $"Selected: <color='white'>M247</color>";
                placedPrefab = Resources.Load<GameObject>("Prefabs/M247");
            break;
            case 5:
                selectionText.text = $"Selected: <color='white'>Typhoon</color>";
                placedPrefab = Resources.Load<GameObject>("Prefabs/Typhoon");
            break;
        }
    }

    public void RemoveObject()
    {
        foreach (GameObject placedObject in Object.FindObjectsOfType<GameObject>()) 
        {
            //Destroy(placedObject);
            placedObject.GetComponent<Renderer>().enabled = false;
            //placedObject.SetActive(false);
        }             
    }

//new additons
/*    public void ChangePrefabTo(string prefabName)
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
            case "Dassault Rafale":
                selectionText.text = $"Selected: <color='white'>{prefabName}</color>";
            break;
            case "M247":
                selectionText.text = $"Selected: <color='white'>{prefabName}</color>";
            break;
        }
    }*/
//new additons

    private void Dismiss() => welcomePanel.SetActive(false);

    /*public void ScaleChanged(float value)
    {
        float newValue = scaleSlider.value;

        if(applyScalingPerObject){
            if(lastSelectedObject != null && lastSelectedObject.Selected)
            {
                placedPrefab.transform.parent.localScale = new Vector3(newValue,newValue,newValue); //Vector3.one * newValue;
            }
        }
        else 
            aRSessionOrigin.transform.localScale = new Vector3(newValue,newValue,newValue);

       // scaleTextValue.text = $"Scale {newValue}";
    }*/

    void Update()
    {
        // do not capture events unless the welcome panel is hidden
        if(welcomePanel.activeSelf)
            return;

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;
            
            touchPosition = touch.position;

            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                /*if(Physics.Raycast(ray, out hitObject))
                {
                    lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();
                    if(lastSelectedObject != null)
                    {
                        PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                        foreach(PlacementObject placementObject in allOtherObjects)
                        {
                            if(placementObject != lastSelectedObject){
                                placementObject.Selected = false;
                            }
                            else
                                placementObject.Selected = true;
                        }
                    }
                }*/
                if(arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    //if(placedObject == null)
                    //{
                        placedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation)/*.GetComponent<PlacementObject>()*/;
                    //}
                }
            }  

            if(touch.phase == TouchPhase.Moved)
            {
                if(arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    //if(placedObject != null /*&& lastSelectedObject.Selected*/)
                    //{
                        placedObject.transform.parent.position = hitPose.position;
                        placedObject.transform.parent.rotation = hitPose.rotation;
                    //}
                }
            }
        }
    }
}