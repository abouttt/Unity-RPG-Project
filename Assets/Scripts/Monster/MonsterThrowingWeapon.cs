using UnityEngine;

public class MonsterThrowingWeapon : MonoBehaviour
{
    [SerializeField]
    private MonsterData _goblinThrowerData;
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
        if (other.CompareTag("Player"))
        {
            if (Player.Movement.IsRolling)
            {
                return;
            }

            Player.Battle.TakeDamage(null, transform.position, _goblinThrowerData.Damage, false);
        }
        else if (other.CompareTag("Shield"))
        {
            Player.Battle.HitShield();
        }

        Managers.Resource.Destroy(gameObject);
    }

    private void OnDisable()
    {
        _hasDir = false;
    }
}
