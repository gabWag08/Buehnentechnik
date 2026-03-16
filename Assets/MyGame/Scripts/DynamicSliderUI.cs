using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicSliderUI : MonoBehaviour
{
    public GameObject sliderPrefab;
    public Transform sliderParent;

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
        // alte Slider löschen
        foreach (GameObject s in sliders)
        {
            Destroy(s);
        }

        sliders.Clear();
        currentTargets.Clear();

        // neue Slider erstellen
        foreach (GameObject t in targets)
        {
            GameObject newSlider = Instantiate(sliderPrefab, sliderParent);
            sliders.Add(newSlider);
            currentTargets.Add(t);
        }
    }
}