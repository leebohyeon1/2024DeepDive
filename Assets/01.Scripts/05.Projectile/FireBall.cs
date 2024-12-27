using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _lifetime = 10f;

    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.transform.GetComponent<Player>();
            player.TakeDamage(_damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("House"))
        {
            GameManager.Instance.TakeHouseDamage(_damage);
            Destroy(gameObject);
        }
    }

    public void InitialBall(int damage)
    {
        _damage = damage;
    }
}
