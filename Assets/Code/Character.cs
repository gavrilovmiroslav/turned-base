using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

[ExecuteInEditMode]
public class Character : MonoBehaviour
{
    public static Vector3 SHADOW_SIZE = new Vector3(0.1f, 0.05f, 0.1f);

    [SerializeField]
    public CharacterTemplate Template;
    public CharacterConfiguration Configuration;

    public static List<Character> Characters = new();

    public Transform Visual;

    private Animator _Animator;
    private SpriteRenderer _SpriteRenderer;
    private CircleCollider2D _Collider;
    private Transform _Shadow;
    private Vector3 _StartingVisualScale;

    private int _Health;
    private int _Moves;
    private int _MaxHealth;
    private Kind _Kind;
    private CombatSide _Side;

    public Kind Kind { get { return _Kind; } }
    public CombatSide Side { get { return _Side; } }

    public List<Ability> AppliedAbilities = new();

    public int Health
    {
        get { return _Health; }
        set
        {
            var oldHealth = _Health;
            _Health = value;
            if (_Health < 0) _Health = 0;
            if (_Health > _MaxHealth) _Health = _MaxHealth;
        }
    }

    public int Moves
    {
        get { return _Moves; }
        set
        {
            var oldPower = _Moves;
            _Moves = value;
            if (_Moves < 0) _Moves = 0;
        }
    }

    public void SquashVisual(float stretch)
    {
        var scale = _StartingVisualScale;
        scale.x = 1.0f + stretch;
        scale.y = 1.0f - stretch;
        Visual.localScale = scale;
    }

    public void StretchVisual(float stretch)
    {
        var scale = _StartingVisualScale;
        scale.x = 1.0f - stretch;
        scale.y = 1.0f + stretch;
        Visual.localScale = scale;
    }

    private void GetAllComponents()
    {
        Visual = transform.GetChild(1);
        _StartingVisualScale = Visual.localScale;
        _Collider = GetComponent<CircleCollider2D>();
        _Animator = Visual.gameObject.GetComponent<Animator>();
        _SpriteRenderer = Visual.gameObject.GetComponent<SpriteRenderer>();
        _Shadow = transform.GetChild(0);
    }

    public void Spawn(CharacterTemplate chr, CombatSide side)
    {
        Template = chr;
        _Kind = chr.Kind;
        _Side = side;
        Configuration = CharacterManager.GetInstance().Characters.Where(c => c.Kind == _Kind).ToList()[0];
        Debug.Assert(Configuration != null);
        _MaxHealth = chr.Health;
        Health = chr.Health;
        Moves = chr.Moves;

        GetAllComponents();
        SetupCharacter();
    }

    public void Start()
    {
        Characters.Add(this);
    }

    public static List<Character> GetAllCharacters()
    {
        return Characters;
    }

    public static List<Character> GetAliveCharacters()
    {
        return Characters.Where(c => c.Health > 0).ToList();
    }

    public void SetupCharacter()
    {
        if (Configuration != null)
        {
            _Collider.offset = Configuration.ColliderOffset;
            _Collider.radius = Configuration.ColliderSize;

            _Shadow.localPosition = new Vector3(Configuration.ShadowOffset, 0, 0);
            _Shadow.localScale = SHADOW_SIZE * Configuration.ShadowSize;

            _SpriteRenderer.flipX = _Side == CombatSide.Foe;

            _Animator.runtimeAnimatorController = Configuration.Animation;
        }
    }

    public void AddAppliedAbility(Ability ability)
    {
        AppliedAbilities.Add(ability);
        GetComponentInChildren<CharacterHUD>().UpdateView();
    }

    public void RemoveTopAbility()
    {
        AppliedAbilities.RemoveAt(0);
        GetComponentInChildren<CharacterHUD>().UpdateView();
    }

    public void OnHit(GameObject a, GameObject b)
    {
        var ca = a.GetComponent<Character>();
        var cb = b.GetComponent<Character>();

        if (ca == this && cb != null)
        {
            int hit = CharacterManager.GetInstance().IncreaseHitStreak();
            if (hit <= 3)
            {
                Ability ability = Ability.None;

                switch(hit)
                {
                    case 1: ability = this.Template.Ability1; break;
                    case 2: ability = this.Template.Ability2; break;
                    case 3: ability = this.Template.Ability3; break;
                }

                if (ability != Ability.None) 
                {
                    if (ability.ShouldApply(ca, cb))
                    {
                        cb.AddAppliedAbility(ability);

                        if (ability != Ability.Magic)
                        { 
                            foreach (var cursed in CharacterManager.GetInstance().Cursed) 
                            {
                                cursed.AddAppliedAbility(ability);
                            }

                            CharacterManager.GetInstance().Cursed.Clear();
                        }
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    public void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            if (_SpriteRenderer == null || _Animator == null || _Collider == null || _Shadow == null)
            {
                GetAllComponents();
            }

            SetupCharacter();

            _Animator.Update(Time.deltaTime);
        }
    }
#endif

    public void DoSquashAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(Squash(this));
    }

    public void DoStretchAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(Stretch(this));
    }

    public IEnumerator Stretch(Character ch)
    {
        ch.StretchVisual(0.4f);
        yield return new WaitForSeconds(1.0f / 30.0f);
        ch.StretchVisual(0.25f);
        yield return new WaitForSeconds(1.0f / 30.0f);
        ch.StretchVisual(0.0f);
        yield return new WaitForSeconds(1.0f / 30.0f);
    }

    public IEnumerator Squash(Character ch)
    {
        ch.SquashVisual(0.4f);
        yield return new WaitForSeconds(1.0f / 30.0f);
        ch.SquashVisual(0.25f);
        yield return new WaitForSeconds(1.0f / 30.0f);
        ch.SquashVisual(0.0f);
        yield return new WaitForSeconds(1.0f / 30.0f);
    }
}
