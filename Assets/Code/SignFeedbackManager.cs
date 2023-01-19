using UnityEngine;
using System.Collections.Generic;

public class SignFeedbackManager : MonoSingleton<SignFeedbackManager>
{
    [Header("Fling Indicator")]
    public GameObject FlingIndicatorDot;
    public int FlingIndicatorDotsCount = 10;
    public List<GameObject> InstantiatedFlingIndicatorDots = new();

    public void Start()
    {
        for (int i = 0; i < FlingIndicatorDotsCount; i++)
        {
            var dot = Instantiate(FlingIndicatorDot, Vector3.zero, Quaternion.identity);
            dot.SetActive(false);
            InstantiatedFlingIndicatorDots.Add(dot);
        }
    }
}
