using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MaterialAnimationBehaviour : PlayableBehaviour
{
    public Material material;
    public string propertyName;
    public AnimationCurve animationCurve;
    public float rangeLower = 0;
    public float rangeUpper = 1;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (material == null)
            return;

        if (string.IsNullOrEmpty(propertyName))
            return;

        if (animationCurve == null)
            return;

        float timePercentage = (float)(playable.GetTime() / playable.GetDuration());
        float value = animationCurve.Evaluate(timePercentage);
        float scaledValue = rangeLower + ((rangeUpper - rangeLower) * value);
        material.SetFloat(propertyName, scaledValue);
    }
}
