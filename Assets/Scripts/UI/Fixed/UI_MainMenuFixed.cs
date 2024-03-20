using UnityEngine;
using Newtonsoft.Json.Linq;

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
        Managers.UI.Register<UI_MainMenuFixed>(this);

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.ContinueButton).onClick.AddListener(() =>
        {
            Managers.Data.Load<JArray>(SceneManagerEx.SaveKey, out var saveData);
            SceneSaveData sceneSaveData = saveData[0].ToObject<SceneSaveData>();
            SceneType scene = sceneSaveData.Scene;
            Managers.Game.IsDefaultSpawn = false;
            Managers.Scene.LoadScene(scene);
        });

        GetButton((int)Buttons.NewGameButton).onClick.AddListener(() =>
        {
            Managers.Data.ClearSaveData();
            Managers.Game.IsDefaultSpawn = true;
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
            Managers.Data.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        _hasSaveFile = Managers.Data.HasSaveData;
    }

    private void Start()
    {
        ToggleOptionMenu(false);
    }

    private void ToggleOptionMenu(bool toggle)
    {
        if (toggle)
        {
            Managers.UI.Show<UI_GameOptionPopup>();
        }
        else
        {
            Managers.UI.Close<UI_GameOptionPopup>();
        }

        GetButton((int)Buttons.ContinueButton).gameObject.SetActive(!toggle && _hasSaveFile);
        GetButton((int)Buttons.NewGameButton).gameObject.SetActive(!toggle);
        GetButton((int)Buttons.OptionButton).gameObject.SetActive(!toggle);
        GetButton((int)Buttons.BackButton).gameObject.SetActive(toggle);
        GetButton((int)Buttons.ExitButton).gameObject.SetActive(!toggle);
    }
}
