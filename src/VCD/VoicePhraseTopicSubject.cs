using System.Runtime.Serialization;

namespace VCD
{
    public enum VoicePhraseTopicSubject
    {
        [EnumMember(Value = "Date/Time")]
        DateTime,

        [EnumMember(Value = "Addresses")]
        Addresses,

        [EnumMember(Value = "City/State")]
        CityOrState,

        [EnumMember(Value = "Person Names")]
        PersonNames,

        [EnumMember(Value = "Movies")]
        Movies,

        [EnumMember(Value = "Music")]
        Music,

        [EnumMember(Value = "Phone Number")]
        PhoneNumber
    }
}
