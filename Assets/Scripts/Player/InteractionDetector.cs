using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    public bool IsInteracted => _target != null && _target.IsInteracted;
    public bool IsKeyGuideShowed => _keyGuide.gameObject.activeSelf;

    [SerializeField]
    [ReadOnly]
    private Interactive _target;
    private UI_InteractionKeyGuide _keyGuide;
    private bool _isColliderOutFromTarget;

    private void Start()
    {
        _keyGuide = Managers.Resource.Instantiate("UI_InteractionKeyGuide.prefab").GetComponent<UI_InteractionKeyGuide>();
    }

    private void Update()
    {
        if (_target == null)
        {
            return;
        }

        if (!_target.gameObject.activeInHierarchy)
        {
            SetTarget(null);
            return;
        }

        if (_isColliderOutFromTarget && !_target.IsInteracted)
        {
            SetTarget(null);
            return;
        }

        if (Managers.Input.Interaction && _target.CanInteraction && !_target.IsInteracted)
        {
            _keyGuide.gameObject.SetActive(false);
            _target.IsInteracted = true;
            _target.Interaction();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_target != null)
        {
            if (_target.IsInteracted)
            {
                return;
            }
            else if (!_keyGuide.gameObject.activeSelf)
            {
                _keyGuide.gameObject.SetActive(true);
            }
        }

        if (_target == null)
        {
            SetTarget(other.GetComponent<Interactive>());
        }
        else if (_target.gameObject != other.gameObject)
        {
            float targetDistance = Vector3.SqrMagnitude(transform.position - _target.transform.position);
            float otherDistance = Vector3.SqrMagnitude(transform.position - other.transform.position);
            if (otherDistance < targetDistance)
            {
                SetTarget(other.GetComponent<Interactive>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_target == null)
        {
            return;
        }

        if (_target.gameObject != other.gameObject)
        {
            return;
        }

        if (_target.IsInteracted)
        {
            _isColliderOutFromTarget = true;
            return;
        }

        _target.IsInteracted = false;
        SetTarget(null);
    }

    private void SetTarget(Interactive target)
    {
        if (_target != null)
        {
            _target.IsSensed = false;
        }

        _target = target;
        _keyGuide.SetTarget(target);
        _isColliderOutFromTarget = false;

        if (_target != null)
        {
            _target.IsSensed = true;
        }
    }
}
