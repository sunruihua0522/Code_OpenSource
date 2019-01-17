using System;
using M12.Base;
using M12.Definitions;

namespace M12.Exceptions
{
    public class SystemErrorException : Exception
    {
        public SystemErrorException(SystemLastError Error)
        {
            this.Error = Error;
        }

        public SystemLastError Error { get; private set; }

        public override string Message
        {
            get
            {
                return $"{Enum.GetName(typeof(Errors), Error.Error)} occured on {Enum.GetName(typeof(UnitID), Error.UnitID)}";
            }
        }
    }
}
