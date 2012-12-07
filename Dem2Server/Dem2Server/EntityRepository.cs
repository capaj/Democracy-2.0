using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dem2Model;
using Dem2UserCreated;
using Raven.Abstractions.Commands;

namespace Dem2Server
{
    public static class EntityRepository
    {
        private static HashSet<ServerClientEntity> all { get; set; }
        public static IEnumerable<User> allUsers
        {
            get
            {
                return all.Where(x => x is User).Select(x => x as User);
            }
        }
        public static IEnumerable<Voting> allVotings { 
            get{
                return all.Where(x => x is Voting).Select(x=>x as Voting);
            } 
        }        // parliamentary votings for now
        public static IEnumerable<Vote> allVotes {  
            get{
                return all.Where(x => x is Vote).Select(x=>x as Vote);
            }  }
        public static IEnumerable<Comment> allComments {  
            get{
                return all.Where(x => x is Comment).Select(x=>x as Comment);
            }  }
        public static IEnumerable<Listing> allListings {  
            get{
                return all.Where(x => x is Listing).Select(x=>x as Listing);
            }  
        }
        // in order for this static method to work: ServerClientEntity.GetEntityFromSetsByID
        public static Dictionary<string, IEnumerable<ServerClientEntity>> entityNamesToSets = new Dictionary<string, IEnumerable<ServerClientEntity>> {
            {"users", allUsers},
            {"votings", allVotings},
            {"votes", allVotes},
            {"comments", allComments},
            {"listings", allListings},
        };

        internal static bool Initialize()
        {
            all = new HashSet<ServerClientEntity>();
            using (var session = Dem2Hub.docDB.OpenSession())
            {
                foreach (var user in session.Query<User>().ToList())
                {
                    all.Add(user);
                }
                foreach (var voting in session.Query<Voting>().ToList())
                {
                    all.Add(voting);
                }
                foreach (var vote in session.Query<Vote>().ToList())
                {
                    all.Add(vote);
                }

                // var entity = session.Load<Company>(companyId);

            }
            return true;
        }

        public static bool Add(ServerClientEntity entity) {
            var succes = all.Add(entity);
            Console.WriteLine("Adding entity {0} ended with succes:{1}", entity.ToString(), succes);
            return succes;
        }

        public static bool Remove(ServerClientEntity entity) {
            var succes = all.Remove(entity);
            Console.WriteLine("Removing entity {0} ended with succes:{1}", entity.ToString(), succes);
            return succes;
        }

        public static ServerClientEntity GetEntityFromSetsByID(string Id)
        {
            string type = Id.Split('/')[0];
            ServerClientEntity entityOnServer = null;
            try
            {
                entityOnServer = EntityRepository.entityNamesToSets[type].FirstOrDefault(x => x.Id == Id);
            }
            catch (Exception ex)
            {
                if (ExceptionIsCriticalCheck.IsCritical(ex)) throw;
                Console.WriteLine("Entity with ID {0} was not found", Id);
                return null;
            }
            return entityOnServer;
        }

        public static bool DeleteEntityById(string Id)
        {
            string typeStr = Id.Split('/')[0];
            try
            {
                var entityOnServer = EntityRepository.entityNamesToSets[typeStr].FirstOrDefault(x => x.Id == Id);

                Remove(entityOnServer);
                using (var session = Dem2Hub.docDB.OpenSession())
                {
                    session.Advanced.Defer(new DeleteCommandData { Key = entityOnServer.Id });
                    session.SaveChanges();

                }
                //.Advanced.DocumentStore.DatabaseCommands.Delete("posts/1234", null);
            }
            catch (Exception ex)
            {
                if (ExceptionIsCriticalCheck.IsCritical(ex)) throw;
                Console.WriteLine("Entity with ID {0} was not found", Id);
                return false;
            }

            return true;
        }
    }
}
