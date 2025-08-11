namespace Core.Audio
{
    public interface IAudioManager
    {
        float MusicVolume { get; } // 0..1
        float SfxVolume { get; }   // 0..1

        void SetMusicVolume(float value01);
        void SetSfxVolume(float value01);

        void PlayMusic(UnityEngine.AudioClip clip, bool loop = true, float fadeSeconds = 0f);
        void StopMusic(float fadeSeconds = 0f);

        void PlaySfx(UnityEngine.AudioClip clip, float volume01 = 1f);
    }
}
