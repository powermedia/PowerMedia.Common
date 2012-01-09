using RealSystem = System;

namespace PowerMedia.Common.System
{

	[global::System.Serializable]
    public class InvalidStateException : RealSystem.Exception
    {
        public InvalidStateException() { }
        public InvalidStateException(string message) : base(message) { }
        public InvalidStateException(string message, RealSystem.Exception inner) : base(message, inner) { }
        protected InvalidStateException(
          RealSystem.Runtime.Serialization.SerializationInfo info,
          RealSystem.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
