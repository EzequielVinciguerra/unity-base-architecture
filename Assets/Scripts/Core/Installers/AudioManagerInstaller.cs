using Core.Audio;
using Core.Events;
using Core.Services;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Installers
{
    public class AudioManagerInstaller : Installer
    {
        [Header("Mixer")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string musicParam = "MusicVolume";
        [SerializeField] private string sfxParam   = "SfxVolume";

        private IAudioManager _instance;

        public override void Install(ServiceLocator locator)
        {
            // Dependencias
            var bus = locator.Get<IEventBus>();
            if (bus == null)
            {
                Debug.LogError("[AudioManagerInstaller] IEventBus no encontrado. ¿Instalaste el EventBus antes?");
                return;
            }
            if (mixer == null)
            {
                Debug.LogError("[AudioManagerInstaller] AudioMixer no asignado.");
                return;
            }

            _instance ??= new AudioManager(bus, mixer, musicParam, sfxParam);
            locator.Register<IAudioManager>(_instance);
        }

        public override void Uninstall(ServiceLocator locator)
        {
            locator.Unregister<IAudioManager>();
            (_instance as AudioManager)?.Dispose();
            _instance = null;
        }
    }
}
