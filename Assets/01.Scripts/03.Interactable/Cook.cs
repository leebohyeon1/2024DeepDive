using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cook : InteractableObject
{
    public RectTransform TargetZone;
    protected override void Start()
    {
        base.Start();

        EventManager.Instance.AddListener(EVENT_TYPE.COOK, this);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.COOK, this);
    }

    private void OnApplicationQuit()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.COOK, this);
    }

}
