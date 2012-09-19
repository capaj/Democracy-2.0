using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pspScraper
{
    static class czechCalendarHelper
    {
        static public int getMonthIntFromString(string str)     //string can be longer, only the first found(chronologically) is returned
        {
            List<string> czechMonths = new List<string> { "ledna", "února", "března", "dubna", "května", "června", "července", "srpna", "září", "října", "listopadu", "prosince" };
            for (int i = 0; i < czechMonths.Count; i++)
			{
			    if (str.Contains(czechMonths.ElementAt(i)))
                {
                    return i + 1;
                }
            }
            return 0;
        }
      
        //switch (str)
        //{
        //    case "ledna":
        //        return 1;
        //    case "února":
        //        return 2;
        //    case "března":
        //        return 3;
        //    case "dubna":
        //        return 4;
        //    case "května":
        //        return 5;
        //    case "června":
        //        return 6;
        //    case "července":
        //        return 7;
        //    case "srpna":
        //        return 8;
        //    case "září":
        //        return 9;
        //    case "října":
        //        return 10;
        //    case "listopadu":
        //        return 11;
        //    case "prosince":
        //        return 12;
        //    default:
        //        throw new Exception { };
        //}
    }
}
