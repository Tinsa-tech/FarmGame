using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool isSowing = false;
    public bool isSeedModeOn = false;
    [SerializeField]
    public Camera mainCamera;
    [SerializeField]
    public FieldGrid fieldGrid;

    private Plant selected;
    private int money = 10;

    [SerializeField]
    InputActionAsset playerInput;

    public UnityEvent<int> MoneyChanged;

    InputAction clickLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clickLocation = playerInput.FindAction("ClickLocation");
        GameObject uiObject = GameObject.Find("UI");
        try
        {
            UI ui = uiObject.GetComponent<UI>();
            ui.SeedSelectionChanged.AddListener(OnSeedChanged);
            MoneyChanged.AddListener(ui.OnMoneyChanged);
        } catch (NullReferenceException e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }
        if (!isSowing)
        {
            return;
        }

        if (money < selected.seedValue)
        {
            return;
        }

        Vector2 mousePos = clickLocation.ReadValue<Vector2>();
        LayerMask layerMask = LayerMask.GetMask("Clickable");

        RaycastHit hit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(mousePos), out hit, 1000, layerMask))
        {
            Field fieldHit = hit.transform.gameObject.GetComponent<Field>();
            fieldHit.PlantPlant(selected);
            Debug.Log("planted");
            money -= selected.seedValue;
            MoneyChanged.Invoke(money);
        }
    }

    public void OnSeedChanged(Plant newSeed)
    {
        if (newSeed == null)
        {
            if (isSowing)
            {
                toggleSeedMode();
            }
            isSowing = false;
        } 
        else
        {
            if (!isSowing)
            {
                toggleSeedMode();
            }
            isSowing = true;
        }
        selected = newSeed;
    }

    public void toggleSeedMode()
    {
        if (!isSeedModeOn)
        {
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 6;
            mainCamera.gameObject.transform.position = new Vector3(0, 10, -1.5f);
            mainCamera.gameObject.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));

            foreach(Field field in fieldGrid.GetFields())
            {
                if (field.isFull == true)
                {
                    field.planted.showUI();
                }
            }

            isSeedModeOn = true;
        }
        else
        {
            mainCamera.orthographic = false;
            mainCamera.gameObject.transform.position = new Vector3(0, 6, -11);
            mainCamera.gameObject.transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));

            foreach (Field field in fieldGrid.GetFields())
            {
                if (field.isFull == true)
                {
                    field.planted.hideUI();
                }
            }

            isSeedModeOn = false;
        }
    }

    public void OnPlantHarvested(Plant harvested)
    {
        money += harvested.sellValue;
        MoneyChanged.Invoke(money);
    }
}
