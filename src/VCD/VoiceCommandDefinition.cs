using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace VCD
{
    public sealed class VoiceCommandDefinition
    {
        #region Constants

        public const string MissingRoot = "Missing root of the VCD.";
        public const string MissingCommands = "Command list is empty.";
        public const string AmbiguousityCommands = "Ambiguous command names.";

        public const string DefaultLanguage = "en-us";

        public const string Version = "http://schemas.microsoft.com/voicecommands/1.2";

        public readonly List<VoiceCommandSet> commandSets;

        #endregion

        #region Constructors

        public VoiceCommandDefinition(Stream stream)
        {
            try
            {
                XDocument vcd = XDocument.Load(stream);

                if (vcd.Root?.Name.LocalName != "VoiceCommands")
                { throw new FormatException(MissingRoot); }

                this.commandSets = vcd.Root.Elements()
                    .Where(m => m.Name.LocalName == "CommandSet")
                    .Select(m => new VoiceCommandSet(m))
                    .ToList();

                this.Validate();
            }
            catch (Exception e)
            {
                throw new VoiceCommandParseException(e);
            }
        }

        public VoiceCommandDefinition()
        {
            this.commandSets = new List<VoiceCommandSet>();
        }

        #endregion

        #region Properties

        public IEnumerable<VoiceCommandSet> CommandSets => this.commandSets;

        #endregion

        #region Public methods

        public VoiceCommandSet CreateCommandSet(string name)
        {
            return this.CreateCommandSet(name, DefaultLanguage);
        }

        public VoiceCommandSet CreateCommandSet(string name, string language)
        {
            if (this.CommandSets.Any(m => m.Name == name && m.Language == language))
            { throw new ArgumentException(nameof(name)); }

            VoiceCommandSet commandSet = new VoiceCommandSet(name, language);
            this.commandSets.Add(commandSet);
            return commandSet;
        }

        public void Save(Stream stream)
        {
            XDocument vcd = this.ToXml();
            vcd.Save(stream);
        }

        public Task SaveAsync(Stream stream)
        {
            return this.SaveAsync(stream, CancellationToken.None);
        }

        public async Task SaveAsync(Stream stream, CancellationToken token)
        {
            XDocument vcd = await this.ToXmlAsync(token);
            await Task.Run(() =>
            {
                vcd.Save(stream);
            });
        }

        public void Validate()
        {
            if (this.commandSets.Count == 0)
            { throw new FormatException(MissingCommands); }

            if (this.commandSets.Select(m => m.Name + "_" + m.Language).Distinct().Count() != this.commandSets.Count)
            { throw new FormatException(AmbiguousityCommands); }

            foreach (VoiceCommandSet commandSet in this.CommandSets)
            {
                commandSet.Validate();
            }
        }

        public override string ToString()
        {
            XDocument vcd = this.ToXml();
            return vcd.ToString();
        }

        #endregion

        #region Private methods

        private XDocument ToXml()
        {
            XDocument vcd = new XDocument();

            using (XmlWriter writer = vcd.CreateWriter())
            {
                writer.WriteStartElement("VoiceCommands", Version);

                foreach (VoiceCommandSet commandSet in this.CommandSets)
                {
                    commandSet.Save(writer);
                }

                writer.WriteEndElement();
            }

            return vcd;
        }

        private async Task<XDocument> ToXmlAsync(CancellationToken token)
        {
            return await Task<XDocument>.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                XDocument vcd = new XDocument();

                using (XmlWriter writer = vcd.CreateWriter())
                {
                    token.ThrowIfCancellationRequested();
                    writer.WriteStartElement("VoiceCommands", Version);

                    foreach (VoiceCommandSet commandSet in this.CommandSets)
                    {
                        token.ThrowIfCancellationRequested();
                        commandSet.Save(writer);
                        token.ThrowIfCancellationRequested();
                    }

                    token.ThrowIfCancellationRequested();
                    writer.WriteEndElement();
                }

                token.ThrowIfCancellationRequested();

                return vcd;
            });
        }

        #endregion
    }
}
