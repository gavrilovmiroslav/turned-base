using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    [SerializeField]
    public List<AbilityIcon> Icons = new();

    public void UpdateView(Character character)
    {
        Content.sprite = character.Visual.GetComponent<SpriteRenderer>().sprite;

        TitleText.text = $"{character.Kind}";
        HealthText.text = $"{character.Health}";
        MovesText.text = $"{character.Moves}";
        
        Ability_Icon1.sprite = Icons.Where(c => c.Ability == character.Template.Ability1).ToList()[0].Icon;
        Ability_Icon2.sprite = Icons.Where(c => c.Ability == character.Template.Ability2).ToList()[0].Icon;
        Ability_Icon3.sprite = Icons.Where(c => c.Ability == character.Template.Ability3).ToList()[0].Icon;
    }


}