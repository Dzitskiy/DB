using DB1.Entities;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace DB1.Storage
{
    class ClientsStorage
    {
        private readonly ISessionFactory sessionFactory;

        public ClientsStorage(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public List<ClientEntity> GetAllWithMiddleName()
        {
            using (ISession session = sessionFactory.OpenSession()) // unit of work
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    return session.Query<ClientEntity>()                        
                        .Where(it => it.MiddleName != null)                            
                        .ToList();                    
                }
            }
        }

        public int CountDeposits()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    var sum = 0;
                    foreach (var clientEntity in
                        //session.Query<ClientEntity>()                                                

                        //Criteria API:
                        session.QueryOver<ClientEntity>().Fetch(SelectMode.Fetch, it => it.Deposits).List()
                        )
                    {
                        var deposits = clientEntity.Deposits;
                        sum += deposits.Count;
                    }
                    return sum;
                }
            }
        }

        public void SaveOrUpdate(ClientEntity entity)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {                    
                    session.SaveOrUpdate(entity);
                    tx.Commit();
                }
            }
        }
    }
}
