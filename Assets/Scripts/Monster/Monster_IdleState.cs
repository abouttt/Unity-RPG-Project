using UnityEngine;

public class Monster_IdleState : StateMachineBehaviour
{
    public Monster _monster;
    public readonly Collider[] _playerCollider = new Collider[1];

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var monsterCenterPos = _monster.Collider.bounds.center;
        int cnt = Physics.OverlapSphereNonAlloc(monsterCenterPos, _monster.DetectionRadius, _playerCollider, _monster.TargetMask);
        if (cnt != 0)
        {
            Debug.Log("물체 감지");
            var playerCenterPos = _playerCollider[0].bounds.center;
            var centerDir = (playerCenterPos - monsterCenterPos).normalized;
            if (Vector3.Angle(_monster.transform.forward, centerDir) < _monster.DetectionAngle * 0.5f)
            {
                var monsterEyesPos = _monster.Eyes.position;
                var eyesDir = playerCenterPos - monsterEyesPos;
                var EyesToPlayerCenterDist = Vector3.Distance(monsterEyesPos, playerCenterPos);
                if (!Physics.Raycast(monsterEyesPos, eyesDir.normalized, EyesToPlayerCenterDist, _monster.ObstacleMask))
                {
                    Debug.Log("플레이어 감지");
                    Debug.DrawRay(monsterEyesPos, eyesDir, Color.red);
                }
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
