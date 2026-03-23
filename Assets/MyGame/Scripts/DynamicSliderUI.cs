using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicSliderUI : MonoBehaviour
{
    public GameObject sliderPrefab;
    public Transform sliderParent;

    public float minSliderHeight = 20f;
    public float maxSliderHeight = 1920f;
    public float spacing = 1;

    private List<GameObject> currentTargets = new List<GameObject>();
    private List<GameObject> sliders = new List<GameObject>();

    void Update()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Speaker");

        if (targets.Length != currentTargets.Count)
        {
            RefreshSliders(targets);
        }
    }

    void RefreshSliders(GameObject[] targets)
{
    Canvas.ForceUpdateCanvases();
    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)sliderParent);

    foreach (GameObject s in sliders)
    {
        Destroy(s);
    }

    sliders.Clear();
    currentTargets.Clear();

    int count = targets.Length;
    if (count == 0) return;

    RectTransform parentRect = sliderParent.GetComponent<RectTransform>();
    float parentHeight = parentRect.rect.height;

    float totalSpacing = spacing * (count - 1);
    float sliderHeight = (parentHeight - totalSpacing) / count;

    sliderHeight = Mathf.Clamp(sliderHeight, minSliderHeight, maxSliderHeight);

    foreach (GameObject t in targets)
    {
        GameObject newSlider = Instantiate(sliderPrefab, sliderParent);

        RectTransform rt = newSlider.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, sliderHeight);

        sliders.Add(newSlider);
        currentTargets.Add(t);
    }
}
}