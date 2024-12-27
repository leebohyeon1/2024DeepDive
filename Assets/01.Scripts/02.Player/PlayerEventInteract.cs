using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventInteract : MonoBehaviour
{
    public bool IsInteract { get; private set; }

    private int _interactType = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if (IsInteract)
        {
            PerformMiniGame(_interactType);
        }
    }

    public void SetInteract(bool boolean)
    {
        IsInteract = boolean;
    }

    public void SetInteractType(int type)
    {
        _interactType = type;
    }

    private void PerformMiniGame(int type)
    {
        switch (type)
        {
            case 0:

                break;

            case 1:

                break;

            case 2:
                
                break;
        }

        // 미니게임 종료 후 상호작용 상태 해제
        IsInteract = false;
    }
}
