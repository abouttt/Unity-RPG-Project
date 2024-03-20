using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int Damage { get; set; }

    [SerializeField]
    private bool _player;

    [SerializeField]
    private bool _enemy;

    [SerializeField]
    private float _speed;

    private Vector3 _dir;
    private bool _hasDir;

    private void Update()
    {
        if (!_hasDir)
        {
            _dir = (transform.localRotation * Vector3.forward);
            transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
            _hasDir = true;
        }

        transform.position += _dir * (Time.deltaTime * _speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_player)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<Monster>().TakeDamage(Damage);
            }
        }
        else if (_enemy)
        {
            if (other.CompareTag("Player"))
            {
                if (Player.Movement.IsRolling)
                {
                    return;
                }

                Player.Battle.TakeDamage(null, transform.position, Damage, false);
            }
            else if (other.CompareTag("Shield"))
            {
                Player.Battle.HitShield(other.bounds.ClosestPoint(transform.position));
            }
        }

        Managers.Resource.Destroy(gameObject);
    }

    private void OnDisable()
    {
        _hasDir = false;
    }
}
