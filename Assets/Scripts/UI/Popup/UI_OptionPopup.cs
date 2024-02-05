using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_OptionPopup : UI_Popup
{
    enum Buttons
    {
        CloseButton,
    }

    enum Sliders
    {
        BGMSlider,
        SESlider
    }

    enum Dropdowns
    {
        MSAADropdown,
        FrameDropdown,
    }

    enum Toggles
    {
        VSyncToggle,
    }

    protected override void Init()
    {
        base.Init();

        if (UIType == UIType.Popup)
        {
            BindButton(typeof(Buttons));
        }

        Bind<Slider>(typeof(Sliders));
        Bind<TMP_Dropdown>(typeof(Dropdowns));
        Bind<Toggle>(typeof(Toggles));

        Get<Slider>((int)Sliders.BGMSlider).value = Managers.Sound.GetVolume(SoundType.Bgm);
        Get<Slider>((int)Sliders.SESlider).value = Managers.Sound.GetVolume(SoundType.Effect);
        Get<Slider>((int)Sliders.BGMSlider).onValueChanged.AddListener(volume => Managers.Sound.SetVolume(SoundType.Bgm, volume));
        Get<Slider>((int)Sliders.SESlider).onValueChanged.AddListener(volume => Managers.Sound.SetVolume(SoundType.Effect, volume));
        if (UIType == UIType.Popup)
        {
            GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_OptionPopup>);
        }

        MSAADropdownInit();
        FrameDropdownInit();
        VSyncToggleInit();

        Managers.UI.Register<UI_OptionPopup>(this);
    }

    private void MSAADropdownInit()
    {
        for (int i = 0; i < Get<TMP_Dropdown>((int)Dropdowns.MSAADropdown).options.Count; i++)
        {
            int value = int.Parse(Get<TMP_Dropdown>((int)Dropdowns.MSAADropdown).options[i].text);
            if (QualitySettings.antiAliasing == value)
            {
                Get<TMP_Dropdown>((int)Dropdowns.MSAADropdown).value = i;
                break;
            }
        }

        Get<TMP_Dropdown>((int)Dropdowns.MSAADropdown).onValueChanged.AddListener((int index) =>
        {
            QualitySettings.antiAliasing = int.Parse(Get<TMP_Dropdown>((int)Dropdowns.MSAADropdown).options[index].text);
        });
    }

    private void FrameDropdownInit()
    {
        bool isFind = false;

        for (int i = 0; i < Get<TMP_Dropdown>((int)Dropdowns.FrameDropdown).options.Count; i++)
        {
            if (Get<TMP_Dropdown>((int)Dropdowns.FrameDropdown).options[i].text.Equals("제한없음"))
            {
                isFind = true;
            }
            else
            {
                var value = int.Parse(Get<TMP_Dropdown>((int)Dropdowns.FrameDropdown).options[i].text);
                if (Application.targetFrameRate == value)
                {
                    isFind = true;
                }
            }

            if (isFind)
            {
                Get<TMP_Dropdown>((int)Dropdowns.FrameDropdown).value = i;
                break;
            }
        }

        Get<TMP_Dropdown>((int)Dropdowns.FrameDropdown).onValueChanged.AddListener((int value) =>
        {
            if (Get<TMP_Dropdown>((int)Dropdowns.FrameDropdown).options[value].text.Equals("제한없음"))
            {
                Application.targetFrameRate = -1;
            }
            else
            {
                Application.targetFrameRate = int.Parse(Get<TMP_Dropdown>((int)Dropdowns.FrameDropdown).options[value].text);
            }
        });
    }

    private void VSyncToggleInit()
    {
        Get<Toggle>((int)Toggles.VSyncToggle).isOn = QualitySettings.vSyncCount != 0;
        Get<Toggle>((int)Toggles.VSyncToggle).onValueChanged.AddListener((bool isOn) =>
        {
            QualitySettings.vSyncCount = isOn ? 1 : 0;
        });
    }
}
