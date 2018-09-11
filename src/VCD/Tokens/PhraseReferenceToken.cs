using System.Linq;

namespace VCD.Tokens
{
    public sealed class PhraseReferenceToken : IPhraseToken
    {
        #region Fields

        private readonly string label;

        #endregion

        #region Constructors

        internal PhraseReferenceToken(string label, VoiceCommandTokenizedText phrase)
        {
            this.label = label.Trim();
            this.Phrase = phrase;
        }

        #endregion

        #region Properties

        public VoiceCommandTokenizedText Phrase { get; }

        public PhraseTokenType Type => PhraseTokenType.Reference;

        public IVoicePhraseReference Reference =>
            this.Phrase.Parent.Parent.PhraseLists.Cast<IVoicePhraseReference>()
            .Union(this.Phrase.Parent.Parent.PhraseTopics.Cast<IVoicePhraseReference>())
            .SingleOrDefault(m => m.Label == this.label);

        #endregion
    }
}
