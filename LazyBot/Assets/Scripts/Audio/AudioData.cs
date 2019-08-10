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
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float _loudness = 100.0f;

            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float _maxDistance = 10.0f;
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float _minDistance;

            [SerializeField] public bool _cutOnMin = false;
            [SerializeField] public bool _cutOnMax = true;

            [SerializeField] public AnimationCurve _loudnessSpread = AnimationCurve.Linear(0, 1, 1, 0);
        }

        /// <summary>
        /// Settings responsible for audibility.
        /// </summary>
        [System.Serializable]
        public class Audio3D
        {
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float _maxDistance = 10.0f;
            [SerializeField] [Range(0.0f, ushort.MaxValue)] public float _minDistance;

            /// <summary>
            /// Set how audible the Doppler effect is. Use 0 to disable it. 
            /// Use 1 make it audible for fast moving objects.
            /// </summary>
            [SerializeField] [Range(0.0f, 5.0f)] public float _dopplerLevel = 1.0f;
            [SerializeField] [Range(0.0f, 360.0f)] public float _spread = 360.0f;

            [SerializeField] public AnimationCurve _volumeSpread = AnimationCurve.Linear(0, 1, 1, 0);
        }


        [SerializeField] private string _name;

        [SerializeField] private AudioMixerGroup _output;
        [SerializeField] private AudioClip _audioClip;

        [SerializeField] private bool _loop;
        [SerializeField] private bool _mute;

        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float _playDelay;
        /// <summary>
        /// Determines the reverb effect that will be used by the reverb zone.
        /// </summary>
        [SerializeField] [Range(0.0f, 1.1f)] private float _reverbZoneMix = 1.0f;
        [SerializeField] [Range(0.0f, ushort.MaxValue)] private float _playTime;
        /// <summary>
        /// Pitch is a quality that makes a melody go higher or lower.
        /// </summary>
        [SerializeField] [Range(-3.0f, 3.0f)] private float _pitch = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float _volume = 1.0f;
        /// <summary>
        /// Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc).
        /// 0.0 makes the sound full 2D, 1.0 makes it full 3D.
        /// </summary>
        [SerializeField] [Range(0.0f, 1.0f)] private float _spatialBlend;

        [SerializeField] AudioDetection _settingDetection;
        [SerializeField] Audio3D _setting3D;


        /// <summary>
        /// Attached to gameObject audioSource.
        /// </summary>
        private Dictionary<string, AudioSourceEngine> _records;


        /// <summary>
        /// Attached to gameObject audioSource.
        /// </summary>
        public Dictionary<string, AudioSourceEngine> Records
        {
            get
            {
                return (this._records) ??
                    (this._records = new Dictionary<string, AudioSourceEngine>());
            }
        }
        public float OutherRadiusDetection
        {
            get { return this._settingDetection._maxDistance; }
        }
        public AnimationCurve VolumeSpread
        {
            get { return this._setting3D._volumeSpread; }
        }
        public float InnerRadiusDetection
        {
            get { return this._settingDetection._minDistance; }
        }
        public float OutherRadius3D
        {
            get { return this._setting3D._maxDistance; }
        }
        public float InnerRadius3D
        {
            get { return this._setting3D._minDistance; }
        }
        public float AudioLength
        {
            get { return (this._audioClip) ? this._audioClip.length : 0.0f; }
        }
        public float PlayDelay
        {
            get { return this._playDelay; }
            set { this._playDelay = value; }
        }
        public float PlayTime
        {
            get { return this._playTime; }
            set { this._playTime = value; }
        }
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
        public bool Loop
        {
            get { return this._loop; }
            set { this._loop = value; }
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

            if (_playDelay == 0.0f)
                audioSource.Play();
            else
                audioSource.PlayDelayed(_playDelay);

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
            if ((!_output) ||
                (!_audioClip)) return string.Empty;

            string sourceKey = LazyBot.Utility.Data.Hasher.GenerateHash();
            AudioSourceEngine audioSource = gameObject.AddComponent<UnityEngine.AudioSource>();

            audioSource.clip = _audioClip;
            audioSource.outputAudioMixerGroup = _output;
            audioSource.mute = _mute;
            audioSource.playOnAwake = false;
            audioSource.loop = _loop;
            audioSource.volume = _volume;
            audioSource.pitch = _pitch;
            audioSource.spatialBlend = _spatialBlend;
            audioSource.reverbZoneMix = _reverbZoneMix;

            audioSource.dopplerLevel = _setting3D._dopplerLevel;
            audioSource.spread = _setting3D._spread;
            audioSource.minDistance = _setting3D._minDistance;
            audioSource.maxDistance = _setting3D._maxDistance;

            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, _setting3D._volumeSpread);
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

            if (((_settingDetection._cutOnMin) && (distance < _settingDetection._minDistance)) ||
                ((_settingDetection._cutOnMax) && (distance > _settingDetection._maxDistance)))
                return 0.0f;

            float relativeAudibility = LazyBot.Utility.Data.FloatHelper.
                Map(distance, (_settingDetection._minDistance < distance) ? _settingDetection._minDistance : distance,
                    (_settingDetection._maxDistance > distance) ? _settingDetection._maxDistance : distance, 0, 1);

            return _settingDetection._loudness * _settingDetection._loudnessSpread.Evaluate(relativeAudibility);
        }

    }
}
