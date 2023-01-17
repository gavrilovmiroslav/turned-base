using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoSingleton<CharacterManager>
{
    public CharacterConfiguration[] Characters;
    public GameObject CharacterPrefab;

    private int _HitStreak = 0;
    public List<Character> Cursed = new();

    public void ResetHitStreak()
    {
        _HitStreak = 0;
    }

    public int IncreaseHitStreak()
    {
        _HitStreak += 1;
        return _HitStreak;
    } 
}
