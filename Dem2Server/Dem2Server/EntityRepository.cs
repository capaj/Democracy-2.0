using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dem2Model;
using Dem2UserCreated;
using Raven.Abstractions.Commands;
using Raven.Client.Document;

namespace Dem2Server
{
    public static class EntityRepository
    {
        public static HashSet<ServerClientEntity> _all = new HashSet<ServerClientEntity>(); // I would like to have this as private, but according to this: http://msdn.microsoft.com/en-us/library/bb359438.aspx I need to leave it public

        public static HashSet<ServerClientEntity> all
        {
            get { return _all; }
            private set { _all = value; }
        }
        
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

        public static bool Initialize()
        {
            
            foreach (var user in getAll<User>(Dem2Hub.docDB))
            {
                all.Add(user);
            }
            foreach (var voting in getAll<Voting>(Dem2Hub.docDB))
            {
                all.Add(voting);
            }
            foreach (var vote in getAll<Vote>(Dem2Hub.docDB))
            {
                all.Add(vote);
            }
            foreach (var comment in getAll<Comment>(Dem2Hub.docDB))
            {
                all.Add(comment);
            }
            // var entity = session.Load<Company>(companyId);
            var stats = Dem2Hub.GetStatistics();
            Console.WriteLine("Repository initialized with {0} users, {1} votings, {2} votes and {3} comments", stats.userCount, stats.votingCount, stats.voteCount, stats.commentCount);
           
            return true;
        }

        public static bool Add(ServerClientEntity entity) {
            var succes = all.Add(entity);
            if (succes)
            {
                StoreToDB(entity);
                entity.IncrementVersion();      //this should increment from 0 to 1
                Console.WriteLine("Adding entity {0} ended with succes:{1}", entity.ToString(), succes);
            }
            else
            {
                Console.WriteLine("Entity was not added because the same entity is already in hashset.");
            }
            return succes;
        }

        internal static bool Remove(ServerClientEntity entity) {
            var success = all.Remove(entity);
            Console.WriteLine("Removing entity {0} with id {1} ended with succes:{2}", entity.ToString(), entity.Id, success);
            using (var session = Dem2Hub.docDB.OpenSession())
            {
                session.Advanced.Defer(new DeleteCommandData { Key = entity.Id });
                session.SaveChanges();

            }
            return success;
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

        public static bool RemoveEntityById(string Id)
        {
            string typeStr = Id.Split('/')[0];
            bool success = false;
            try
            {
                var entityOnServer = EntityRepository.entityNamesToSets[typeStr].FirstOrDefault(x => x.Id == Id);

                success = Remove(entityOnServer);
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

            return success;
        }

        internal static void StoreToDB(ServerClientEntity entity)
        {
            using (Raven.Client.IDocumentSession session = Dem2Hub.docDB.OpenSession())
            {
                session.Store(entity);

                session.SaveChanges();
            }
        }

        public static List<T> getAll<T>(DocumentStore docDB)
        {
            return getAllFrom(0, new List<T>(), docDB);
        }

        public static List<T> getAllFrom<T>(int startFrom, List<T> list, DocumentStore docDB)
        {
            var allUsers = list;

            using (var session = docDB.OpenSession())
            {
                int queryCount = 0;
                int start = startFrom;
                while (true)
                {
                    var current = session.Query<T>().Take(1024).Skip(start).ToList();
                    queryCount += 1;
                    if (current.Count == 0)
                        break;

                    start += current.Count;
                    allUsers.AddRange(current);

                    if (queryCount >= 30)
                    {
                        return getAllFrom(start, allUsers, docDB);
                    }
                }
            }
            return allUsers;
        }

        public static Type GetRepoTypeById(string IdPrefixStr)
        {
            var IdPrefixToClassnames = new Dictionary<string, string>() { 
                {"votes", "Vote"},
                {"votings", "Voting"},
                {"users", "User"},
                {"comments", "Comment"},
                {"listings", "Listing"},
            };
            var className = IdPrefixToClassnames[IdPrefixStr];
            Type type = Type.GetType("Dem2UserCreated." + className);

            return type;
        }
    }
}
