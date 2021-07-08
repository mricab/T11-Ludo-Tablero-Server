using System;
using System.Runtime.Serialization;

namespace LudoProtocol
{
    [Serializable]
    public class LPackage : ISerializable
    {
        public byte action;
        public string[] contents;

        public LPackage() { }

        public LPackage(byte action, string[] contents)
        {
            this.action = action;
            this.contents = contents;
        }

        protected LPackage(SerializationInfo info, StreamingContext context)
        {
            action = (byte)info.GetByte("action");
            contents = (string[])info.GetValue("contents", typeof(string[]));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("action", action);
            info.AddValue("contents", contents);
        }

        public override string ToString()
        {
            string s = string.Empty;
            if( contents != null)
            {
                foreach (var c in contents)
                {
                    s += " " + c;
                }
            }
            return action.ToString() + s;
        }
    }
}
