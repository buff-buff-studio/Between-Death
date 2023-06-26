using System.Collections.Generic;
using UnityEngine;
using Refactor.Data.Variables;
using UnityEngine.Serialization;

namespace Refactor.Audio
{
    public class AudioSystemController : Singleton<AudioSystemController>
    {
        #region Fields
        [Header("SETTINGS")]
        public int poolSize = 8;

        [SerializeField, HideInInspector]
        public int nextSourceId = 0;

        [FormerlySerializedAs("pallete")] [Header("REFERENCES")]
        public AudioPalette palette;
        public FloatVariable volumeGeneral;
        public FloatVariable volumeFX;
        public FloatVariable volumeMusic;

        [Header("POOL")]
        public List<AudioSource> freeSources = new List<AudioSource>();
        
        [Header("BUSY")]
        public List<AudioPlayer> busy = new List<AudioPlayer>();
        
        [SerializeField, HideInInspector]
        private bool _musicFading = false;
        [SerializeField, HideInInspector]
        private bool _musicFadingIn = false;
        [SerializeField, HideInInspector]
        private float _musicFadeTime = 0;
        [SerializeField, HideInInspector]
        private float _musicFadeProgress = 0;
        [SerializeField, HideInInspector]
        private bool _isPlayingMusic = false;
        [SerializeField, HideInInspector]
        private AudioPlayer _currentMusicPlayer = null;
        #endregion

        #region Unity Callbacks
        public void Awake()
        {
            for(int i = 0; i < poolSize; i ++)
                _CreateNewSource();
        }

        public void OnEnable()
        {
            if(AudioSystem.controller != null) 
                Destroy(gameObject);

            AudioSystem.controller = this;
            DontDestroyOnLoad(gameObject);
        }

        public void OnDisable()
        {
            if(AudioSystem.controller != null && AudioSystem.controller == this)
                AudioSystem.controller = null;
        }

        public void FixedUpdate()
        {
            transform.position = Camera.main.transform.position;

            var volGen = Mathf.Clamp01(volumeGeneral.Value/10f);
            var volFX = Mathf.Clamp01(volumeFX.Value/10) * volGen;
            var volMusic = Mathf.Clamp01(volumeMusic.Value/10) * volGen;

            if(_musicFading && _isPlayingMusic)
            {
                float mfp = _musicFadeProgress += Time.fixedDeltaTime;

                if(_musicFadingIn)
                {
                    volMusic *= mfp/_musicFadeTime;

                    if(mfp >= _musicFadeTime)
                        _musicFading = false;
                }
                else
                {
                    volMusic *= 1f - mfp/_musicFadeTime;

                    if(mfp >= _musicFadeTime)
                        _currentMusicPlayer.Stop();
                }  
            }

            int count = busy.Count;
            for(int i = 0; i < count; i ++)
            {
                AudioPlayer player = busy[i];

                if(player.type is AudioPlayer.Type.FX)
                    player.source.volume = volFX * player.audio.volume * player.volume;
                else
                    player.source.volume = volMusic * player.audio.volume * player.volume;

                if(!player.isPlaying)
                {
                    _FreeAudioPlayer(player);
                    count --;
                    i --;
                }

                switch(player.mode)
                {
                    case AudioPlayer.Mode.Position:
                        transform.position = player.targetPosition;
                        break;

                    case AudioPlayer.Mode.Transform:
                        if(transform.position == null)
                        {
                            transform.position = player.targetTransform.position;
                            continue;
                        }

                        _FreeAudioPlayer(player);
                        count --;
                        i --;  
                        break;
                }
            }
        }
        #endregion

        #region Methods
        public AudioPlayer PlaySound(int sound)
        {
            AudioPalette.Audio audio = palette.GetAudio(sound);
            
            if(audio != null)
            {
                AudioPlayer player = _ReserveSource(AudioPlayer.Type.FX);
                
                if(player != null)
                {
                    player.Play(audio);
                }

                return player;
            }

            return null;
        }

        public void PlayMusic(int music, float fadeIn = 1f, float fadeOut = 1f)
        {
            AudioPalette.Audio audio = palette.GetAudio(music);

            if(audio != null)
            {
                if(_isPlayingMusic)
                {
                    //Stop curent music
                    _currentMusicPlayer.onEnd.RemoveAllListeners();
                    _currentMusicPlayer.onEnd.AddListener(() => PlayMusic(music, fadeIn, fadeOut));
                    
                    _musicFading = true;
                    _musicFadingIn = false;

                    _musicFadeTime = fadeOut;
                    _musicFadeProgress = 0;
                }
                else
                {
                    _musicFading = true;
                    _musicFadingIn = true;

                    _musicFadeTime = fadeIn;
                    _musicFadeProgress = 0;
                    
                    //play new music
                    AudioPlayer player = _ReserveSource(AudioPlayer.Type.Music).Looping();
                
                    if(player != null)
                        player.Play(audio);
                    
                    player.source.volume = 0;

                    _currentMusicPlayer = player;
                    _isPlayingMusic = true;
                }
            }
            else if(_isPlayingMusic)
            {
                //Stop current music
                _musicFading = true;
                _musicFadingIn = false;

                _musicFadeTime = fadeOut;
                _musicFadeProgress = 0;
            }
        }
        
        public void StopAllFX()
        {
            int count = busy.Count;
            for(int i = 0; i < count; i ++)
            {
                AudioPlayer player = busy[i];

                if(player.type is AudioPlayer.Type.FX)
                {
                    _FreeAudioPlayer(player);
                    i --;
                    count --;
                }
            }
        }
        #endregion

        #region Internal Methods
        private AudioPlayer _ReserveSource(AudioPlayer.Type type)
        {
            if(freeSources.Count == 0)
                _CreateNewSource();

            AudioPlayer player = new AudioPlayer(freeSources[0], type);
            player.source.gameObject.SetActive(true);

            freeSources.RemoveAt(0);
            busy.Add(player);
            
            return player;
        }

        public void _FreeAudioPlayer(AudioPlayer player)
        {
            if(_isPlayingMusic && _currentMusicPlayer == player)
            {
                _musicFading = false;

                _isPlayingMusic = false;
                _currentMusicPlayer = null;
            }

            busy.Remove(player);
            
            if(player.source.isPlaying)
                player.source.Stop();

            player.source.clip = null;
            player.source.gameObject.SetActive(false);

            if(freeSources.Count >= poolSize)
                Destroy(player.source.gameObject);
            else
                freeSources.Add(player.source);

            player.onEnd?.Invoke();
        }

        private void _CreateNewSource()
        {
            int id = nextSourceId ++;
            
            GameObject o = new GameObject($"Audio Source ({id})");
            freeSources.Add(o.AddComponent<AudioSource>());
            o.SetActive(false);
            o.transform.parent = transform;
        }
        #endregion
    }
}