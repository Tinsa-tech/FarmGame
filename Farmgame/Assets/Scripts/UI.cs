using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public UnityEvent<Plant> SeedSelectionChanged;

    Plant seedSelected;

    [SerializeField]
    private Plant corn;
    [SerializeField]
    private Plant tomato;
    [SerializeField]
    private Toggle cornToggle;
    [SerializeField]
    private Toggle tomatoToggle;
    [SerializeField]
    private TMP_Text moneyText;

    public void Start()
    {
        cornToggle.GetComponentInChildren<Text>().text = "Corn\n" + corn.seedValue.ToString();
        tomatoToggle.GetComponentInChildren<Text>().text = "Tomato\n" + tomato.seedValue.ToString();
    }

    public void OnCornValueChanged(bool newValue)
    {
        if (newValue)
        {
            seedSelected = corn;
            tomatoToggle.SetIsOnWithoutNotify(false);
        }
        else
        {
            seedSelected = null;
        }
        SeedSelectionChanged.Invoke(seedSelected);
    }

    public void OnTomatoValueChanged(bool newValue)
    {
        if (newValue)
        {
            seedSelected = tomato;
            cornToggle.SetIsOnWithoutNotify(false);
        }
        else 
        {
            seedSelected = null;
        }
        SeedSelectionChanged.Invoke(seedSelected);
    }

    public void OnMoneyChanged(int newValue)
    {
        moneyText.text = newValue.ToString();
    }

}
