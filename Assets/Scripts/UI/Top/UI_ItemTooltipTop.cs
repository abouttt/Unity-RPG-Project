using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_ItemTooltipTop : UI_Base
{
    enum GameObjects
    {
        ItemTooltip,
    }

    enum Texts
    {
        ItemNameText,
        ItemTypeText,
        ItemDescText,
    }

    public UI_BaseSlot Target
    {
        get => _target;
        set
        {
            _target = value;
            if (_target == null)
            {
                Close();
            }
        }
    }

    [SerializeField]
    [Tooltip("Distance from mouse")]
    private Vector2 _deltaPosition;

    [Space(10)]
    [SerializeField]
    private Color _lowColor;
    [SerializeField]
    private Color _normalColor;
    [SerializeField]
    private Color _rareColor;
    [SerializeField]
    private Color _legendaryColor;

    private UI_BaseSlot _target;
    private ItemData _itemData;
    private RectTransform _rt;
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        _rt = GetObject((int)GameObjects.ItemTooltip).GetComponent<RectTransform>();
    }

    private void Start()
    {
        Managers.UI.Register<UI_ItemTooltipTop>(this);
        Close();
    }

    private void Update()
    {
        SetPosition(Mouse.current.position.ReadValue());

        if (_target == null)
        {
            return;
        }

        if (!_target.gameObject.activeInHierarchy)
        {
            Target = null;
            return;
        }

        if (UI_BaseSlot.IsDragging)
        {
            Close();
            return;
        }

        if (_target.HasObject)
        {
            if (_target.ObjectRef is Item item)
            {
                SetItemData(item.Data);
            }
            else if (_target.ObjectRef is ItemData itemData)
            {
                SetItemData(itemData);
            }
            else
            {
                Close();
            }
        }
        else if (!_target.HasObject)
        {
            Close();
        }
    }

    private void Close()
    {
        _itemData = null;
        GetObject((int)GameObjects.ItemTooltip).SetActive(false);
    }

    private void SetItemData(ItemData itemData)
    {
        GetObject((int)GameObjects.ItemTooltip).SetActive(true);

        if (_itemData != null && _itemData.Equals(itemData))
        {
            return;
        }

        _itemData = itemData;
        GetText((int)Texts.ItemNameText).text = _itemData.ItemName;
        SetItemQualityColor(_itemData.ItemQuality);
        SetType(_itemData.ItemType);
        SetDescription(_itemData);
    }

    private void SetItemQualityColor(ItemQuality itemQuality)
    {
        GetText((int)Texts.ItemNameText).color = itemQuality switch
        {
            ItemQuality.Low => _lowColor,
            ItemQuality.Normal => _normalColor,
            ItemQuality.Rare => _rareColor,
            ItemQuality.Legendary => _legendaryColor,
            _ => Color.red,
        };
    }

    private void SetType(ItemType itemType)
    {
        GetText((int)Texts.ItemTypeText).text = itemType switch
        {
            ItemType.Equipment => "[장비 아이템]",
            ItemType.Consumable => "[소비 아이템]",
            ItemType.Etc => "[기타 아이템]",
            _ => "[NULL]"
        };
    }

    private void SetDescription(ItemData itemData)
    {
        _sb.Clear();

        if (itemData is not EtcItemData)
        {
            _sb.Append($"제한 레벨 : {itemData.LimitLevel} \n");
        }

        if (itemData is EquipmentItemData equipmentItemData)
        {
            _sb.Append("\n");
            AppendValueIfGreaterThan0("공격력", equipmentItemData.Damage);
            AppendValueIfGreaterThan0("방어력", equipmentItemData.Defense);
            AppendValueIfGreaterThan0("체력", equipmentItemData.HP);
            AppendValueIfGreaterThan0("마나", equipmentItemData.MP);
            AppendValueIfGreaterThan0("기력", equipmentItemData.SP);
        }
        else if (itemData is ConsumableItemData consumableItemData)
        {
            _sb.Append($"소비 개수 : {consumableItemData.RequiredCount}\n");
        }

        if (_sb.Length > 0)
        {
            _sb.Append("\n");
        }

        if (itemData.Description.Length > 0)
        {
            _sb.Append($"{itemData.Description}\n\n");
        }

        GetText((int)Texts.ItemDescText).text = _sb.ToString();
    }

    private void AppendValueIfGreaterThan0(string text, int value)
    {
        if (value > 0)
        {
            _sb.Append($"{text} +{value}\n");
        }
    }

    private void SetPosition(Vector3 position)
    {
        var nextPosition = new Vector3
        {
            x = position.x + (_rt.rect.width * 0.5f) + _deltaPosition.x,
            y = position.y + (_rt.rect.height * 0.5f) + _deltaPosition.y
        };

        _rt.position = nextPosition;
    }
}
