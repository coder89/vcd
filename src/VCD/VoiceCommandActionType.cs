using System.Runtime.Serialization;

namespace VCD
{
    public enum VoiceCommandActionType
    {
        [EnumMember(Value = "Navigate")]
        Navigate,

        [EnumMember(Value = "VoiceCommandService")]
        Service
    }
}
