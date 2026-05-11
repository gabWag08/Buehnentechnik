using UnityEngine;
using UnityEngine.UI;

public class XRScalling : MonoBehaviour
{
    public Slider xrScaleSlider;
    public Transform xrRig;
    private int defaultScale;
    
    void Start()
    {
        xrScaleSlider.onValueChanged.AddListener(OnXRScaleChanged);
        defaultScale = 10;
        xrScaleSlider.value = defaultScale;
    }

    void OnXRScaleChanged(float value)
    {
        if (xrRig != null)
        {
            xrRig.localScale = Vector3.one * value;
        }
    }

    public void ResetValue()
    {
        xrScaleSlider.value = defaultScale;
    }
}
