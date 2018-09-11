using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VCD.Tokens;

namespace VCD
{
    public sealed class VoiceCommandSet
    {
        #region Fields

        private readonly List<VoiceCommand> commands;
        private readonly List<VoicePhraseList> phraseLists;
        private readonly List<VoicePhraseTopics> phraseTopics;

        #endregion

        #region Constructors

        internal VoiceCommandSet(XElement commandSet)
        {
            // Language attribute
            XAttribute language = commandSet?.Attribute(XName.Get("lang", XNamespace.Xml.NamespaceName));
            if (string.IsNullOrWhiteSpace(language?.Value))
            { throw new FormatException(); }
            this.Language = language.Value;

            // Name attribute
            XAttribute name = commandSet.Attributes()
                .SingleOrDefault(m => m.Name.LocalName == "Name");
            this.Name = name?.Value;
            if (this.Name?.EndsWith($"_{this.Language}") ?? false)
            {
                this.Name = this.Name.Substring(0, this.Name.Length - 1 - this.Language.Length);
            }

            // AppName node
            XElement appName = commandSet.Elements()
                .SingleOrDefault(m => m.Name.LocalName == "AppName");
            if (string.IsNullOrWhiteSpace(appName?.Value))
            { throw new FormatException(); }
            this.AppName = appName.Value.Trim();

            // Example node
            this.Example = new VoiceCommandExample(commandSet);

            // Command nodes
            this.commands = commandSet.Elements()
                .Where(m => m.Name.LocalName == "Command")
                .Select(m => new VoiceCommand(m, this))
                .ToList();

            // PhraseList nodes
            this.phraseLists = commandSet.Elements()
                .Where(m => m.Name.LocalName == "PhraseList")
                .Select(m => new VoicePhraseList(m))
                .ToList();

            // PhraseTopic nodes
            this.phraseTopics = commandSet.Elements()
                .Where(m => m.Name.LocalName == "PhraseTopic")
                .Select(m => new VoicePhraseTopics(m))
                .ToList();
        }

        internal VoiceCommandSet(string name, string language)
        {
            if (string.IsNullOrWhiteSpace(name))
            { throw new ArgumentException(nameof(name)); }

            if (string.IsNullOrWhiteSpace(language))
            { throw new ArgumentException(nameof(language)); }

            this.Name = name;
            this.Language = language;
            this.commands = new List<VoiceCommand>();
            this.phraseLists = new List<VoicePhraseList>();
            this.phraseTopics = new List<VoicePhraseTopics>();
        }

        #endregion

        #region Properties

        public string Name { get; }

        public string Language { get; }

        public string AppName { get; set; }

        public VoiceCommandExample Example { get; set; }

        public IEnumerable<VoiceCommand> Commands => this.commands;

        public IEnumerable<VoicePhraseList> PhraseLists => this.phraseLists;

        public IEnumerable<VoicePhraseTopics> PhraseTopics => this.phraseTopics;

        #endregion

        #region Public methods

        public VoiceCommand CreateCommand(string name)
        {
            if (this.Commands.Any(m => m.Name == name))
            { throw new ArgumentException(nameof(name)); }

            VoiceCommand result = new VoiceCommand(name, this);
            this.commands.Add(result);
            return result;
        }

        public VoicePhraseList CreateVoicePharseList(string label)
        {
            if (this.PhraseLists.Any(m => m.Label == label))
            { throw new ArgumentException(nameof(label)); }

            VoicePhraseList result = new VoicePhraseList(label);
            this.phraseLists.Add(result);
            return result;
        }

        public VoicePhraseTopics CreateVoicePhraseTopic(string label)
        {
            if (this.PhraseTopics.Any(m => m.Label == label))
            { throw new ArgumentException(nameof(label)); }

            VoicePhraseTopics result = new VoicePhraseTopics(label);
            this.phraseTopics.Add(result);
            return result;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.AppName))
            { throw new FormatException(); }

            if (this.Example == null)
            { throw new FormatException(); }

            if (this.commands.Count == 0)
            { throw new FormatException(); }
            if (this.commands.Select(m => m.Name).Distinct().Count() != this.commands.Count)
            { throw new FormatException(); }

            // IDs cannot be ambiguous
            if (this.phraseLists.Select(m => m.Label).Distinct().Count() != this.phraseLists.Count)
            { throw new FormatException(); }
            if (this.phraseTopics.Select(m => m.Label).Distinct().Count() != this.phraseTopics.Count)
            { throw new FormatException(); }
            if (this.phraseTopics.Any(m => this.phraseLists.Any(n => n.Label == m.Label)))
            { throw new FormatException(); }

            foreach (VoicePhraseList phraseList in this.PhraseLists)
            {
                phraseList.Validate();
            }

            foreach (VoicePhraseTopics phraseTopic in this.PhraseTopics)
            {
                phraseTopic.Validate();
            }

            foreach (VoiceCommand command in this.Commands)
            {
                command.Validate();
            }


            IEnumerable<IVoicePhraseReference> references = this.phraseLists.Cast<IVoicePhraseReference>().Union(this.phraseTopics.Cast<IVoicePhraseReference>());
            if (references.Any(m => true))
            {
                var tmp = references.ToArray();
                PhraseReferenceToken[] tokens = this.Commands
                    .SelectMany(m => m.Phrases)
                    .SelectMany(m => m.Tokens)
                    .OfType<PhraseReferenceToken>()
                    .ToArray();

                if (!references.All(m => tokens.Any(n => n.Reference == m)))
                { throw new FormatException(); }
            }
        }

        #endregion

        #region Internal methods

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("CommandSet");
            writer.WriteAttributeString("lang", XNamespace.Xml.NamespaceName, this.Language);
            writer.WriteAttributeString("Name", $"{this.Name}_{this.Language}");

            writer.WriteStartElement("AppName");
            writer.WriteString(this.AppName);
            writer.WriteEndElement();

            this.Example.Save(writer);

            foreach (VoiceCommand command in this.Commands)
            {
                command.Save(writer);
            }

            foreach (VoicePhraseList parseLists in this.PhraseLists)
            {
                parseLists.Save(writer);
            }

            foreach (VoicePhraseTopics phraseTopic in this.PhraseTopics)
            {
                phraseTopic.Save(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
