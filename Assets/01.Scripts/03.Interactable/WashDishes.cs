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

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.WASH_DISHES, this);
    }

    private void OnApplicationQuit()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.WASH_DISHES, this);
    }
}
