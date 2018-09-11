using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace VCD
{
    public sealed class VoicePhraseList : IVoicePhraseReference
    {
        #region Constructors

        internal VoicePhraseList(XElement container)
        {
            // Label attribute
            XAttribute label = container?.Attributes()
                .SingleOrDefault(m => m.Name.LocalName == "Label");
            if (string.IsNullOrWhiteSpace(label?.Value))
            { throw new FormatException(); }
            this.Label = label.Value;

            // Item nodes
            this.Items = container.Elements()
                .Where(m => m.Name.LocalName == "Item")
                .Select(m => new VoicePharseListItem(m.Value))
                .ToList();
        }

        internal VoicePhraseList(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) { throw new ArgumentException(nameof(label)); }

            this.Label = label;
            this.Items = new List<VoicePharseListItem>();
        }

        #endregion

        #region Properties

        public string Label { get; private set; }

        public ICollection<VoicePharseListItem> Items { get; }

        #endregion

        #region Public methods

        public void Validate()
        {
            if (this.Items.Count == 0)
            { throw new FormatException(); }

            if (this.Items.Select(m => m.Text).Distinct().Count() != this.Items.Count)
            { throw new FormatException(); }
        }

        #endregion

        #region Internal methods

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("PhraseList");
            writer.WriteAttributeString("Label", this.Label);

            foreach (VoicePharseListItem item in this.Items)
            {
                item.Save(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
