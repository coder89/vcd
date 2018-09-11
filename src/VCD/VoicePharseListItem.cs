using System;
using System.Xml;

namespace VCD
{
    public sealed class VoicePharseListItem
    {
        #region Constructors

        public VoicePharseListItem(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            { throw new ArgumentException(nameof(text)); }

            this.Text = text.Trim();
        }

        #endregion

        #region Properties

        public string Text { get; }

        #endregion

        #region Internal methods

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("Item");
            writer.WriteString(this.Text);
            writer.WriteEndElement();
        }

        #endregion
    }
}
