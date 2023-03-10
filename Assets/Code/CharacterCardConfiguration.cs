using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum Ability
{
    Hit = 0,      // on hit, deals 1 damage
    Critical = 1, // on hit, deals N damage, where N is the number of useful moves
    Healing = 2,  // on hit, heals 1 health to friend
    Shield = 3,   // on hit, gives +1 health to friend
    Loot = 4,     // every hit counts as +1 gold
    Skill = 8,    // next turn, every 1 becomes N, where N is the number of useful moves
    Curse = 10,   // ???
}

public static class AbilityExtensions
{
    public static IEnumerator Perform(this Ability ability, Character character)
    {
        switch(ability)
        {
            case Ability.Hit: 
            case Ability.Critical: 
            case Ability.Loot:
            case Ability.Shield:
            case Ability.Healing:
            case Ability.Skill:
            case Ability.Curse:
            default:
                yield return new WaitForSeconds(0.5f);
                break;
        }
    }

    public static bool ShouldApply(this Ability ability, Character a, Character b)
    { 
        switch (ability)
        {
            case Ability.Hit: 
                return false;

            case Ability.Critical: 
            case Ability.Loot:
                return a.Side != b.Side;

            case Ability.Shield:
            case Ability.Healing:
                return a.Side == b.Side;

            case Ability.Skill:
            case Ability.Curse:
            default:
                return true;
        }
    }
}

public enum Kind
{                   // H   M   E
    Goblin = 0,     // 1   1   2/3
    Imp = 1,        // 1-2 1   2/2/3/5
    Chort = 2,      // 2-4 1-2 3/3/4/5
    Ooze = 3,       // 4-5 1   4/4/5/6
    Demon = 4,      // 7-9 1-2 5/9/11

    Cleric = 100,
    Druid = 101,
    Knight = 102,
    Necromancer = 103,
}

public class Item
{
    public Sprite Image;
}

[System.Serializable]
public struct CharacterTemplate
{
    public Kind Kind;
    public int Gold;

    public int Health;
    public int Moves;

    public Ability Ability1;
    public Ability Ability2;
    public Ability Ability3;

    public Item CarriedItem;

    public override string ToString()
    {
        return $"{Kind} ({Health}/{Moves}/${Gold}) {Ability1} {Ability2} {Ability3}";
    }
}

public static class CharacterCardConfiguration
{
    public static void CreateTeam(int n, int maxSize, ref List<CharacterTemplate> team)
    {
        while (team.Count < maxSize && n > 0)
        {
            var member = CreateRandom(n);
            team.Add(member);
            n = member.Gold - 1;
        }
    }

    public static CharacterTemplate CreateRandom(int n)
    {
        CharacterTemplate template = new();        

        int kindPoints = Random.Range(0, n / 2);
        if (kindPoints > (int)Kind.Demon)
            kindPoints = (int)Kind.Demon;
        n -= kindPoints;
        
        var kind = (Kind)kindPoints;
        int health = 0;
        int moves = 0;
        List<int> maxAbilities = new();
        List<int> energy = null;

        switch (kind)
        {
            case Kind.Goblin:
                health = 1; moves = 1; energy = new List<int>() { 2, 3 }; maxAbilities = new List<int> { 0, 0, 1, 2 }; break;
            case Kind.Imp:
                health = Random.Range(1, 2); moves = 1; energy = new List<int>() { 2, 2, 3, 5 }; maxAbilities = new List<int> { 1, 2, 2 }; break;
            case Kind.Chort:
                health = Random.Range(2, 4); moves = Random.Range(1, 2); energy = new List<int>() { 3, 3, 4, 5 }; maxAbilities = new List<int> { 2, 2, 2, 3 }; break;
            case Kind.Ooze:
                health = Random.Range(4, 5); moves = 1; energy = new List<int>() { 4, 4, 5, 6 }; maxAbilities = new List<int> { 1, 2, 2, 2, 3 }; break;
            case Kind.Demon:
                health = Random.Range(7, 9); moves = Random.Range(1, 2); energy = new List<int>() { 5, 9, 11 }; maxAbilities = new List<int> { 2, 3, 3, 3 }; break;               
        }

        template.Kind = kind;
        template.Health = health;
        template.Moves = moves;

        var energyCount = energy[Random.Range(0, energy.Count)];
        var abilityCount = maxAbilities[Random.Range(0, maxAbilities.Count)];

        for (int i = 0; i < abilityCount; i++)
        {
            if (Random.Range(0.0f, 1.0f) > 0.1f)
            {
                foreach (var ab in new Ability[] { Ability.Curse, Ability.Skill, Ability.Loot, Ability.Shield, Ability.Healing, Ability.Critical, Ability.Hit })
                {
                    if (Random.Range(0.0f, 1.0f) > 0.5f && (int)ab <= energyCount)
                    {
                        switch (i)
                        {
                            case 0: template.Ability1 = ab; break;
                            case 1: template.Ability2 = ab; break;
                            case 2: template.Ability3 = ab; break;
                        }
                        energyCount -= (int)ab;
                        break;
                    }
                }
            }
        }

        template.Gold = n;
        return template;
    }
}