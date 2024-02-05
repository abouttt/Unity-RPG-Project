using UnityEngine;
using DG.Tweening;

public class UI_TopCanvas : UI_Base
{
    enum GameObjects
    {
        InitBG,
    }

    enum Buttons
    {
        GameMenuButton,
    }

    protected override void Init()
    {
        Managers.UI.Register<UI_TopCanvas>(this);

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.GameMenuButton).onClick.AddListener(Managers.UI.ShowOrClose<UI_GameMenuPopup>);
        GetButton((int)Buttons.GameMenuButton).gameObject.SetActive(false);
    }

    public void FadeInitBG()
    {
        GetObject((int)GameObjects.InitBG).GetComponent<DOTweenAnimation>().DOPlay();
    }

    public void ToggleGameMenuButton(bool toggle)
    {
        GetButton((int)Buttons.GameMenuButton).gameObject.SetActive(toggle);
    }
}
