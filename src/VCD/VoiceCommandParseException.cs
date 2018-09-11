using System;

namespace VCD
{
    public sealed class VoiceCommandParseException : AggregateException
    {
        #region Constructors

        public VoiceCommandParseException()
        { }

        public VoiceCommandParseException(Exception e)
            : base(string.Empty, e)
        { }

        #endregion
    }
}