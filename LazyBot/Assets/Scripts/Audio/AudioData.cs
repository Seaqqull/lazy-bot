using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

namespace LazyBot.Audio.Data
{
    using AudioSourceEngine = UnityEngine.AudioSource;
    /// <summary>
    /// Keeps audio settings responsible for detection (by enemies) and audibility.
    /// </summary>
    [System.Serializable]
    public class AudioSource
    {
        /// <summary>
        /// Settings responsible for its detection.
        /// </summary>
        [System.Serializable]
        public class AudioDetection
        {
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float m_loudness = 100.0f;

            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float m_maxDistance = 10.0f;
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float m_minDistance;

            [SerializeField] public bool m_cutOnMin = false;
            [SerializeField] public bool m_cutOnMax = true;

            [SerializeField] public AnimationCurve m_loudnessSpread = AnimationCurve.Linear(0, 1, 1, 0);
        }

        /// <summary>
        /// Settings responsible for audibility.
        /// </summary>
        [System.Serializable]
        public class Audio3D
        {
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float m_maxDistance = 10.0f;
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float m_minDistance;

            /// <summary>
            /// Set how audible the Doppler effect is. Use 0 to disable it. 
            /// Use 1 make it audible for fast moving objects.
            /// </summary>
            [SerializeField] [Range(0.0f, 5.0f)] public float m_dopplerLevel = 1.0f;
            [SerializeField] [Range(0.0f, 360.0f)] public float m_spread = 360.0f;

            [SerializeField] public AnimationCurve m_volumeSpread = AnimationCurve.Linear(0, 1, 1, 0);
        }


        [SerializeField] private string m_name;

        [SerializeField] private AudioMixerGroup m_output;
        [SerializeField] private AudioClip m_audioClip;

        [SerializeField] private bool m_loop;
        [SerializeField] private bool m_mute;

        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_playDelay;
        /// <summary>
        /// Determines the reverb effect that will be used by the reverb zone.
        /// </summary>
        [SerializeField] [Range(0.0f, 1.1f)] private float m_reverbZoneMix = 1.0f;
        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float m_playTime;
        /// <summary>
        /// Pitch is a quality that makes a melody go higher or lower.
        /// </summary>
        [SerializeField] [Range(-3.0f, 3.0f)] private float m_pitch = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_volume = 1.0f;
        /// <summary>
        /// Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc).
        /// 0.0 makes the sound full 2D, 1.0 makes it full 3D.
        /// </summary>
        [SerializeField] [Range(0.0f, 1.0f)] private float m_spatialBlend;

        [SerializeField] AudioDetection m_settingDetection;
        [SerializeField] Audio3D m_setting3D;


        /// <summary>
        /// Attached to gameObject audioSource.
        /// </summary>
        private Dictionary<string, AudioSourceEngine> m_records;


        /// <summary>
        /// Attached to gameObject audioSource.
        /// </summary>
        public Dictionary<string, AudioSourceEngine> Records
        {
            get
            {
                return (this.m_records) ??
                    (this.m_records = new Dictionary<string, AudioSourceEngine>());
            }
        }
        public float OutherRadiusDetection
        {
            get { return this.m_settingDetection.m_maxDistance; }
        }
        public AnimationCurve VolumeSpread
        {
            get { return this.m_setting3D.m_volumeSpread; }
        }
        public float InnerRadiusDetection
        {
            get { return this.m_settingDetection.m_minDistance; }
        }
        public float OutherRadius3D
        {
            get { return this.m_setting3D.m_maxDistance; }
        }
        public float InnerRadius3D
        {
            get { return this.m_setting3D.m_minDistance; }
        }
        public float AudioLength
        {
            get { return (this.m_audioClip) ? this.m_audioClip.length : 0.0f; }
        }
        public float PlayDelay
        {
            get { return this.m_playDelay; }
            set { this.m_playDelay = value; }
        }
        public float PlayTime
        {
            get { return this.m_playTime; }
            set { this.m_playTime = value; }
        }
        public string Name
        {
            get { return this.m_name; }
            set { this.m_name = value; }
        }
        public bool Loop
        {
            get { return this.m_loop; }
            set { this.m_loop = value; }
        }


        /// <summary>
        /// Plays (instant or delayed, depends on settings) audio with name.
        /// </summary>
        /// <param name="audioKey">Audio name.</param>
        /// <returns>Is audio playback has been started.</returns>
        public bool Play(string audioKey)
        {
            AudioSourceEngine audioSource;

            if (!Records.TryGetValue(audioKey, out audioSource))
                return false;

            if (m_playDelay == 0.0f)
                audioSource.Play();
            else
                audioSource.PlayDelayed(m_playDelay);

            return true;
        }

        /// <summary>
        /// Instant plays audio with name.
        /// </summary>
        /// <param name="audioKey">Audio name.</param>
        /// <returns>Is audio playback has been started.</returns>
        public bool PlayInstant(string audioKey)
        {
            AudioSourceEngine audioSource;

            if (!Records.TryGetValue(audioKey, out audioSource))
                return false;

            audioSource.Play();

            return true;
        }

        /// <summary>
        /// Searches for audio with name.
        /// </summary>
        /// <param name="audioKey">Audio name.</param>
        /// <returns>Is audio was found.</returns>
        public bool ContainAudio(string audioKey)
        {
            return Records.ContainsKey(audioKey);
        }

        /// <summary>
        /// Stops playing audio and destroy it.
        /// </summary>
        /// <param name="audioKey">Audio name.</param>
        /// <returns>Is audio was found and destroyed.</returns>
        public bool DestroyAudioSource(string audioKey)
        {
            AudioSourceEngine audioSource;

            if (!Records.TryGetValue(audioKey, out audioSource))
                return false;

            audioSource.Stop();
            UnityEngine.Object.Destroy(audioSource);

            Records.Remove(audioKey);

            return true;
        }

        /// <summary>
        /// Plays audio with name after some delay.
        /// </summary>
        /// <param name="audioKey">Audio name.</param>
        /// <param name="delay">Playback delay.</param>
        /// <returns>Is audio was found and seted to play after delay.</returns>
        public bool PlayDelayed(string audioKey, float delay)
        {
            AudioSourceEngine audioSource;

            if (!Records.TryGetValue(audioKey, out audioSource))
                return false;

            audioSource.PlayDelayed(delay);

            return true;
        }

        /// <summary>
        /// Attaches audio source to gameObject.
        /// </summary>
        /// <param name="gameObject">Object to which audio source will be attached.</param>
        /// <returns>Key of initialized audio source.</returns>
        public string InitializeAudioSource(GameObject gameObject)
        {
            if ((!m_output) ||
                (!m_audioClip)) return string.Empty;

            string sourceKey = LazyBot.Utility.Data.Hasher.GenerateHash();
            AudioSourceEngine audioSource = gameObject.AddComponent<UnityEngine.AudioSource>();

            audioSource.clip = m_audioClip;
            audioSource.outputAudioMixerGroup = m_output;
            audioSource.mute = m_mute;
            audioSource.playOnAwake = false;
            audioSource.loop = m_loop;
            audioSource.volume = m_volume;
            audioSource.pitch = m_pitch;
            audioSource.spatialBlend = m_spatialBlend;
            audioSource.reverbZoneMix = m_reverbZoneMix;

            audioSource.dopplerLevel = m_setting3D.m_dopplerLevel;
            audioSource.spread = m_setting3D.m_spread;
            audioSource.minDistance = m_setting3D.m_minDistance;
            audioSource.maxDistance = m_setting3D.m_maxDistance;

            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, m_setting3D.m_volumeSpread);
            audioSource.rolloffMode = AudioRolloffMode.Custom;

            Records.Add(sourceKey, audioSource);

            return sourceKey;
        }

        /// <summary>
        /// Calculates loudness of audio at position of listener from position of source.
        /// </summary>
        /// <param name="source">Position of audio source in scene.</param>
        /// <param name="listener">Position of listener in scene.</param>
        /// <returns>Loudness of audio.</returns>
        public float GetAudibility(Vector3 source, Vector3 listener)
        {
            if (Records.Count == 0) return 0.0f;

            float distance = Vector3.Distance(source, listener);

            if (((m_settingDetection.m_cutOnMin) && (distance < m_settingDetection.m_minDistance)) ||
                ((m_settingDetection.m_cutOnMax) && (distance > m_settingDetection.m_maxDistance)))
                return 0.0f;

            float relativeAudibility = LazyBot.Utility.Data.FloatHelper.
                Map(distance, (m_settingDetection.m_minDistance < distance) ? m_settingDetection.m_minDistance : distance,
                    (m_settingDetection.m_maxDistance > distance) ? m_settingDetection.m_maxDistance : distance, 0, 1);

            return m_settingDetection.m_loudness * m_settingDetection.m_loudnessSpread.Evaluate(relativeAudibility);
        }

    }
}
