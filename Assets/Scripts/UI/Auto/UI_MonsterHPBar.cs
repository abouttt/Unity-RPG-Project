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
    private float _hpBarHeight;

    [SerializeField]
    private float _showHPBarTime;

    [SerializeField]
    private float _showDamageTime;

    private Monster _target;
    private UI_FollowWorldObject _followTarget;
    private int _targetPrevHP;
    private int _totalDamage;
    private float _currentShowHPBarTime;
    private float _currentShowDamageTime;
    private bool _isChanged;    // Ÿ�� �������¿��� �������� �޾Ҵ��� -> Ÿ�� ������ Ǯ� ��� �������� ����.

    protected override void Init()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        _followTarget = GetComponent<UI_FollowWorldObject>();

        GetText((int)Texts.DamageText).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_target == null || !_target.gameObject.activeSelf)
        {
            Clear();
            return;
        }

        if (_target.IsLockOnTarget)
        {
            _currentShowHPBarTime = 0f;
        }
        else
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
                    if (Player.Camera.LockOnTarget != _target.transform)
                    {
                        _isChanged = false;
                        gameObject.SetActive(false);
                        return;
                    }
                }
            }
        }

        if (GetText((int)Texts.DamageText).gameObject.activeSelf)
        {
            _currentShowDamageTime += Time.deltaTime;
            if (_currentShowDamageTime >= _showDamageTime)
            {
                GetText((int)Texts.DamageText).gameObject.SetActive(false);
                _totalDamage = 0;
            }
        }
    }

    public void SetTarget(Monster target)
    {
        if (target == null)
        {
            Clear();
            return;
        }

        _target = target;
        _followTarget.SetTarget(target.transform, new Vector3(0.0f, target.Collider.bounds.extents.y + _hpBarHeight, 0f));
        _targetPrevHP = _target.CurrentHP;
        _target.HPChanged += RefreshHP;
        RefreshHP();
    }

    private void RefreshHP()
    {
        if (_target == null)
        {
            return;
        }

        gameObject.SetActive(true);
        GetImage((int)Images.HPBar).fillAmount = (float)_target.CurrentHP / _target.Data.MaxHP;
        _totalDamage += _targetPrevHP - _target.CurrentHP;
        _targetPrevHP = _target.CurrentHP;
        _currentShowHPBarTime = 0f;

        if (_target.CurrentDamage != 0)
        {
            _currentShowDamageTime = 0f;
            _isChanged = true;
            GetText((int)Texts.DamageText).text = _totalDamage.ToString();
            GetText((int)Texts.DamageText).gameObject.SetActive(true);
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
        GetText((int)Texts.DamageText).gameObject.SetActive(false);
        Managers.Resource.Destroy(gameObject);
    }
}
