using DB1.Entities;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace DB1.Storage
{
    class DepositsStorage
    {
        private readonly ISessionFactory sessionFactory;

        public DepositsStorage(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public List<DepositEntity> GetAll()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    return session.Query<DepositEntity>()
                        .OrderBy(it => it.CreatedAt)
                        .ToList();
                }
            }
        }

        public void SaveOrUpdate(DepositEntity entity)
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

        public void Save(DepositEntity entity)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Save(entity);
                    tx.Commit();
                }
            }
        }

        public void SaveAll(IEnumerable<DepositEntity> entities)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    foreach (var entity in entities)
                    {
                        session.Save(entity);
                    }
                    tx.Commit();
                }
            }
        }
    }
}
