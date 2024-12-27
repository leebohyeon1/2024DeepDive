using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int _maxHp;
    private int _curHp;

    private float _moveSpeed;

    private int _attackDamage;

    private void Start()
    {
        InitialPlayer();
    }

    private void Update()
    {
        
    }

    private void InitialPlayer()
    {
        _curHp = _maxHp;

        
    }
}
