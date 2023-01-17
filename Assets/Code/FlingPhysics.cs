using System.Collections;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public struct FlingCollision
{
    public GameObject Attacker;
    public GameObject Target;
    public Vector3 Position;
}

public class FlingPhysics : MonoSingleton<FlingPhysics>
{
    public FlingPhysicsConfiguration Configuration;
    
    public delegate void HitDetectedEvent(GameObject a, GameObject b);
    public event HitDetectedEvent OnHitDetected;

    public readonly List<FlingBody> Bodies = new();
    public HashSet<FlingBody> Moving = new();
    private readonly HashSet<long> RecentCollisions = new();

    public long HashCollision(FlingBody fa, FlingBody fb)
    {
        return fa.GetHashCode() ^ fb.GetHashCode();
    }

    public void DisableCollision(FlingBody fa, FlingBody fb)
    {
        var hash = HashCollision(fa, fb);
        RecentCollisions.Add(hash);
        StartCoroutine(EnableCollisionAfterTime(hash, Configuration.MinimalSpeed));
    }

    public IEnumerator EnableCollisionAfterTime(long hash, float time)
    {
        yield return new WaitForSeconds(time);
        RecentCollisions.Remove(hash);
    }

    public void HitDetected(RaycastHit2D hit, GameObject a, GameObject b)
    {
        var fa = a.GetComponent<FlingBody>();
        var fb = b.GetComponent<FlingBody>();
        var hash = HashCollision(fa, fb);

        if (!fa.IsStatic)
            Moving.Add(fa);

        if (!fb.IsStatic)
            Moving.Add(fb);

        if (!RecentCollisions.Contains(hash))
        {
            OnHitDetected?.Invoke(a, b);
            DisableCollision(fa, fb);
            StartCoroutine(HitReaction(hit, fa, fb));
        }
    }

    public IEnumerator HitReaction(RaycastHit2D hit, FlingBody fa, FlingBody fb)
    {
        var faster = fa.Speed.sqrMagnitude >= fb.Speed.sqrMagnitude ? fa : fb;
        var slower = fa == faster ? fb : fa;

        var chf = faster.GetComponent<Character>();
        var chs = slower.GetComponent<Character>();
        
        chf?.DoStretchAnimation();       
        chs?.DoSquashAnimation();

        if (faster.IsStatic)
            yield break;

        if (slower.IsStatic)
        {
            faster.Speed = Vector2.Reflect(faster.Speed, hit.normal);
        }
        else
        {
            slower.Speed = faster.Speed;
            var fastSpeed = faster.Speed;
            faster.Speed = Vector2.zero;
            yield return new WaitForSeconds(0.1f);
            faster.Speed = Vector2.Reflect(fastSpeed, hit.normal);
        }
    }

    public void Update()
    {
        var moving = new HashSet<FlingBody>(Moving);
        foreach (var mover in moving)
        {
            if (mover.Speed.magnitude < FlingPhysics.GetInstance().Configuration.MinimalSpeed)
            {
                Moving.Remove(mover);
            }
        }
    }

    public void AddFlingBody(FlingBody body)
    {
        Bodies.Add(body);
    }

    public void RemoveFlingBody(FlingBody body)
    {
        Bodies.Remove(body);
    }

    public IEnumerator WaitForFlingsToEnd()
    {
        while (Moving.Count > 0)
        {
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1.0f);
    }

    public void OnGUI()
    {
        GUILayout.Label($"Everything still: {Moving.Count}");
        if (GUILayout.Button("Random Push"))
        {
            var nonStatic = Bodies.Where(x => !x.IsStatic).ToList();
            var body = nonStatic[Random.Range(0, nonStatic.Count())];
            if (body != null)
            {
                body.SetSpeed(Random.insideUnitCircle.normalized * 10);
            }
        }
    }
}
