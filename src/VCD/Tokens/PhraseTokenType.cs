namespace VCD.Tokens
{
    public enum PhraseTokenType
    {
        /// <summary>
        /// The token is is of type <see cref="PhraseTextToken" />.
        /// </summary>
        Text,

        /// <summary>
        /// The token is is of type <see cref="PhraseReferenceToken" />.
        /// </summary>
        Reference,

        /// <summary>
        /// The token is is of type <see cref="PhraseAppNameToken" />.
        /// </summary>
        AppName,

        /// <summary>
        /// The token is is of type <see cref="PhraseEllipsisToken" />.
        /// </summary>
        Ellipsis
    }
}
