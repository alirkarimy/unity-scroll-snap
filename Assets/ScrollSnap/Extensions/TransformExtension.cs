using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension 
{

    public static void SetLossyScale(this Transform target, Vector3 lossyScale)
    {
        if (target.parent == null)
        {
            target.localScale = lossyScale;
        }
        else
        {
            Vector3 parentLossyScale = target.parent.lossyScale;
            target.localScale = new Vector3(
                lossyScale.x / parentLossyScale.x,
                lossyScale.y / parentLossyScale.y,
                lossyScale.z / parentLossyScale.z
            );
        }
    }
}
