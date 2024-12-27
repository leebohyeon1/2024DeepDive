using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;

    [SerializeField]
    private int _maxHp = 0;
    private int _curHp = 0;

    [SerializeField]
    private float _moveSpeed = 10f;
    private float _velocity = 0; // 이동 방향

    [SerializeField]
    private int _attackDamage = 2;

    private bool _canInteract = false;
    private Transform[] _eventObjs;
   
    private void Start()
    {
        InitialPlayer();
    }

    private void Update()
    {
        Move();

        if (_canInteract && Input.GetKeyDown(KeyCode.E)) 
        {
            Interact();
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


    private void InitialPlayer()
    {
        _rb = GetComponent<Rigidbody2D>();


        _curHp = _maxHp;

        _eventObjs = new Transform[GameManager.Instance.GetEventObjs().Length];
        _eventObjs = GameManager.Instance.GetEventObjs();



    }

    private void Move()
    {
        _velocity = Input.GetAxisRaw("Horizontal");

        _rb.velocity = new Vector3(_velocity * _moveSpeed, 0, 0);

    }

    private void Interact()
    {
        GameObject eventObj = CalcurateDistance();

        InteractableObject interactable = eventObj.GetComponent<InteractableObject>();

        Debug.Log(eventObj.name);
    }

    public void TakeDamage(int damage)
    {
        _curHp -= damage;

        if (_curHp <= 0) 
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.GAME_OVER, this);
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
}
