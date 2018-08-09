/*
* Copyright Incago Studio
* http://blog.rewuio.com/
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoonaLegend;

namespace DoonaLegend
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        public bool musicOn = true;
        public string preferenceMusicState = "music_state";
        public AudioClip[] tracks;
        public bool playOnStart = true;

        public int defaultTrack = 0;
        public string preferenceName = "music_volume";
        public bool initialVolumeFromPreference = true;
        public float _volume = 0.5f;
        public float _pitch = 1.0f;
        public bool ScaleOutputVolume = true;

        private AudioSource audio;

        public float volume
        {
            get { return _volume; }
            set
            {
                if (ScaleOutputVolume)
                {
                    audio.volume = ScaleVolume(value);
                }
                else
                {
                    audio.volume = value;
                }
                _volume = value;
            }
        }

        public float pitch
        {
            get { return _pitch; }
            set
            {
                audio.pitch = value;
                _pitch = value;
            }
        }

        private static MusicManager _instance;
        public static MusicManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<MusicManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        void Awake()
        {
            if (_instance == null)
            {
                DontDestroyOnLoad(gameObject);
                _instance = this;
                Init();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void Init()
        {
            audio = GetComponent<AudioSource>();

            audio.clip = tracks[defaultTrack];
            if (GetMusicState())
            {
                // volume = _volume;
                // if (initialVolumeFromPreference)
                //     volume = GetVolumePreference();
                volume = 0.5f;
            }
            else
            {
                volume = 0;
            }

            AudioSource audioSrc = GetComponent<AudioSource>();
            audio.rolloffMode = AudioRolloffMode.Linear;
            audio.loop = true;
            audio.dopplerLevel = 0f;
            audio.spatialBlend = 0f;

        }

        void Start()
        {
            if (playOnStart)
            {
                Play();
            }
        }

        public bool GetMusicState()
        {
            return PlayerPrefs.GetInt(preferenceMusicState, 1) == 1 ? true : false;
        }

        public void SetMusicState(bool value)
        {
            PlayerPrefs.SetInt(preferenceMusicState, value ? 1 : 0);
            PlayerPrefs.Save();
            if (value)
            {
                volume = 0.5f;
                // volume = _volume;
                // if (initialVolumeFromPreference)
                //     volume = GetVolumePreference();
            }
            else
            {
                volume = 0;
            }
        }

        public void PlayTrack(int i)
        {
            audio.Stop();
            audio.clip = tracks[i];
            audio.Play();
        }

        public void SetTrack(int i)
        {
            audio.clip = tracks[i];
        }

        public void Pause()
        {
            audio.Pause();
        }

        public void Play()
        {
            audio.Play();
        }

        public void Stop()
        {
            audio.Stop();
        }

        public void Fade(float targetVolume, float fadeTime)
        {
            //Debug.Log(volume);
            //Debug.Log(targetVolume);
            //LeanTween.value(gameObject, SetVolume, volume, targetVolume, fadeTime);
        }

        public void FadeOut(float fadeTime)
        {
            StartCoroutine(FadeOutAsync(fadeTime));
        }

        IEnumerator FadeOutAsync(float fadeTime)
        {
            Fade(0f, fadeTime);
            yield return new WaitForSeconds(fadeTime);
            Pause();
        }

        public void FadeIn(float fadeTime)
        {
            Play();
            Fade(1.0f, fadeTime);
        }

        public void SlidePitch(float targetPitch, float fadeTime)
        {
            //LeanTween.value(gameObject, SetPitch, pitch, targetPitch, fadeTime);
        }

        public float GetVolumePreference()
        {
            return PlayerPrefs.GetFloat(preferenceName, volume);
        }

        public void SaveCurrentVolumePreference()
        {
            SaveVolumePreference(volume);
        }

        public void SaveVolumePreference(float v)
        {
            PlayerPrefs.SetFloat(preferenceName, v);
            PlayerPrefs.Save();
        }

        public void SetPitch(float p)
        {
            pitch = p;
        }

        public void SetVolume(float v)
        {
            if (ScaleOutputVolume)
            {
                audio.volume = ScaleVolume(v);
            }
            else
            {
                audio.volume = v;
            }
        }

        // TODO: we should consider using this dB scale as an option when porting these changes
        //       over to unity-bowerbird: http://wiki.unity3d.com/index.php?title=Loudness
        /*
        *   Quadratic scaling of actual volume used by AudioSource. Approximates the proper exponential.
        */
        public float ScaleVolume(float v)
        {
            v = Mathf.Pow(v, 1.414f);
            return Mathf.Clamp(v, 0f, 1f);
        }
    }
}