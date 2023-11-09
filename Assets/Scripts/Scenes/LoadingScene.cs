using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : BaseScene
{
    [SerializeField]
    private Image _background;
    [SerializeField]
    private Image _loadingBar;

    protected override void Init()
    {
        base.Init();

        //Managers.UI.Get<UI_TopCanvas>().ActiveFalseInitBG();
    }

    private void Start()
    {
        _background.sprite = LoadSetting.GetInstance.Background[Managers.Scene.NextScene];
        _background.color = Color.white;
        if (_background.sprite == null)
        {
            _background.color = Color.black;
        }

        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(Managers.Scene.NextScene.ToString());
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            timer += Time.unscaledDeltaTime;
            if (op.progress < 0.9f)
            {
                _loadingBar.fillAmount = Mathf.Lerp(op.progress, 1f, timer);
                if (_loadingBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                _loadingBar.fillAmount = Mathf.Lerp(_loadingBar.fillAmount, 1f, timer);
                if (_loadingBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
