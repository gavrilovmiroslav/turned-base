using DG.Tweening;

using UnityEngine;

public class AbstractFlingController : MonoBehaviour
{
    public virtual void Play() { }

    public void Done()
    {
        TurnStateManager.GetInstance().EndPlay();
    }
}
