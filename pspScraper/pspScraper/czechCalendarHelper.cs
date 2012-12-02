using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pspScraper
{
    static class czechCalendarHelper
    {
        static List<string> czechMonths = new List<string> { "ledna", "února", "března", "dubna", "května", "června", "července", "srpna", "září", "října", "listopadu", "prosince" };
        static List<string> czechDays = new List<string> { "pondělí", "úterý", "středa", "čtvrtek", "pátek", "sobota", "neděle" };
        
        public static int getMonthFromString(string str)     //string can be longer, only the first found(chronologically) is returned
        {
            for (int i = 0; i < czechMonths.Count; i++)
            {
                if (str.Contains(czechMonths.ElementAt(i)))
                {
                    return i + 1;
                }
            }
            return 0;   //has not found
        }

        public static int getDayFromString(string str) {
            for (int i = 0; i < czechDays.Count; i++)
            {
                if (str.Contains(czechDays.ElementAt(i)))
                {
                    return i + 1;
                }
            }
            return 0;   //has not found
        }

        //public static bool containsMonthExperimental(string str) {
        //    //return iterateAndRun(str, (x) => { return (bool)x.Contains(str) }, czechMonths);
        //}

        //public static int iterateAndRun(string str, Func<string, dynamic> func, List<string> list)
        //{
        //    dynamic retVal = 0;
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        retVal = func(list.ElementAt(i));
        //    }
        //    return retVal;
        //}
    }
}
