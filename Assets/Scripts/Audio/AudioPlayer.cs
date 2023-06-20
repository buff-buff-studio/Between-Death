using System;
using UnityEngine;
using UnityEngine.Events;
using Unity.Mathematics;

namespace Refactor.Audio
{
    [Serializable]
    public class AudioPlayer
    {
        [Serializable]
        public enum Type
        {
            FX,
            Music
        }

        [Serializable]
        public enum Mode
        {
            Default,
            Position,
            Transform
        }

        public bool isPlaying => source.isPlaying;
        public bool loop => source.loop;
    
        [Header("SETTINGS")]
        public Mode mode = Mode.Default;
        public Type type => _type;
        private Type _type = Type.FX;
        public Vector3 targetPosition;
        public Transform targetTransform;
        public float volume 
        {
            get => _volume;
            set => math.clamp(value, 0, 1);
        }
        [SerializeField, HideInInspector]
        private float _volume = 1f;

        [Header("AUDIO")]
        public AudioPallete.Audio audio;
        public AudioSource source;

        [Header("CALLBACK")]
        public UnityEvent onEnd = new UnityEvent();

        public AudioPlayer(AudioSource source, Type type)
        {
            this.source = source;
            source.volume = 0;
            source.transform.localPosition = Vector3.zero;
            source.loop = false;
            _type = type;
        }

        public void Play(AudioPallete.Audio audio)
        {
            this.audio = audio;
            source.clip = audio.clip;
            source.Play();
        }

        public void Stop()
        {
            AudioSystem.controller._FreeAudioPlayer(this);
        }

        public AudioPlayer At(Vector3 position)
        {
            mode = Mode.Position;
            targetPosition = position;

            return this;
        }

        public AudioPlayer At(Transform target)
        {
            mode = Mode.Transform;
            targetTransform = target;
            return this;
        } 

        public AudioPlayer Looping(bool looping = true)
        {
            source.loop = looping;
            return this;
        }

        public AudioPlayer Volume(float volume)
        {
            this.volume = volume;
            return this;
        }

        public AudioPlayer OnEnd(UnityAction callback)
        {
            onEnd.AddListener(callback);
            return this;
        }
    }
}