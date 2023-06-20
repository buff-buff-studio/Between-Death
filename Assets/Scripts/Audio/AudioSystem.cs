using UnityEngine;

namespace Refactor.Audio
{
    public static class AudioSystem
    {
        /// <summary>
        /// Represents silent sound hash id
        /// Used to stop music/represent null values
        /// </summary>
        public const int SILENT = 0;

        /// <summary>
        /// Default music fade in time
        /// </summary>
        public const float DEFAULT_MUSIC_FADE_IN = 1;

        /// <summary>
        /// Default music fade out time
        /// </summary>
        public const float DEFAULT_MUSIC_FADE_OUT = 1;

        /// <summary>
        /// Current audio system controller
        /// </summary>
        public static AudioSystemController controller;

        public static int HashString(string sound)
        {   
            if(sound == null)
                return SILENT;

            return Animator.StringToHash(sound);
        }

        public static AudioPlayer PlaySound(string sound)
        {   
            return controller.PlaySound(Animator.StringToHash(sound));
        }

        public static AudioPlayer PlaySound(int sound)
        {
            return controller.PlaySound(sound);
        }

        public static void PlayMusic(string music, float fadeIn = DEFAULT_MUSIC_FADE_IN, float fadeOut = DEFAULT_MUSIC_FADE_OUT)
        {
            controller.PlayMusic(Animator.StringToHash(music), fadeIn, fadeOut);
        }

        public static void PlayMusic(int music, float fadeIn = DEFAULT_MUSIC_FADE_IN, float fadeOut = DEFAULT_MUSIC_FADE_OUT)
        {
            controller.PlayMusic(music, fadeIn, fadeOut);
        }

        public static void StopMusic(float fadeOut = DEFAULT_MUSIC_FADE_OUT)
        {
            controller.PlayMusic(SILENT, 0, fadeOut);
        }

        public static void StopAllFX()
        {
            controller.StopAllFX();
        }
    }
}