using System.Collections;
using System.Collections.Generic;
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
        var monsterBodyPos = _monster.Collider.bounds.center;
        int cnt = Physics.OverlapSphereNonAlloc(monsterBodyPos, _monster.DetectionRadius, _playerCollider, _monster.TargetMask);
        if (cnt != 0)
        {
            Debug.Log("물체 감지");
            var playerBodyPos = _playerCollider[0].bounds.center;
            var dir = playerBodyPos - monsterBodyPos;
            if (Vector3.Angle(_monster.transform.forward, dir) < _monster.DetectionAngle * 0.5f)
            {
                var dist = Vector3.Distance(monsterBodyPos, playerBodyPos);
                if (!Physics.Raycast(monsterBodyPos, dir.normalized, dist, _monster.ObstacleMask))
                {
                    Debug.Log("플레이어 감지");
                    Debug.DrawRay(monsterBodyPos, dir, Color.red);
                }
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
