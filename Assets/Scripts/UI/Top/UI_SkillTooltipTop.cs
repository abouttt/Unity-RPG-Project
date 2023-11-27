using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_SkillTooltipTop : UI_Base
{
    enum GameObjects
    {
        SkillTooltip,
    }

    enum Texts
    {
        SkillNameText,
        SkillTypeText,
        SkillDescText,
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

    private UI_BaseSlot _target;
    private SkillData _skillData;
    private RectTransform _rt;
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        _rt = GetObject((int)GameObjects.SkillTooltip).GetComponent<RectTransform>();
    }

    private void Start()
    {
        Managers.UI.Register<UI_SkillTooltipTop>(this);
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
            if (_target.ObjectRef is Skill skill)
            {
                SetSkillData(skill);
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
        _skillData = null;
        GetObject((int)GameObjects.SkillTooltip).SetActive(false);
    }

    private void SetSkillData(Skill skill)
    {
        GetObject((int)GameObjects.SkillTooltip).SetActive(true);

        if (_skillData != null && !_skillData.Equals(skill.Data))
        {
            return;
        }

        _skillData = skill.Data;
        GetText((int)Texts.SkillNameText).text = _skillData.SkillName;
        GetText((int)Texts.SkillTypeText).text = $"[{_skillData.SkillType}]";
        SetDescription(skill);
    }

    private void SetDescription(Skill skill)
    {
        _sb.Clear();
        _sb.AppendFormat("{0}\n\n", _skillData.Description);
        _sb.AppendFormat("※습득조건※\n");

        foreach (var parent in skill.Parents)
        {
            if (parent.Children.TryGetValue(skill, out var level))
            {
                _sb.AppendFormat("- {0} Lv.{1}\n", parent.Data.SkillName, level);
            }
        }

        _sb.AppendFormat("- 필요 스킬 포인트 : {0}\n", _skillData.RequiredSkillPoint);
        _sb.AppendFormat("- 제한레벨 : {0}\n\n", _skillData.RequiredPlayerLevel);
        GetText((int)Texts.SkillDescText).text = _sb.ToString();
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
