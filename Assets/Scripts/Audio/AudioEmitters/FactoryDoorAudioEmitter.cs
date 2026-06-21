namespace Audio.AudioEmitters
{
    public class FactoryDoorAudioEmitter : AudioEmitterBase
    {
        public override void SetAudioEventName()
        {
            AudioEventName = AudioEventNames.FactoryDoorOpen;
        }
    }
}
