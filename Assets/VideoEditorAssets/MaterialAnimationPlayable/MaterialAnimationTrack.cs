using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(1f, 0f, 0.7330103f)]
[TrackClipType(typeof(MaterialAnimationClip))]
public class MaterialAnimationTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<MaterialAnimationMixerBehaviour>.Create (graph, inputCount);
    }
}
