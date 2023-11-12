using UnityEngine;
using DG.Tweening;

public class UI_TopCanvas : UI_Base
{
    enum GameObjects
    {
        InitBG,
    }

    protected override void Init()
    {
        BindObject(typeof(GameObjects));
    }

    private void Start()
    {
        Managers.UI.Register<UI_TopCanvas>(this);
    }

    public void ActiveFalseInitBG()
    {
        GetObject((int)GameObjects.InitBG).SetActive(false);
    }

    public void FadeInitBG()
    {
        GetObject((int)GameObjects.InitBG).GetComponent<DOTweenAnimation>().DOPlay();
    }
}
