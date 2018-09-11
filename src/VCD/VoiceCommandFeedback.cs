using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VCD.Tokens;

namespace VCD
{
    public sealed class VoiceCommandFeedback : VoiceCommandTokenizedText
    {
        #region Constructors

        internal VoiceCommandFeedback(XElement feedback, VoiceCommand parent)
            : base(feedback, parent)
        { }

        internal VoiceCommandFeedback(string text, VoiceCommand parent)
            : base(text, parent)
        { }

        #endregion

        #region Public methods

        public override void Validate()
        {
            base.Validate();
            this.ValidateInternal();
        }

        #endregion

        #region Internal methods

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("Feedback");
            writer.WriteString(this.Text);
            writer.WriteEndElement();
        }

        #endregion

        #region Private methods

        private void ValidateInternal()
        {
            // Should not contain only optional tokens
            if (this.Tokens.Any(m => m.Type == PhraseTokenType.Ellipsis))
            { throw new FormatException(); }
        }

        #endregion
    }
}
