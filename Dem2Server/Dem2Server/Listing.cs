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

        public override bool Unsubscribe(Dem2Model.User theUser)
        {
            var ret = base.Unsubscribe(theUser);
            if (subscribedUsers.Count == 0)
            {
                Delete();
            }
            return ret;
        }

        public override string ToString()
        {
            if (JSONQuery == null)
            {
                return this.ToString();
            }
            else
            {
                return JSONQuery.ToString();
            }

        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null || JSONQuery == null)
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
            return JSONQuery.sourceJSON.Equals(second);
        }

        public bool Equals(Listing second)
        {
            // If parameter is null return false:
            if ((object)second == null || JSONQuery == null || second.JSONQuery == null)
            {
                return false;
            }

            // Return true if the fields match:
            return JSONQuery.Equals(second.JSONQuery);
        }

        public override int GetHashCode()
        {
            return JSONQuery.GetHashCode();
        }

    }

    
}
