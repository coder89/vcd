using System.Runtime.Serialization;

namespace VCD
{
    public enum VoiceCommandRequireAppName
    {
        [EnumMember(Value = "BeforePhrase")]
        BeforePhrase,

        [EnumMember(Value = "AfterPhrase")]
        AfterPhrase,

        [EnumMember(Value = "BeforeOrAfterPhrase")]
        BeforeOrAfterPhrase,

        [EnumMember(Value = "ExplicitlySpecified")]
        ExplicitlySpecified
    }
}
