using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using UnityEditor;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace UnityEngine.Timeline
{
	public class VideoPlayableBehaviour : PlayableBehaviour
    {
        public RawImage image;

		public VideoClip videoClip;
        public bool mute = false;
        public VideoAudioOutputMode audioOutputMode;
        public AudioSource audioSource;
        public bool loop = true;
        public double preloadTime = 0.3;

        public double startTime = 0;
        public double endTime = -1;

        private bool playedOnce = false;
        private bool preparing = false;
        private double timeAtLastStep;
        private long oldFrame = 0;
        private Texture2D thumb;
        private GameObject temp;
        private long startFrame = 0;
        private long endFrame = -1;

        private RenderTexture renderTexture;
        private VideoPlayer videoPlayer;
        private bool wasPlayingLastFrame;

        void OnNewFrame(VideoPlayer source, long frameIdx)
        {
            if (frameIdx >= startFrame)
            {
                if (source == null)
                {
                    return;
                }

                Texture2D videoFrame = new Texture2D((int)videoClip.width, (int)videoClip.height);
                RenderTexture rt = source.texture as RenderTexture;

                if (rt == null)
                {
                    return;
                }

                if (videoFrame.width != rt.width || videoFrame.height != rt.height)
                {
                    videoFrame.Resize(rt.width, rt.height);
                }
                RenderTexture.active = rt;
                videoFrame.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                videoFrame.Apply();
                thumb = videoFrame;
                Graphics.Blit(thumb, renderTexture);
                source.Pause();
                source.Stop();
                source.enabled = false;
                
                if (temp != null)
                {
                    if (Application.isPlaying)
                    {
                        Object.Destroy(source);
                        Object.Destroy(temp);
                    }
                    else
                    {
                        Object.DestroyImmediate(source);
                        Object.DestroyImmediate(temp);
                    }
                }
            } else
            {
                source.frame = startFrame;
            }
        }

        void SetThumbnail() {
            startFrame = (long)Mathf.Floor((float)(videoPlayer.frameRate * startTime));
            if (endTime != -1)
            {
                endFrame = (long)Mathf.Floor((float)(videoPlayer.frameRate * endTime));
            } else
            {
                endFrame = (long)videoPlayer.frameCount - 1;
            }

            if (thumb == null)
            {
                renderTexture.Release();
                if (temp == null)
                {
                    temp = new GameObject("temp" + Guid.NewGuid());
                }

                VideoPlayer tempVideoPlayer = temp.GetComponent<VideoPlayer>();
                if (tempVideoPlayer == null)
                {
                    tempVideoPlayer = temp.AddComponent<VideoPlayer>();
                }

                tempVideoPlayer.timeReference = Application.isPlaying ? VideoTimeReference.ExternalTime :
                                                                    VideoTimeReference.ExternalTime;
                tempVideoPlayer.playOnAwake = false;
                tempVideoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.APIOnly;
                tempVideoPlayer.audioOutputMode = audioOutputMode;
                if (audioOutputMode == VideoAudioOutputMode.AudioSource && audioSource != null)
                {
                    tempVideoPlayer.SetTargetAudioSource(0, audioSource);
                }
                tempVideoPlayer.clip = videoClip;
                tempVideoPlayer.frame = startFrame;
                tempVideoPlayer.sendFrameReadyEvents = true;
                tempVideoPlayer.frameReady += OnNewFrame;
                tempVideoPlayer.Prepare();
                tempVideoPlayer.Play();
            } else
            {
                Graphics.Blit(thumb, renderTexture);
            }
        }

        void Update() {
            if (videoPlayer == null || videoClip == null)
                return;

            if (endTime == -1) {
                endTime = videoPlayer.length;
            }
            if (oldFrame < startFrame)
            {
                oldFrame = startFrame;
            }
            if (videoPlayer.frame < oldFrame) {
                videoPlayer.frame = oldFrame;
            }

            if (videoPlayer.isActiveAndEnabled) {
                if (videoPlayer.time < endTime)
                {
                    videoPlayer.Pause();
                    double timeSinceLastStep = Time.time - timeAtLastStep;
                    double framesSinceLastStep = videoPlayer.frameRate * timeSinceLastStep;
                    long framesToAdd = (long)Mathf.Floor((float)framesSinceLastStep);
                    for (int i = 0; i < framesToAdd; i++)
                    {
                        videoPlayer.StepForward();
                        oldFrame++;
                    }
                    timeAtLastStep += framesToAdd / videoPlayer.frameRate;
                }
            }
            else
            {
                timeAtLastStep = Time.time;
            }

            bool isPlayingThisFrame = videoPlayer.isActiveAndEnabled && videoPlayer.frame < endFrame;
            if (loop && wasPlayingLastFrame && !isPlayingThisFrame)
            {
                oldFrame = startFrame;
                videoPlayer.frame = startFrame;
                playedOnce = true;
                timeAtLastStep = Time.time;
                videoPlayer.enabled = true;
                videoPlayer.Play();
            }

            wasPlayingLastFrame = isPlayingThisFrame;
        }

        public void PrepareVideo()
        {
            if (videoPlayer == null || renderTexture == null || videoClip == null || image == null)
                return;

            videoPlayer.targetCameraAlpha = 0.0f;

            if (videoPlayer.clip != videoClip)
                StopVideo();

            if (videoPlayer.isPrepared || preparing)
                return;

            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoClip;
            videoPlayer.playOnAwake = false;
            videoPlayer.waitForFirstFrame = true;
		    videoPlayer.isLooping = loop;

            for (ushort i = 0; i < videoClip.audioTrackCount; ++i)
            {
                if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
                    videoPlayer.SetDirectAudioMute(i, mute || !Application.isPlaying);
                else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
                {
                    AudioSource audioSource = videoPlayer.GetTargetAudioSource(i);
                    if (audioSource != null)
                        audioSource.mute = mute || !Application.isPlaying;
                }
            }

            videoPlayer.time = startTime;
            videoPlayer.Prepare();
            preparing = true;
        }

        public override void PrepareFrame(Playable playable, FrameData info)
		{
            if (videoPlayer == null || renderTexture == null || videoClip == null || image == null)
                return;

            videoPlayer.timeReference = Application.isPlaying ? VideoTimeReference.ExternalTime :
                                                                VideoTimeReference.Freerun;

            if (videoPlayer.isPlaying && Application.isPlaying)
            {
                videoPlayer.externalReferenceTime = playable.GetTime();
            }
            else if (!Application.isPlaying)
            {
                SyncVideoToPlayable(playable);
            }

            if (Application.isPlaying)
            {
                Update();
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (videoPlayer == null || renderTexture == null || videoClip == null || image == null)
                return;

            if (!playedOnce)
            {
                PlayVideo();
                SyncVideoToPlayable(playable);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (videoPlayer == null || renderTexture == null || videoClip == null || image == null)
                return;

            if (Application.isPlaying)
                PauseVideo();
            else
            {
                StopVideo();
            }

            if (info.effectivePlayState == PlayState.Paused)
            {
                SetThumbnail();
            }
        }

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
            if (videoPlayer == null || renderTexture == null || videoClip == null || image == null)
                return;

            videoPlayer.targetCameraAlpha = info.weight;

		    if (Application.isPlaying)
		    {
		        for (ushort i = 0; i < videoPlayer.clip.audioTrackCount; ++i)
		        {
		            if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
		                videoPlayer.SetDirectAudioVolume(i, info.weight);
		            else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
		            {
		                AudioSource audioSource = videoPlayer.GetTargetAudioSource(i);
		                if (audioSource != null)
		                    audioSource.volume = info.weight;
		            }
		        }
		    }
		}

		public override void OnGraphStart(Playable playable)
		{
            if (videoClip == null || image == null)
                return;

            renderTexture = RenderTexture.GetTemporary((int)videoClip.width, (int)videoClip.height);
            videoPlayer = image.gameObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.playbackSpeed = 1;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.clip = videoClip;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;
            image.texture = (Texture)renderTexture;

            SetThumbnail();
            playedOnce = false;
		}

		public override void OnGraphStop(Playable playable)
		{
            if (videoClip == null || image == null)
                return;

            if (!Application.isPlaying)
            {
                StopVideo();
                if (videoPlayer != null)
                    Object.DestroyImmediate(videoPlayer);
            } else
            {
                if (videoPlayer != null)
                    Object.Destroy(videoPlayer);
            }

            if (renderTexture != null)
                RenderTexture.ReleaseTemporary(renderTexture);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            StopVideo();

            if (videoClip == null || image == null)
                return;

            if (videoPlayer != null)
            {
                if (!Application.isPlaying)
                {
                    Object.DestroyImmediate(videoPlayer);
                }
                else
                {
                    Object.Destroy(videoPlayer);
                }
            }

            if (renderTexture != null)
                RenderTexture.ReleaseTemporary(renderTexture);
        }

        public void PlayVideo()
        {
            timeAtLastStep = Time.time;

            if (videoPlayer == null)
                return;

            videoPlayer.Play();
            preparing = false;

            if (!Application.isPlaying)
                PauseVideo();
        }

        public void PauseVideo()
        {
            if (videoPlayer == null)
                return;

            videoPlayer.Pause();
            preparing = false;
        }

        public void StopVideo()
        {
            if (videoPlayer == null)
                return;

            playedOnce = false;
            videoPlayer.Stop();
            preparing = false;
        }

        private void SyncVideoToPlayable(Playable playable)
        {
            if (videoPlayer == null || videoPlayer.clip == null)
                return;

            float clipSectionFrameCount = endFrame - startFrame;
            float clipSectionLength = clipSectionFrameCount / videoPlayer.frameRate;
            playedOnce = playable.GetTime() * videoPlayer.playbackSpeed >= clipSectionLength;
            if (playedOnce && !loop)
            {
                videoPlayer.frame = endFrame;
                return;
            }

            videoPlayer.targetTexture = renderTexture;
            double remainderTime = (playable.GetTime() * videoPlayer.playbackSpeed) % clipSectionLength;
            double timePercentage = remainderTime / clipSectionLength;
            long frameNum = (long)Mathf.Floor((float)timePercentage * clipSectionFrameCount) + startFrame;
            videoPlayer.frame = frameNum;
        }
    }
}
