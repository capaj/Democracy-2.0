using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pspScraper
{
    static class ScraperStringHelper
    {
        static public string RemoveHTMLmarkup(string str) {
            //var tags = new string[]{"&nbsp;", "\n" };
            //foreach (var tag in tags)
            //{
            //    str = str.Replace(tag, "");
            //}

            return str.Replace("&nbsp;", " ").Replace("\n", "");
        }
        
        static public SortedDictionary<UInt16, UInt32> GetNumbersFromString(string str) {
            var numbersInString = new SortedDictionary<UInt16, UInt32>();
            var numberStr = "";
            
            for (UInt16 i = 0; i < str.Length; i++)
            {
                if (Char.IsDigit(str[i]))
                {
                    numberStr = numberStr + str[i];
                    if (Char.IsDigit(str[i+1]) == false)    // we will convert and add a new number to resulting array only if the next character is not a digit
                    {
                        numbersInString.Add(i, Convert.ToUInt32(numberStr));
                        numberStr = "";
                    }
                }
         
            }
            return numbersInString;     //returns dictionary which key is a position of a number in a string, the value is a number itself
        }
    }
}
