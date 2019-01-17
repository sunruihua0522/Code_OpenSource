using System;
using M12.Definitions;

namespace M12.Exceptions
{
    public class UnitErrorException : Exception
    {
        public UnitErrorException(UnitID UnitID, Errors Error)
        {
            this.UnitID = UnitID;
            this.Error = Error;
        }

        public UnitID UnitID { get; private set; }
        public Errors Error { get; private set; }

        public override string Message
        {
           get
            {
                return $"{Enum.GetName(typeof(Errors), Error)} occured on {Enum.GetName(typeof(UnitID), UnitID)}";
            }
        }
    }
}
