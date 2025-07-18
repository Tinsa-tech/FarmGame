using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Gardener : Worker
{
    public override void doJob(Field doJobOn)
    {
        doJobOn.Water();
    }
}
