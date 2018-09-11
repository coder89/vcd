using System.Runtime.Serialization;

namespace VCD
{
    public enum VoicePhraseTopicScenario
    {
        [EnumMember(Value = "Natural Language")]
        NaturalLanguage,

        [EnumMember(Value = "Search")]
        Search,

        [EnumMember(Value = "Short Message")]
        ShortMessage,

        [EnumMember(Value = "Dictation")]
        Dictation,

        [EnumMember(Value = "Commands")]
        Commands,

        [EnumMember(Value = "Form Filling")]
        FormFilling,
    }
}
