using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

namespace GGJ2023
{
    public class AudioManager : MonoBehaviour
    {
        #region Fields and Properties
        public static AudioManager Instance;  

        [Header("Music")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip mainMenuClip; 
        [SerializeField] private AudioClip inGameClip;
        [SerializeField] private AudioClip endGameClip;
        [SerializeField] private float fadingDuration = .55f;

        [Header("SFX")]
        [SerializeField] private AudioSource sfxSource; 
        private List<AudioSource> audioSources = new List<AudioSource>();

        [SerializeField] private AudioClip snapClip;
        [SerializeField] private AudioClip tilePoseClip;
        [SerializeField] private AudioClip drawClip;

        public AudioClip SnapClip => snapClip; 
        public AudioClip TilePoseClip => tilePoseClip; 
        public AudioClip DrawClip => drawClip; 

        #endregion 

        #region UnityMethods
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(this); 
        }

        private void Start()
        {
            GameManager.OnGameReady += StartInGameMusic;
            GameManager.OnGameStopped += StartEndGameMusic; 
        }

        private void OnDestroy()
        {
            GameManager.OnGameReady -= StartInGameMusic;
            GameManager.OnGameStopped -= StartEndGameMusic;
        }
        #endregion

        #region Music Methods
        private Sequence fadeSequence; 
        private void StartInGameMusic()
        {
            fadeSequence = DOTween.Sequence();
            fadeSequence.Append(musicSource.DOFade(0, fadingDuration));
            fadeSequence.AppendCallback(PlayInGameMusic); 

            void PlayInGameMusic()
            {
                musicSource.Stop(); 
                musicSource.volume = 1.0f;
                musicSource.clip = inGameClip; 
                musicSource.Play();
            }
        }

        private void StartEndGameMusic()
        {
            fadeSequence = DOTween.Sequence();
            fadeSequence.Append(musicSource.DOFade(0, fadingDuration));
            fadeSequence.AppendCallback(PlayEndGameMusic);

            void PlayEndGameMusic()
            {
                musicSource.Stop();
                musicSource.volume = 1.0f;
                musicSource.clip = endGameClip;
                musicSource.loop = false;
                musicSource.Play();
            }

        }

        public void RestartMainMenuMusic()
        {
            fadeSequence = DOTween.Sequence();
            fadeSequence.Append(musicSource.DOFade(0, fadingDuration));
            fadeSequence.AppendCallback(PlayMainMenuMusic);

            void PlayMainMenuMusic()
            {
                musicSource.Stop();
                musicSource.volume = 1.0f;
                musicSource.clip = mainMenuClip;
                musicSource.loop = true;
                musicSource.Play();
            }
        }
        #endregion

        #region SFX
        public void PlaySFX(AudioClip _clip, float _volume = 1.0f)
        {
            AudioSource _source;
            if(audioSources.Count == 0)
            {
                _source = Instantiate(sfxSource, Vector2.zero, Quaternion.identity, transform); 
            }
            else
            {
                _source = audioSources[0];
                audioSources.RemoveAt(0); 
            }
            _source.clip = _clip; 
            _source.Play(); 
            Sequence _audioSequence = DOTween.Sequence();
            _audioSequence.AppendInterval(_clip.length);
            _audioSequence.AppendCallback(() => SendToPool(_source)); 

            void SendToPool(AudioSource _source)
            {
                _source.Stop();
                audioSources.Add(_source); 
            }
        }
        #endregion 
    }
}
