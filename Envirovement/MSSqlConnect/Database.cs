using FluentAssertions.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace MSSqlConnect
{
    public class Database : IDisposable
    {
        private readonly string _connectionString;
        private readonly bool _useSingletone;
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private DataTable _dataTable;
        private IConfiguration configuration;

        public Database(string connectionString, bool useSingletone = true)
        {
            _connectionString = connectionString;
            _useSingletone = useSingletone;
        }

        //public Database(bool useSingletone = true)
        //{
        //    _connectionString = @"Data Source=GTPPENVIROVEMEN\SQLEXPRESS;Initial Catalog=EnvirovementReports;User ID=bachexa;Password=Zatara./12!!";
        //        //@"server = GTPPENVIROVEMEN\SQLEXPRESS; database = EnvirovementReports; integrated security = true";
        //    _useSingletone = useSingletone;
        //}

        //public Database( bool useSingletone = true)
        //{
        //    _connectionString = configuration.GetConnectionString("MyConnectionString");
        //    _useSingletone = useSingletone;
        //}


        public List<T> SqlQuery<T>(string script, params SqlParameter[] parameters)
        {
            SqlCommand command = GetCommand(script, parameters);
            if (!command.Connection.State.HasFlag(ConnectionState.Open))
            {
                command.Connection.Open();
            }

            try
            {
                List<T> resultList = new List<T>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T result = MapDataToObject<T>(reader);
                        resultList.Add(result);
                    }
                }

                return resultList;
            }
            finally
            {
                if (_transaction == null)
                {
                    command.Connection.Close();
                }
            }
        }

        private T MapDataToObject<T>(SqlDataReader reader)
        {
            T result = Activator.CreateInstance<T>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (!reader.IsDBNull(i))
                {
                    Type propertyType = typeof(T).GetProperty(reader.GetName(i))?.PropertyType;
                    if (propertyType != null)
                    {
                        object value = Convert.ChangeType(reader.GetValue(i), propertyType);
                        typeof(T).GetProperty(reader.GetName(i))?.SetValue(result, value);
                    }
                }
            }
            return result;
        }

        public SqlConnection GetConnection()
        {
            if (!_useSingletone || _connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            }
            return _connection;
        }

        public void BeginTransaction()
        {
            if (!_useSingletone)
            {
                throw new NotSupportedException("Transaction is Supported Only Singletone");
            }
            if (!GetConnection().State.HasFlag(ConnectionState.Open))
            {
                GetConnection().Open();
            }
            _transaction = GetConnection().BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (!_useSingletone)
            {
                throw new NotSupportedException("Transaction is Supported Only Singletone");
            }
            if (_transaction == null)
            {
                throw new NullReferenceException("No Active Transaction Found");
            }
            _transaction.Commit();
            _transaction = null;
        }

        public void RollbackTransaction()
        {
            if (!_useSingletone)
            {
                throw new NotSupportedException("Transaction is Supported Only Singletone");
            }
            if (_transaction == null)
            {
                throw new NullReferenceException("No Active Transaction Found");
            }
            _transaction.Rollback();
            _transaction = null;
        }

        public DataTable Getdatadable(string name, Type type)
        {
            DataColumn datacolum = new DataColumn(name, type);
            _dataTable = new DataTable()
            {
                PrimaryKey = new DataColumn[] { datacolum }
            };

            _dataTable.Columns.Add(datacolum);
            return _dataTable;
        }

        public SqlCommand GetCommand(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlCommand command = new SqlCommand()
            {
                Connection = GetConnection(),
                CommandText = commandText,
                CommandType = commandType,
            };
            command.Parameters.AddRange(parameters);
            if (_transaction != null)
            {
                command.Transaction = _transaction;
            }
            return command;
        }

        public SqlCommand GetCommand(string commandText, params SqlParameter[] parameters)
        {
            return GetCommand(commandText, CommandType.Text, parameters);
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlCommand command = GetCommand(commandText, commandType, parameters);
            if (!command.Connection.State.HasFlag(ConnectionState.Open))
            {
                command.Connection.Open();
            }

            try
            {
                return command.ExecuteNonQuery();
            }
            finally
            {
                if (_transaction == null)
                {
                    command.Connection.Close();
                }

            }
        }

        public int ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(commandText, CommandType.Text, parameters);
        }

        public object ExecuteScalar(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlCommand command = GetCommand(commandText, commandType, parameters);
            if (!command.Connection.State.HasFlag(ConnectionState.Open))
            {
                command.Connection.Open();
            }

            try
            {
                return command.ExecuteScalar();
            }
            finally
            {
                if (_transaction == null)
                {
                    command.Connection.Close();
                }
            }
        }

        public object ExecuteScalar(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteScalar(commandText, CommandType.Text, parameters);
        }

        public SqlDataReader ExecuteReader(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlCommand command = GetCommand(commandText, commandType, parameters);
            command.Connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public SqlDataReader ExecuteReader(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteReader(commandText, CommandType.Text, parameters);
        }

        public void ExecuteNonQuery(params SqlCommand[] commands)
        {
            SqlConnection connection = GetConnection();
            connection.Open();
            try
            {
                foreach (var command in commands)
                {
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public DataTable GetTable(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlCommand command = GetCommand(commandText, commandType, parameters);
            if (!command.Connection.State.HasFlag(ConnectionState.Open))
            {
                command.Connection.Open();
            }
            try
            {
                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                return dataTable;
            }
            finally
            {
                if (_transaction == null)
                {
                    command.Connection.Close();
                }
            }
        }

        public DataTable GetTable(string commandText, params SqlParameter[] parameters)
        {
            return GetTable(commandText, CommandType.Text, parameters);
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }
    }
}