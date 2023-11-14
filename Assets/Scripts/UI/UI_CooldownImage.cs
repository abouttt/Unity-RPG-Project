using UnityEngine;
using UnityEngine.UI;

public class UI_CooldownImage : UI_Base
{
    private Cooldown _cooldownRef;
    private Image _cooldownImage;

    protected override void Init()
    {
        _cooldownImage = GetComponent<Image>();
    }

    private void LateUpdate()
    {
        if (_cooldownRef.Current > 0f)
        {
            _cooldownImage.fillAmount = _cooldownRef.Current / _cooldownRef.Max;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetCooldown(Cooldown cooldown)
    {
        _cooldownRef = cooldown;
        if (_cooldownRef.Current > 0)
        {
            gameObject.SetActive(true);
        }
    }

    public void Clear()
    {
        _cooldownRef = null;
        gameObject.SetActive(false);
    }
}
