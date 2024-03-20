using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UI_ConversationPopup : UI_Popup, IPointerClickHandler
{
    enum Texts
    {
        NPCNameText,
        ScriptText,
    }

    enum Buttons
    {
        CloseButton,
    }

    [SerializeField]
    private float _typingSpeed;
    private NPC _npc;
    private int _currentIndex = 0;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_ConversationPopup>);
    }

    private void Start()
    {
        Managers.UI.Register<UI_ConversationPopup>(this);

        Closed += () =>
        {
            _npc = null;
            GetText((int)Texts.ScriptText).DOKill();
            Managers.UI.Get<UI_NPCMenuPopup>().PopupRT.gameObject.SetActive(true);
        };
    }

    public void SetNPC(NPC npc)
    {
        _npc = npc;
        _currentIndex = 0;
        GetText((int)Texts.NPCNameText).text = npc.NPCName;
        GetText((int)Texts.ScriptText).text = null;
        GetText((int)Texts.ScriptText).DOText(
            npc.ConversationScripts[_currentIndex], _npc.ConversationScripts[_currentIndex].Length / _typingSpeed);
    }

    // ��ȭ ���� Ŭ���� ��簡 ������ �ִ� �����̶�� ��� ��簡 ��µǰ�
    // �̹� ��簡 �� ��µ� ���¶�� ���� ��ȭ�� �Ѿ�ų� ������ �����Ѵ�.
    public void OnPointerClick(PointerEventData eventData)
    {
        var script = GetText((int)Texts.ScriptText);
        if (script.text.Length == _npc.ConversationScripts[_currentIndex].Length)
        {
            _currentIndex++;
            if (_currentIndex >= _npc.ConversationScripts.Count)
            {
                Managers.UI.Close<UI_ConversationPopup>();
                return;
            }

            script.text = null;
            script.DOText(_npc.ConversationScripts[_currentIndex], _npc.ConversationScripts[_currentIndex].Length / _typingSpeed);
        }
        else
        {
            script.DOKill();
            script.text = _npc.ConversationScripts[_currentIndex];
        }
    }
}
