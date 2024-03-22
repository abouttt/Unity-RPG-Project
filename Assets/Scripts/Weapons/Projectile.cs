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

    [SerializeField]
    private string _hitEffectAddressableName;

    private Vector3 _dir;
    private bool _hasDir;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Projectile");
    }

    private void Update()
    {
        if (!_hasDir)
        {
            _dir = transform.localRotation * Vector3.forward;
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
                if (!Player.Battle.IsDefending)
                {
                    return;
                }

                Player.Battle.HitShield(other.ClosestPoint(transform.position));
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Managers.Resource.Instantiate(_hitEffectAddressableName, transform.position, null, true);
        }

        Managers.Resource.Destroy(gameObject);
    }

    private void OnDisable()
    {
        _hasDir = false;
    }
}