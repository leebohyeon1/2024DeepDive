using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{


    [SerializeField]
    private float _attackRange = 5f;
    [SerializeField]
    private float _attackCooldown = 5f;

    [SerializeField]
    private GameObject _projectile;
    [SerializeField]
    private Transform _firePoint; 
    [SerializeField]
    private LayerMask _targetLayer;

    private float _lastAttackTime;

    protected override void Update()
    {
        if(_isDead)
        {
            return;
        }
        
        CheckAttackCondition();
    }

    private void CheckAttackCondition()
    {
        // ���� ���� ���� (Wizard�� �ٶ󺸴� ����)
        Vector2 rayDirection = transform.right;

        // ���� �߻�
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, _attackRange, _targetLayer);

        // ���̿� ������ ���
        if (hit.collider != null)
        {
            _animator.SetBool("IsMove", false);
            StopMoving();

            // ��Ÿ�� Ȯ�� �� ����
            if (Time.time >= _lastAttackTime + _attackCooldown)
            {
                Attack();
                _lastAttackTime = Time.time;
            }
        }
        else
        {
            _animator.SetBool("IsMove", true);
            Move(); 
        }
    }

    private void Attack()
    {
        if (_projectile != null && _firePoint != null)
        {
            _animator.SetTrigger("Attack");

            GameObject newProjectile = Instantiate(_projectile, _firePoint.position, Quaternion.identity);

            newProjectile.GetComponent<FireBall>().InitialBall(_attackDamage);
            newProjectile.GetComponent<Rigidbody2D>().velocity = Vector2.right * 10f;  // ����ü �ӵ�
        }
    }

    private void StopMoving()
    {
        _rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * _attackRange);
    }
}
