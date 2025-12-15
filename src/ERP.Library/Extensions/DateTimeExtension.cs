using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Extensions
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 取得 23:59:59
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime To235959(this DateTime date)
        {
            return date.Date.AddDays(1).AddSeconds(-1);
        }

        /// <summary>
        /// 轉換為 民國年(字串)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static string ToRocDateStr(this object date, string mark = "-")
        {
            if (date == null)
            {
                return string.Empty;
            }
            if (!DateTime.TryParse(date.ToString(), out DateTime processdate))
            {
                return string.Empty;
            }
            if (processdate == DateTime.MinValue)
            {
                return string.Empty;
            }
            if (mark == "ch")
            {
                return $"{processdate.GetRocYear()}年{processdate.Month}月{processdate.Day}日";
            }
            return processdate.ToRocDate($"yyy{mark}MM{mark}dd");
        }

        /// <summary>
        /// 取得民國年
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetRocYear(this DateTime date)
        {
            var year = date.Year;
            if (date.IsRocYear())
            {
                return year;
            }
            return (year - 1911);
        }
        /// <summary>
        /// 轉換為 民國年
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToRocDate(this DateTime dateTime, string format = "yyy-MM-dd")
        {
            if (IsRocYear(dateTime))
            {
                return dateTime.ToString(format);
            }

            var eraYear = dateTime.Year.ToString();

            var year = dateTime.GetRocYear().ToString();
            var rocDate = dateTime.ToString(format).Replace(eraYear, year);
            return rocDate;
        }
        /// <summary>
        /// 是否為民國年
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsRocYear(this DateTime date)
        {
            int year = date.Year;
            return !(year >= 1912);
        }
    }
}
