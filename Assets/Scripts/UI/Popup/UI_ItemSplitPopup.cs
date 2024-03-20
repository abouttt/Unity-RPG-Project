using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class UI_ItemSplitPopup : UI_Popup
{
    enum GameObjects
    {
        ItemPrice,
    }

    enum Texts
    {
        GuideText,
        PriceText,
    }

    enum Buttons
    {
        UpButton,
        DownButton,
        OKButton,
        NOButton,
    }

    enum InputFields
    {
        InputField,
    }

    public int CurrentCount { get; private set; }

    private int _price;
    private int _minCount;
    private int _maxCount;

    private DOTweenAnimation _dotween;

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));

        _dotween = PopupRT.GetComponent<DOTweenAnimation>();

        Get<TMP_InputField>((int)InputFields.InputField).onValueChanged.AddListener(delegate { OnValueChanged(); });
        Get<TMP_InputField>((int)InputFields.InputField).onEndEdit.AddListener(delegate { OnEndEdit(); });

        GetButton((int)Buttons.UpButton).onClick.AddListener(() => OnClickUpOrDownButton(1));
        GetButton((int)Buttons.DownButton).onClick.AddListener(() => OnClickUpOrDownButton(-1));
        GetButton((int)Buttons.NOButton).onClick.AddListener(Managers.UI.Close<UI_ItemSplitPopup>);
    }

    private void Start()
    {
        Managers.UI.Register<UI_ItemSplitPopup>(this);

        Showed += () =>
        {
            PopupRT.localScale = new Vector3(0f, 1f, 1f);
            _dotween.DORestart();
        };
    }

    public void SetEvent(UnityAction call, string text, int minCount, int maxCount, int price = 0, bool showPrice = false)
    {
        GetButton((int)Buttons.OKButton).onClick.RemoveAllListeners();
        GetButton((int)Buttons.OKButton).onClick.AddListener(() => { Managers.UI.Close<UI_ItemSplitPopup>(); });
        GetButton((int)Buttons.OKButton).onClick.AddListener(call);

        CurrentCount = _maxCount = maxCount;
        _minCount = minCount;
        _price = price;

        GetText((int)Texts.GuideText).text = text;
        Get<TMP_InputField>((int)InputFields.InputField).text = CurrentCount.ToString();
        GetText((int)Texts.PriceText).color = (_price * CurrentCount) <= Player.Status.Gold ? Color.white : Color.red;
        GetObject((int)GameObjects.ItemPrice).SetActive(showPrice);
    }

    private void OnValueChanged()
    {
        var inputField = Get<TMP_InputField>((int)InputFields.InputField);
        CurrentCount = string.IsNullOrEmpty(inputField.text) ? 0 : int.Parse(inputField.text);

        if (inputField.text.Length > 0)
        {
            int count = int.Parse(inputField.text);
            if (count > _maxCount)
            {
                CurrentCount = _maxCount;
                inputField.text = CurrentCount.ToString();
            }
        }

        int totalPrice = _price * CurrentCount;
        GetText((int)Texts.PriceText).text = totalPrice.ToString();
        GetText((int)Texts.PriceText).color = totalPrice <= Player.Status.Gold ? Color.white : Color.red;
    }

    private void OnEndEdit()
    {
        var inputField = Get<TMP_InputField>((int)InputFields.InputField);
        int nextCount = string.IsNullOrEmpty(inputField.text) ? 0 : int.Parse(inputField.text);
        CurrentCount = Mathf.Clamp(nextCount, _minCount, _maxCount);
        inputField.text = CurrentCount.ToString();
        inputField.caretPosition = inputField.text.Length;
    }

    private void OnClickUpOrDownButton(int count)
    {
        CurrentCount = Mathf.Clamp(CurrentCount + count, _minCount, _maxCount);
        Get<TMP_InputField>((int)InputFields.InputField).text = CurrentCount.ToString();
    }
}
