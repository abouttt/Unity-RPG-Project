using UnityEngine;

public class UI_MainMenuFixed : UI_Base
{
    enum Buttons
    {
        ContinueButton,
        NewGameButton,
        OptionButton,
        BackButton,
        ExitButton,
    }

    private bool _hasSaveFile = false;

    protected override void Init()
    {
        BindButton((typeof(Buttons)));

        GetButton((int)Buttons.ContinueButton).onClick.AddListener(() =>
        {
            SceneType scene = Managers.Data.LoadScene();
            Managers.Game.IsDefaultSpawnPosition = false;
            Managers.Scene.LoadScene(scene);
        });

        GetButton((int)Buttons.NewGameButton).onClick.AddListener(() =>
        {
            Managers.Data.ClearSaveDatas();
            Managers.Game.IsDefaultSpawnPosition = true;
            Managers.Scene.LoadScene(SceneType.VillageScene);
        });

        GetButton((int)Buttons.OptionButton).onClick.AddListener(() =>
        {
            ToggleOptionMenu(true);
        });

        GetButton((int)Buttons.BackButton).onClick.AddListener(() =>
        {
            ToggleOptionMenu(false);
        });

        GetButton((int)Buttons.ExitButton).onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        _hasSaveFile = Managers.Data.HasSaveDatas();
    }

    private void Start()
    {
        Managers.UI.Register<UI_MainMenuFixed>(this);
        Managers.UI.Get<UI_OptionPopup>().gameObject.FindChild("CloseButton", recursive: true).SetActive(false);
        ToggleOptionMenu(false);
    }

    private void ToggleOptionMenu(bool show)
    {
        if (show)
        {
            Managers.UI.Show<UI_OptionPopup>();
        }
        else
        {
            Managers.UI.Close<UI_OptionPopup>();
        }

        GetButton((int)Buttons.ContinueButton).gameObject.SetActive(!show && _hasSaveFile);
        GetButton((int)Buttons.NewGameButton).gameObject.SetActive(!show);
        GetButton((int)Buttons.OptionButton).gameObject.SetActive(!show);
        GetButton((int)Buttons.BackButton).gameObject.SetActive(show);
        GetButton((int)Buttons.ExitButton).gameObject.SetActive(!show);
    }
}
