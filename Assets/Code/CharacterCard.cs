using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public struct AbilityIcon
{
    public Ability Ability;
    public Sprite Icon;
}

public class CharacterCard : MonoSingleton<CharacterCard>
{
    [Header("Bindings")]
    public SpriteRenderer Content;
    public SpriteRenderer Ability_Icon1;
    public SpriteRenderer Ability_Icon2;
    public SpriteRenderer Ability_Icon3;

    public TMPro.TextMeshPro TitleText; 
    public TMPro.TextMeshPro HealthText;
    public TMPro.TextMeshPro MovesText;

    [Header("Settings")]
    [SerializeField]
    public List<AbilityIcon> Icons = new();
    public Color EnabledColor;
    public Color DisabledColor;
    public Transform OffscreenPivot;
    public Transform OnscreenPivot;
    public GameObject AbilityIconPrefab;

    public Sprite GetIcon(Ability ability)
    {
         return Icons.Where(c => c.Ability == ability).ToList()[0].Icon;
    }

    public void UpdateView(Character character)
    {
        Content.sprite = character.Visual.GetComponent<SpriteRenderer>().sprite;
        Content.flipX = character.Side == CombatSide.Foe;
        TitleText.text = $"{character.Kind}";
        HealthText.text = $"{character.Health}";
        MovesText.text = $"{character.Moves}";
        
        Ability_Icon1.sprite = GetIcon(character.Template.Ability1);
        Ability_Icon1.color = (character.Template.Ability1 == Ability.None) ? DisabledColor : EnabledColor;
        Ability_Icon2.sprite = GetIcon(character.Template.Ability2);
        Ability_Icon2.color = (character.Template.Ability2 == Ability.None) ? DisabledColor : EnabledColor;
        Ability_Icon3.sprite = GetIcon(character.Template.Ability3);
        Ability_Icon3.color = (character.Template.Ability3 == Ability.None) ? DisabledColor : EnabledColor;
    }

    public IEnumerator WaitForViewUpdate(Character character)
    {
        yield return this.transform.DOLocalMove(OffscreenPivot.localPosition, 0.5f).WaitForCompletion();
        UpdateView(character);
        yield return this.transform.DOLocalMove(OnscreenPivot.localPosition, 0.5f).WaitForCompletion();
    }
}