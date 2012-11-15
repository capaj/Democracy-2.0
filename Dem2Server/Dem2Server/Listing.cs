using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Dem2Server;

namespace Dem2UserCreated
{
    public class Listing:Dem2Server.ServerClientEntity
    {
        public Query JSONQuery { get; set; }

        public Listing(Query query)
        {
            JSONQuery = query;
        }

        public List<string> GetEntities() {
            IEnumerable<string> list = null;
            try
            {
                if (JSONQuery.descending)
                {
                    list = Dem2Hub.entityNamesToSets[JSONQuery.StrOfType]
                        .OrderByDescending(x => x.GetType().GetProperty(JSONQuery.sortBy).GetValue(x, null))
                        .Take(JSONQuery.count)
                        .Select(x => x.Id);
                }
                else
                {
                    list = Dem2Hub.entityNamesToSets[JSONQuery.StrOfType]
                        .OrderBy(x => x.GetType().GetProperty(JSONQuery.sortBy).GetValue(x, null))
                        .Take(JSONQuery.count)
                        .Select(x => x.Id);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
            return list.ToList();
            //TODO implement properly
        }
    }

    public class Query 
    {
        public string sourceJSON { get; set; }
        public bool descending { get; set; }
        public string sortBy { get; set; }      //
        public string StrOfType { get; set; }   //used in looking up the right hashset
        public int count { get; set; }      //how many entities should we return
        [JsonIgnore]
        public Type ofType { 
            get { 
                return Dem2Hub.entityNamesToSets[StrOfType].GetType();
            } 
        }
        
        public Query(string json)
        {
            JsonConvert.DeserializeObject<Query>(sourceJSON);
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

        
    }
}
