using Unity.VisualScripting;
using UnityEngine;

public class FieldGrid : MonoBehaviour
{
    [SerializeField]
    public GameObject field;
    [SerializeField]
    public Vector2Int gridSize;
    [SerializeField]
    public Vector2 spacing;

    private Field[,] fields;
    private Waypoint[,] waypoints;

    public void Start()
    {
        fields = new Field[gridSize.x, gridSize.y];
        waypoints = new Waypoint[gridSize.x, gridSize.y * 2];

        Worker gardener = GameObject.Find("Gardener").GetComponent<Worker>();
        Worker harvester = GameObject.Find("Harvester").GetComponent<Worker>();

        Player player = GameObject.Find("PlayerObject").GetComponent<Player>();

        Vector2 offset = new Vector2(gridSize.x / 2 * spacing.x , gridSize.y / 2 * spacing.y );
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GameObject inst = Instantiate(field, new Vector3(x * spacing.x - offset.x, 0, y * spacing.y - offset.y), Quaternion.identity);
                inst.transform.parent = this.transform;
                Field instField = inst.GetComponent<Field>();
                fields[x, y] = instField;
                instField.posInGrid = new Vector2Int(x, y);
                instField.PlantNeedsWatering.AddListener(gardener.OnNewJob);
                instField.PlantNeedsHarvesting.AddListener(harvester.OnNewJob);
                instField.PlantHarvested.AddListener(player.OnPlantHarvested);

                
                Vector3 waypointPosition = new Vector3(x * spacing.x - offset.x - spacing.x / 2, 0, y * spacing.y - offset.y - spacing.y / 2);
                Waypoint waypoint = new Waypoint(new Vector2Int(x, 2 * y), waypointPosition);
                Vector3 waypoint2Position = new Vector3(x * spacing.x - offset.x - spacing.x / 2, 0, y * spacing.y - offset.y);
                Waypoint waypoint2 = new Waypoint(new Vector2Int(x, 2 * y + 1), waypoint2Position);
                waypoints[x, y * 2] = waypoint;
                waypoints[x, y * 2 + 1] = waypoint2;
                
            }
        }
        Waypoint gardenerWaypoint = waypoints[0, 0];
        gardener.transform.position = gardenerWaypoint.position;
        gardener.currentWaypoint = gardenerWaypoint;
        gardener.fieldGrid = this;

        Waypoint harvesterWaypoint = waypoints[gridSize.x - 1, 0];
        harvester.transform.position = harvesterWaypoint.position;
        harvester.currentWaypoint = harvesterWaypoint;
        harvester.fieldGrid = this;

    }

    public Waypoint[] FindPath(Vector2Int from, Vector2Int to)
    {
        Waypoint[] path;
        if (from.Equals(to))
        {
            path = new Waypoint[0];
            return path;
        }
        if (from.x == to.x)
        {
            path = new Waypoint[1];
            path[0] = waypoints[to.x, to.y];
            return path;
        }
        if (to.y % 2 == 0)
        {
            path = new Waypoint[2];
            path[0] = waypoints[from.x, to.y];
            path[1] = waypoints[to.x, to.y];
        }
        else
        {
            path = new Waypoint[3];
            path[0] = waypoints[from.x, to.y - 1];
            path[1] = waypoints[to.x, to.y - 1];
            path[2] = waypoints[to.x, to.y];
        }
        return path;

    }

    public static Vector2Int FieldPosToWaypointPos(Vector2Int fieldPos)
    {
        Vector2Int WaypointPos = new Vector2Int(fieldPos.x, fieldPos.y * 2 + 1);
        return WaypointPos;
    }

    public Field[,] GetFields()
    {
        return fields;
    }
}
