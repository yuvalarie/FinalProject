using Audio;

namespace Audio.Voice
{
    public enum CharacterType
    {
        Helda,
        Boss,
    }

    public static class VoiceProfileFactory
    {
        public static VoiceProfile Get(CharacterType type)
        {
            return type switch
            {
                CharacterType.Helda => new VoiceProfile(AudioEventNames.CharacterVoice, new[] { new FmodParameter { name = "VoicePitch", value = 1f } }),
                CharacterType.Boss  => new VoiceProfile(AudioEventNames.CharacterVoice, new[] { new FmodParameter { name = "VoicePitch", value = -1f } }),
                _                   => new VoiceProfile(AudioEventNames.CharacterVoice, new[] { new FmodParameter { name = "VoicePitch", value = 0f } })
            };
        }
    }
}
