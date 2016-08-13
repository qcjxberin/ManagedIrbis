﻿/* IrbisConnection.cs -- client for IRBIS-server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using AM;
using AM.Collections;
using AM.IO;
using AM.Runtime;
using AM.Threading;

using CodeJam;

using JetBrains.Annotations;

using ManagedIrbis.Executive;
using ManagedIrbis.Gbl;
using ManagedIrbis.Network;
using ManagedIrbis.Network.Commands;
using ManagedIrbis.Network.Sockets;
using ManagedIrbis.Search;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// Client for IRBIS-server.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class IrbisConnection
        : IDisposable,
        IHandmadeSerializable
    {
        #region Constants

        /// <summary>
        /// Таймаут получения ответа от сервера по умолчанию.
        /// </summary>
        public const int DefaultTimeout = 30000;

        #endregion

        #region Events

        /// <summary>
        /// Вызывается перед уничтожением объекта.
        /// </summary>
        public event EventHandler Disposing;

        #endregion

        #region Properties

#if !NETCORE

        /// <summary>
        /// Версия клиента.
        /// </summary>
        public static Version ClientVersion = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version;

#endif

        /// <summary>
        /// Признак занятости клиента.
        /// </summary>
        [NotNull]
        public BusyState Busy { get; private set; }

        /// <summary>
        /// Адрес сервера.
        /// </summary>
        /// <value>Адрес сервера в цифровом виде.</value>
        public string Host { get; set; }

        /// <summary>
        /// Порт сервера.
        /// </summary>
        /// <value>Порт сервера (по умолчанию 6666).</value>
        public int Port { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        /// <value>Имя пользователя.</value>
        public string Username { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        /// <value>Пароль пользователя.</value>
        public string Password { get; set; }

        /// <summary>
        /// Имя базы данных.
        /// </summary>
        /// <value>Служебное имя базы данных (например, "IBIS").</value>
        public string Database { get; set; }

        /// <summary>
        /// Тип АРМ.
        /// </summary>
        /// <value>По умолчанию <see cref="IrbisWorkstation.Cataloger"/>.
        /// </value>
        public IrbisWorkstation Workstation { get; set; }

        /// <summary>
        /// Идентификатор клиента.
        /// </summary>
        public int ClientID { get { return _clientID; } }

        /// <summary>
        /// Номер команды.
        /// </summary>
        public int QueryID { get { return _queryID; } }

        /// <summary>
        /// Executive engine.
        /// </summary>
        [NotNull]
        public AbstractEngine Executive { get; private set; }

        /// <summary>
        /// Command factory.
        /// </summary>
        [NotNull]
        public CommandFactory CommandFactory { get; private set; }

        ///// <summary>
        ///// Конфигурация клиента.
        ///// </summary>
        ///// <value>Высылается сервером при подключении.</value>
        //public string Configuration
        //{
        //    get { return _configuration; }
        //}

        /// <summary>
        /// Статус подключения к серверу.
        /// </summary>
        /// <value>Устанавливается в true при успешном выполнении
        /// <see cref="Connect"/>, сбрасывается при выполнении
        /// <see cref="Disconnect"/> или <see cref="Dispose"/>.
        /// </value>
        public bool Connected
        {
            get { return _connected; }
        }

        /// <summary>
        /// Таймаут получения ответа от сервера в миллисекундах
        /// (для продвинутых функций).
        /// </summary>
        [DefaultValue(DefaultTimeout)]
        public int Timeout { get; set; }

        /// <summary>
        /// Признак: команда прервана.
        /// </summary>
        [DefaultValue(false)]
        public bool Interrupted { get; set; }

        /// <summary>
        /// Socket.
        /// </summary>
        [NotNull]
        public AbstractClientSocket Socket { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        /// <remarks>
        /// Обратите внимание, деструктор не нужен!
        /// Он помешает сохранению состояния клиента
        /// при сериализации и последующему восстановлению,
        /// т. к. попытается закрыть уже установленное
        /// соединение. Восстановленная копия клиента
        /// ломанётся в закрытое соедиение, и выйдет облом.
        /// </remarks>
        public IrbisConnection()
        {
            Busy = new BusyState();

            Host = ConnectionSettings.DefaultHost;
            Port = ConnectionSettings.DefaultPort;
            Database = ConnectionSettings.DefaultDatabase;
            Username = null;
            Password = null;
            Workstation = ConnectionSettings.DefaultWorkstation;

            Executive = new StandardEngine(this, null);
            CommandFactory = CommandFactory
                .GetDataultFactory(this);
            Socket = new SimpleClientSocket(this);
        }

        /// <summary>
        /// Конструктор с подключением.
        /// </summary>
        public IrbisConnection
            (
                [NotNull] string connectionString
            )
            : this()
        {
            ParseConnectionString(connectionString);
            Connect();
        }

        #endregion

        #region Private members

        //private string _configuration;
        private bool _connected;

        // ReSharper disable InconsistentNaming
        private int _clientID;
        private int _queryID;
        // ReSharper restore InconsistentNaming

        private static Random _random = new Random();

#if !PocketPC
        //private IrbisSocket _socket;
#endif

        private readonly Stack<string> _databaseStack
            = new Stack<string>();

        // ReSharper disable InconsistentNaming
        internal int GenerateClientID()
        {
            _clientID = _random.Next(1000000, 9999999);

            return _clientID;
        }
        // ReSharper restore InconsistentNaming

        internal int IncrementCommandNumber()
        {
            return ++_queryID;
        }

        internal void ResetCommandNumber()
        {
            _queryID = 0;
        }

        internal void Disconnect()
        {
            Disposing.Raise
                (
                    this
                );

            if (_connected)
            {
                UniversalCommand command = new UniversalCommand
                    (
                        this,
                        CommandCode.UnregisterClient,
                        Username
                    )
                {
                    AcceptAnyResponse = true
                };

                ExecuteCommand(command);
                _connected = false;
            }
        }

        #endregion

        // =========================================================

        #region Public methods

        /// <summary>
        /// Актуализация всех неактуализированных записей
        /// в указанной базе данных.
        /// </summary>
        public void ActualizeDatabase
            (
                [NotNull] string database
            )
        {
            Code.NotNullNorEmpty(database, "database");

            ActualizeRecord
                (
                    database,
                    0
                );
        }

        // =========================================================

        /// <summary>
        /// Actualize given record (if not yet).
        /// </summary>
        /// <remarks>If MFN=0, then all non actualized
        /// records in the database will be actualized.
        /// </remarks>
        public void ActualizeRecord
            (
                [NotNull] string database,
                int mfn
            )
        {
            Code.NotNullNorEmpty(database, "database");
            Code.Nonnegative(mfn, "mfn");

            ExecuteCommand
                (
                    CommandCode.ActualizeRecord,
                    database,
                    mfn
                );
        }

        // =========================================================

        /// <summary>
        /// Clone the connection.
        /// </summary>
        [NotNull]
        public IrbisConnection Clone()
        {
            IrbisConnection result = Clone(Connected);

            return result;
        }

        /// <summary>
        /// Clone the connection.
        /// </summary>
        [NotNull]
        public IrbisConnection Clone
            (
                bool connect
            )
        {
            // TODO clone socket?

            IrbisConnection result = new IrbisConnection
            {
                Host = Host,
                Port = Port,
                Username = Username,
                Password = Password,
                Database = Database,
                Workstation = Workstation,
                Timeout = Timeout,
                // Socket = Socket.Clone ()
            };

            if (connect)
            {
                result.Connect();
            }

            return result;
        }

        // ========================================================

        /// <summary>
        /// Establish connection (if not yet).
        /// </summary>
        public string Connect()
        {
            // TODO use Executive

            if (!_connected)
            {
                ConnectCommand command
                    = CommandFactory.GetConnectCommand();
                ClientQuery query = command.CreateQuery();
                ServerResponse result = command.Execute(query);
                command.CheckResponse(result);
                _connected = true;
                return command.Configuration;
            }

            return null;
        }

        // ========================================================

        /// <summary>
        /// GBL for virtual record.
        /// </summary>
        [NotNull]
        public MarcRecord CorrectVirtualRecord
            (
                [NotNull] string database,
                [NotNull] MarcRecord record,
                [NotNull] GblStatement[] statements
            )
        {
            Code.NotNullNorEmpty(database, "database");
            Code.NotNull(record, "record");
            Code.NotNull(statements, "statements");

            GblVirtualCommand command
                = CommandFactory.GetGblVirtualCommand();
            command.Database = database;
            command.Record = record;
            command.Statements = statements;

            ExecuteCommand(command);

            return command.Result
                .ThrowIfNull("command.Result");
        }

        /// <summary>
        /// GBL for virtual record.
        /// </summary>
        [NotNull]
        public MarcRecord CorrectVirtualRecord
            (
                [NotNull] string database,
                [NotNull] MarcRecord record,
                [NotNull] string filename
            )
        {
            Code.NotNullNorEmpty(database, "database");
            Code.NotNull(record, "record");
            Code.NotNullNorEmpty(filename, "filename");

            GblVirtualCommand command
                = CommandFactory.GetGblVirtualCommand();
            command.Database = database;
            command.Record = record;
            command.FileName = filename;

            ExecuteCommand(command);

            return command.Result
                .ThrowIfNull("command.Result");
        }

        // ========================================================

        /// <summary>
        /// Create the database.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public void CreateDatabase
            (
                [NotNull] string databaseName,
                [NotNull] string description,
                bool readerAccess,
                [CanBeNull] string template
            )
        {
            Code.NotNullNorEmpty(databaseName, "databaseName");
            Code.NotNullNorEmpty(description, "description");

            CreateDatabaseCommand command
                = CommandFactory.GetCreateDatabaseCommand();
            command.Database = databaseName;
            command.Description = description;
            command.ReaderAccess = readerAccess;
            command.Template = template;

            ExecuteCommand(command);
        }

        // =========================================================

        /// <summary>
        /// Create dictionary index for specified database.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public void CreateDictionary
            (
                [NotNull] string databaseName
            )
        {
            // TODO Create CreateDictionaryCommand

            Code.NotNullNorEmpty(databaseName, "databaseName");

            UniversalCommand command
                = CommandFactory.GetUniversalCommand
                (
                    CommandCode.CreateDictionary,
                    databaseName
                );
            command.RelaxResponse = true;

            ExecuteCommand(command);
        }

        // ========================================================

        /// <summary>
        /// Delete the database.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public void DeleteDatabase
            (
                [NotNull] string databaseName
            )
        {
            // TODO Create DeleteDatabaseCommand

            Code.NotNullNorEmpty(databaseName, "databaseName");

            ExecuteCommand
                (
                    CommandCode.DeleteDatabase,
                    databaseName
                );
        }

        // =========================================================

        #region ExecuteCommand

        /// <summary>
        /// Execute any command.
        /// </summary>
        [NotNull]
        public ServerResponse ExecuteCommand
            (
                [NotNull] AbstractCommand command
            )
        {
            Code.NotNull(command, "command");

            ExecutionContext context = new ExecutionContext
                (
                    this,
                    command
                );
            ServerResponse result
                = Executive.ExecuteCommand(context);

            return result;
        }

        /// <summary>
        /// Execute command.
        /// </summary>
        [NotNull]
        public ServerResponse ExecuteCommand
            (
                [NotNull] string commandCode,
                params object[] arguments
            )
        {
            Code.NotNullNorEmpty(commandCode, "commandCode");

            UniversalCommand command
                = CommandFactory.GetUniversalCommand
                (
                    commandCode,
                    arguments
                );

            return ExecuteCommand(command);
        }

        #endregion

        // =========================================================

        #region FormatRecord

        /// <summary>
        /// Форматирование записи.
        /// </summary>
        [CanBeNull]
        public string FormatRecord
            (
                [NotNull] string format,
                int mfn
            )
        {
            Code.Positive(mfn, "mfn");
            Code.NotNull(format, "format");

            FormatCommand command = CommandFactory.GetFormatCommand();
            command.FormatSpecification = format;
            command.MfnList.Add(mfn);

            ExecuteCommand(command);

            string result = command.FormatResult
                .ThrowIfNullOrEmpty("command.FormatResult")
                [0];

            return result;
        }

        /// <summary>
        /// Форматирование записи.
        /// </summary>
        [CanBeNull]
        public string FormatRecord
            (
                [NotNull] string format,
                [NotNull] MarcRecord record
            )
        {
            Code.NotNull(format, "format");
            Code.NotNull(record, "record");

            FormatCommand command = CommandFactory.GetFormatCommand();
            command.FormatSpecification = format;
            command.VirtualRecord = record;

            ExecuteCommand(command);

            string result = command.FormatResult
                .ThrowIfNullOrEmpty("command.FormatResult")
                [0];

            return result;
        }

        /// <summary>
        /// Форматирование записей.
        /// </summary>
        [NotNull]
        public string[] FormatRecords
            (
                [NotNull] string database,
                [NotNull] string format,
                [NotNull] IEnumerable<int> mfnList
            )
        {
            Code.NotNull(mfnList, "mfnList");
            Code.NotNullNorEmpty(database, "database");
            Code.NotNull(format, "format");

            FormatCommand command = CommandFactory.GetFormatCommand();
            command.Database = database;
            command.FormatSpecification = format;
            command.MfnList.AddRange(mfnList);

            if (command.MfnList.Count == 0)
            {
                return new string[0];
            }

            ExecuteCommand(command);

            string[] result = command.FormatResult
                .ThrowIfNull("command.FormatResult");

            return result;
        }

        #endregion

        // =========================================================

        /// <summary>
        /// Получение информации о базе данных.
        /// </summary>
        /// <returns>Cписок логически удаленных, физически удаленных, 
        /// неактуализированных и заблокированных записей.</returns>
        [NotNull]
        public DatabaseInfo GetDatabaseInfo
            (
                [NotNull] string databaseName
            )
        {
            // TODO create DatabaseInfoCommand

            Code.NotNullNorEmpty(databaseName, "databaseName");

            UniversalCommand command
                = CommandFactory.GetUniversalCommand
                (
                    CommandCode.RecordList,
                    databaseName
                );

            ServerResponse response = ExecuteCommand(command);
            DatabaseInfo result
                = DatabaseInfo.ParseServerResponse(response);
            result.Name = databaseName;

            return result;
        }

        /// <summary>
        /// Get stat for the database.
        /// </summary>
        [NotNull]
        public string GetDatabaseStat
            (
                [NotNull] StatDefinition definition
            )
        {
            Code.NotNull(definition, "definition");

            DatabaseStatCommand command
                = CommandFactory.GetDatabaseStatCommand();
            command.Definition = definition;

            ExecuteCommand(command);
            string result = command.Result
                .ThrowIfNull("command.Result");

            return result;
        }

        // =========================================================

        /// <summary>
        /// Get next mfn for current database.
        /// </summary>
        public int GetMaxMfn()
        {
            return GetMaxMfn(Database);
        }

        /// <summary>
        /// Get next mfn for given database.
        /// </summary>
        public int GetMaxMfn
            (
                [CanBeNull] string database
            )
        {
            // TODO Create GetMaxMfnCommand

            if (ReferenceEquals(database, null))
            {
                database = Database;
            }

            UniversalCommand command
                = CommandFactory.GetUniversalCommand
                (
                    CommandCode.GetMaxMfn,
                    database
                );

            ServerResponse response = ExecuteCommand(command);
            int result = response.ReturnCode;

            return result;
        }

        // =========================================================

        /// <summary>
        /// Get server stat.
        /// </summary>
        [NotNull]
        public ServerStat GetServerStat()
        {
            // TODO Create ServerStatCommand

            ServerResponse response = ExecuteCommand
                (
                    CommandCode.GetServerStat
                );
            ServerStat result = ServerStat.Parse(response);

            return result;
        }

        // =========================================================

        /// <summary>
        /// Get server version.
        /// </summary>
        [NotNull]
        public IrbisVersion GetServerVersion()
        {
            // TODO Create ServerVersionCommand

            ServerResponse response
                = ExecuteCommand(CommandCode.ServerInfo);
            IrbisVersion result
                = IrbisVersion.ParseServerResponse(response);

            return result;
        }

        // =========================================================

        /// <summary>
        /// Global correction.
        /// </summary>
        [NotNull]
        public GblResult GlobalCorrection
            (
                [CanBeNull] string database,
                [CanBeNull] string searchExpression,
                int firstRecord,
                int numberOfRecords,
                int minMfn,
                int maxMfn,
                [CanBeNull] int[] mfnList,
                bool actualize,
                bool formalControl,
                bool autoin,
                [NotNull] GblStatement[] statements
            )
        {
            Code.NotNull(statements, "statements");

            if (string.IsNullOrEmpty(database))
            {
                database = Database;
            }

            GblCommand command = CommandFactory.GetGblCommand();
            command.SearchExpression = searchExpression;
            command.AutoIn = autoin;
            command.Actualize = actualize;
            command.FormalControl = formalControl;
            command.Database = database;
            command.FirstRecord = firstRecord;
            command.NumberOfRecords = numberOfRecords;
            command.MfnList = mfnList;
            command.Statements = statements;
            command.MinMfn = minMfn;
            command.MaxMfn = maxMfn;

            ExecuteCommand(command);

            return command.Result
                .ThrowIfNull("command.Result");
        }

        /// <summary>
        /// Global correction.
        /// </summary>
        /// <remarks>Filename = @filename without
        /// extension</remarks>
        [NotNull]
        public GblResult GlobalCorrection
            (
                [CanBeNull] string database,
                [CanBeNull] string searchExpression,
                int firstRecord,
                int numberOfRecords,
                int minMfn,
                int maxMfn,
                [CanBeNull] int[] mfnList,
                bool actualize,
                bool formalControl,
                bool autoin,
                [NotNull] string fileName
            )
        {
            Code.NotNullNorEmpty(fileName, "fileName");

            if (string.IsNullOrEmpty(database))
            {
                database = Database;
            }

            GblCommand command = CommandFactory.GetGblCommand();
            command.SearchExpression = searchExpression;
            command.AutoIn = autoin;
            command.Actualize = actualize;
            command.FormalControl = formalControl;
            command.Database = database;
            command.FirstRecord = firstRecord;
            command.NumberOfRecords = numberOfRecords;
            command.MfnList = mfnList;
            command.FileName = fileName;
            command.MinMfn = minMfn;
            command.MaxMfn = maxMfn;

            ExecuteCommand(command);

            return command.Result
                .ThrowIfNull("command.Result");
        }

        // =========================================================

        /// <summary>
        /// Get list of the databases.
        /// </summary>
        [NotNull]
        public DatabaseInfo[] ListDatabases
            (
                [NotNull] string listFile
            )
        {
            // TODO Create ListDatabasesCommand

            Code.NotNull(listFile, "listFile");

            string menuFile = this.ReadTextFile
                (
                    IrbisPath.Data,
                    listFile
                );
            string[] lines = menuFile.SplitLines();
            DatabaseInfo[] result
                = DatabaseInfo.ParseMenu(lines);

            return result;
        }

        /// <summary>
        /// Get list of the databases.
        /// </summary>
        [NotNull]
        public DatabaseInfo[] ListDatabases()
        {
            return ListDatabases
                (
                    IrbisConstants.AdministratorDatabaseList
                );
        }

        // =========================================================

        /// <summary>
        /// List server files by the specification.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public string[] ListFiles
            (
                [NotNull] FileSpecification specification
            )
        {
            Code.NotNull(specification, "specification");

            specification.Verify(true);

            ListFilesCommand command = new ListFilesCommand(this);
            command.Specifications.Add(specification);

            ExecuteCommand(command);

            string[] result = command.Files;
            if (ReferenceEquals(result, null))
            {
                throw new IrbisException("file list is null");
            }

            return result;
        }

        /// <summary>
        /// List server files by the specification.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public string[] ListFiles
            (
                [NotNull] FileSpecification[] specifications
            )
        {
            Code.NotNull(specifications, "specifications");

            ListFilesCommand command = new ListFilesCommand(this);
            foreach (FileSpecification specification in specifications)
            {
                specification.Verify(true);
                command.Specifications.Add(specification);
            }

            ExecuteCommand(command);

            string[] result = command.Files;
            if (ReferenceEquals(result, null))
            {
                throw new IrbisException("file list is null");
            }

            return result;
        }

        // =========================================================

        /// <summary>
        /// List server processes.
        /// </summary>
        [NotNull]
        public IrbisProcessInfo[] ListProcesses()
        {
            // TODO Create ListProcessesCommand

            ServerResponse response = ExecuteCommand
                (
                    CommandCode.GetProcessList
                );
            IrbisProcessInfo[] result
                = IrbisProcessInfo.Parse(response);

            return result;
        }

        // =========================================================

        /// <summary>
        /// List users.
        /// </summary>
        [NotNull]
        public UserInfo[] ListUsers()
        {
            // TODO Create ListUsersCommand

            ServerResponse response = ExecuteCommand
                (
                    CommandCode.GetUserList
                );
            UserInfo[] result = UserInfo.Parse(response);

            return result;
        }

        // =========================================================

        /// <summary>
        /// No operation.
        /// </summary>
        public void NoOp()
        {
            // TODO Create NopCommand

            ExecuteCommand(CommandCode.Nop);
        }

        /// <summary>
        /// Парсинг строки подключения.
        /// </summary>
        public void ParseConnectionString
            (
                [NotNull] string connectionString
            )
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }
            connectionString = Regex.Replace
                (
                    connectionString,
                    @"\s+",
                    string.Empty
                );
            if (string.IsNullOrEmpty(connectionString)
                 || !connectionString.Contains("="))
            {
                throw new ArgumentException("connectionString");
            }

            Regex regex = new Regex
                (
                    "(?<name>[^=;]+?)=(?<value>[^;]+)",
                    RegexOptions.IgnoreCase
                    | RegexOptions.IgnorePatternWhitespace
                );
            MatchCollection matches = regex.Matches(connectionString);
            foreach (Match match in matches)
            {
                string name =
                    match.Groups["name"].Value.ToLower();
                string value = match.Groups["value"].Value;
                switch (name)
                {
                    case "host":
                    case "server":
                    case "address":
                        Host = value;
                        break;
                    case "port":
                        Port = int.Parse(value);
                        break;
                    case "user":
                    case "username":
                    case "name":
                    case "login":
                        Username = value;
                        break;
                    case "pwd":
                    case "password":
                        Password = value;
                        break;
                    case "db":
                    case "catalog":
                    case "database":
                        Database = value;
                        break;
                    case "arm":
                    case "workstation":
                        Workstation = (IrbisWorkstation)(byte)(value[0]);
                        break;
                    case "log":
                        SetLogging(value);
                        break;
                    //case "data":
                    //    UserData = value;
                    //    break;
                    //case "debug":
                    //    StartDebug(value);
                    //    break;
                    //case "etr":
                    //case "stage":
                    //    StageOfWork = value;
                    //    break;
                    default:
                        throw new ArgumentException("connectionString");
                }
            }
        }

        /// <summary>
        /// Возврат к предыдущей базе данных.
        /// </summary>
        /// <returns>Текущая база данных.</returns>
        [CanBeNull]
        public string PopDatabase()
        {
            string result = Database;

            if (_databaseStack.Count != 0)
            {
                Database = _databaseStack.Pop();
            }

            return result;
        }

        /// <summary>
        /// Print table.
        /// </summary>
        [NotNull]
        public string PrintTable
            (
                [NotNull] TableDefinition tableDefinition
            )
        {
            Code.NotNull(tableDefinition, "tableDefinition");

            PrintTableCommand command
                = CommandFactory.GetPrintTableCommand();
            command.Definition = tableDefinition;

            ExecuteCommand(command);

            return command.Result;
        }

        /// <summary>
        /// Временное переключение на другую базу данных.
        /// </summary>
        /// <returns>Предыдущая база данных.</returns>
        [CanBeNull]
        public string PushDatabase
            (
                [NotNull] string newDatabase
            )
        {
            Code.NotNullNorEmpty(newDatabase, "newDatabase");

            string result = Database;
            _databaseStack.Push(Database);
            Database = newDatabase;

            return result;
        }

        // ========================================================

        /// <summary>
        /// Read binary file from server file system.
        /// </summary>
        [CanBeNull]
        public byte[] ReadBinaryFile
            (
                [NotNull] FileSpecification file
            )
        {
            Code.NotNull(file, "file");

            ReadBinaryFileCommand command
                = CommandFactory.GetReadBinaryFileCommand();
            command.File = file;

            ExecuteCommand(command);

            return command.Content;
        }

        // ========================================================

        /// <summary>
        /// Read term postings.
        /// </summary>
        [NotNull]
        public TermPosting[] ReadPostings
            (
                [CanBeNull] string databaseName,
                [NotNull] string term,
                int numberOfPostings,
                int firstPosting,
                [CanBeNull] string format
            )
        {
            Code.NotNullNorEmpty(term, "term");

            ReadPostingsCommand command
                = CommandFactory.GetReadPostingsCommand();
            command.Database = databaseName;
            command.Term = term;
            command.NumberOfPostings = numberOfPostings;
            command.FirstPosting = firstPosting;
            command.Format = format;

            ExecuteCommand(command);

            return command.Postings
                .ThrowIfNull("command.Postings");
        }

        /// <summary>
        /// Read term postings.
        /// </summary>
        [NotNull]
        public TermPosting[] ReadPostings
            (
                [CanBeNull] string databaseName,
                [NotNull] string[] terms,
                int numberOfPostings,
                int firstPosting,
                [CanBeNull] string format
            )
        {
            Code.NotNull(terms, "terms");

            if (terms.Length == 0)
            {
                return new TermPosting[0];
            } 

            ReadPostingsCommand command
                = CommandFactory.GetReadPostingsCommand();
            command.Database = databaseName;
            command.ListOfTerms = terms;
            command.NumberOfPostings = numberOfPostings;
            command.FirstPosting = firstPosting;
            command.Format = format;

            ExecuteCommand(command);

            return command.Postings.ThrowIfNull("command.Postings");
        }


        // ========================================================

        #region ReadRecord

        /// <summary>
        /// Чтение, блокирование и расформатирование записи.
        /// </summary>
        [NotNull]
        public MarcRecord ReadRecord
            (
                [NotNull] string database,
                int mfn,
                bool lockFlag,
                [CanBeNull] string format
            )
        {
            Code.NotNullNorEmpty(database, "database");
            Code.Positive(mfn, "mfn");

            ReadRecordCommand command
                = CommandFactory.GetReadRecordCommand();
            command.Mfn = mfn;
            command.Database = database;
            command.Lock = lockFlag;
            command.Format = format;

            ExecuteCommand(command);

            return command.ReadRecord
                .ThrowIfNull("no record retrieved");
        }

        /// <summary>
        /// Чтение указанной версии и расформатирование записи.
        /// </summary>
        /// <remarks><c>null</c>означает, что затребованной
        /// версии записи нет.</remarks>
        [CanBeNull]
        public MarcRecord ReadRecord
            (
                [NotNull] string database,
                int mfn,
                int versionNumber,
                [CanBeNull] string format
            )
        {
            Code.NotNullNorEmpty(database, "database");
            Code.Positive(mfn, "mfn");

            ReadRecordCommand command
                = CommandFactory.GetReadRecordCommand();
            command.Mfn = mfn;
            command.Database = database;
            command.VersionNumber = versionNumber;
            command.Format = format;

            ExecuteCommand(command);

            return command.ReadRecord;
        }

        /// <summary>
        /// Read multiple records.
        /// </summary>
        [NotNull]
        public MarcRecord[] ReadRecords
            (
                [CanBeNull] string database,
                [NotNull] IEnumerable<int> mfnList
            )
        {
            Code.NotNull(mfnList, "mfnList");

            if (string.IsNullOrEmpty(database))
            {
                database = Database;
            }

            FormatCommand command = CommandFactory.GetFormatCommand();
            command.Database = database;
            command.FormatSpecification = IrbisFormat.All;
            command.MfnList.AddRange(mfnList);

            if (command.MfnList.Count == 0)
            {
                return new MarcRecord[0];
            }

            if (command.MfnList.Count == 1)
            {
                int mfn = command.MfnList[0];

                MarcRecord record = ReadRecord
                    (
                        database,
                        mfn,
                        false,
                        null
                    );

                return new[] {record};
            }

            ExecuteCommand(command);

            MarcRecord[] result = MarcRecordUtility.ParseAllFormat
                (
                    database,
                    this,
                    command.FormatResult
                        .ThrowIfNullOrEmpty("command.FormatResult")
                );
            Debug.Assert
                (
                    command.MfnList.Count == result.Length,
                    "some records not retrieved"
                );

            return result;
        }

        #endregion

        // ========================================================

        /// <summary>
        /// Read search terms from index.
        /// </summary>
        [NotNull]
        public TermInfo[] ReadTerms
            (
                [NotNull] string startTerm,
                int numberOfTerms,
                bool reverseOrder,
                [CanBeNull] string format
            )
        {
            Code.NotNull(startTerm, "startTerm");

            ReadTermsCommand command
                = CommandFactory.GetReadTermsCommand();
            command.Database = Database;
            command.StartTerm = startTerm;
            command.NumberOfTerms = numberOfTerms;
            command.ReverseOrder = reverseOrder;
            command.Format = format;

            ExecuteCommand(command);

            return command.Terms.ThrowIfNull("command.Terms");
        }

        // ========================================================

        /// <summary>
        /// Read text file from the server.
        /// </summary>
        [CanBeNull]
        public string ReadTextFile
            (
                [NotNull] FileSpecification fileSpecification
            )
        {
            Code.NotNull(fileSpecification, "fileSpecification");

            ReadFileCommand command = new ReadFileCommand(this);
            command.Files.Add(fileSpecification);

            ExecuteCommand(command);
            string result = command.Result
                .ThrowIfNullOrEmpty("command.Result")[0];

            return result;
        }

        /// <summary>
        /// Чтение текстовых файлов с сервера.
        /// </summary>
        [NotNull]
        public string[] ReadTextFiles
            (
                [NotNull] FileSpecification[] files
            )
        {
            Code.NotNull(files, "files");

            if (files.Length == 0)
            {
                return new string[0];
            }

            ReadFileCommand command
                = CommandFactory.GetReadFileCommand();
            command.Files.AddRange(files);

            ExecuteCommand(command);

            string[] result = command.Result
                .ThrowIfNullOrEmpty("command.Result");

            return result;
        }

        // =========================================================

        /// <summary>
        /// Reload dictionary index for specified database.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public void ReloadDictionary
            (
                [NotNull] string databaseName
            )
        {
            // TODO Create ReloadDictionaryCommand

            Code.NotNullNorEmpty(databaseName, "databaseName");

            ExecuteCommand
                (
                    CommandCode.ReloadDictionary,
                    databaseName
                );
        }

        // =========================================================

        /// <summary>
        /// Reload master file for specified database.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public void ReloadMasterFile
            (
                [NotNull] string databaseName
            )
        {
            // TODO Create ReloadMasterFileCommand

            Code.NotNullNorEmpty(databaseName, "databaseName");

            ExecuteCommand
                (
                    CommandCode.ReloadMasterFile,
                    databaseName
                );
        }

        // =========================================================

        /// <summary>
        /// Restart server.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public void RestartServer()
        {
            // TODO Create RestartServerCommand

            ExecuteCommand(CommandCode.RestartServer);
        }

        // =========================================================

        /// <summary>
        /// Поиск записей.
        /// </summary>
        [NotNull]
        public int[] Search
            (
                [NotNull] string expression
            )
        {
            Code.NotNull(expression, "expression");

            SearchCommand command = CommandFactory.GetSearchCommand();
            command.SearchExpression = expression;

            ExecuteCommand(command);

            int[] result = FoundItem.ConvertToMfn
                (
                    command.Found.ThrowIfNull("Found")
                );

            return result;
        }

        // =========================================================

        /// <summary>
        /// Sequential search.
        /// </summary>
        [NotNull]
        public int[] SequentialSearch
            (
                [CanBeNull] string database,
                [NotNull] string expression,
                int firstRecord,
                int numberOfRecords,
                int minMfn,
                int maxMfn,
                [NotNull] string sequential,
                [CanBeNull] string format
            )
        {
            Code.NotNull(expression, "expression");
            Code.NotNull(sequential, "sequential");

            if (string.IsNullOrEmpty(database))
            {
                database = Database;
            }

            SearchCommand command = CommandFactory.GetSearchCommand();
            command.Database = database;
            command.SearchExpression = expression;
            command.FirstRecord = firstRecord;
            command.NumberOfRecords = numberOfRecords;
            command.MinMfn = minMfn;
            command.MaxMfn = maxMfn;
            command.SequentialSpecification = sequential;
            command.FormatSpecification = format;

            ExecuteCommand(command);

            int[] result = FoundItem.ConvertToMfn
                (
                    command.Found.ThrowIfNull("Found")
                );

            return result;
        }

        // =========================================================

        /// <summary>
        /// Set execution engine.
        /// </summary>
        public void SetEngine
            (
                [NotNull] AbstractEngine engine
            )
        {
            Code.NotNull(engine, "engine");

            Executive = engine;
        }

        // =========================================================

        /// <summary>
        /// Set logging socket, gather debug info to specified path.
        /// </summary>
        public void SetLogging
            (
                [NotNull] string loggingPath
            )
        {
            Code.NotNullNorEmpty(loggingPath, "loggingPath");

            AbstractClientSocket oldSocket = Socket;
            if (oldSocket is LoggingClientSocket)
            {
                return;
            }

            LoggingClientSocket newSocket = new LoggingClientSocket
                (
                    this,
                    Socket,
                    loggingPath
                );

            DirectoryUtility.ClearDirectory(loggingPath);

            SetSocket(newSocket);
        }

        // =========================================================

        /// <summary>
        /// 
        /// </summary>
        public void SetRetry
            (
                int retryCount,
                [CanBeNull] Func<Exception, bool> resolver
            )
        {
            RetryClientSocket oldSocket = Socket
                as RetryClientSocket;

            if (retryCount <= 0)
            {
                if (!ReferenceEquals(oldSocket, null))
                {
                    SetSocket(oldSocket.InnerSocket);
                }
            }
            else
            {
                RetryClientSocket newSocket = new RetryClientSocket
                    (
                        this,
                        Socket,
                        new RetryManager(retryCount, resolver)
                    );

                if (ReferenceEquals(oldSocket, null))
                {
                    SetSocket(newSocket);
                }
            }
        }

        // =========================================================

        /// <summary>
        /// Set
        /// <see cref="T:ManagedIrbis.Network.Sockets.AbstractClientSocket"/>.
        /// </summary>
        public void SetSocket
            (
                [NotNull] string typeName
            )
        {
            Code.NotNullNorEmpty(typeName, "typeName");

            Type type = Type.GetType(typeName, true);
            AbstractClientSocket socket
                = (AbstractClientSocket) Activator.CreateInstance(type);
            SetSocket(socket);
        }

        /// <summary>
        /// Set
        /// <see cref="T:ManagedIrbis.Network.Sockets.AbstractClientSocket"/>.
        /// </summary>
        public void SetSocket
            (
                [NotNull] AbstractClientSocket socket
            )
        {
            Code.NotNull(socket, "socket");

            Socket = socket;
        }

        // =========================================================

        /// <summary>
        /// Опустошение базы данных.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public void TruncateDatabase
            (
                [NotNull] string databaseName
            )
        {
            // TODO TruncateDatabaseCommand

            Code.NotNullNorEmpty(databaseName, "databaseName");

            ExecuteCommand
                (
                    CommandCode.EmptyDatabase,
                    databaseName
                );
        }

        // =========================================================

        /// <summary>
        /// Unlock specified database.
        /// </summary>
        public void UnlockDatabase
            (
                [NotNull] string databaseName
            )
        {
            // TODO UnlockDatabaseCommand

            Code.NotNullNorEmpty(databaseName, "databaseName");

            ExecuteCommand
                (
                    CommandCode.UnlockDatabase,
                    databaseName
                );
        }

        // =========================================================

        /// <summary>
        /// Unlock specified records.
        /// </summary>
        public void UnlockRecords
            (
                [NotNull] string database,
                params int[] mfnList
            )
        {
            // TODO UnlockRecordsCommand

            Code.NotNullNorEmpty(database, "database");

            if (mfnList.Length == 0)
            {
                return;
            }

            List<object> arguments
                = new List<object>(mfnList.Length + 1) { Database };
            arguments.AddRange(mfnList.Cast<object>());

            ExecuteCommand
                (
                    CommandCode.UnlockRecords,
                    arguments.ToArray()
                );
        }

        // =========================================================

        /// <summary>
        /// Update server INI-file for current client.
        /// </summary>
        public void UpdateIniFile
            (
                [NotNull] string[] text
            )
        {
            // TODO UpdateIniFileCommand

            Code.NotNull(text, "text");

            if (text.Length == 0)
            {
                return;
            }

            UniversalTextCommand command =
                CommandFactory.GetUniversalTextCommand
                (
                    CommandCode.UpdateIniFile,
                    text,
                    IrbisEncoding.Ansi
                );

            ExecuteCommand(command);
        }

        // ========================================================

        /// <summary>
        /// Update user list on the server.
        /// </summary>
        public void UpdateUserList
            (
                [NotNull] UserInfo[] userList
            )
        {
            Code.NotNull(userList, "userList");

            UpdateUserListCommand command
                = CommandFactory.GetUpdateUserListCommand();
            command.UserList = userList;

            ExecuteCommand(command);
        }

        // ========================================================

        #region WriteRecord

        /// <summary>
        /// Create or update existing record in the database.
        /// </summary>
        [NotNull]
        public MarcRecord WriteRecord
            (
                [NotNull] MarcRecord record,
                bool lockFlag,
                bool actualize
            )
        {
            Code.NotNull(record, "record");

            WriteRecordCommand command = new WriteRecordCommand(this)
            {
                Record = record,
                Actualize = actualize,
                Lock = lockFlag
            };

            ExecuteCommand(command);

            MarcRecord result = command.Record;

            if (ReferenceEquals(result, null))
            {
                throw new IrbisException("result record is null");
            }

            return result;
        }

        // ========================================================

        /// <summary>
        /// Create or update many records.
        /// </summary>
        [NotNull]
        public MarcRecord[] WriteRecords
            (
                [NotNull] MarcRecord[] records,
                bool lockFlag,
                bool actualize
            )
        {
            Code.NotNull(records, "records");

            if (records.Length == 0)
            {
                return records;
            }

            WriteRecordsCommand command = new WriteRecordsCommand(this)
            {
                Actualize = actualize,
                Lock = lockFlag
            };
            foreach (MarcRecord record in records)
            {
                RecordReference reference = new RecordReference(record)
                {
                    HostName = Host,
                    Database = Database
                };
                command.References.Add(reference);
            }

            ExecuteCommand(command);

            return records;
        }


        #endregion

        // ========================================================

        /// <summary>
        /// Write text file to the server.
        /// </summary>
        public void WriteTextFile
            (
                [NotNull] FileSpecification file
            )
        {
            Code.NotNull(file, "file");

            WriteFileCommand command
                = CommandFactory.GetWriteFileCommand();
            command.Files.Add(file);

            ExecuteCommand(command);
        }

        /// <summary>
        /// Write text files to the server.
        /// </summary>
        public void WriteTextFiles
            (
                params FileSpecification[] files
            )
        {
            WriteFileCommand command
                = CommandFactory.GetWriteFileCommand();
            foreach (FileSpecification file in files)
            {
                command.Files.Add(file);
            }

            ExecuteCommand(command);
        }

        #endregion

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated
        /// with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }

        #endregion

        #region IHandmadeSerializable members

        /// <summary>
        /// Просим объект восстановить свое состояние из потока.
        /// </summary>
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Host = reader.ReadNullableString();
            Port = reader.ReadPackedInt32();
            Username = reader.ReadNullableString();
            Password = reader.ReadNullableString();
            Database = reader.ReadNullableString();
            Workstation = (IrbisWorkstation)reader.ReadPackedInt32();
        }

        /// <summary>
        /// Просим объект сохранить себя в потоке.
        /// </summary>
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            writer
                .WriteNullable(Host)
                .WritePackedInt32(Port)
                .WriteNullable(Username)
                .WriteNullable(Password)
                .WriteNullable(Database)
                .WritePackedInt32((int)Workstation);
        }

        #endregion

        #region Object members

        #endregion
    }
}