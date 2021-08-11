using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementWithManySinglePrefabSelectionController : MonoBehaviour
{
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

    //to scale the object
    [SerializeField]
    private bool applyScalingPerObject = false;

    [SerializeField]
    private Slider scaleSlider;

    [SerializeField]
    private Text scaleTextValue;


    private GameObject placedObject;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private ARSessionOrigin aRSessionOrigin;

    private bool onTouchHold = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private PlacementObject lastSelectedObject;

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
        dismissButton.onClick.AddListener(Dismiss);
        scaleSlider.onValueChanged.AddListener(ScaleChanged);
        removeObject.onClick.AddListener(RemoveObject);
    }

    public void RemoveObject()
    {
        Destroy(lastSelectedObject);              
    }

    private void Dismiss() => welcomePanel.SetActive(false);

    public void ScaleChanged(float newValue)
    {
        if(applyScalingPerObject){
            if(lastSelectedObject != null && lastSelectedObject.Selected)
            {
                lastSelectedObject.transform.parent.localScale = Vector3.one * newValue;
            }
        }
        else 
            aRSessionOrigin.transform.localScale = Vector3.one * newValue;

        scaleTextValue.text = $"Scale {newValue}";
    }

    public void Update()
    {
        // do not capture events unless the welcome panel is hidden
        if(welcomePanel.activeSelf)
            return;

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            touchPosition = touch.position;

            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if(Physics.Raycast(ray, out hitObject))
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
                }
                if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    if(lastSelectedObject == null)
                    {
                        lastSelectedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                    }
                }
            }  

            if(touch.phase == TouchPhase.Moved)
            {
                if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    if(lastSelectedObject != null && lastSelectedObject.Selected)
                    {
                        lastSelectedObject.transform.parent.position = hitPose.position;
                        lastSelectedObject.transform.parent.rotation = hitPose.rotation;
                    }
                }
            }
        }
    }
}