using UnityEngine;

public class Skill_Punch : Skill
{
    public Skill_Punch(SkillData data)
        : base(data)
    { }

    public override bool Use()
    {
        if (!CheckCanUse())
        {
            return false;
        }

        Debug.Log("Skill Punch!!!");

        return true;
    }
}
