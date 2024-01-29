using UnityEngine;

public class UI_MonsterHPBar : UI_Base
{
    enum Images
    {
        HPBar,
    }

    enum Texts
    {
        DamageText,
    }

    [SerializeField]
    private float _showHPBarTime;
    [SerializeField]
    private float _showDamageTime;

    private MonsterController _target;
    private UI_FollowTarget _followTarget;
    private int _targetPrevHP;
    private int _totalDamage;
    private float _currentShowHPBarTime;
    private float _currentShowDamageTime;
    private bool _isChanged;    // 타겟 고정상태에서 데미지를 받았는지 -> 타겟 고정을 풀어도 계속 보여지기 위함.

    protected override void Init()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        _followTarget = GetComponent<UI_FollowTarget>();

        GetText((int)Texts.DamageText).enabled = false;
    }

    private void Update()
    {
        if (_target == null || !_target.gameObject.activeSelf)
        {
            Clear();
            Managers.Resource.Destroy(gameObject);
            return;
        }

        if (!_target.IsLockOnTarget)
        {
            if (!_isChanged)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                _currentShowHPBarTime += Time.deltaTime;
                if (_currentShowHPBarTime >= _showHPBarTime)
                {
                    gameObject.SetActive(false);
                    return;
                }
            }
        }

        if (GetText((int)Texts.DamageText).enabled)
        {
            _currentShowDamageTime += Time.deltaTime;
            if (_currentShowDamageTime >= _showDamageTime)
            {
                GetText((int)Texts.DamageText).enabled = false;
                _totalDamage = 0;
            }
        }
    }

    public void SetTarget(MonsterController target)
    {
        _target = target;

        if (_target == null)
        {
            Clear();
            Managers.Resource.Destroy(gameObject);
        }
        else
        {
            _followTarget.SetTarget(target.transform);
            _targetPrevHP = _target.CurrentHP;
            _target.HPChanged += RefreshHP;
            RefreshHP();
        }
    }

    private void RefreshHP()
    {
        if (_target == null)
        {
            return;
        }

        gameObject.SetActive(true);
        GetImage((int)Images.HPBar).fillAmount = (float)_target.CurrentHP / _target.Stat.MaxHP;
        _totalDamage += _targetPrevHP - _target.CurrentHP;
        _targetPrevHP = _target.CurrentHP;
        _currentShowHPBarTime = 0f;

        if (_target.CurrentDamage != 0)
        {
            _currentShowDamageTime = 0f;
            _isChanged = true;
            GetText((int)Texts.DamageText).text = _totalDamage.ToString();
            GetText((int)Texts.DamageText).enabled = true;
        }
    }

    private void Clear()
    {
        if (_target != null)
        {
            _target.HPChanged -= RefreshHP;
        }

        _target = null;
        _totalDamage = 0;
        _isChanged = false;
        GetText((int)Texts.DamageText).enabled = false;
    }
}
