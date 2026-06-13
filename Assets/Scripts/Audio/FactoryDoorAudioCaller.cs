namespace Audio
{
    public class FactoryDoorAudioCaller : AudioCallerBase
    {
        private protected override void SetAudioEventName()
        {
            AudioEventName = AudioEventNames.FactoryDoorOpen;
        }
    }
}
