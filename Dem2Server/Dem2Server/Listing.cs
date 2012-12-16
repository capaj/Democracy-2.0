﻿using System;
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
                IEnumerable<string> list = null;
                try
                {
                    if (JSONQuery.descending)
                    {
                        list = EntityRepository.entityNamesToSets[JSONQuery.ofTypeInStr]
                            .OrderByDescending(x => x.GetType().GetProperty(JSONQuery.sortByProp).GetValue(x, null))
                            .Take(JSONQuery.pageSize)
                            .Select(x => x.Id);     //listings only contain entity Ids
                    }
                    else
                    {
                        list = EntityRepository.entityNamesToSets[JSONQuery.ofTypeInStr]
                            .OrderBy(x => x.GetType().GetProperty(JSONQuery.sortByProp).GetValue(x, null))
                            .Take(JSONQuery.pageSize)
                            .Select(x => x.Id);     //listings only contain entity Ids
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                return list.ToList();
            } 
        }

    }

    
}
