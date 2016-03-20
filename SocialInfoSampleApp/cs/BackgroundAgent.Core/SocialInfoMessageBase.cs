using System;
using Windows.Foundation.Collections;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    /// <summary>
    /// A base class implementation for an ISocialInfoMessage to implement the required
    /// properties.
    /// </summary>
    public abstract class SocialInfoMessageBase : ISocialInfoMessage
    {
        /// <summary>
        /// The type of operation that will be performed
        /// </summary>
        protected OperationType mOperationType;

        /// <summary>
        /// The packed major and minor version of the protocol
        /// </summary>
        protected UInt32 mVersion;

        protected SocialInfoMessageBase(UInt32 version)
        {
            Version = version;
        }

        public UInt32 MajorVersion
        {
            get
            {
                return (mVersion >> 16);
            }
        }

        public UInt32 MinorVersion
        {
            get
            {
                return (mVersion & 0x00FF);
            }
        }

        public OperationType OperationType
        {
            get
            {
                return this.mOperationType;
            }

            protected set
            {
                this.mOperationType = value;
            }
        }

        public UInt32 Version
        {
            get
            {
                return this.mVersion;
            }

            protected set
            {
                this.mVersion = value;
            }
        }

        /// <summary>
        /// Serializes a message back into a ValueSet.
        /// </summary>
        /// <remarks>
        /// Not all message types need the ability to serialize themselves.
        /// This method should be overriden in the objects that require serialization.
        /// </remarks>
        public virtual ValueSet Serialize()
        {
            ValueSet fields = new ValueSet();

            fields.Add(
                MessageBodyPayloadField.Type.GetName(),
                (UInt32)OperationType);

            fields.Add(
                MessageBodyPayloadField.Version.GetName(),
                Version);

            return fields;
        }
    }
}
