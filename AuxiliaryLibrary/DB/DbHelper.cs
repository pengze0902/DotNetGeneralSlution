using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Threading;
using AuxiliaryLibrary.Configuration;

namespace AuxiliaryLibrary.DB
{
    /// <summary>
    /// 数据库辅助类
    /// </summary>
    public class DbHelper
    {
        private const string SlotName = "DBConnectionSlot";

        #region 公共变量

        public static readonly DbHelper Instance = new DbHelper();
        
        #endregion

        #region 属性

        /// <summary>
        /// 获取默认的数据库连接
        /// </summary>
        /// <returns></returns>
        private ConnectionStringSettings GetDefaultConnection()
        {
            return ConfigurationHelper.GetConnectionSetting("DefaultConnectionString");
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerName">数据库提供程序名称</param>
        /// <returns><c>IDbConnection</c>对象</returns>
        private IDbConnection CreateConnection(string connectionString, string providerName)
        {
            IDbConnection connection;
            const int retryTimes = 1;
            switch (providerName)
            {
                case ProviderString.Oracle:
                    connection = new OracleConnection(connectionString);
                    break;
                case ProviderString.SqlServer:
                    connection = new SqlConnection(connectionString);
                    break;
                case ProviderString.Aceess:
                    connection = new OleDbConnection(connectionString);
                    break;
                default:
                    connection = new OracleConnection(connectionString);
                    break;
            }
            for (var i = 0; i <= retryTimes - 1; i++)
            {
                try
                {
                    connection.Open();
                    break;
                }
                catch (Exception ex)
                {
                    if (i == retryTimes - 1)
                    {
                        throw new Exception("数据库连接失败:" + ex.Message);
                    }
                }
            }
            return connection;
        }

        /// <summary>
        /// 根据数据库连接对象，获取提供程序名称
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <returns>提供程序名称</returns>
        private string GetProviderName(IDbConnection connection)
        {
            if (connection is OracleConnection)
            {
                return ProviderString.Oracle;
            }
            if (connection is SqlConnection)
            {
                return ProviderString.SqlServer;
            }
            if (connection is OleDbConnection)
            {
                return ProviderString.Aceess;
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据数据库类型，获取提供程序名称
        /// </summary>
        /// <param name="type">数据库类型</param>
        /// <returns>提供程序名称</returns>
        private string GetProviderName(DbType type)
        {
            var result = string.Empty;
            switch (type)
            {
                case DbType.Oracle:
                    result = ProviderString.Oracle;
                    break;
                case DbType.SqlServer:
                    result = ProviderString.SqlServer;
                    break;
                case DbType.Access:
                    result = ProviderString.Aceess;
                    break;
            }
            return result;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerName">提供程序名称</param>
        /// <param name="beginTransaction">是否开启数据库连接级别事务，默认为否（请勿与TransactionScope同时使用）</param>
        /// <returns><c>DbConnection</c>对象</returns>
        private Connection OpenConnection(string connectionString, string providerName, bool beginTransaction = false)
        {
            var localSlot = Thread.GetNamedDataSlot(SlotName);
            var connectionList = (Thread.GetData(localSlot) as Collection<Connection>) ??
                                 new Collection<Connection>();
            if (connectionList.Count > 0)
            {
                for (var i = connectionList.Count - 1; i >= 0; i--)
                {
                    if (connectionList[i].Conn != null && connectionString.Contains(connectionList[i].Conn.ConnectionString) &&
                        GetProviderName(connectionList[i].Conn).Equals(providerName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (connectionList[i].Conn.State == ConnectionState.Open)
                        {
                            return connectionList[i];
                        }
                        connectionList.RemoveAt(i);
                    }
                }
            }
            var conn = CreateConnection(connectionString, providerName);
            var connection = new Connection { Conn = conn };
            if (beginTransaction)
            {
                connection.Transaction = conn.BeginTransaction();
            }
            connectionList.Add(connection);
            Thread.SetData(localSlot, connectionList);
            return connection;
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="type">数据库类型</param>
        /// <param name="beginTransaction">是否开启数据库连接级别事务，默认为否（请勿与TransactionScope同时使用）</param>
        /// <returns><c>DbConnection</c>对象</returns>
        public Connection OpenConnection(string connectionString, DbType type, bool beginTransaction = false)
        {
            return OpenConnection(connectionString, GetProviderName(type), beginTransaction);
        }

        /// <summary>
        /// 打开默认数据库连接
        /// </summary>
        /// <param name="beginTransaction">是否开启数据库连接级别事务，默认为否（请勿与TransactionScope同时使用）</param>
        /// <returns><c>DbConnection</c>对象</returns>
        public Connection OpenConnection(bool beginTransaction = false)
        {
            var defaultConnectionSetting = GetDefaultConnection();
            return OpenConnection(defaultConnectionSetting.ConnectionString, defaultConnectionSetting.ProviderName, beginTransaction);
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="conn">连接实例</param>
        public void CloseConnection(Connection conn)
        {
            if (conn == null) return;
            var localSlot = Thread.GetNamedDataSlot(SlotName);
            var connectionList = (Thread.GetData(localSlot) as Collection<Connection>) ??
                                 new Collection<Connection>();
            var providerName = GetProviderName(conn.Conn);

            if (connectionList.Count > 0)
            {
                foreach (var connection in connectionList)
                {
                    if (connection.Conn.ConnectionString.Equals(conn.Conn.ConnectionString) &&
                        GetProviderName(connection.Conn).Equals(providerName))
                    {
                        if (connection.Conn.State != ConnectionState.Closed)
                        {
                            connection.Conn.Close();
                            connection.Conn.Dispose();
                        }
                        connectionList.Remove(connection);
                        break;
                    }
                }
                Thread.SetData(localSlot, connectionList);
            }
        }

        /// <summary>
        /// 关闭所有数据库连接
        /// </summary>
        public void CloseAllConnection()
        {
            var localSlot = Thread.GetNamedDataSlot(SlotName);
            var connectionList = (Thread.GetData(localSlot) as Collection<Connection>) ??
                                 new Collection<Connection>();
            if (connectionList.Count > 0)
            {
                foreach (var connection in connectionList)
                {
                    if (connection.Conn != null && connection.Conn.State != ConnectionState.Closed)
                    {
                        connection.Conn.Close();
                        connection.Conn.Dispose();
                    }
                }
            }
            Thread.SetData(localSlot, null);
        }

        public IDbCommand GetCommand(Connection connection, string commandText, CommandType commandType,
            IDictionary<string, object> parameters = null)
        {
            IDbCommand command = null;
            var providerName = GetProviderName(connection.Conn);
            switch (providerName)
            {
                case ProviderString.Oracle:
                    command = new OracleCommand
                    {
                        CommandText = commandText,
                        CommandType = commandType,
                        Connection = connection.Conn as OracleConnection
                    };
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            command.Parameters.Add(new OracleParameter(key, parameters[key] ?? DBNull.Value));
                        }
                    }
                    break;
                case ProviderString.SqlServer:
                    command = new SqlCommand
                    {
                        CommandType = commandType,
                        CommandText = commandText,
                        Connection = connection.Conn as SqlConnection
                    };
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            command.Parameters.Add(new SqlParameter(key, parameters[key] ?? DBNull.Value));
                        }
                    }
                    break;
                case ProviderString.Aceess:
                    command = new OleDbCommand
                    {
                        CommandText = commandText,
                        Connection = connection.Conn as OleDbConnection,
                        CommandType = commandType
                    };
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            command.Parameters.Add(new OleDbParameter(key, parameters[key] ?? DBNull.Value));
                        }
                    }
                    break;
            }
            if (command != null && connection.Transaction != null)
            {
                command.Transaction = connection.Transaction;
            }
            return command;
        }

        public IDbCommand GetCommand(string connectionString, DbType type, string commandText,
            CommandType commandType,
            IDictionary<string, object> parameters = null)
        {
            var connection = OpenConnection(connectionString, type);
            return GetCommand(connection, commandText, commandType, parameters);
        }

        public IDbCommand GetCommand(string connectionString, string providerName, string commandText,
            CommandType commandType,
            IDictionary<string, object> parameters = null)
        {
            var connection = OpenConnection(connectionString, providerName);
            return GetCommand(connection, commandText, commandType, parameters);
        }

        public IDbCommand GetCommand(string commandText, CommandType commandType,
            IDictionary<string, object> parameters = null)
        {
            var connection = OpenConnection();
            return GetCommand(connection, commandText, commandType, parameters);
        }

        public IDbDataAdapter GetDataAdapter(Connection connection, string commandText, CommandType commandType,
            IDictionary<string, object> parameters = null, params string[] cursorParameters)
        {
            IDbDataAdapter dataAdapter = null;
            var providerName = GetProviderName(connection.Conn);
            switch (providerName)
            {
                case ProviderString.Oracle:
                    var oracleCommand = new OracleCommand
                    {
                        CommandText = commandText,
                        Connection = connection.Conn as OracleConnection,
                        CommandType = commandType
                    };
                    if (connection.Transaction != null)
                    {
                        oracleCommand.Transaction = connection.Transaction as OracleTransaction;
                    }
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            oracleCommand.Parameters.Add(new OracleParameter(key, parameters[key] ?? DBNull.Value));
                        }
                    }
                    if (cursorParameters != null && cursorParameters.Length > 0)
                    {
                        foreach (var parameter in cursorParameters)
                        {
                            oracleCommand.Parameters.Add(new OracleParameter(parameter,
                               OracleType.Cursor)).Direction =
                                ParameterDirection.Output;
                        }
                    }
                    dataAdapter = new OracleDataAdapter(oracleCommand);
                    break;
                case ProviderString.SqlServer:
                    var sqlCommand = new SqlCommand
                    {
                        CommandText = commandText,
                        Connection = connection.Conn as SqlConnection,
                        CommandType = commandType
                    };
                    if (connection.Transaction != null)
                    {
                        sqlCommand.Transaction = connection.Transaction as SqlTransaction;
                    }
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            sqlCommand.Parameters.Add(new SqlParameter(key, parameters[key] ?? DBNull.Value));
                        }
                    }
                    dataAdapter = new SqlDataAdapter(sqlCommand);
                    break;
                case ProviderString.Aceess:
                    var accessCommand = new OleDbCommand
                    {
                        CommandText = commandText,
                        Connection = connection.Conn as OleDbConnection,
                        CommandType = commandType
                    };
                    if (connection.Transaction != null)
                    {
                        accessCommand.Transaction = connection.Transaction as OleDbTransaction;
                    }
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            accessCommand.Parameters.Add(new OleDbParameter(key, parameters[key] ?? DBNull.Value));
                        }
                    }
                    dataAdapter = new OleDbDataAdapter(accessCommand);
                    break;
            }
            return dataAdapter;
        }

        public IDbDataAdapter GetDataAdapter(string connectionString, DbType type, string commandText,
            CommandType commandType,
            IDictionary<string, object> parameters = null, params string[] cursorParameters)
        {
            var connection = OpenConnection(connectionString, type);
            return GetDataAdapter(connection, commandText, commandType, parameters, cursorParameters);
        }

        public IDbDataAdapter GetDataAdapter(string connectionString, string providerName, string commandText,
            CommandType commandType,
            IDictionary<string, object> parameters = null, params string[] cursorParameters)
        {
            var connection = OpenConnection(connectionString, providerName);
            return GetDataAdapter(connection, commandText, commandType, parameters, cursorParameters);
        }

        public IDbDataAdapter GetDataAdapter(string commandText,
            CommandType commandType,
            IDictionary<string, object> parameters = null, params string[] cursorParameters)
        {
            var connection = OpenConnection();
            return GetDataAdapter(connection, commandText, commandType, parameters, cursorParameters);
        }

        public IDataReader GetDataReader(Connection connection, string commandText, CommandType commandType,
            IDictionary<string, object> parameters = null,
            params string[] cursorParameters)
        {
            IDataReader dataReader = null;
            var providerName = GetProviderName(connection.Conn);
            switch (providerName)
            {
                case ProviderString.Oracle:
                    var command = new OracleCommand
                    {
                        CommandText = commandText,
                        Connection = connection.Conn as OracleConnection,
                        CommandType = commandType
                    };
                    if (connection.Transaction != null)
                    {
                        command.Transaction = connection.Transaction as OracleTransaction;
                    }
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            command.Parameters.Add(new OracleParameter(key, parameters[key] ?? DBNull.Value));
                        }
                    }
                    if (cursorParameters != null && cursorParameters.Length > 0)
                    {
                        foreach (var parameter in cursorParameters)
                        {
                            command.Parameters.Add(new OracleParameter(parameter, OracleType.Cursor))
                                .Direction =
                                ParameterDirection.Output;
                        }
                    }
                    dataReader = command.ExecuteReader();
                    break;
                case ProviderString.SqlServer:
                    var sqlCommand = new SqlCommand
                    {
                        CommandText = commandText,
                        Connection = connection.Conn as SqlConnection,
                        CommandType = commandType
                    };
                    if (connection.Transaction != null)
                    {
                        sqlCommand.Transaction = connection.Transaction as SqlTransaction;
                    }
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            sqlCommand.Parameters.Add(new SqlParameter(key, parameters[key] ?? DBNull.Value));
                        }
                    }
                    dataReader = sqlCommand.ExecuteReader();
                    break;
                case ProviderString.Aceess:
                    var accessCommand = new OleDbCommand
                    {
                        CommandText = commandText,
                        Connection = connection.Conn as OleDbConnection,
                        CommandType = commandType
                    };
                    if (connection.Transaction != null)
                    {
                        accessCommand.Transaction = connection.Transaction as OleDbTransaction;
                    }
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            accessCommand.Parameters.Add(new OleDbParameter(key, parameters[key] ?? DBNull.Value));
                        }
                    }
                    dataReader = accessCommand.ExecuteReader();
                    break;
            }
            return dataReader;
        }

        public IDataReader GetDataReader(string connectionString, DbType type, string commandText,
            CommandType commandType,
            IDictionary<string, object> parameters = null,
            params string[] cursorParameters)
        {
            var connection = OpenConnection(connectionString, type);
            return GetDataReader(connection, commandText, commandType, parameters, cursorParameters);
        }

        public IDataReader GetDataReader(string connectionString, string providerName, string commandText,
            CommandType commandType,
            IDictionary<string, object> parameters = null,
            params string[] cursorParameters)
        {
            var connection = OpenConnection(connectionString, providerName);
            return GetDataReader(connection, commandText, commandType, parameters, cursorParameters);
        }

        public IDataReader GetDataReader(string commandText,
            CommandType commandType,
            IDictionary<string, object> parameters = null,
            params string[] cursorParameters)
        {
            var connection = OpenConnection();
            return GetDataReader(connection, commandText, commandType, parameters, cursorParameters);
        }

        public IDataReader GetDataReader(string connectionString, string commandText,
            CommandType commandType,
            IDictionary<string, object> parameters = null,
            params string[] cursorParameters)
        {
            var connection = OpenConnection(connectionString,DbType.Oracle);
            return GetDataReader(connection, commandText, commandType, parameters, cursorParameters);
        }

        #endregion
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DbType
    {
        Oracle,
        SqlServer,
        Access
    }

    /// <summary>
    /// 数据库Provider字符串
    /// </summary>
    struct ProviderString
    {
        public const string Oracle = "System.Data.OracleClient";

        /// <summary>
        /// Sql Server
        /// </summary>
        public const string SqlServer = "System.Data.SqlClient";

        /// <summary>
        /// Access
        /// </summary>
        public const string Aceess = "System.Data.OleDb";
    }

    /// <summary>
    /// 数据库连接
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// 数据库连接接口<c>IDbConnection</c>对象
        /// </summary>
        public IDbConnection Conn { get; set; }

        /// <summary>
        /// /// <summary>
        /// 数据库事务接口<c>IDbTransaction</c>对象
        /// </summary>
        /// </summary>
        public IDbTransaction Transaction { get; set; }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
            }
        }
    }
}
