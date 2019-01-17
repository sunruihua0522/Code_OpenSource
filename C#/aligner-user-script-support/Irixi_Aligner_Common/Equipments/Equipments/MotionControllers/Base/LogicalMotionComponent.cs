using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.MotionControllers.Base
{
    public class LogicalMotionComponent : List<LogicalAxis>, IHashable
    {
        #region Constructors

        public LogicalMotionComponent(string Caption, string Icon, bool IsAligner)
        {
            this.Caption = Caption;
            this.Icon = Icon;
            this.IsAligner = IsAligner;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Get the component name displayed caption of document panel
        /// </summary>
        public string Caption { private set; get; }

        /// <summary>
        /// Get the icon shown on the caption and the show/hide button
        /// </summary>
        public string Icon { private set; get; }

        /// <summary>
        /// Get whether the motion component is the alignment component
        /// </summary>
        public bool IsAligner { private set; get; }

        public string HashString
        {
            get
            {
                StringBuilder factor = new StringBuilder();

                foreach (var axis in this)
                {
                    factor.Append(axis.HashString);
                }

                return HashGenerator.GetHashSHA256(factor.ToString());
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Methods

        public LogicalAxis FindAxisByHashString(string HashString)
        {
            var ret = this.Where(la => la.HashString == HashString);
            var cnt = ret.Count();
            if(cnt <= 0)
                throw new Exception("not axis was found in LMC.");
            else if (cnt > 1)
                throw new Exception("too many axes have been found in LMC with the same hash string.");
            else
                return ret.First();
        }

        /// <summary>
        /// Move a set of axes
        /// </summary>
        /// <returns></returns>
        public void MoveToPresetPosition(MassMoveArgs Args)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this.Caption;
        } 

        #endregion
    }
}
