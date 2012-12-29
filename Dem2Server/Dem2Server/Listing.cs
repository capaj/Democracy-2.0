using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Dem2Server;

namespace Dem2UserCreated
{
    public class Listing : ServerClientEntity
    {
        public Query JSONQuery { get; set; }

        public Listing(Query query)
        {
            JSONQuery = query;
        }

        public List<string> list { 
            get{
                IEnumerable<Dem2Server.ServerClientEntity> list = null;
                try
                {
                    if (JSONQuery.descending)
                    {
                        list = EntityRepository.entityNamesToSets[JSONQuery.ofTypeInStr]
                            .OrderByDescending(x => x.GetType().GetProperty(JSONQuery.sortByProp).GetValue(x, null));     //listings only contain entity Ids
                    }
                    else
                    {
                        list = EntityRepository.entityNamesToSets[JSONQuery.ofTypeInStr]
                            .OrderBy(x => x.GetType().GetProperty(JSONQuery.sortByProp).GetValue(x, null));
                                 //listings only contain entity Ids
                    }
                    foreach (var item in JSONQuery.propertiesEqualValues)
                    {
                        list = list.Where(x => (string)x.GetType().GetProperty(item.Key).GetValue(x, null) == (string)item.Value);
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                return list.Select(x => x.Id).Skip(JSONQuery.toSkip).Take(JSONQuery.pageSize).ToList();
            } 
        }

    }

    
}
