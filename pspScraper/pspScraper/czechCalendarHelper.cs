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

    }
}
