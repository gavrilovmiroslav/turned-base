using UnityEngine;

public enum CombatSide
{
    Friend,
    Foe
}

[CreateAssetMenu]
public class CharacterConfiguration : ScriptableObject
{
    public Kind Kind;

    [Header("Visuals")]
    public RuntimeAnimatorController Animation;
    public float ShadowSize;
    public float ShadowOffset;
    [Range(0f, 1f)]
    public float Jiggle = 0.5f;
    [Range(1, 3)]
    public int Weight = 1;

    [Header("Collider")]
    public Vector2 ColliderOffset;
    public float ColliderSize;

    [Header("HUD")]
    public Vector2 HUDOffset;
}
