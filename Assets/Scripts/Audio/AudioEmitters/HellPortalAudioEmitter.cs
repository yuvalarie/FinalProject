namespace Audio.AudioEmitters
{
    public class HellPortalAudioEmitter : AudioEmitterBase
    {
        public override void SetAudioEventName()
        {
            AudioEventName = AudioEventNames.HellPortal;
        }

        public void PlayHellAudio()
        {
            StartAudio(("Indicator", 0f));
        }
        
        public void StopHellAudio()
        {
            SetParameter("Indicator", 1f);
            // AudioInstance.release();
        }
    }
}
