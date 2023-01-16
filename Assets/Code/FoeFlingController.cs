using System.Collections;

using UnityEngine;

public class FoeFlingController : AbstractFlingController
{
    public override void Play() 
    {
        StartCoroutine(Think());
    }

    public IEnumerator Think()
    {
        yield return new WaitForSeconds(1.0f);
        var body = GetComponent<FlingBody>();
        if (body != null)
        {
            body.SetSpeed(Random.insideUnitCircle.normalized * 10);
        }

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FlingPhysics.GetInstance().WaitForFlingsToEnd());
        yield return new WaitForSeconds(1.0f);
        Done();
    }
}

