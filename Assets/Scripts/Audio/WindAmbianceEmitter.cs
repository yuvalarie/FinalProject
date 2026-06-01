namespace Audio
{
    public class WindAmbianceEmitter : AmbianceEmitter
    {
            private void Start()
            {
                Emitter = AudioManager.Instance.InitializeEventEmitter(FMODEvents.Instance.windAmbiance, gameObject);
                Emitter.Play();
            }
    }
}
