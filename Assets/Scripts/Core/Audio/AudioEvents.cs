using Core.Events;

namespace Core.Audio
{
    public struct MusicVolumeChanged : IEvent
    {
        public float Value;

        public MusicVolumeChanged(float value)
        {
            Value = value;
        }
    }

    public struct SfxVolumeChanged : IEvent
    {
        public float Value;

        public SfxVolumeChanged(float value)
        {
            Value = value;
        }
    }
}
