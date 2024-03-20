using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Collider Collider { get; private set; }

    [SerializeField]
    private string _hitEffectAddressableName;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Weapon");

        Collider = GetComponent<Collider>();
        Collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Monster>().TakeDamage(Player.EquipmentInventory.GetItem(EquipmentType.Weapon).EquipmentData.Damage))
        {
            Managers.Resource.Instantiate(_hitEffectAddressableName, other.bounds.center, null, true);
        }
    }
}
