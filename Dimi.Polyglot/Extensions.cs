using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dimi.Polyglot.Extensions
{
    /// <summary>
    /// Some useful extensions
    /// </summary>
    public static class Extensions
    {
       

        public static int ToInt(this string str)
        {
            int tmpInt;
            if (!int.TryParse(str, out tmpInt))
            {
                return 0;
            }
            else
            {
                return tmpInt;
            }
        }

       
        public static int ToInt(this object str)
        {
            int tmpInt;

            string strtTemp = string.Empty;

            if (str != null)
            {
                strtTemp = str.ToString();
            }

            if (!int.TryParse(strtTemp, out tmpInt))
            {
                return 0;
            }
            else
            {
                return tmpInt;
            }
        }

        public static int ToInt(this object str, int defaultVal)
        {
            int tmpInt;
            if (!int.TryParse(str.ToString(), out tmpInt))
            {
                return defaultVal;
            }
            else
            {
                return tmpInt;
            }
        }

        public static string ToStr(this object str)
        {
            string strtTemp = string.Empty;

            if (str != null)
            {
                strtTemp = str.ToString();
            }

            return strtTemp;
        }

       
    }
}
