using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public List<Field> jobQueue;
    public Waypoint currentWaypoint;
    public FieldGrid fieldGrid;

    [SerializeField]
    private float speed;

    private bool isWorking;
    private Animator anim;

    [SerializeField]
    private float turnAnimLength;
    [SerializeField]
    private float turnAnimDegrees;
    [SerializeField]
    private float workAnimLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jobQueue = new List<Field>();
        anim = GetComponent<Animator>();
    }

    public void OnNewJob(Field toDo)
    {
        jobQueue.Add(toDo);
        if (!isWorking)
        {
            isWorking = true;
            StartCoroutine(doJobCoroutine());
        }
    }

    public IEnumerator doJobCoroutine()
    {
        if (90 % turnAnimDegrees != 0)
        {
            Debug.LogWarning("turn Animation is not compatible with 90 Degree Turns");
        }
        int nrTurnsTo90Degrees = (int)Mathf.Floor(90 / turnAnimDegrees);
        while (jobQueue.Count > 0)
        {
            Field goTo = jobQueue[0];
            jobQueue.RemoveAt(0);
            Vector2Int waypointPos = FieldGrid.FieldPosToWaypointPos(goTo.posInGrid);

            Waypoint[] path = fieldGrid.FindPath(currentWaypoint.gridPosition, waypointPos);

            foreach (Waypoint waypoint in path)
            {
                Vector3 dir = waypoint.position - this.transform.position;

                float angle = Vector3.SignedAngle(dir, this.transform.forward, Vector3.up);

                int count = 0;

                if (angle < 91 && angle > 89)
                {
                    anim.SetBool("walk", false);
                    anim.SetBool("turnLeft", true);
                    angle = 90;
                    count = nrTurnsTo90Degrees;
                }
                else if (angle < -89 && angle > -91)
                {
                    anim.SetBool("walk", false);
                    anim.SetBool("turnRight", true);
                    angle = -90;
                    count = nrTurnsTo90Degrees;
                }
                else if (angle < -179 && angle > 179)
                {
                    anim.SetBool("walk", false);
                    anim.SetBool("turnRight", true);
                    angle = 180;
                    count = nrTurnsTo90Degrees * 2;
                }

                for (int i = 0; i < count; i++)
                {
                    yield return new WaitForSeconds(turnAnimLength);
                    transform.Rotate(Vector3.up, Mathf.Sign(angle) * -turnAnimDegrees);
                }

                anim.SetBool("turnLeft", false);
                anim.SetBool("turnRight", false);
                anim.SetBool("walk", true);
                dir = waypoint.position - this.transform.position;
                float dist = dir.magnitude;
                dir = Vector3.Normalize(dir);
                float distRemaining = dist;
                while (distRemaining > 0)
                {
                    Vector3 translate = dir * Time.deltaTime * speed;
                    if (translate.magnitude > distRemaining)
                    {
                        translate = dir * distRemaining;
                    }
                    this.transform.Translate(translate, Space.World);
                    distRemaining -= translate.magnitude;
                    yield return null;
                }
                currentWaypoint = waypoint;
            }

            float angleToField = Vector3.SignedAngle(Vector3.right, transform.forward, Vector3.up);
            bool needsTurn = false;
            if (angleToField < 91 && angleToField > 89)
            {
                anim.SetBool("walk", false);
                anim.SetBool("turnLeft", true);
                angleToField = 90;
                needsTurn = true;
            }
            else if (angleToField < -89 && angleToField > -91)
            {
                anim.SetBool("walk", false);
                anim.SetBool("turnRight", true);
                angleToField = -90;
                needsTurn = true;
            }

            if (needsTurn)
            {
                for (int i = 0; i < nrTurnsTo90Degrees; i++)
                {
                    yield return new WaitForSeconds(turnAnimLength);
                    transform.Rotate(Vector3.up, Mathf.Sign(angleToField) * -turnAnimDegrees);
                }
                anim.SetBool("turnRight", false);
                anim.SetBool("turnLeft", false);
            }

            anim.SetBool("work", true);
            yield return new WaitForSeconds(workAnimLength);
            anim.SetBool("work", false);
            doJob(goTo);
        }

        isWorking = false;
    }

    public virtual void doJob(Field doJobOn){}
}
