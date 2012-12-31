using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dem2Server
{
    public class Query
    {
        public string sourceJSON { get; set; }
        public bool descending { get; set; }
        public string sortByProp { get; set; }      //
        public Dictionary<string,string> propertiesEqualValues { get; set; }
        public string ofTypeInStr { get; set; }   //used in looking up the right hashset
        public UInt16 pageSize { get; set; }      //how many entities should we return
        public int toSkip { get; set; }      //how many entities should we return
        [JsonIgnore]
        public Type ofType
        {
            get
            {
                return EntityRepository.entityNamesToSets[ofTypeInStr].GetType();
            }
        }

        public Query()
        {
        }

        public Query MakeQueryFromJson(string json)
        {
            var q = JsonConvert.DeserializeObject<Query>(sourceJSON);
            q.sourceJSON = json;

            return q;
        }

        public override string ToString()
        {
            if (sourceJSON != null)
            {
                return sourceJSON;
            }
            else
            {
                return this.ToString();
            }

        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null || sourceJSON == null )
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Query second = obj as Query;
            if ((System.Object)second == null || second.sourceJSON == null)
            {
                return false;
            }

            // Return true if the fields match:
            return sourceJSON.Equals(second.sourceJSON);
        }

        public bool Equals(Query second)
        {
            // If parameter is null return false:
            if ((object)second == null || sourceJSON == null || second.sourceJSON == null)
            {
                return false;
            }

            // Return true if the fields match:
            return sourceJSON.Equals(second.sourceJSON);
        }

        public override int GetHashCode()
        {
            return sourceJSON.GetHashCode();
        }


    }
}
