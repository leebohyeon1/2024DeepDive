using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabySeat : InteractableObject
{
    protected override void Start()
    {
        base.Start();

        EventManager.Instance.AddListener(EVENT_TYPE.BABY_SEAT, this);
    }

}
