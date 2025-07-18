using UnityEngine;

public class Waypoint
{
    public Vector2Int gridPosition;
    public Vector3 position;

    public Waypoint(Vector2Int gridPos, Vector3 pos) {
        gridPosition = gridPos;
        position = pos;
    }
}
