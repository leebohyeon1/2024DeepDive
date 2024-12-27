using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private PlayerEventInteract _playerEventInteract;

    [SerializeField]
    private int _maxHp = 0;
    private int _curHp = 0;

    [SerializeField]
    private float _moveSpeed = 10f;
    private float _velocity = 0; // 이동 방향

    [SerializeField]
    private int _attackDamage = 2;
    [SerializeField]
    private Vector2 _attackRange;  // 공격 범위
    [SerializeField] 
    private LayerMask _enemyLayer;      // 공격할 대상 (적 레이어)
    [SerializeField] 
    private float _attackCooldown = 0.5f; // 공격 쿨타임

    private float _lastAttackTime = 0f;

    private bool _canInteract = false;
 
    private Transform[] _eventObjs;
   
    private void Start()
    {
        InitialPlayer();
    }

    private void Update()
    {

        if(_playerEventInteract.IsInteract)
        {
            return;
        }

        Move();

        if (_canInteract && Input.GetKeyDown(KeyCode.E)) 
        {
            Interact();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Time.time >= _lastAttackTime + _attackCooldown)
            {
                Debug.Log("공격");
                PerformAttack();
                _lastAttackTime = Time.time;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Interactable"))
        {
            _canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            _canInteract = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(_spriteRenderer == null)
        {
            return;
        }
        Gizmos.DrawWireCube(transform.position + new Vector3((_spriteRenderer.flipX == true? 1 : -1) * (_attackRange.x / 2), 0f ), _attackRange);
    }

    private void InitialPlayer()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerEventInteract = GetComponent<PlayerEventInteract>();

        _curHp = _maxHp;

        _eventObjs = new Transform[GameManager.Instance.GetEventObjs().Length];
        _eventObjs = GameManager.Instance.GetEventObjs();



    }

    private void Move()
    {
        _velocity = Input.GetAxisRaw("Horizontal");

        _rb.velocity = new Vector2(_velocity * _moveSpeed, 0);

        if(_velocity == 1)
        {
            _spriteRenderer.flipX = true;
        }
        else if(_velocity == -1)
        {
            _spriteRenderer.flipX = false;
        }

    }

    private void Interact()
    {
        GameObject eventObj = CalcurateDistance();

        InteractableObject interactable = eventObj.GetComponent<InteractableObject>();

        if(interactable.CanInteract)
        {
            _playerEventInteract.SetInteract(true);
            _playerEventInteract.SetInteractType(interactable.InteractType);
        }
    }

    private GameObject CalcurateDistance()
    {
        GameObject closestObj = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform obj in _eventObjs)
        {
            if (obj != null)
            {
                float distance = Vector2.Distance(transform.position, obj.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObj = obj.gameObject;
                }
            }
        }

        return closestObj;
    }

    private void PerformAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(transform.position + 
            new Vector3((_spriteRenderer.flipX == true ? 1 : -1) * (_attackRange.x / 2), 0f), _attackRange, 0,_enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetComponent<FireBall>() != null)
            {
                Destroy(enemy.gameObject);
            }

            Enemy damageable = enemy.GetComponent<Enemy>();
            if (damageable != null)
            {
                damageable.TakeDamage(_attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        _curHp -= damage;

        if (_curHp <= 0) 
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.GAME_OVER, this);
        }
    }

   
}
