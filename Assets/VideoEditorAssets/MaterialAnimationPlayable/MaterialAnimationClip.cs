using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MaterialAnimationClip : PlayableAsset, ITimelineClipAsset
{
    public Material material;
    public string propertyName;
    public AnimationCurve animationCurve;
    public float rangeLower = 0;
    public float rangeUpper = 1;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MaterialAnimationBehaviour>.Create (graph);

        MaterialAnimationBehaviour behaviour = playable.GetBehaviour ();

        behaviour.material = material;
        behaviour.propertyName = propertyName;
        behaviour.animationCurve = animationCurve;
        behaviour.rangeLower = rangeLower;
        behaviour.rangeUpper = rangeUpper;

        return playable;
    }
}
