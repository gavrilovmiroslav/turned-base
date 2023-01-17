using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ElementList : MonoBehaviour
{
    public float ElementWidth = 0.1f;

    void Update()
    {
        var childCount = transform.childCount;
        var width = ElementWidth * childCount;

        if (childCount == 1)
        {
            transform.GetChild(0).localPosition = new Vector3(0.0f, 0.0f, 0.0f);            
        }
        else 
        { 
            var position = -width / 2.0f;

            for (int i = 0; i < childCount; i++)
            {
                transform.GetChild(i).localPosition = new Vector3(position, 0.0f, 0.0f);
                position += ElementWidth;
            }
        }
    }
}
