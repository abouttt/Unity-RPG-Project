using System.Text;
using UnityEngine;

public class Skill_BodyStrengthening : Skill
{
    public Skill_BodyStrengthening(SkillData data)
        : base(data)
    { }

    public override bool Use()
    {
        if (CurrentLevel > 1)
        {
            Player.Status.ExtraPerStat -= Data.StatTable[CurrentLevel - 2];
        }

        Player.Status.ExtraPerStat += Data.StatTable[CurrentLevel - 1];

        return true;
    }

    public override int ResetSkill()
    {
        if (CurrentLevel > 0)
        {
            Player.Status.ExtraPerStat -= Data.StatTable[CurrentLevel - 1];
        }

        return base.ResetSkill();
    }

    protected override void RefreshStatDescription()
    {
        var sb = new StringBuilder(50);

        if (CurrentLevel > 0)
        {
            sb.AppendLine($"현재 레벨 : 체력, 공격력, 방어력 {Data.StatTable[CurrentLevel - 1].HP}% 증가");
        }

        if (CurrentLevel < Data.MaxLevel)
        {
            sb.AppendLine($"다음 레벨 : 체력, 공격력, 방어력 {Data.StatTable[CurrentLevel].HP}% 증가");
        }

        Data.StatDescription = sb.ToString();
    }
}
