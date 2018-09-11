using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VCD.Tokens;

namespace VCD
{
    public sealed class VoiceCommand
    {
        #region Fields

        private readonly List<VoiceCommandPhrase> phrases;

        #endregion

        #region Constructors

        internal VoiceCommand(XElement command, VoiceCommandSet parent)
        {
            this.Parent = parent;

            // Name attribute
            XAttribute name = command.Attributes()
                .SingleOrDefault(m => m.Name.LocalName == "Name");
            if (string.IsNullOrWhiteSpace(name?.Value))
            { throw new FormatException(); }
            this.Name = name.Value;

            // Example node
            this.Example = new VoiceCommandExample(command);

            // ListenFor nodes
            this.phrases = new List<VoiceCommandPhrase>(command.Elements()
                .Where(m => m.Name.LocalName == "ListenFor")
                .Select(m => new VoiceCommandPhrase(m, this)));

            // Feedback node
            XElement feedback = command.Elements()
                .Single(m => m.Name.LocalName == "Feedback");
            this.Feedback = new VoiceCommandFeedback(feedback, this);

            // Navigate or VoiceCommandService node
            XElement action = command.Elements()
                .Single(m => m.Name.LocalName == "Navigate" || m.Name.LocalName == "VoiceCommandService");
            this.Action = new VoiceCommandAction(action);
        }

        internal VoiceCommand(string name, VoiceCommandSet parent)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new FormatException(); }

            this.Parent = parent;
            this.Name = name;
            this.phrases = new List<VoiceCommandPhrase>();
        }

        #endregion

        #region Properties

        public VoiceCommandSet Parent { get; }

        public string Name { get; }

        public VoiceCommandExample Example { get; set; }

        public IEnumerable<VoiceCommandPhrase> Phrases => this.phrases;

        public VoiceCommandFeedback Feedback { get; private set; }

        public VoiceCommandAction Action { get; set; }

        #endregion

        #region Public methods

        public VoiceCommandPhrase CreateCommandPhrase(string phrase)
        {
            return this.CreateCommandPhrase(phrase, VoiceCommandRequireAppName.BeforePhrase);
        }

        public VoiceCommandPhrase CreateCommandPhrase(string phrase, VoiceCommandRequireAppName requireAppName)
        {
            if (this.phrases.Any(m => m.Text == phrase))
            { throw new ArgumentException(nameof(phrase)); }

            VoiceCommandPhrase result = new VoiceCommandPhrase(phrase, requireAppName, this);
            this.phrases.Add(result);
            return result;
        }

        public void SetFeedback(string text)
        {
            this.Feedback = new VoiceCommandFeedback(text, this);
        }

        public void Validate()
        {
            if (this.Example == null)
            { throw new FormatException(); }

            if (this.Action == null)
            { throw new FormatException(); }

            if (this.Feedback == null)
            { throw new FormatException(); }

            if (this.phrases.Count == 0)
            { throw new FormatException(); }

            if (this.Phrases
                .SelectMany(m => m.Tokens)
                .OfType<PhraseReferenceToken>()
                .Cast<PhraseReferenceToken>()
                .Any(m => m.Reference == null))
            { throw new FormatException(); }

            this.Feedback.Validate();

            var specialTokens = this.Feedback.Tokens
                .OfType<PhraseReferenceToken>()
                .Cast<PhraseReferenceToken>()
                .OrderBy(m => m.Reference.Label);

            foreach (VoiceCommandPhrase phrase in this.Phrases)
            {
                phrase.Validate();

                var phraseSpecialTokens = phrase.Tokens
                    .OfType<PhraseReferenceToken>()
                    .Cast<PhraseReferenceToken>()
                    .OrderBy(m => m.Reference.Label);

                // All special tokens from Feedback must appear in the phrase
                if (!specialTokens.All(m => phraseSpecialTokens.Any(n => n.Reference.Label == m.Reference.Label)))
                { throw new FormatException(); }
            }
        }

        #endregion

        #region Internal methods

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("Command");
            writer.WriteAttributeString("Name", this.Name);

            // Example node
            this.Example.Save(writer);

            // ListenFor nodes
            foreach (VoiceCommandPhrase phrase in this.Phrases)
            {
                phrase.Save(writer);
            }

            // Feedback node
            this.Feedback.Save(writer);

            // Navigate or VoiceCommandService node
            this.Action.Save(writer);

            writer.WriteEndElement();
        }

        #endregion
    }
}
