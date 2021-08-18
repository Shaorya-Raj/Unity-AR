using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scale : MonoBehaviour
{

    private Slider scaleSlider;
    private Slider rotateSlider;
    public float scaleMinValue;
    public float scaleMaxValue;
    public float rotMinValue;
    public float rotMaxValue;
    // Start is called before the first frame update
    void Start()
    {
        scaleSlider = GameObject.Find("ScaleSlider").GetComponent<Slider>();
        scaleSlider.minValue = scaleMinValue;
        scaleSlider.maxValue = scaleMaxValue;

        scaleSlider.onValueChanged.AddListener(ScaleSliderUpdate); 

        scaleSlider = GameObject.Find("RotateSlider").GetComponent<Slider>();
        scaleSlider.minValue = rotMinValue;
        scaleSlider.maxValue = rotMaxValue;

        scaleSlider.onValueChanged.AddListener(RotateSliderUpdate);
    }

    // Update is called once per frame
    void ScaleSliderUpdate(float value)
    {
        transform.localScale = new Vector3(value, value, value);
    }

    void RotateSliderUpdate(float value)
    {
        transform.localEulerAngles = new Vector3(transform.rotation.x, value, transform.rotation.z);
    }
}
