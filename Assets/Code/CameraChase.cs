using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
public class CameraChase : MonoSingleton<CameraChase>
{
    public void Update()
    {
        var center = Vector3.zero;
        int parts = 0;

        if (TurnStateManager.GetInstance().Current != null)
        {
            center = TurnStateManager.GetInstance().Current.transform.position;
            parts++;
        }

        var movingParts = FlingPhysics.GetInstance().Moving;
        if (movingParts.Count > 0)
        {            
            foreach (var moving in movingParts)
            {
                center += moving.transform.position;                
            }

            parts += movingParts.Count;
            center /= parts;
        }

        this.transform.position = center;
    }
}