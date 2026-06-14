using Audio.AudioEmitters;

namespace Audio
{
    public class FactoryDoorAudioEmitter : AudioEmitterBase
    {
        protected override void SetAudioEventName()
        {
            AudioEventName = AudioEventNames.FactoryDoorOpen;
        }
    }
}
