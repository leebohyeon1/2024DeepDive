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
    protected float _moveSpeed = 2f;  // �̵� �ӵ�
    [SerializeField] 
    protected int _attackDamage = 1;  // ���� ������
    [SerializeField]
    protected float _deathForce = 5f;   // ���� �� ���ư��� ��
    [SerializeField]
    protected float _torqueAngle = 60f; // ȸ���� (��ũ)
    [Space(20f)]
    [SerializeField]
    private float _destroyDelay = 3f; // ȭ�� �ۿ��� ���ŵǴ� �ð�
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

    // �� ��� ó��
    protected virtual void Die()
    {
        _isDead = true;

        _rb.velocity = Vector2.zero;

        // ���� ������� ���ư����� ���� ����
        _rb.AddForce(new Vector2(-1, 1) * _deathForce, ForceMode2D.Impulse);
        _rb.gravityScale = 0.5f;  // �ε巴�� ���������� �߷� ����


        transform.rotation = Quaternion.Euler(0, 0, _torqueAngle); 


        // ���� �ð� �Ŀ� ����
        Destroy(gameObject, _destroyDelay);
    }

    // ȭ�� ������ ������ ��� ������Ʈ ����
    private void OnBecameInvisible()
    {
        if (_isDead)
        {
            Destroy(gameObject);
        }
    }

}
