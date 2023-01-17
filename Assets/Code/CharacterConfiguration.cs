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

    [Header("Collider")]
    public Vector2 ColliderOffset;
    public float ColliderSize;

    [Header("HUD")]
    public Vector2 HUDOffset;
}
