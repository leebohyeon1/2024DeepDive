using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D _rb;
    protected Animator _animator;

    [SerializeField]
    protected int _maxHp = 0;
    protected int _curHp = 0;

    [SerializeField]
    protected float _moveSpeed = 2f;  // 이동 속도
    [SerializeField] 
    protected int _attackDamage = 1;  // 공격 데미지
    [SerializeField]
    protected float _deathForce = 5f;   // 죽을 때 날아가는 힘
    [SerializeField]
    protected float _torqueAngle = 60f; // 회전력 (토크)
    [Space(20f)]
    [SerializeField]
    private float _destroyDelay = 3f; // 화면 밖에서 제거되는 시간
    protected bool _isDead = false;

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
        _animator = GetComponent<Animator>();

        _curHp = _maxHp;
    }

    protected virtual void Move()
    {
        if (_isDead)
        {
            return;
        }
        
        _rb.velocity = new Vector2(_moveSpeed, _rb.velocity.y);
    }

    public virtual void TakeDamage(int damage)
    {
        _curHp -= damage;

        if (_curHp <= 0 && !_isDead)
        {
            Die();
            GameManager.Instance.IncreaseKillCount();
        }
    }

    // 적 사망 처리
    protected virtual void Die()
    {
        _isDead = true;

        _rb.velocity = Vector2.zero;

        // 좌측 상단으로 날아가도록 힘을 가함
        _rb.AddForce(new Vector2(-1, 1) * _deathForce, ForceMode2D.Impulse);
        _rb.gravityScale = 0.5f;  // 부드럽게 떨어지도록 중력 설정


        transform.rotation = Quaternion.Euler(0, 0, _torqueAngle); 


        // 일정 시간 후에 제거
        Destroy(gameObject, _destroyDelay);
    }

    // 화면 밖으로 나가면 즉시 오브젝트 제거
    private void OnBecameInvisible()
    {
        if (_isDead)
        {
            Destroy(gameObject);
        }
    }

}
