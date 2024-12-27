using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cook : InteractableObject
{
    protected override void Start()
    {
        base.Start();

        EventManager.Instance.AddListener(EVENT_TYPE.COOK, this);
    }
}
