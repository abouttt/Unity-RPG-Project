using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private Interactive _target;
    private UI_InteractionKeyGuide _keyGuide;
    private bool _isOnKeyGuide;

    private void Start()
    {
        _keyGuide = Managers.UI.Get<UI_AutoCanvas>().InteractionKeyGuide;
    }

    private void Update()
    {
        if (_target == null)
        {
            return;
        }

        if (!CanInteraction())
        {
            return;
        }

        if (Managers.Input.Interaction && _target.CanInteraction)
        {
            _keyGuide.SetTarget(null);
            _isOnKeyGuide = false;
            _target.Interaction();
        }
    }

    private void SetTarget(Interactive target)
    {
        _target = target;
        _keyGuide.SetTarget(target);
        _isOnKeyGuide = target != null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!CanInteraction())
        {
            return;
        }

        if (!other.CompareTag("Interactive"))
        {
            return;
        }

        if (_target == null)
        {
            SetTarget(other.GetComponent<Interactive>());
        }
        else
        {
            if (_target.gameObject != other.gameObject)
            {
                var targetDistance = Vector3.SqrMagnitude(transform.position - _target.transform.position);
                var otherDistance = Vector3.SqrMagnitude(transform.position - other.transform.position);
                if (otherDistance < targetDistance)
                {
                    SetTarget(other.GetComponent<Interactive>());
                }
            }
        }

        if (!_isOnKeyGuide)
        {
            SetTarget(other.GetComponent<Interactive>());
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

        SetTarget(null);
    }

    public bool CanInteraction()
    {
        return !(Managers.UI.IsOn<UI_LootPopup>() || Managers.UI.IsOn<UI_NPCMenuPopup>());
    }
}
