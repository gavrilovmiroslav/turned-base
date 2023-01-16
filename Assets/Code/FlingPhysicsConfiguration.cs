
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class FlingPhysicsConfiguration : ScriptableObject
{
    [Header("Fling Parameters")]
    public float FrictionFactor = 0.89f;
    public float MinimalSpeed = 0.1f;
    public float MinimalDistance = 0.02f;

    [Header("Contact Filters")]
    public ContactFilter2D WallFilter;
    public ContactFilter2D CreatureFilter;
}