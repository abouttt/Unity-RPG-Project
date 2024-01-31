using UnityEngine;

public class Monster_IdleState : StateMachineBehaviour
{
    private Monster _monster;
    private readonly Collider[] _playerCollider = new Collider[1];

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var monsterCenterPos = _monster.Collider.bounds.center;
        int cnt = Physics.OverlapSphereNonAlloc(monsterCenterPos, _monster.DetectionRadius, _playerCollider, _monster.TargetMask);
        if (cnt != 0)
        {
            var playerCenterPos = _playerCollider[0].bounds.center;
            var centerDir = (playerCenterPos - monsterCenterPos).normalized;
            if (Vector3.Angle(_monster.transform.forward, centerDir) < _monster.DetectionAngle * 0.5f)
            {
                var monsterEyesPos = _monster.Eyes.position;
                var eyesDir = playerCenterPos - monsterEyesPos;
                var EyesToPlayerCenterDist = Vector3.Distance(monsterEyesPos, playerCenterPos);
                if (!Physics.Raycast(monsterEyesPos, eyesDir.normalized, EyesToPlayerCenterDist, _monster.ObstacleMask))
                {
                    _monster.Animator.SetTrigger(_monster.AnimIDTracking);
                }
            }
        }
    }
}
