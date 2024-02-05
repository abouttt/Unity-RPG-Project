using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UI_ConversationPopup : UI_Popup, IPointerClickHandler
{
    enum Buttons
    {
        CloseButton,
    }

    enum Texts
    {
        NPCNameText,
        ScriptText,
    }

    [SerializeField]
    private float _typingSpeed;
    private NPC _target;
    private int _currentIndex = 0;

    protected override void Init()
    {
        base.Init();

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_ConversationPopup>);

        Managers.UI.Register<UI_ConversationPopup>(this);
    }

    private void Start()
    {
        Closed += () =>
        {
            GetText((int)Texts.ScriptText).DOKill();
            Managers.UI.Get<UI_NPCMenuPopup>().ToggleMenu(true);
        };
    }

    public void SetNPC(NPC npc)
    {
        _target = npc;
        _currentIndex = 0;
        GetText((int)Texts.NPCNameText).text = npc.Name;
        GetText((int)Texts.ScriptText).text = null;
        GetText((int)Texts.ScriptText).DOText(
            npc.ConversationScripts[_currentIndex], _target.ConversationScripts[_currentIndex].Length / _typingSpeed);
    }

    // ��ȭ ���� Ŭ���� ��簡 ������ �ִ� �����̶�� ��� ��簡 ��µǰ�
    // �̹� ��簡 �� ��µ� ���¶�� ���� ��ȭ�� �Ѿ�ų� ������ �����Ѵ�.
    public void OnPointerClick(PointerEventData eventData)
    {
        var script = GetText((int)Texts.ScriptText);
        if (script.text.Length == _target.ConversationScripts[_currentIndex].Length)
        {
            _currentIndex++;
            if (_currentIndex >= _target.ConversationScripts.Count)
            {
                Managers.UI.Close<UI_ConversationPopup>();
                return;
            }

            script.text = null;
            script.DOText(_target.ConversationScripts[_currentIndex], _target.ConversationScripts[_currentIndex].Length / _typingSpeed);
        }
        else
        {
            script.DOKill();
            script.text = _target.ConversationScripts[_currentIndex];
        }
    }
}
