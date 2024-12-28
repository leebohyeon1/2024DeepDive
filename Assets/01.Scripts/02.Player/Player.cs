using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour, IListener
{
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private PlayerEventInteract _playerEventInteract;
    private Animator _animator;

    [SerializeField] private int _maxHp = 100;
    private int _curHp;

    [SerializeField] private float _moveSpeed = 10f;
    private float _velocity = 0f;

    [SerializeField] private int _attackDamage = 2;
    [SerializeField] private Vector2 _attackRange;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private Animator _fireAnimator;
    [SerializeField] private SpriteRenderer _fireSprite;

    private float _lastAttackTime = 0f;
    private bool _canInteract = false;
    private bool _isAttack = false;
    private bool _isInteracting = false;

    private Transform[] _eventObjs;
    [SerializeField] private Slider _hpBar;

    private SpriteRenderer _houseChildRenderer = null;  // House 자식의 SpriteRenderer 저장용

    private void Start()
    {
        InitializePlayer();
    }

    private void Update()
    {
        if (_playerEventInteract.IsInteract || _isAttack || _isInteracting) return;

        if (_canInteract && Input.GetKeyDown(KeyCode.E) && !_isInteracting)
        {
            Interact();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !_isInteracting && Time.time >= _lastAttackTime + _attackCooldown)
        {
            PerformAttack();
            _lastAttackTime = Time.time;
            return;
        }

        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            _canInteract = true;
        }

        // House와 충돌한 경우
        if (collision.CompareTag("House"))
        {
            if (_houseChildRenderer == null)
            {
                _houseChildRenderer = collision.GetComponent<SpriteRenderer>();
            }

            if (_houseChildRenderer != null)
            {
                // 투명화 (0.3초에 걸쳐 0.3의 알파 값으로 변경)
                _houseChildRenderer.DOFade(0.0f, 0.3f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            _canInteract = false;
        }

        // House와 떨어졌을 때
        if (collision.CompareTag("House") && _houseChildRenderer != null)
        {
            // 불투명하게 복원 (0.3초에 걸쳐 알파값 1로 복구)
            _houseChildRenderer.DOFade(1f, 0.3f);
            _houseChildRenderer = null;  // 참조 초기화
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.STOP_INTERACT, this);
    }

    private void OnApplicationQuit()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.STOP_INTERACT, this);
    }


    // 디버그 공격 범위 시각화
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_spriteRenderer == null) return;

        Vector3 attackPosition = transform.position + new Vector3((_spriteRenderer.flipX ? 1 : -1) * (_attackRange.x / 2), 0f);
        Gizmos.DrawWireCube(attackPosition, _attackRange);
    }

    // 플레이어 초기화
    private void InitializePlayer()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerEventInteract = GetComponent<PlayerEventInteract>();
        _animator = GetComponent<Animator>();

        _curHp = _maxHp;
        _hpBar.value = 1f;

        _eventObjs = GameManager.Instance.GetEventObjs();
        _playerEventInteract.SetUI(_eventObjs[0].GetComponent<InteractableObject>().Slider,
                                   _eventObjs[0].GetComponent<Cook>().TargetZone,
                                   _eventObjs[1].GetComponent<InteractableObject>().Slider,
                                   _eventObjs[2].GetComponent<InteractableObject>().Slider);

        EventManager.Instance.AddListener(EVENT_TYPE.STOP_INTERACT, this);
    }

    // 이벤트 리스너 (애니메이션 상태 확인)
    public void OnEvent(EVENT_TYPE Event_type, Component Sender, object Param = null)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Player_Action"))
        {
            _playerEventInteract.SetInteract(false);
            _playerEventInteract.TriggerGameOver();
            _animator.SetTrigger("EndAction");
            _isInteracting = false;
        }
    }

    // 이동 처리
    private void Move()
    {
        _velocity = Input.GetAxisRaw("Horizontal");
        _rb.velocity = new Vector2(_velocity * _moveSpeed, _rb.velocity.y);

        if (_velocity != 0)
        {
            _spriteRenderer.flipX = _velocity > 0;
            _animator.SetBool("IsMove", true);
        }
        else
        {
            _animator.SetBool("IsMove", false);
        }
    }

    // 상호작용 처리
    private void Interact()
    {
        GameObject eventObj = CalculateClosestObject();

        if (eventObj == null) return;

        InteractableObject interactable = eventObj.GetComponent<InteractableObject>();
        if (interactable.CanInteract)
        {
            _animator.SetTrigger("Action");
            _rb.velocity = Vector2.zero;
            transform.position = new Vector2(eventObj.transform.position.x, transform.position.y);

            _playerEventInteract.SetInteract(true);
            _playerEventInteract.SetInteractType(interactable.InteractType);
            _isInteracting = true;
        }
    }

    // 가장 가까운 상호작용 오브젝트 계산
    private GameObject CalculateClosestObject()
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

    // 공격 처리
    private void PerformAttack()
    {
        _isAttack = true;
        _rb.velocity = Vector2.zero;
        _animator.SetTrigger("Attack");
    }

    public void Attack()
    {
        _fireSprite.flipX = _spriteRenderer.flipX ? true : false;
        Vector3 firePos = _fireAnimator.gameObject.transform.localPosition;
        _fireAnimator.transform.localPosition = new Vector2((_spriteRenderer.flipX ? 2.8f : -2.8f), firePos.y);
        
        _fireAnimator.SetTrigger("Fire");
 

        Vector3 attackPosition = transform.position + new Vector3((_spriteRenderer.flipX ? 1 : -1) * (_attackRange.x / 2), 0f);
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPosition, _attackRange, 0, _enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetComponent<FireBall>() != null)
            {
                Destroy(enemy.gameObject);
            }

            Enemy damageable = enemy.GetComponent<Enemy>();
            damageable?.TakeDamage(_attackDamage);
        }
    }

    // 플레이어 데미지 처리
    public void TakeDamage(int damage)
    {
        _curHp = Mathf.Max(_curHp - damage, 0);
        float targetValue = (float)_curHp / _maxHp;
        _hpBar.DOValue(targetValue, 0.3f).SetEase(Ease.OutCubic);

        if (_curHp == 0)
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.GAME_OVER, this);
        }
    }

    // 공격 종료 처리
    public void EndAttack()
    {
        _isAttack = false;
    }
}
