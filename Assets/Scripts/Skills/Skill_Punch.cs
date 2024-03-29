using UnityEngine;

public class Skill_Punch : Skill
{
    public Skill_Punch(SkillData data)
        : base(data)
    { }

    public override bool Use()
    {
        if (!CanUse())
        {
            return false;
        }

        Debug.Log("Punch");
        return true;
    }

    protected override void RefreshStatDescription()
    {
        Data.StatDescription = "Desc";
    }
}
