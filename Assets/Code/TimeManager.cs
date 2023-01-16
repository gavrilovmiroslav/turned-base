using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager>
{
    public Queue<IEnumerator> _Next = new();
    public float TimeBetweenTicks = 0.1f;

    void Start()
    {
        StartCoroutine(Tick());
    }

    public void Enqueue(IEnumerator next)
    {
        _Next.Enqueue(next);
    }

    IEnumerator Tick()
    {
        while (true)
        {
            if (_Next.Count > 0)
            {
                var next = _Next.Dequeue();
                yield return StartCoroutine(next);
            }

            yield return new WaitForSeconds(TimeBetweenTicks);
        }
    }
}
