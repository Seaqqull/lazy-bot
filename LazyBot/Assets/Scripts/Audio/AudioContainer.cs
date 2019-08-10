using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace LazyBot.Audio
{
    /// <summary>
    /// Controls playback of audio.
    /// </summary>
    public class AudioContainer : MonoBehaviour
    {
        [SerializeField] private string _name;

        /// <summary>
        /// Is container can contain other containers.
        /// </summary>
        [SerializeField] private bool _isAccommodate = true;
        [SerializeField] private bool _isActive = true;
        /// <summary>
        /// Is container can be child in other container.
        /// </summary>
        [SerializeField] private bool _isNested = true;

        [SerializeField] private List<LazyBot.Audio.Data.AudioSource> _audios;


        private List<LazyBot.Audio.AudioContainer> _containers;


        public List<LazyBot.Audio.AudioContainer> Containers
        {
            get
            {
                return (this._containers) ??
                    (this._containers = new List<LazyBot.Audio.AudioContainer>());
            }
            set
            {
                this._containers = value;
            }
        }
        public List<LazyBot.Audio.Data.AudioSource> Audios
        {
            get
            {
                return (this._audios) ??
                    (this._audios = new List<LazyBot.Audio.Data.AudioSource>());
            }
        }
        /// <summary>
        /// Is container can contain other containers.
        /// </summary>
        public bool IsAccommodate
        {
            get { return this._isAccommodate; }
        }
        public bool IsActive
        {
            get { return this._isActive; }
            set { this._isActive = value; }
        }
        /// <summary>
        /// Is container can be child in other container.
        /// </summary>
        public bool IsNested
        {
            get { return this._isNested; }
        }


        private void Awake()
        {
            if (!IsAccommodate) return;

            Containers = GetComponentsInChildren<LazyBot.Audio.AudioContainer>().
                OfType<LazyBot.Audio.AudioContainer>().ToList();

            for (int i = 0; i < _containers.Count; i++)
            {
                if ((!_containers[i].IsNested) ||
                    (_containers[i].gameObject.transform.parent != transform)) // exclude not direct childs
                {
                    _containers.RemoveAt(i--);
                }
            }
        }


        /// <summary>
        /// Plays audio.
        /// </summary>
        /// <param name="index">Index of audio.</param>
        /// <returns>MD5 hash key of audio.</returns>
        private string Play(int index)
        {
            if ((index < 0) ||
                (index >= _audios.Count))
                return string.Empty;

            string audioKey = _audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            _audios[index].Play(audioKey);

            if (!_audios[index].Loop)
                RunLater(
                    () => _audios[index].DestroyAudioSource(audioKey),
                    _audios[index].PlayDelay + ((_audios[index].PlayTime == 0.0f) ? _audios[index].AudioLength : _audios[index].PlayTime)
                );

            return audioKey;
        }

        /// <summary>
        /// Stops playback of audio.
        /// </summary>
        /// <param name="index">Index of audio.</param>
        /// <param name="audioKey">MD5 hash key of audio.</param>
        /// <returns>Is audio was found and stop is executable.</returns>
        private bool Stop(int index, string audioKey)
        {
            if ((index < 0) ||
                (index >= _audios.Count) ||
                (!_audios[index].ContainAudio(audioKey)))
                return false;

            RunLater(() => _audios[index].DestroyAudioSource(audioKey),
                _audios[index].AudioLength - _audios[index].Records[audioKey].time);

            return true;
        }


        /// <summary>
        /// Instant plays audio.
        /// </summary>
        /// <param name="index">Index of audio.</param>
        /// <returns>MD5 hash key of audio.</returns>
        private string PlayInstant(int index)
        {
            if ((index < 0) ||
                (index >= _audios.Count))
                return string.Empty;

            string audioKey = _audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            _audios[index].PlayInstant(audioKey);

            if (!_audios[index].Loop)
                RunLater(
                    () => _audios[index].DestroyAudioSource(audioKey),
                    ((_audios[index].PlayTime == 0.0f) ? _audios[index].AudioLength : _audios[index].PlayTime)
                );

            return audioKey;
        }

        /// <summary>
        /// Instant stops playback of audio.
        /// </summary>
        /// <param name="index">Index of audio.</param>
        /// <param name="audioKey">MD5 hash key of audio.</param>
        /// <returns>Is audio was found and stop is executable.</returns>
        private bool StopInstant(int index, string audioKey)
        {
            if ((index < 0) ||
                (index >= _audios.Count) ||
                (!_audios[index].ContainAudio(audioKey)))
                return false;

            _audios[index].DestroyAudioSource(audioKey);

            return true;
        }


        /// <summary>
        /// Plays audio after delay.
        /// </summary>
        /// <param name="index">Index of audio.</param>
        /// <param name="delay">Playback delay.</param>
        /// <returns>MD5 hash key of audio.</returns>
        private string PlayDelayed(int index, float delay)
        {
            if ((index < 0) ||
                (index >= _audios.Count))
                return string.Empty;

            string audioKey = _audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            _audios[index].PlayDelayed(audioKey, delay);

            if (!_audios[index].Loop)
                RunLater(
                    () => _audios[index].DestroyAudioSource(audioKey),
                    delay + ((_audios[index].PlayTime == 0.0f) ? _audios[index].AudioLength : _audios[index].PlayTime)
                );

            return audioKey;
        }

        /// <summary>
        /// Stops playback of audio after delay.
        /// </summary>
        /// <param name="index">Index of audio.</param>
        /// <param name="delay">Delay of stopping.</param>
        /// <param name="audioKey">MD5 hash key of audio.</param>
        /// <returns>Is audio was found and stop is executable.</returns>
        private bool StopDelayed(int index, float delay, string audioKey)
        {
            if ((index < 0) ||
                (index >= _audios.Count) ||
                (!_audios[index].ContainAudio(audioKey)))
                return false;

            RunLater(() => _audios[index].DestroyAudioSource(audioKey), delay);

            return true;
        }


        /// <summary>
        /// Plays audio instant or after delay (depends on settings).
        /// </summary>
        /// <param name="index">Index of audio.</param>
        /// <param name="playTime">Playback time.</param>
        /// <returns>MD5 hash key of audio.</returns>
        private string Play(int index, float playTime)
        {
            if ((index < 0) ||
                (index >= _audios.Count))
                return string.Empty;

            string audioKey = _audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            _audios[index].Play(audioKey);

            RunLater(
                () => _audios[index].DestroyAudioSource(audioKey),
                _audios[index].PlayDelay + ((playTime < _audios[index].AudioLength) ? playTime : _audios[index].AudioLength)
            );

            return audioKey;
        }

        /// <summary>
        /// Instant plays audio.
        /// </summary>
        /// <param name="index">Index of audio.</param>
        /// <param name="playTime">Playback time.</param>
        /// <returns>MD5 hash key of audio.</returns>
        private string PlayInstant(int index, float playTime)
        {
            if ((index < 0) ||
                (index >= _audios.Count))
                return string.Empty;

            string audioKey = _audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            _audios[index].PlayInstant(audioKey);

            RunLater(
                () => _audios[index].DestroyAudioSource(audioKey),
                ((playTime < _audios[index].AudioLength) ? playTime : _audios[index].AudioLength)
            );

            return audioKey;
        }

        /// <summary>
        /// Plays audio after delay.
        /// </summary>
        /// <param name="index">Index of audio.</param>
        /// <param name="delay">Playback delay.</param>
        /// <param name="playTime">Playback time.</param>
        /// <returns>MD5 hash key of audio.</returns>
        private string PlayDelayed(int index, float delay, float playTime)
        {
            if ((index < 0) ||
                (index >= _audios.Count))
                return string.Empty;

            string audioKey = _audios[index].InitializeAudioSource(gameObject);

            if (audioKey == string.Empty)
                return string.Empty;

            _audios[index].PlayDelayed(audioKey, delay);

            RunLater(
                () => _audios[index].DestroyAudioSource(audioKey),
                delay + ((playTime < _audios[index].AudioLength) ? playTime : _audios[index].AudioLength)
            );

            return audioKey;
        }

        /// <summary>
        /// Run custom method after delay.
        /// </summary>
        /// <param name="method">Custom method.</param>
        /// <param name="waitSeconds">Delay.</param>
        private void RunLater(System.Action method, float waitSeconds)
        {
            if (waitSeconds < 0 || method == null)
            {
                return;
            }
            StartCoroutine(RunLaterCoroutine(method, waitSeconds));
        }

        /// <summary>
        /// Run custom method after delay.
        /// </summary>
        /// <param name="method">Custom method.</param>
        /// <param name="waitSeconds">Delay.</param>
        private IEnumerator RunLaterCoroutine(System.Action method, float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            method();
        }


        /// <summary>
        /// Plays audio instant or after delay (depends on settings).
        /// </summary>
        /// <param name="audioName">Audio name to be played.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>MD5 hash key of audio.</returns>
        public string Play(string audioName, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < _audios.Count; i++)
            {
                if (_audios[i].Name == audioName)
                {
                    return Play(i);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < _containers.Count; i++)
            {
                string audioKey = _containers[i].Play(audioName, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        /// <summary>
        /// Stops playback of audio.
        /// </summary>
        /// <param name="audioName">Audio name to be played.</param>
        /// <param name="audioKey">MD5 hash key of audio.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>Is audio was found and stop is executable.</returns>
        public bool Stop(string audioName, string audioKey, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < _audios.Count; i++)
            {
                if (_audios[i].Name == audioName)
                {
                    return Stop(i, audioKey);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < _containers.Count; i++)
            {
                if (_containers[i].Stop(audioName, audioKey, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Instant plays audio.
        /// </summary>
        /// <param name="audioName">Audio name to be played.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>MD5 hash key of audio.</returns>
        public string PlayInstant(string audioName, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < _audios.Count; i++)
            {
                if (_audios[i].Name == audioName)
                {
                    return PlayInstant(i);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < _containers.Count; i++)
            {
                string audioKey = _containers[i].PlayInstant(audioName, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        /// <summary>
        /// Instant stops playback of audio.
        /// </summary>
        /// <param name="audioName">Audio name to be played.</param>
        /// <param name="audioKey">MD5 hash key of audio.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>Is audio was found and stop is executable.</returns>
        public bool StopInstant(string audioName, string audioKey, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < _audios.Count; i++)
            {
                if (_audios[i].Name == audioName)
                {
                    return StopInstant(i, audioKey);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < _containers.Count; i++)
            {
                if (_containers[i].StopInstant(audioName, audioKey, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Plays audio after delay.
        /// </summary>
        /// <param name="audioName">Audio name to be played.</param>
        /// <param name="delay">Playback delay.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>MD5 hash key of audio.</returns>
        public string PlayDelayed(string audioName, float delay, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < _audios.Count; i++)
            {
                if (_audios[i].Name == audioName)
                {
                    return PlayDelayed(i, delay);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < _containers.Count; i++)
            {
                string audioKey = _containers[i].PlayDelayed(audioName, delay, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        /// <summary>
        /// Stops playback of audio after delay.
        /// </summary>
        /// <param name="audioName">Audio name to be played.</param>
        /// <param name="delay">Delay of stopping.</param>
        /// <param name="audioKey">MD5 hash key of audio.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>Is audio was found and stop is executable.</returns>
        public bool StopDelayed(string audioName, float delay, string audioKey, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < _audios.Count; i++)
            {
                if (_audios[i].Name == audioName)
                {
                    return StopDelayed(i, delay, audioKey);
                }
            }

            if (!searchInNested) return false;

            for (int i = 0; i < _containers.Count; i++)
            {
                if (_containers[i].StopDelayed(audioName, delay, audioKey, !onlyDirect, false))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Plays audio instant or after delay (depends on settings).
        /// </summary>
        /// <param name="audioName">Audio name to be played.</param>
        /// <param name="playTime">Playback time.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>MD5 hash key of audio.</returns>
        public string Play(string audioName, float playTime, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < _audios.Count; i++)
            {
                if (_audios[i].Name == audioName)
                {
                    return Play(i, playTime);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < _containers.Count; i++)
            {
                string audioKey = _containers[i].Play(audioName, playTime, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        /// <summary>
        /// Instant plays audio.
        /// </summary>
        /// <param name="audioName">Audio name to be played.</param>
        /// <param name="playTime">Playback time.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>MD5 hash key of audio.</returns>
        public string PlayInstant(string audioName, float playTime, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < _audios.Count; i++)
            {
                if (_audios[i].Name == audioName)
                {
                    return PlayInstant(i, playTime);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < _containers.Count; i++)
            {
                string audioKey = _containers[i].PlayInstant(audioName, playTime, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        /// <summary>
        /// Plays audio after delay.
        /// </summary>
        /// <param name="audioName">Audio name to be played.</param>
        /// <param name="delay">Playback delay.</param>
        /// <param name="playTime">Playback time.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>MD5 hash key of audio.</returns>
        public string PlayDelayed(string audioName, float delay, float playTime, bool searchInNested = true, bool onlyDirect = true)
        {
            for (int i = 0; i < _audios.Count; i++)
            {
                if (_audios[i].Name == audioName)
                {
                    return PlayDelayed(i, delay, playTime);
                }
            }

            if (!searchInNested) return string.Empty;

            for (int i = 0; i < _containers.Count; i++)
            {
                string audioKey = _containers[i].PlayDelayed(audioName, delay, playTime, !onlyDirect, false);

                if (audioKey != string.Empty)
                    return audioKey;
            }

            return string.Empty;
        }

        /// <summary>
        /// Calculates maximum audibility of all active audios.
        /// </summary>
        /// <param name="source">Position of audio emitter.</param>
        /// <param name="listener">Position of listener.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>Maximum loudness of active audios.</returns>
        public float GetAudibility(Vector3 source, Vector3 listener, bool searchInNested = true, bool onlyDirect = true)
        {
            float noise = 0.0f, noiseT;

            for (int i = 0; i < _audios.Count; i++)
            {
                noiseT = _audios[i].GetAudibility(source, listener);
                if (noiseT > noise) noise = noiseT;
            }

            if (!searchInNested) return noise;

            for (int i = 0; i < _containers.Count; i++)
            {
                noiseT = _containers[i].GetAudibility(source, listener, !onlyDirect, false);
                if (noiseT > noise) noise = noiseT;
            }

            return noise;
        }

        /// <summary>
        /// Calculates maximum audibility of all active audios.
        /// </summary>
        /// <param name="listener">Position of listener.</param>
        /// <param name="attentionToChildPosition">Is position in child containers noted.</param>
        /// <param name="searchInNested">Search in child containers.</param>
        /// <param name="onlyDirect">Only direct childs.</param>
        /// <returns>Maximum loudness of active audios.</returns>
        public float GetAudibility(Vector3 listener, bool attentionToChildPosition = true, bool searchInNested = true, bool onlyDirect = true)
        {
            float noise = 0.0f, noiseT;

            for (int i = 0; i < _audios.Count; i++)
            {
                noiseT = _audios[i].GetAudibility(transform.position, listener);
                if (noiseT > noise) noise = noiseT;
            }

            if (!searchInNested) return noise;

            for (int i = 0; i < _containers.Count; i++)
            {
                if (attentionToChildPosition)
                    noiseT = _containers[i].GetAudibility(listener, attentionToChildPosition, !onlyDirect, false);
                else
                    noiseT = _containers[i].GetAudibility(transform.position, listener, !onlyDirect, false);

                if (noiseT > noise) noise = noiseT;
            }

            return noise;
        }

    }
}
