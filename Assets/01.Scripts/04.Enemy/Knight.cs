using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Enemy
{
    private bool _isKnockedBack = false;

    [SerializeField]
    private Vector2 _knockBackForce;
    [SerializeField]
    private float _knockbackDistance;
    [SerializeField]
    private float _knockbackTime;

    protected override void Start()
    {
        base.Start();
        _animator.SetBool("IsMove", true);
    }

    protected override void Move()
    {
        if(_isKnockedBack)
        {
            _animator.speed = 0;
            return;
        }

        _animator.speed = 1;
        base.Move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.transform.GetComponent<Player>();
            player.TakeDamage(_attackDamage);

            Knockback();
        }
        else if(collision.gameObject.CompareTag("House"))
        {
            GameManager.Instance.TakeHouseDamage(_attackDamage);

            Knockback();
        }

        if(collision.gameObject.CompareTag("Ground"))
        {
            returnState();
        }

    }

    private void Knockback()
    {
        if (!_isKnockedBack)
        {
            _isKnockedBack = true;

            float initialVelocity = CalculateKnockbackVelocity(_knockbackDistance, _knockbackTime);

            Vector2 knockbackForce = new Vector2(_knockBackForce.x * initialVelocity, Mathf.Abs(_knockBackForce.x * initialVelocity));

            _rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        }
    }

    private float CalculateKnockbackVelocity(float distance, float time)
    {
        return distance / time;
    }

    private void returnState() 
    {
        _isKnockedBack = false;
    }
}
