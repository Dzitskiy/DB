using Dapper;
using DB1.Config;
using DB1.Entities;
using DB1.Storage;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.Attributes;
using NHibernate.Tool.hbm2ddl;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DB1
{
    class Program
    {
        static void Main(string[] args)
        {
            //GetVersion();

            //CreateClientsTable();
            //CreateDepositsTable();
            //InsertClientsSimple();
            //InsertClientsWithParams();
            //InsertClientsMultipleCommands();
            //Transaction();

            //DapperUpsert();
            //JoinDapper();


            EFInsert();
            //EFUpdate();
            //EFGetAll();
            //EFGetOne();
            //EFDelete();

            //NHibernateInsertClient();
            //NHibernateInsertDeposit();
            //NHibernateInsertClientThenDeposit();

            //NHibernateInsertDepositWithClient();
            //NHibernateNPlus1();


            Console.ReadKey();
        }

        const string connectionString = "Host=localhost;Username=postgres;Password=password;Database=otus";

        #region ADONET
        /// <summary>
        /// Подключение к БД и получение версии
        /// </summary>
        static void GetVersion()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var sql = "SELECT version()";

                using var cmd = new NpgsqlCommand(sql, connection);

                var version = cmd.ExecuteScalar().ToString();

                Console.WriteLine($"PostgreSQL version: {version}");
            }
        }


        /// <summary>
        /// Создание таблицы
        /// </summary>
        static void CreateClientsTable()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
CREATE SEQUENCE clients_id_seq;

CREATE TABLE clients
(
    id              BIGINT                      NOT NULL    DEFAULT NEXTVAL('clients_id_seq'),
    first_name      CHARACTER VARYING(255)      NOT NULL,
    last_name       CHARACTER VARYING(255)      NOT NULL,
    middle_name     CHARACTER VARYING(255),
    email           CHARACTER VARYING(255)      NOT NULL,
  
    CONSTRAINT clients_pkey PRIMARY KEY (id),
    CONSTRAINT clients_email_unique UNIQUE (email)
);

CREATE INDEX clients_last_name_idx ON clients(last_name);
CREATE UNIQUE INDEX clients_email_unq_idx ON clients(lower(email));
";

            using var cmd = new NpgsqlCommand(sql, connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Created CLIENTS table. Affected rows count: {affectedRowsCount}");
        }

        /// <summary>
        /// Создание таблицы с внешним ключом
        /// </summary>
        static void CreateDepositsTable()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
CREATE SEQUENCE deposits_id_seq;

CREATE TABLE deposits
(
    id              BIGINT                      NOT NULL    DEFAULT NEXTVAL('deposits_id_seq'),
    client_id       BIGINT                      NOT NULL,
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL,    
  
    CONSTRAINT deposits_pkey PRIMARY KEY (id),
    CONSTRAINT deposits_fk_client_id FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE
);
";

            using var cmd = new NpgsqlCommand(sql, connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Created DEPOSITS table. Affected rows count: {affectedRowsCount}");
        }

        /// <summary>
        /// Вставка без параметров
        /// </summary>
        static void InsertClientsSimple()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var firstName = "Иван";
            var lastName = "Иванов";
            var sql = $@"
INSERT INTO clients(first_name, last_name, middle_name, email) 
VALUES ('{firstName}', '{lastName}', 'Иванович', 'ivan@mail.ru');
";

            using var cmd = new NpgsqlCommand(sql, connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Insert into CLIENTS table. Affected rows count: {affectedRowsCount}");
        }

        /// <summary>
        /// Вставка с параметрами
        /// </summary>
        static void InsertClientsWithParams()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
INSERT INTO clients(first_name, last_name, middle_name, email) 
VALUES (:first_name, :last_name, :middle_name, :email);
";

            using var cmd = new NpgsqlCommand(sql, connection);
            var parameters = cmd.Parameters;
            parameters.Add(new NpgsqlParameter("first_name", "Константин"));
            parameters.Add(new NpgsqlParameter("last_name", "Константинов"));
            parameters.Add(new NpgsqlParameter("middle_name", "Константинович"));
            parameters.Add(new NpgsqlParameter("email", "konst@rambler.ru"));

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Insert into CLIENTS table. Affected rows count: {affectedRowsCount}");
        }

        /// <summary>
        /// Несколько команд в одном соединение + выборка из таблицы
        /// </summary>
        static void InsertClientsMultipleCommands()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
INSERT INTO clients(first_name, last_name, middle_name, email) 
VALUES (:first_name, :last_name, :middle_name, :email);
";

            using var cmd1 = new NpgsqlCommand(sql, connection);
            var parameters = cmd1.Parameters;
            parameters.Add(new NpgsqlParameter("first_name", "Иван"));
            parameters.Add(new NpgsqlParameter("last_name", "Петров"));
            parameters.Add(new NpgsqlParameter("middle_name", "Петрович"));
            parameters.Add(new NpgsqlParameter("email", "petr@yandex.ru"));

            var affectedRowsCount = cmd1.ExecuteNonQuery().ToString();

            Console.WriteLine($"Insert into CLIENTS table. Affected rows count: {affectedRowsCount}");

            sql = @"
SELECT first_name, last_name, middle_name, email FROM clients
WHERE first_name<>:first_name
";

            using var cmd2 = new NpgsqlCommand(sql, connection);
            parameters = cmd2.Parameters;
            parameters.Add(new NpgsqlParameter("first_name", "Иван"));

            var reader = cmd2.ExecuteReader();
            while (reader.Read())
            {
                var firstName = reader.GetString(0);
                var lastName = reader.GetString(1);
                var middleName = reader.GetString(2);
                var email = reader.GetString(3);

                Console.WriteLine($"Read: [firstName={firstName},lastName={lastName},middleName={middleName},email={email}]");
            }
        }

        /// <summary>
        /// Транзакция + возврат идентификатора вставленной записи
        /// </summary>
        static void Transaction()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                var sql = @"
INSERT INTO clients(first_name, last_name, middle_name, email) 
VALUES (:first_name, :last_name, :middle_name, :email)
RETURNING id;
";

                using var cmd1 = new NpgsqlCommand(sql, connection);
                var parameters = cmd1.Parameters;
                parameters.Add(new NpgsqlParameter("first_name", "Александр"));
                parameters.Add(new NpgsqlParameter("last_name", "Александров"));
                parameters.Add(new NpgsqlParameter("middle_name", "Александрович"));
                parameters.Add(new NpgsqlParameter("email", "alex@yandex.ru"));

                var clientId = (long)cmd1.ExecuteScalar();
                Console.WriteLine($"Insert into CLIENTS table. ClientId = {clientId}");

                // Специально кидаем исключение
                //throw new ApplicationException("Deliberate exception");

                sql = @"
INSERT INTO deposits(client_id, created_at) 
VALUES (:client_id, :created_at);
";

                using var cmd2 = new NpgsqlCommand(sql, connection);
                parameters = cmd2.Parameters;
                parameters.Add(new NpgsqlParameter("client_id", clientId));
                parameters.Add(new NpgsqlParameter("created_at", DateTime.Now));

                var affectedRowsCount = cmd2.ExecuteNonQuery().ToString();

                Console.WriteLine($"Insert into DEPOSITS table. Affected rows count: {affectedRowsCount}");

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Console.WriteLine($"Rolled back the transaction");
                return;
            }
        }
		#endregion

		#region NHibernate

		static ISessionFactory sessionFactory;
		static ISessionFactory SessionFactory
		{
			get
			{
				if (sessionFactory == null)
				{
					var configProperties = new Dictionary<string, string>
					{
						{
							NHibernate.Cfg.Environment.ConnectionDriver,
							typeof (NHibernate.Driver.NpgsqlDriver).FullName
						},
						{
							NHibernate.Cfg.Environment.Dialect,
							typeof (NHibernate.Dialect.PostgreSQL82Dialect).FullName
						},
						{
							NHibernate.Cfg.Environment.ConnectionString,
							connectionString
						},
                        //{ 
                        //    "show_sql", 
                        //    "true" 
                        //}
                    };

					var serializer = HbmSerializer.Default;
					serializer.Validate = true;

					var configuration = new Configuration()
						.SetProperties(configProperties)
						.SetNamingStrategy(new PostgresNamingStrategy())
						.AddInputStream(serializer.Serialize(Assembly.GetExecutingAssembly()))
						.SetInterceptor(new SqlDebugOutputInterceptor());

					new SchemaUpdate(configuration).Execute(true, true);

					sessionFactory = configuration.BuildSessionFactory();
				}
				return sessionFactory;
			}
		}

		/// <summary>
		/// Знакомство с NHibernate (вставка, выборка)
		/// </summary>
		static void NHibernateInsertClient()
        {
            var storage = new ClientsStorage(SessionFactory);
            var client = new ClientEntity
            {
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Иванович",
                Email = "ivan@mail.ru"
            };
            storage.SaveOrUpdate(client);
            Console.WriteLine($"NHibernate Insert into CLIENTS table");

            var clients = storage.GetAllWithMiddleName();

            Console.WriteLine($"Returned clients = {string.Join(',', clients.Select(it => it.FirstName))}");
        }

        /// <summary>
        /// NHibernate: связи между вставляемыми объектами
        /// </summary>
        static void NHibernateInsertDeposit()
        {
            var clientsStorage = new ClientsStorage(SessionFactory);
            var client = clientsStorage.GetAllWithMiddleName()[0];
            var depositsStorage = new DepositsStorage(SessionFactory);
            var deposit = new DepositEntity()
            {
                Client = client,
                CreatedAt = DateTime.Now
            };
            depositsStorage.SaveOrUpdate(deposit);
            Console.WriteLine($"NHibernate Insert into DEPOSITS table");

            var deposits = depositsStorage.GetAll();

            Console.WriteLine($"Returned deposits = {string.Join(',', deposits.Select(it => it.CreatedAt))}");
        }

        /// <summary>
        /// NHibernate: вставка сначала родительского объекта, потом дочернего
        /// </summary>
        static void NHibernateInsertClientThenDeposit()
        {
            var clientsStorage = new ClientsStorage(SessionFactory);
            var client = new ClientEntity
            {
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Иванович",
                Email = "ivan@mail.ru"
            };
            clientsStorage.SaveOrUpdate(client);
            Console.WriteLine($"NHibernate Insert into CLIENTS table");

            var depositsStorage = new DepositsStorage(SessionFactory);
            var deposit = new DepositEntity()
            {
                Client = client,
                CreatedAt = DateTime.Now
            };
            depositsStorage.SaveOrUpdate(deposit);
            Console.WriteLine($"NHibernate Insert into DEPOSITS table");

            var deposits = depositsStorage.GetAll();

            Console.WriteLine($"Returned deposits = {string.Join(',', deposits.Select(it => it.CreatedAt))}");
        }

        /// <summary>
        /// NHibernate: сохранение только дочернего объекта
        /// </summary>
        static void NHibernateInsertDepositWithClient()
        {
            var depositsStorage = new DepositsStorage(SessionFactory);
            var client = new ClientEntity
            {
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Иванович",
                Email = "ivan@mail.ru"
            };
            var deposit = new DepositEntity()
            {
                Client = client,
                CreatedAt = DateTime.Now
            };
            depositsStorage.Save(deposit);
            Console.WriteLine($"NHibernate Insert into DEPOSITS & CLIENTS table");

            var deposits = depositsStorage.GetAll();

            Console.WriteLine($"Returned deposits = {string.Join(',', deposits.Select(it => it.CreatedAt))}");
        }

        /// <summary>
        /// NHibernate: N + 1
        /// </summary>
        static void NHibernateNPlus1()
        {
            var depositsStorage = new DepositsStorage(SessionFactory);
            var client1 = new ClientEntity
            {
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Иванович",
                Email = "ivan@mail.ru"
            };
            var deposit1 = new DepositEntity()
            {
                Client = client1,
                CreatedAt = DateTime.Now
            };
            var client2 = new ClientEntity
            {
                FirstName = "Иван",
                LastName = "Петров",
                MiddleName = "Петрович",
                Email = "petr@rambler.ru"
            };
            var deposit2 = new DepositEntity()
            {
                Client = client2,
                CreatedAt = DateTime.Now
            };
            depositsStorage.SaveAll(new[] { deposit1, deposit2 });
            Console.WriteLine($"NHibernate Insert into DEPOSITS & CLIENTS table");

            var depositsCount = new ClientsStorage(SessionFactory).CountDeposits();
            Console.WriteLine($@"Returned deposits = {depositsCount}");
        }

		#endregion

		#region dapper
		// Dapper: Upsert
		static void DapperUpsert()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using NpgsqlTransaction transaction = connection.BeginTransaction();

            var clientDapperEntity = new ClientDapperEntity
            {
                Id = 100_000L,
                FirstName = "Вася",
                LastName = "Петров",
                MiddleName = "Петрович",
                Email = "petr@rambler.ru"
            };

            ClientDapperEntity existingClient = connection.QueryFirstOrDefault<ClientDapperEntity>(
                    @"SELECT 
                        id,
                        first_name FirstName,
                        last_name LastName,
                        middle_name MiddleName,
                        email
                      FROM clients WHERE clients.id=@LookUpId",
                    new
                    {
                        LookUpId = clientDapperEntity.Id,
                    },
                    transaction);

            if (existingClient != null)
            {
                int affectedRowsCount = connection.Execute
                (@"UPDATE clients SET 
                    first_name = @FirstName,
                    last_name = @LastName,
                    middle_name = @MiddleName,
                    email = @Email
                   WHERE clients.id=@ToUpdateId",
                    new
                    {
                        ToUpdateId = existingClient.Id,
                        clientDapperEntity.FirstName,
                        clientDapperEntity.LastName,
                        clientDapperEntity.MiddleName,
                        clientDapperEntity.Email,
                    },
                    transaction);

                Console.WriteLine($"Dapper Update CLIENTS table: {affectedRowsCount} rows");
            }
            else
            {
                long newId = connection.QueryFirst<long>
                (@"INSERT INTO clients(id, first_name, last_name, middle_name, email) " +
                 @"VALUES(@Id, @FirstName, @LastName, @MiddleName, @Email) " +
                 @"RETURNING id",
                    clientDapperEntity,
                    transaction);

                Console.WriteLine($"Dapper Insert into CLIENTS table: {newId}");
            }

            transaction.Commit();
        }

        // Dapper: Joins
        static void JoinDapper()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            // multi mapping with Dapper
            IEnumerable<DepositDapperEntity> joinedDeposits = connection.Query<DepositDapperEntity, ClientDapperEntity, DepositDapperEntity>(
                @"SELECT 
                    d.id,
                    d.client_id ClientId,
                    d.created_at CreatedAt,
                    c.id,
                    c.first_name FirstName,
                    c.last_name LastName,
                    c.middle_name MiddleName,
                    c.email
                FROM deposits d
                JOIN clients c on c.id = d.client_id
                ORDER BY c.last_name",

                (deposit, client) =>
                {
                    deposit.Client = client;
                    return deposit;
                },

                splitOn: "Id" // optional
            );
            var joinedDeposit = joinedDeposits.First();

            Console.WriteLine($"Joined deposit: {joinedDeposit}");
        }

		#endregion dapper

		#region EFCore
		static void EFInsert() 
        {
            using (DataContext db = new DataContext()) 
            {
                var client = new Client
                {
                    id = 90,
                    first_name = "Test",
                    last_name = "Test",
                    email = "Test",
                };
                db.clients.Add(client);

                db.SaveChanges();
                Console.WriteLine("Record Added");
            }
        }

		static void EFUpdate()
		{
			using (DataContext db = new DataContext())
			{
                var client = db.clients.FirstOrDefault(x => x.id == 1);
                if (client != null) 
                {
                    client.first_name = "Вася";
					db.Update(client);
					db.SaveChanges();
				}
				Console.WriteLine("Record Updated");
			}
		}

        static void EFGetAll()
        {
			using (DataContext db = new DataContext())
			{
                var objects = db.clients.ToArray();
				Console.WriteLine("count:"+objects.Count());
			}
		}

		static void EFGetOne()
		{
			using (DataContext db = new DataContext())
			{
                var objects = db.clients.FirstOrDefault(x=>x.id==1);
				Console.WriteLine(objects.first_name);
			}
		}

		static void EFDelete()
		{
			using (DataContext db = new DataContext())
			{
				var obj = db.clients.FirstOrDefault(x => x.id == 1);
                if (obj!=null)
                {
                    db.clients.Remove(obj);
					db.SaveChanges();
					Console.WriteLine("объект удалён");
				}
                else
				Console.WriteLine("объект пустой");
			}
		}

		#endregion
	}
}