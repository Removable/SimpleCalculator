using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCalculator
{
    public static class Utility
    {
        #region 自定义扩展方法

        public static Int32 ToInt32(this object obj, int defaultNum = 0)
        {
            try
            {
                var num = 0;
                Int32.TryParse(obj.ToString2(), out num);
                return num;
            }
            catch
            {
                return defaultNum;
            }
        }

        public static float ToFloat(this object obj, float defaultNum = 0.0f)
        {
            try
            {
                var num = 0.0f;
                float.TryParse(obj.ToString2(), out num);
                return num;
            }
            catch
            {
                return defaultNum;
            }
        }

        public static double ToDouble(this object obj, double defaultNum = 0.0d)
        {
            try
            {
                var num = 0.0d;
                double.TryParse(obj.ToString2(), out num);
                return num;
            }
            catch
            {
                return defaultNum;
            }
        }

        public static string ToString2(this object obj)
        {
            try
            {
                return Convert.ToString(obj);
            }
            catch
            {
                return string.Empty;
            }
        }


        #endregion
    }
}
