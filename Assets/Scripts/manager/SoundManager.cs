/*
* Copyright Incago Studio
* http://blog.rewuio.com/
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoonaLegend;

/// <summary>
/// 작성일 : 2016-01-14
/// 작성자 : 김준학(incago@asnet.co.kr)
/// 게임내에서 재생되는 소리(음악, 효과음)를 관리하는 매니져입니다.
/// 오브젝트 풀링이 성능에 영향을 미칠지 판단하여 작성하여야 합니다.
/// 오디오매니져라는 이름을 사용하지 않은것은 유니티 셋팅의 오디오매니져와의 네이밍 혼란을 막기위한것입니다.
/// 참고 : https://github.com/omgwtfgames/unity-bowerbird/tree/master/Scripts/Sound
/// </summary>
///

namespace DoonaLegend
{
    public class SoundManager : MonoBehaviour
    {
        public bool soundOn = true;
        public string preferenceSFXState = "sfx_state";
        public float volume = 1.0f;
        public bool ScaleOutputVolume = true;
        public string preferenceName = "sfx_volume";
        public bool initialVolumeFromPreference = true;
        public bool useFilenameAsSoundName = true;
        public List<string> soundNames;
        public AudioClip[] sounds;
        Dictionary<string, AudioClip> soundMap = new Dictionary<string, AudioClip>();

        // we keep tally of how many sfx of a particular name are playing simultaneously
        // and don't play more than a certain number at a time (for efficiency doesn't
        // account for sounds that are manually stopped, assumes they play for the full duration
        // after being triggered)
        public int maxSimlutSfxOfOneType = 4;
        Dictionary<string, int> currentlyPlayingCount = new Dictionary<string, int>();

        // We essentially create a static object pool, with a set number of
        // AudioSources ('channels'). We cycle through these each time we play
        // a sound - this will reduce the change of one sample somehow clobbering
        // another, and allows one channel to play at a different pitch without
        // disrupting other playing samples.
        public int numberOfChannels = 4;
        private AudioSource[] channels;
        private int channelIndex = 0;

        private bool[] channelPlayStatus;

        private static SoundManager _instance;
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<SoundManager>();
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
            // Debug.Log("Sound Manager Init");
            channels = new AudioSource[numberOfChannels];
            channelPlayStatus = new bool[numberOfChannels];
            for (int i = 0; i < sounds.Length; i++)
            {
                if (useFilenameAsSoundName)
                {
                    soundNames.Add(sounds[i].name);
                    soundMap.Add(sounds[i].name, sounds[i]);
                }
                else
                {
                    soundMap.Add(soundNames[i], sounds[i]);
                }
                currentlyPlayingCount.Add(soundNames[i], 0);
            }

            // generate attached gameobjects "Channel1", "Channel2" with AudioSources
            // accessible in the channel array
            for (int j = 0; j < numberOfChannels; j++)
            {
                GameObject c = new GameObject();
                c.name = "Channel" + j;
                c.transform.parent = transform;
                AudioSource a = c.AddComponent<AudioSource>();
                a.rolloffMode = AudioRolloffMode.Linear;
                a.spatialBlend = 0f;
                a.dopplerLevel = 0f;
                a.loop = false;
                channels[j] = a;
            }

            if (initialVolumeFromPreference)
                volume = GetVolumePreference();

            soundOn = GetSFXState();
        }

        void Update()
        {
            if (channelPlayStatus != null && channelPlayStatus.Length > 0)
            {
                for (int i = 0; i < numberOfChannels; i++)
                {
                    channelPlayStatus[i] = channels[i].isPlaying;
                }
            }
        }

        public void ResetParam()
        {
            for (int i = 0; i < channelPlayStatus.Length; i++)
            {
                channelPlayStatus[i] = false;
            }

            List<string> keys = new List<string>();
            foreach (string key in currentlyPlayingCount.Keys)
            {
                keys.Add(key);
            }

            foreach (string key in keys)
            {
                currentlyPlayingCount[key] = 0;
            }
        }

        public bool GetSFXState()
        {
            return PlayerPrefs.GetInt(preferenceSFXState, 1) == 1 ? true : false;
        }

        public void SetSFXState(bool value)
        {
            PlayerPrefs.SetInt(preferenceSFXState, value ? 1 : 0);
            PlayerPrefs.Save();
            soundOn = value;
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

        public void Play(string soundname)
        {
            // Debug.Log("SoundManager.Play(" + soundname + ")");
            Play(soundname, volume, 1.0f);
        }

        public void Play(string soundname, float volume, float pitch)
        {
            if (!soundMap.ContainsKey(soundname))
            {
                Debug.LogWarning("SoundManager: Tried to play undefined sound: " + soundname);
                return;
            }

            if (soundOn)
            {
                if (currentlyPlayingCount[soundname] > maxSimlutSfxOfOneType)
                {
                    //Debug.Log("이미 플레이되는 사운드가 있어서 스킵함");
                    return;
                }
                int channelIndex = 0;
                bool hasUsableChannel = false;

                for (channelIndex = 0; channelIndex < channels.Length; channelIndex++)
                {
                    if (!channels[this.channelIndex].isPlaying)
                    {
                        hasUsableChannel = true;
                        break;
                    }
                }
                if (!hasUsableChannel)
                {
                    //Debug.Log("사용가능한 채널이 없어서 스킵함");
                    return;
                }

                channels[this.channelIndex].pitch = pitch;
                channels[this.channelIndex].volume = volume;
                float v = volume;
                if (ScaleOutputVolume) v = ScaleVolume(volume);
                channels[this.channelIndex].PlayOneShot(soundMap[soundname], v);
                currentlyPlayingCount[soundname] += 1;
                StartCoroutine("decrementPlayCountInFuture", soundname);
                incrementChannelIndex();
            }
        }

        public void Play(string soundname, float volume)
        {
            Play(soundname, volume, 1.0f);
        }

        // TODO: we should consider using this dB scale as an option when porting these changes
        //       over to unity-bowerbird: http://wiki.unity3d.com/index.php?title=Loudness
        /*
        *   Quadratic scaling of actual volume used by AudioSource. Approximates the proper exponential.
        */
        public float ScaleVolume(float v)
        {
            v = Mathf.Pow(v, 1.414f);
            return Mathf.Clamp01(v);
        }

        private void incrementChannelIndex()
        {
            if (channelIndex < channels.Length - 1)
            {
                channelIndex++;
            }
            else
            {
                channelIndex = 0;
            }
        }

        private IEnumerator decrementPlayCountInFuture(string soundname)
        {
            if (Time.timeScale > 0)
            {
                yield return new WaitForSeconds(soundMap[soundname].length);
            }
            if (currentlyPlayingCount[soundname] > 0)
            {
                currentlyPlayingCount[soundname] -= 1;
            }
        }
    }
}
