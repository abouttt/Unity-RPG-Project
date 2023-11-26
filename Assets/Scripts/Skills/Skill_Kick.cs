using UnityEngine;

public class Skill_Kick : Skill
{
    public Skill_Kick(SkillData data)
        : base(data)
    { }

    public override bool Use()
    {
        if (!CheckCanUse())
        {
            return false;
        }

        Debug.Log("Skill Kick!!!");

        return true;
    }
}
