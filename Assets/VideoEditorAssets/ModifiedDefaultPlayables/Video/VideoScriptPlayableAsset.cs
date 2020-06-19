using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Diagnostics;

namespace UnityEngine.Timeline
{
	[Serializable]
    public class VideoScriptPlayableAsset : PlayableAsset
	{
        public ExposedReference<RawImage> image;

        [SerializeField, NotKeyable]
		public VideoClip videoClip;

        [SerializeField, NotKeyable]
        public bool mute = false;

        [SerializeField, NotKeyable]
        public VideoAudioOutputMode audioOutputMode = VideoAudioOutputMode.None;

        public ExposedReference<AudioSource> audioSource;

        [SerializeField, NotKeyable]
        public bool loop = true;

        [SerializeField, NotKeyable]
        public double preloadTime = 0.3;

        [SerializeField, NotKeyable]
        public double startTime = 0;

        [SerializeField, NotKeyable]
        public double endTime = -1;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
            ScriptPlayable<VideoPlayableBehaviour> playable =
                ScriptPlayable<VideoPlayableBehaviour>.Create(graph);

            VideoPlayableBehaviour playableBehaviour = playable.GetBehaviour();

            playableBehaviour.videoClip = videoClip;
            playableBehaviour.mute = mute;
            playableBehaviour.loop = loop;
            playableBehaviour.preloadTime = preloadTime;
            playableBehaviour.startTime = startTime;
            playableBehaviour.endTime = endTime;
            playableBehaviour.image = image.Resolve(graph.GetResolver());
            playableBehaviour.audioOutputMode = audioOutputMode;
            playableBehaviour.audioSource = audioSource.Resolve(graph.GetResolver());

            return playable;
		}
    }
}
