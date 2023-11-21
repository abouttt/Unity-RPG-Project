using UnityEngine;
using UnityEngine.Events;

public class UI_NPCMenuSubitem : UI_Base
{
    enum Buttons
    {
        Button,
    }

    enum Texts
    {
        Text,
    }

    protected override void Init()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
    }

    public void SetEvent(string text, UnityAction call)
    {
        GetText((int)Texts.Text).text = text;
        GetButton((int)Buttons.Button).onClick.AddListener(call);
    }

    public void Clear()
    {
        GetButton((int)Buttons.Button).onClick.RemoveAllListeners();
    }
}
