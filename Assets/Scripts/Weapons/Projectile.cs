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
    private bool _canDestroy;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Projectile");
        _rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 force)
    {
        _rb.velocity = Vector3.zero;
        _rb.AddForce(force, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        _canDestroy = true;

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

                Player.Battle.TakeDamage(null, other.ClosestPoint(transform.position), Damage, false);
            }
            else if (other.CompareTag("Shield"))
            {
                if (!Player.Battle.IsDefending)
                {
                    return;
                }

                Player.Battle.HitShield(other.ClosestPoint(transform.position));
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                _canDestroy = false;
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Managers.Resource.Instantiate(_hitEffectAddressableName, other.ClosestPoint(transform.position), null, true);
        }

        if (_canDestroy)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}
