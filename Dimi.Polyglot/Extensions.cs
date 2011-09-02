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
            return !int.TryParse(str, out tmpInt) ? 0 : tmpInt;
        }


        public static int ToInt(this object str)
        {
            int tmpInt;

            var strtTemp = string.Empty;

            if (str != null)
            {
                strtTemp = str.ToString();
            }

            return !int.TryParse(strtTemp, out tmpInt) ? 0 : tmpInt;
        }

        public static int ToInt(this object str, int defaultVal)
        {
            int tmpInt;
            return !int.TryParse(str.ToString(), out tmpInt) ? defaultVal : tmpInt;
        }

        public static string ToStr(this object str)
        {
            var strtTemp = string.Empty;

            if (str != null)
            {
                strtTemp = str.ToString();
            }

            return strtTemp;
        }
    }
}