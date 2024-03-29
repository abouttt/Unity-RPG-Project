using System;
using UnityEngine;

[Serializable]
public class PlayerStatData
{
    public int HP;
    public int MP;
    public float SP;
    public int XP;
    public int Damage;
    public int Defense;

    public static PlayerStatData operator +(PlayerStatData a, PlayerStatData b)
    {
        return new PlayerStatData
        {
            HP = a.HP + b.HP,
            MP = a.MP + b.MP,
            SP = a.SP + b.SP,
            XP = a.XP + b.XP,
            Damage = a.Damage + b.Damage,
            Defense = a.Defense + b.Defense,
        };
    }

    public static PlayerStatData operator -(PlayerStatData a, PlayerStatData b)
    {
        return new PlayerStatData
        {
            HP = a.HP - b.HP,
            MP = a.MP - b.MP,
            SP = a.SP - b.SP,
            XP = a.XP - b.XP,
            Damage = a.Damage - b.Damage,
            Defense = a.Defense - b.Defense,
        };
    }
}