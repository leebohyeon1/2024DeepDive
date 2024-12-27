using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D _rb;

    [SerializeField]
    protected int _maxHp = 0;
    protected int _curHp = 0;

    [SerializeField]
    protected float _moveSpeed = 2f;  // 이동 속도
    [SerializeField] 
    protected int _attackDamage = 1;  // 공격 데미지

    protected virtual void Start()
    {
        InitialEnemy();
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void InitialEnemy()
    {
        _rb = GetComponent<Rigidbody2D>();
        _curHp = _maxHp;
    }

    protected virtual void Move()
    {
        _rb.velocity = new Vector2(_moveSpeed, _rb.velocity.y);
    }

    public virtual void TakeDamage(int damage)
    {
        _curHp -= damage;

        if(_curHp <= 0 )
        {
            Destroy(gameObject);
        }
    }


}
