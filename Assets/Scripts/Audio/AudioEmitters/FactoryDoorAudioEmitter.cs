namespace Audio.AudioEmitters
{
    public class FactoryDoorAudioEmitter : AudioEmitterBase
    {
        protected override void SetAudioEventName()
        {
            AudioEventName = AudioEventNames.FactoryDoorOpen;
        }
    }
}
