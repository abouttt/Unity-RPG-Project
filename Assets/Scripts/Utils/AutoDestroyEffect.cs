using System.Collections;
using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    [SerializeField]
    private bool _isPassiveDestruction;
    [SerializeField]
    private float _destroyTime;

    private ParticleSystem _particleSystem;
    private Coroutine _destoryCoroutine;
    private static WaitForSeconds _delay;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        if (_isPassiveDestruction)
        {
            _delay = new(_destroyTime);
        }
    }

    private void OnEnable()
    {
        if (_isPassiveDestruction)
        {
            _destoryCoroutine = StartCoroutine(Destory());
        }
    }

    private void OnDisable()
    {
        if (_isPassiveDestruction)
        {
            if (_destoryCoroutine != null)
            {
                StopCoroutine(_destoryCoroutine);
            }
        }
    }

    private void OnParticleSystemStopped()
    {
        Managers.Resource.Destroy(gameObject);
    }

    private IEnumerator Destory()
    {
        yield return _delay;

        _particleSystem.Stop();
    }
}
