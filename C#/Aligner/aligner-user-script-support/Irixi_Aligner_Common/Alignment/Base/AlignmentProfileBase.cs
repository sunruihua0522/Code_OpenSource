using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Irixi_Aligner_Common.Alignment.Base
{
    public class AlignmentProfileBase : IAlignmentArgsProfile
    {
        [HashIgnore]
        public string HashString { set; get; }

        public int Speed { get; set; }

        public virtual void FromArgsInstance(IAlignmentArgs Args)
        {
            throw new NotImplementedException();
        }

        public virtual void ToArgsInstance(SystemService Service, IAlignmentArgs Args)
        {
            throw new NotImplementedException();
        }

        public virtual bool Validate()
        {
            return this.HashString == this.GetHashString();
        }

        /// <summary>
        /// Calculate hash string by all the properties except the ones marked with HashIgnore
        /// </summary>
        /// <returns></returns>
        public virtual string GetHashString()
        {
            // get the list of my properties those do not marked as [HashIgnore]
            Type mytype = this.GetType();
            List<PropertyInfo> properties = new List<PropertyInfo>(mytype.GetProperties());
            var validprop = properties.Where(item => item.GetCustomAttribute<HashIgnoreAttribute>() == null).Select((pi, str) =>
            {
                return pi.GetValue(this);
            }).ToArray();
            return HashGenerator.GetHashSHA256(String.Join(",", validprop));

        }

        public override string ToString()
        {
            return HashString;
        }
    }
}
