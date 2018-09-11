namespace VCD.Tokens
{
    public sealed class PhraseAppNameToken : IPhraseToken
    {
        #region Constructors

        internal PhraseAppNameToken() { }

        #endregion

        #region Properties

        public PhraseTokenType Type => PhraseTokenType.AppName;

        #endregion
    }
}
