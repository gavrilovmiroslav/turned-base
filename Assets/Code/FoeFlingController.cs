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
        yield return new WaitForSeconds(Random.Range(1.0f, 1.5f));

        var body = GetComponent<FlingBody>();
        if (body != null)
        {
            body.SetSpeed(Random.insideUnitCircle.normalized * (10.0f / (float)GetComponent<Character>().Configuration.Weight));
        }

        yield return CharacterCard.GetInstance().WaitForOffscreen();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FlingPhysics.GetInstance().WaitForFlingsToEnd());
        GetComponentInChildren<SpriteOutline>().OutlineColor = Color.gray;
        yield return new WaitForSeconds(1.0f);
        Done();
    }
}

