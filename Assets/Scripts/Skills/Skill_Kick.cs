using UnityEngine;

public class Skill_Kick : Skill
{
    public Skill_Kick(SkillData data)
        : base(data)
    { }

    public override bool Use()
    {
        if (!CanUse())
        {
            return false;
        }

        Debug.Log("Kick");
        return true;
    }
}
