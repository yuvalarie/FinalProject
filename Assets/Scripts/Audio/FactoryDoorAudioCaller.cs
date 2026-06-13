namespace Audio
{
    public class FactoryDoorAudioCaller : AudioCallerBase
    {
        protected override void SetAudioEventName()
        {
            AudioEventName = AudioEventNames.FactoryDoorOpen;
        }
    }
}
