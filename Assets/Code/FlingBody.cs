using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class FlingBody : MonoBehaviour
{
    public bool IsStatic = false;
    public Vector2 Speed = Vector2.zero;

    
    private Collider2D _Collider;
    private List<RaycastHit2D> _Hits = new();
    private ContactFilter2D _Filter;

    public void Start()
    {
        _Collider = GetComponent<Collider2D>();
        var config = FlingPhysics.GetInstance().Configuration;
        _Filter = IsStatic ? config.WallFilter : config.CreatureFilter;
        FlingPhysics.GetInstance().AddFlingBody(this);
    }

    public void OnDestroy()
    {
        FlingPhysics.GetInstance().RemoveFlingBody(this);
    }

    public void AddForce(Vector2 force)
    {
        Speed += force;
    }

    public void SetSpeed(Vector2 force)
    {
        Speed = force;
    }

    public void Stop()
    {
        Speed = Vector2.zero;
    }

    public void Update()
    {
        var physics = FlingPhysics.GetInstance().Configuration;

        if (Speed.sqrMagnitude > physics.MinimalDistance)
        {
            var ds = new Vector3(Speed.x, Speed.y, 0.0f) * Time.deltaTime;
            if (_Collider != null && !IsStatic)
            {
                _Collider.Cast(ds, _Filter, _Hits, ds.magnitude, false);
                foreach (var hit in _Hits)
                {
                    FlingPhysics.GetInstance().HitDetected(hit, this.gameObject, hit.collider.gameObject);
                }
            }

            this.transform.position += new Vector3(Speed.x, Speed.y, 0.0f) * Time.deltaTime;
            Speed *= physics.FrictionFactor;

            if (Speed.sqrMagnitude <= physics.MinimalDistance)
            {
                Speed = Vector2.zero;
            }
        }
    }
}
