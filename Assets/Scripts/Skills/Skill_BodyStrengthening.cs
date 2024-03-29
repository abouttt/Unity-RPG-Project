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
            sb.AppendLine($"���� ���� : ü��, ���ݷ�, ���� {Data.StatTable[CurrentLevel - 1].HP}% ����");
        }

        if (CurrentLevel < Data.MaxLevel)
        {
            sb.AppendLine($"���� ���� : ü��, ���ݷ�, ���� {Data.StatTable[CurrentLevel].HP}% ����");
        }

        Data.StatDescription = sb.ToString();
    }
}
