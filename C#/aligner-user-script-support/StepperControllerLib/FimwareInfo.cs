using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrixiStepperControllerHelper
{
    public class FimwareInfo
    {
        public int VerMajor
        {
            internal set;
            get;
        }

        public int VerMinor
        {
            internal set;
            get;
        }

        public int VerRev
        {
            internal set;
            get;
        }

        public DateTime CompiledDate
        {
            internal set;
            get;
        }

        public bool Parse(byte[] data)
        {
            this.VerMajor = BitConverter.ToInt32(data, 1);
            this.VerMinor = BitConverter.ToInt32(data, 5);
            this.VerRev = BitConverter.ToInt32(data, 9);

            int year = BitConverter.ToInt32(data, 13);
            int month = BitConverter.ToInt32(data, 17);
            int day = BitConverter.ToInt32(data, 21);

            this.SetCompiledDate(year, month, day);

            return Validate();

        }

        public override string ToString()
        {
            return string.Format("VER {0}.{1}.{2}, {3:yyyy/MM/dd}", new object[] { VerMajor, VerMinor, VerRev, CompiledDate });
        }

        #region private methods

        private void SetCompiledDate(int year, int month, int day)
        {
            try
            {
                CompiledDate = new DateTime(year, month, day);
            }
            catch
            {
                SetToDefault();
            }
        }

        /// <summary>
        /// Clear all values
        /// </summary>
        private void SetToDefault()
        {
            VerMajor = 0;
            VerMinor = 0;
            VerRev = 0;
            CompiledDate = DateTime.MinValue;
        }

        /// <summary>
        /// Validate the firmware information,
        /// major version >= 1
        /// compiled date >= 2017/7/1
        /// </summary>
        /// <returns></returns>
        private bool Validate()
        {
            if (VerMajor >= 1 && CompiledDate >= new DateTime(2017, 7, 1))
                return true;
            else
                return false;
        }

        #endregion

    }
}
