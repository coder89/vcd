using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VCD.Internal;

namespace VCD
{
    public sealed class VoicePhraseTopics : IVoicePhraseReference
    {
        #region Constructors

        internal VoicePhraseTopics(XElement container)
        {
            // Label attribute
            XAttribute label = container?.Attributes()
                .SingleOrDefault(m => m.Name.LocalName == "Label");
            if (string.IsNullOrWhiteSpace(label?.Value))
            { throw new FormatException(); }
            this.Label = label.Value;

            // Scenario attribute
            XAttribute scenario = container.Attributes()
                .SingleOrDefault(m => m.Name.LocalName == "Scenario");
            if (!string.IsNullOrWhiteSpace(scenario?.Value))
            {
                this.Scenario = scenario.Value.FromEnumString<VoicePhraseTopicScenario>();
            }

            // Subject nodes
            this.Subjects = container.Elements()
                .Where(m => m.Name.LocalName == "Subject")
                .Select(m => m.Value.FromEnumString<VoicePhraseTopicSubject>())
                .ToList();
        }

        internal VoicePhraseTopics(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
            { throw new ArgumentException(nameof(label)); }

            this.Label = label;
            this.Subjects = new List<VoicePhraseTopicSubject>();
        }

        #endregion

        #region Properties

        public string Label { get; }

        public VoicePhraseTopicScenario Scenario { get; set; } = VoicePhraseTopicScenario.Dictation;

        public ICollection<VoicePhraseTopicSubject> Subjects { get; }

        #endregion

        #region Public methods

        public void Validate()
        {
            if (this.Subjects.Distinct().Count() != this.Subjects.Count)
            { throw new FormatException(); }
        }

        #endregion

        #region Internal methods

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("PhraseTopic");
            writer.WriteAttributeString("Label", this.Label);

            if (this.Scenario != VoicePhraseTopicScenario.Dictation)
            {
                writer.WriteAttributeString("Scenario", this.Scenario.ToEnumString());
            }

            foreach (VoicePhraseTopicSubject subject in this.Subjects)
            {
                writer.WriteStartElement("Subject");
                writer.WriteString(subject.ToEnumString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
