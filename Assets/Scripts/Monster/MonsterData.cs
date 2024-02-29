using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monster Data", fileName = "Monster Data")]
public class MonsterData : ScriptableObject
{
    [field: SerializeField]
    public string MonsterID { get; private set; }
    [field: SerializeField]
    public string MonsterName {  get; private set; }
    [field: SerializeField]
    public int MaxHP { get; private set; }
    [field: SerializeField]
    public int Damage { get; private set; }
    [field: SerializeField]
    public int Defense { get; private set; }
    [field: SerializeField]
    public int DropMinXP { get; private set; }
    [field: SerializeField]
    public int DropMaxXP { get; private set; }
    [field: SerializeField]
    public int DropMinGold { get; private set; }
    [field: SerializeField]
    public int DropMaxGold { get; private set; }
    [field: SerializeField]
    public List<LootData> LootData { get; private set; }

    public int GetXP() => Random.Range(DropMinXP, DropMaxXP + 1);

    public int GetGold() => Random.Range(DropMinGold, DropMaxGold + 1);

    public void DropItems(Vector3 position)
    {
        if (LootData == null || LootData.Count == 0)
        {
            return;
        }

        var go = Managers.Resource.Instantiate("FieldItem.prefab", null, true);
        var fieldItem = go.GetComponent<FieldItem>();

        foreach (var data in LootData)
        {
            if (Random.Range(0, 101) <= data.DropProbability)
            {
                fieldItem.AddItem(data.ItemData, Random.Range(data.Count.Min, data.Count.Max + 1));
            }
        }

        go.transform.position = position;
    }
}
