using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class UI_ConfirmationPopup : UI_Popup
{
    enum Texts
    {
        ConfirmationText,
        OKText,
        NOText,
    }

    enum Buttons
    {
        OKButton,
        NOButton,
    }

    private DOTweenAnimation _dotween;

    protected override void Init()
    {
        base.Init();

        _dotween = PopupRT.GetComponent<DOTweenAnimation>();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.NOButton).onClick.AddListener(Managers.UI.Close<UI_ConfirmationPopup>);
    }

    private void Start()
    {
        Managers.UI.Register<UI_ConfirmationPopup>(this);

        Showed += () =>
        {
            PopupRT.localScale = new Vector3(0f, 1f, 1f);
            _dotween.DORestart();
        };
    }

    // 예 버튼에 이벤트 설정.
    public void SetEvent(UnityAction call, string text, string OKText = "예", string NOText = "아니오", bool hasOK = true, bool hasNO = true)
    {
        GetButton((int)Buttons.OKButton).onClick.RemoveAllListeners();
        GetButton((int)Buttons.OKButton).onClick.AddListener(call);
        GetButton((int)Buttons.OKButton).onClick.AddListener(Managers.UI.Close<UI_ConfirmationPopup>);
        GetButton((int)Buttons.OKButton).gameObject.SetActive(hasOK);
        GetButton((int)Buttons.NOButton).gameObject.SetActive(hasNO);
        GetText((int)Texts.OKText).text = OKText;
        GetText((int)Texts.NOText).text = NOText;
        GetText((int)Texts.ConfirmationText).text = text;
    }
}
