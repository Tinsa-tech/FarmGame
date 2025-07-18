using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Plant : MonoBehaviour
{
    [SerializeField]
    private float timeToGrow = 0;
    [SerializeField]
    private float timeToRipen = 0;

    [SerializeField]
    private GameObject halfGrown;
    [SerializeField]
    private GameObject fullGrown;

    private GameObject representation;

    private bool isGrowing = false;

    public UnityEvent finishedGrowthCycle;

    [SerializeField]
    private Canvas ui;
    [SerializeField]
    private TMP_Text progressText;
    [SerializeField]
    private Slider progressSlider;

    [SerializeField]
    public int sellValue;
    [SerializeField]
    public int seedValue;

    public enum GrowState {NEW, HALF, FULL}

    private GrowState state = GrowState.NEW;

    private bool isUIVisible = false;

    public void StartGrowing()
    {
        if (isGrowing)
        {
            return;
        }

        if (state == GrowState.NEW)
        {
            StartCoroutine(GrowCoroutine(timeToGrow));
        } else if (state == GrowState.HALF)
        {
            StartCoroutine(GrowCoroutine(timeToRipen));
        }
    }

    public void Water()
    {
        StartGrowing();
    }

    private IEnumerator GrowCoroutine(float time)
    {
        float growTime = 0.0f;
        isGrowing = true;
        while (growTime < time)
        {
            growTime += Time.deltaTime;
            progressSlider.value = growTime / time;
            progressText.text = "Growing... \n finished in " + Mathf.Floor(time - growTime).ToString();
            yield return null;
        }
        AdvanceState();
        isGrowing = false;
        finishedGrowthCycle.Invoke();
    }

    private void AdvanceState()
    {
        switch (state){
            case GrowState.NEW:
                state = GrowState.HALF;
                if (representation)
                {
                    Destroy(representation);
                }
                representation = Instantiate(halfGrown, this.transform.position, Quaternion.identity);
                representation.transform.parent = this.transform;
                progressText.text = "Needs Water!";
                
                break;
            case GrowState.HALF:
                state = GrowState.FULL;
                if (representation)
                {
                    Destroy(representation);
                }
                representation = Instantiate(fullGrown, this.transform.position, Quaternion.identity);
                representation.transform.parent = this.transform;
                progressText.text = "Fully Grown!";
                break;
        }
    }

    public Plant.GrowState GetGrowthState()
    {
        return state;
    }

    public void showUI()
    {
        ui.gameObject.SetActive(true);
        isUIVisible = true;
    }

    public void hideUI()
    {
        ui.gameObject.SetActive(false);
        isUIVisible = false;
    }
}
