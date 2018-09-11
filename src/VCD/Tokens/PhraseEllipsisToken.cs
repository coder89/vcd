namespace VCD.Tokens
{
    public sealed class PhraseEllipsisToken : IPhraseToken
    {
        #region Constructors

        internal PhraseEllipsisToken()
        { }

        #endregion

        #region Properties

        public PhraseTokenType Type => PhraseTokenType.Ellipsis;

        #endregion
    }
}
