using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VCD.Tokens;

namespace VCD
{
    public abstract class VoiceCommandTokenizedText
    {
        #region Constants

        public const string SquareL = "\\[";
        public const string SquareR = "\\]";
        public const string RoundL = "\\(";
        public const string RoundR = "\\)";
        public const string ClosureL = "\\{";
        public const string ClosureR = "\\}";
        public const string NoBracketsGroup = "[^" + SquareL + SquareR + RoundL + RoundR + ClosureL + ClosureR + "]+";
        private const string OptionalGroup = "(" + SquareL + NoBracketsGroup + SquareR + ")";
        private const string TopicGroup = "(" + ClosureL + NoBracketsGroup + ClosureR + ")";
        private const string InvalidGroup = "(" + RoundL + NoBracketsGroup + RoundR + ")";
        private const string Expression = OptionalGroup + "|" + TopicGroup + "|" + InvalidGroup + "|(" + NoBracketsGroup + ")|(" + SquareL + ")|(" + SquareR + ")|(" + RoundL + ")|(" + RoundR + ")|(" + ClosureL + ")|(" + ClosureR + ")";

        #endregion

        #region Fields

        private readonly List<IPhraseToken> tokenList = new List<IPhraseToken>();

        #endregion

        #region Constructors

        internal VoiceCommandTokenizedText(XElement command, VoiceCommand parent)
        {
            this.Parent = parent;

            if (string.IsNullOrWhiteSpace(command.Value))
            { throw new FormatException(); }
            this.Text = command.Value;

            this.Tokenize();
            this.Tokens = new ReadOnlyCollection<IPhraseToken>(this.tokenList);
        }

        internal VoiceCommandTokenizedText(string text, VoiceCommand parent)
        {
            this.Parent = parent;

            if (string.IsNullOrWhiteSpace(text))
            { throw new ArgumentException(nameof(text)); }
            this.Text = text;

            this.Tokenize();
            this.Tokens = new ReadOnlyCollection<IPhraseToken>(this.tokenList);
        }

        #endregion

        #region Properties

        public VoiceCommand Parent { get; }

        public string Text { get; }

        public IReadOnlyCollection<IPhraseToken> Tokens { get; }

        #endregion

        #region Public methods

        public virtual void Validate()
        {
            if (this.Tokens
                .OfType<PhraseReferenceToken>()
                .Cast<PhraseReferenceToken>()
                .Any(m => m.Reference == null))
            { throw new FormatException(); }
        }

        #endregion

        #region Private methods

        private void Tokenize()
        {
            MatchCollection matchCollection = Regex.Matches(this.Text, Expression);

            foreach (Match match in matchCollection)
            {
                string tokenString = match.Groups[0].Value.Trim();
                if (!string.IsNullOrWhiteSpace(tokenString))
                {
                    if (tokenString.StartsWith("{"))
                    {
                        if (tokenString.EndsWith("}"))
                        {
                            if (tokenString == "{builtin:AppName}")
                            { this.tokenList.Add(new PhraseAppNameToken()); }
                            else if (tokenString == "{*}")
                            { this.tokenList.Add(new PhraseEllipsisToken()); }
                            else
                            { this.tokenList.Add(new PhraseReferenceToken(tokenString.Substring(1, tokenString.Length - 2), this)); }
                        }
                        else
                        { throw new FormatException(); }
                    }
                    else if (tokenString.StartsWith("["))
                    {
                        if (tokenString.EndsWith("]"))
                        { this.tokenList.Add(new PhraseTextToken(tokenString.Substring(1, tokenString.Length - 2), true)); }
                        else
                        { throw new FormatException(); }
                    }
                    else if (tokenString.StartsWith("{") ||
                             tokenString.StartsWith("}") ||
                             tokenString.StartsWith("[") ||
                             tokenString.StartsWith("]") ||
                             tokenString.StartsWith("(") ||
                             tokenString.StartsWith(")"))
                    { throw new FormatException(); }
                    else
                    { this.tokenList.Add(new PhraseTextToken(tokenString)); }
                }
            }
        }

        #endregion
    }
}
