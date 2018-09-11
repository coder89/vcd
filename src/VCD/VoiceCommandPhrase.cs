using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VCD.Internal;
using VCD.Tokens;

namespace VCD
{
    public sealed class VoiceCommandPhrase : VoiceCommandTokenizedText
    {
        #region Constructors

        internal VoiceCommandPhrase(XElement command, VoiceCommand parent)
            : base(command, parent)
        {
            // Scenario attribute
            XAttribute requireAppName = command.Attributes()
                .SingleOrDefault(m => m.Name.LocalName == "RequireAppName");
            if (!string.IsNullOrWhiteSpace(requireAppName?.Value))
            {
                this.RequireAppName = requireAppName.Value.FromEnumString<VoiceCommandRequireAppName>();
            }
            else
            {
                this.RequireAppName = VoiceCommandRequireAppName.BeforePhrase;
            }
        }

        internal VoiceCommandPhrase(string text, VoiceCommandRequireAppName requireAppName, VoiceCommand parent)
            : base(text, parent)
        {
            this.RequireAppName = requireAppName;
            this.ValidateInternal();
        }

        #endregion

        #region Properties

        public VoiceCommandRequireAppName RequireAppName { get; }

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
            writer.WriteStartElement("ListenFor");

            if (this.RequireAppName != VoiceCommandRequireAppName.BeforePhrase)
            {
                writer.WriteAttributeString("RequireAppName", this.RequireAppName.ToEnumString());
            }

            writer.WriteString(this.Text);
            writer.WriteEndElement();
        }

        #endregion

        #region Private methods

        private void ValidateInternal()
        {
            // Should not contain only optional tokens
            if (this.Tokens.Where(m =>
                    !(m.Type == PhraseTokenType.Text && ((PhraseTextToken)m).IsOptional) &&
                      m.Type != PhraseTokenType.AppName).Count() == 0)
            { throw new FormatException(); }

            if (this.RequireAppName == VoiceCommandRequireAppName.ExplicitlySpecified)
            {
                // Should contain explicit app name
                if (!this.Tokens.Any(m => m.Type == PhraseTokenType.AppName))
                { throw new FormatException(); }

                // Should contain explicit text before explicit app name
                if (this.Tokens.TakeWhile(m => m.Type != PhraseTokenType.AppName)
                               .Where(m => m.Type != PhraseTokenType.Text || !((PhraseTextToken)m).IsOptional)
                               .Count() == 0)
                { throw new FormatException(); }
            }
        }

        #endregion
    }
}
