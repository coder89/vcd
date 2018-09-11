namespace VCD.Tokens
{
    public sealed class PhraseTextToken : IPhraseToken
    {
        #region Constructors

        internal PhraseTextToken(string value)
        {
            this.Value = value;
        }

        internal PhraseTextToken(string value, bool isOptional)
        {
            this.Value = value;
            this.IsOptional = isOptional;
        }

        #endregion

        #region Properties

        public string Value { get; }

        public bool IsOptional { get; }

        public PhraseTokenType Type => PhraseTokenType.Text;

        #endregion
    }
}
