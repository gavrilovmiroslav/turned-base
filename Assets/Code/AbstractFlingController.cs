using DG.Tweening;

using UnityEngine;

public class AbstractFlingController : MonoBehaviour
{
    public virtual void Play() { }

    public Character Character { get { return GetComponent<Character>(); } }

    public void Done()
    {
        TurnStateManager.GetInstance().EndPlay();
    }
}
