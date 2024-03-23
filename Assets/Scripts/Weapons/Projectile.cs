using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int Damage { get; set; }

    [SerializeField]
    private bool _player;

    [SerializeField]
    private bool _enemy;

    [SerializeField]
    private string _hitEffectAddressableName;

    private Rigidbody _rb;
    private Collider _collider;
    private bool _canDestroy;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Projectile");
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void Shoot(Vector3 force)
    {
        _rb.velocity = Vector3.zero;
        _rb.AddForce(force, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        _canDestroy = false;

        if (_player)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<Monster>().TakeDamage(Damage);
                _canDestroy = true;
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
                _canDestroy = true;
            }
            else if (other.CompareTag("Shield"))
            {
                if (!Player.Battle.IsDefending)
                {
                    return;
                }

                Player.Battle.HitShield(other.ClosestPoint(transform.position));
                _canDestroy = true;
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Managers.Resource.Instantiate(_hitEffectAddressableName, transform.position, null, true);
            _canDestroy = true;
        }

        if (_canDestroy)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}
