using DG.Tweening;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraPan : MonoSingleton<CameraPan>
{    
    private bool _PanTouchInitiated = false;
    private Vector3 _TouchStartPosition;
    public float TransitionDuration = 0.2f;
    public Transform CameraTarget;

    public static bool CanPan = true;

    public IEnumerator PanTo(Vector3 target)
    {
        yield return CameraTarget.DOMove(target, TransitionDuration).WaitForCompletion();
    }

    private void Update()
    {
        if (CanPan)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _PanTouchInitiated = true;
                _TouchStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0) && _PanTouchInitiated)
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var dir = _TouchStartPosition - pos;
                CameraTarget.position += dir;
            }
            else
            {
                if (_PanTouchInitiated)
                {
                    _PanTouchInitiated = false;
                }
            }
        }
        else
        {
            _PanTouchInitiated = false;
        }
    }
}
