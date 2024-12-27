using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashDishes : InteractableObject
{
    protected override void Start()
    {
        base.Start();

        EventManager.Instance.AddListener(EVENT_TYPE.WASH_DISHES, this);
    }
    
   
}
