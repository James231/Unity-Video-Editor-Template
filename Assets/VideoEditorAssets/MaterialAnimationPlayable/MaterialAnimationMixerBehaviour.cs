using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MaterialAnimationMixerBehaviour : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        int inputCount = playable.GetInputCount ();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<MaterialAnimationBehaviour> inputPlayable = (ScriptPlayable<MaterialAnimationBehaviour>)playable.GetInput(i);
            MaterialAnimationBehaviour input = inputPlayable.GetBehaviour ();
            
            // Use the above variables to process each frame of this playable.
            
        }
    }
}
