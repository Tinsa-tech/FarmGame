using UnityEngine;
using UnityEngine.Events;

public class Field : MonoBehaviour
{
    public Plant planted;
    public bool isFull = false;
    public Vector2Int posInGrid;

    public UnityEvent<Field> PlantNeedsWatering;
    public UnityEvent<Field> PlantNeedsHarvesting;
    public UnityEvent<Plant> PlantHarvested;

    public void PlantPlant(Plant toPlant)
    {
        planted = Instantiate(toPlant, this.transform.position + new Vector3(0, 0.15f, 0), Quaternion.identity, this.transform);

        planted.finishedGrowthCycle.AddListener(onPlantGrowthFinished);
        planted.StartGrowing();


        isFull = true;
    }

    private void onPlantGrowthFinished()
    {
        if (planted.GetGrowthState() == Plant.GrowState.HALF)
        {
            PlantNeedsWatering.Invoke(this);
        }
        else if (planted.GetGrowthState() == Plant.GrowState.FULL)
        {
            PlantNeedsHarvesting.Invoke(this);
        }
    }

    public void Harvest()
    {
        PlantHarvested.Invoke(planted);
        Destroy(planted.gameObject);
        isFull = false;
    }

    public void Water()
    {
        planted.Water();
    }
}
