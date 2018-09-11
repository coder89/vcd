using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace VCD
{
    public sealed class VoiceCommandExample
    {
        #region Constructors

        internal VoiceCommandExample(XElement container)
        {
            XElement example = container?.Elements()
                .SingleOrDefault(m => m.Name.LocalName == "Example");
            if (string.IsNullOrWhiteSpace(example?.Value))
            { throw new FormatException(); }

            this.Text = example.Value;
        }

        public VoiceCommandExample(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            { throw new ArgumentException(nameof(text)); }

            this.Text = text;
        }

        #endregion

        #region Properties

        public string Text { get; }

        #endregion

        #region Internal methods

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("Example");
            writer.WriteString(this.Text);
            writer.WriteEndElement();
        }

        #endregion
    }
}
