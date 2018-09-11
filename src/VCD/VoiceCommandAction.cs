using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VCD.Internal;

namespace VCD
{
    public sealed class VoiceCommandAction
    {
        #region Constructors

        public VoiceCommandAction()
        {
            this.Type = VoiceCommandActionType.Navigate;
        }

        public VoiceCommandAction(VoiceCommandActionType type, string target)
        {
            this.Type = type;
            this.Target = target;

            if (this.Type == VoiceCommandActionType.Service &&
                string.IsNullOrWhiteSpace(this.Target))
            { throw new ArgumentException(nameof(target)); }
        }

        internal VoiceCommandAction(XElement action)
        {
            this.Type = action.Name.LocalName.FromEnumString<VoiceCommandActionType>();

            XAttribute target = action.Attributes()
                .SingleOrDefault(m => m.Name.LocalName == "Target");
            this.Target = target?.Value;

            if (this.Type == VoiceCommandActionType.Service &&
                string.IsNullOrWhiteSpace(this.Target))
            { throw new FormatException(); }
        }

        #endregion

        #region Properties 

        public VoiceCommandActionType Type { get; }

        public string Target { get; }

        #endregion

        #region Internal methods

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement(this.Type.ToEnumString());

            if (!string.IsNullOrWhiteSpace(this.Target))
            {
                writer.WriteAttributeString("Target", this.Target);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
