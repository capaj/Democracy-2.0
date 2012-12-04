using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace pspScraper
{
    static class ScraperStringHelper
    {
                
        static public SortedDictionary<UInt16, UInt32> GetNumbersFromString(string str) {
            var numbersInString = new SortedDictionary<UInt16, UInt32>();
            var numberStr = "";
            var startIndexOfTheNumber = -1;

            for (UInt16 i = 0; i < str.Length; i++)
            {
                if (Char.IsDigit(str[i]))
                {
                    if (startIndexOfTheNumber == -1)
                    {
                        startIndexOfTheNumber = i;
                    }
                    
                    numberStr = numberStr + str[i];

                    if (str.Length > i+1)    //check for the end of string
                    {
                        if (Char.IsDigit(str[i+1]) == false)    // we will convert and add a new number to resulting array only if the next character is not a digit
                        {
                            numbersInString.Add((UInt16)startIndexOfTheNumber, Convert.ToUInt32(numberStr));
                            startIndexOfTheNumber = -1;
                            numberStr = "";
                        } 
                    }
                }
         
            }
            return numbersInString;     //returns dictionary which key is a position of a number in a string, the value is a number itself
        }

        static public List<string> SplitByString(string input, string delimiter) {
            
            var output = new List<string>();
            while (input != "")
            {
                var index = input.IndexOf(delimiter);
                if (index == -1)
                {
                    output.Add(input);
                    break;
                }
                else
                {
                    output.Add(input.Substring(0, index));
                    input = input.Substring(index + delimiter.Length);
                }
            }
            return output;
        }
    }
}
