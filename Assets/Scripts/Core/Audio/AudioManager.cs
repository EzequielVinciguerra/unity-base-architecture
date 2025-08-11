using Core.Events;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio
{
    public class AudioManager : IAudioManager
    {
        private readonly IEventBus _eventBus;
        private readonly AudioMixer _mixer;
        private readonly string _musicParam;
        private readonly string _sfxParam;

        private readonly GameObject _rootGO;
        private readonly AudioSource _musicSource;
        private readonly AudioSource _sfxSource;

        public float MusicVolume { get; private set; } = 1f;
        public float SfxVolume { get; private set; } = 1f;

        // Fade state
        private bool _fadingOut;
        private bool _fadingIn;
        private float _fadeTime;
        private float _fadeTimer;
        private AudioClip _nextClip;

        public AudioManager(IEventBus eventBus, AudioMixer mixer, string musicParam, string sfxParam)
        {
            _eventBus = eventBus;
            _mixer = mixer;
            _musicParam = musicParam;
            _sfxParam = sfxParam;

            _rootGO = new GameObject("[AudioManager]");
            Object.DontDestroyOnLoad(_rootGO);

            _rootGO.AddComponent<AudioManagerUpdater>().Init(this);

            _musicSource = _rootGO.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;

            _sfxSource = _rootGO.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
            
            ApplyMixerVolume(_musicParam, MusicVolume);
            ApplyMixerVolume(_sfxParam, SfxVolume);
        }

        public void SetMusicVolume(float volumValue)
        {
            MusicVolume = Mathf.Clamp01(volumValue);
            ApplyMixerVolume(_musicParam, MusicVolume);
            PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
            _eventBus.Publish(new MusicVolumeChanged(MusicVolume));
        }

        public void SetSfxVolume(float volumValue)
        {
            SfxVolume = Mathf.Clamp01(volumValue);
            ApplyMixerVolume(_sfxParam, SfxVolume);
            PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
            _eventBus.Publish(new SfxVolumeChanged (SfxVolume ));
        }

        public void PlayMusic(AudioClip clip, bool loop = true, float fadeSeconds = 0f)
        {
            if (fadeSeconds > 0f && _musicSource.isPlaying)
            {
                _fadingOut = true;
                _fadingIn = false;
                _fadeTimer = 0f;
                _fadeTime = fadeSeconds;
                _nextClip = clip;
                return;
            }

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = 1f;
            _musicSource.Play();
        }

        public void StopMusic(float fadeSeconds = 0f)
        {
            if (fadeSeconds > 0f && _musicSource.isPlaying)
            {
                _fadingOut = true;
                _fadingIn = false;
                _fadeTimer = 0f;
                _fadeTime = fadeSeconds;
                _nextClip = null;
                return;
            }

            _musicSource.Stop();
            _musicSource.clip = null;
        }

        public void PlaySfx(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        private void ApplyMixerVolume(string param, float value)
        {
            var dB = (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
            _mixer.SetFloat(param, dB);
        }

        public void Update(float deltaTime)
        {
            if (_fadingOut)
            {
                _fadeTimer += deltaTime;
                _musicSource.volume = Mathf.Lerp(1f, 0f, _fadeTimer / _fadeTime);

                if (_fadeTimer >= _fadeTime)
                {
                    _musicSource.Stop();
                    _musicSource.volume = 0f;
                    _fadingOut = false;

                    if (_nextClip != null)
                    {
                        _musicSource.clip = _nextClip;
                        _musicSource.Play();
                        _fadingIn = true;
                        _fadeTimer = 0f;
                    }
                }
            }
            else if (_fadingIn)
            {
                _fadeTimer += deltaTime;
                _musicSource.volume = Mathf.Lerp(0f, 1f, _fadeTimer / _fadeTime);

                if (_fadeTimer >= _fadeTime)
                {
                    _musicSource.volume = 1f;
                    _fadingIn = false;
                }
            }
        }

        public void Dispose()
        {
            if (_rootGO != null)
                Object.Destroy(_rootGO);
        }
    }

    // This MonoBehaviour calls AudioManager.Update()
    public class AudioManagerUpdater : MonoBehaviour
    {
        private AudioManager _manager;

        public void Init(AudioManager manager) => _manager = manager;

        private void Update()
        {
            _manager?.Update(Time.unscaledDeltaTime);
        }
    }
}