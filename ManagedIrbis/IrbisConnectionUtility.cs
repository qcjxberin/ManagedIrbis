﻿/* IrbisConnectionUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AM;
using AM.Collections;

#if !NETCORE
using AM.Configuration;
#endif

using AM.IO;
using AM.Runtime;

using CodeJam;

using JetBrains.Annotations;

using ManagedIrbis.Gbl;
using ManagedIrbis.Menus;
using ManagedIrbis.Network;
using ManagedIrbis.Network.Commands;
using ManagedIrbis.Network.Sockets;
using ManagedIrbis.Search;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public static class IrbisConnectionUtility
    {
        #region Constants

        #endregion

        #region Properties

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Execute arbitrary command.
        /// </summary>
        [NotNull]
        public static ServerResponse ExecuteArbitraryCommand
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string commandCode,
                params object[] arguments
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(commandCode, "commandCode");

            UniversalCommand command = new UniversalCommand
                (
                    connection,
                    commandCode,
                    arguments
                )
            {
                AcceptAnyResponse = true
            };
            ServerResponse result = connection.ExecuteCommand(command);

            return result;
        }

        // ========================================================

        /// <summary>
        /// Read menu from server.
        /// </summary>
        [NotNull]
        public static MenuFile ReadMenu
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string fileName
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(fileName, "fileName");

            FileSpecification fileSpecification = new FileSpecification
                (
                    IrbisPath.MasterFile,
                    connection.Database,
                    fileName
                );
            string text = connection.ReadTextFile(fileSpecification);
            MenuFile result = MenuFile.ParseServerResponse(text);

            return result;
        }

        // ========================================================


        /// <summary>
        /// Remove logging from socket.
        /// </summary>
        public static void RemoveLogging
            (
                [NotNull] this IrbisConnection connection
            )
        {
            Code.NotNull(connection, "connection");

            LoggingClientSocket oldSocket = connection.Socket
                as LoggingClientSocket;
            if (!ReferenceEquals(oldSocket, null))
            {
                AbstractClientSocket newSocket = oldSocket.InnerSocket;
                connection.SetSocket(newSocket);
            }
        }

        // ========================================================

        /// <summary>
        /// Стандартные наименования для ключа строки подключения
        /// к серверу ИРБИС64.
        /// </summary>
        public static string[] ListStandardConnectionStrings()
        {
            return new[]
            {
                "irbis-connection",
                "irbis-connection-string",
                "irbis64-connection",
                "irbis64",
                "connection-string"
            };
        }

        /// <summary>
        /// Получаем строку подключения в app.settings.
        /// </summary>
        public static string GetStandardConnectionString()
        {
            return ConfigurationUtility.FindSetting
                (
                    ListStandardConnectionStrings()
                );
        }

        // ========================================================

        /// <summary>
        /// Получаем уже подключенного клиента.
        /// </summary>
        /// <exception cref="IrbisException">
        /// Если строка подключения в app.settings не найдена.
        /// </exception>
        public static IrbisConnection GetClientFromConfig()
        {
            string connectionString = GetStandardConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new IrbisException
                    (
                        "Connection string not specified!"
                    );
            }

            IrbisConnection result = new IrbisConnection(connectionString);

            return result;
        }

        // ========================================================

        [NotNull]
        public static GblResult GlobalCorrection
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string fileName,
                [NotNull] Encoding encoding,
                [CanBeNull] string database,
                [CanBeNull] string searchExpression,
                int firstRecord,
                int numberOfRecords,
                int minMfn,
                int maxMfn,
                [CanBeNull] int[] mfnList,
                bool actualize,
                bool formalControl,
                bool autoin
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(fileName, "fileName");
            Code.NotNull(encoding, "encoding");

            if (string.IsNullOrEmpty(database))
            {
                database = connection.Database;
            }

            GblFile file = GblFile.ParseLocalFile(fileName, encoding);

            GblCommand command = new GblCommand(connection)
            {
                SearchExpression = searchExpression,
                AutoIn = autoin,
                Actualize = actualize,
                FormalControl = formalControl,
                Database = database,
                FirstRecord = firstRecord,
                NumberOfRecords = numberOfRecords,
                MfnList = mfnList,
                Statements = file.Statements
                    .ThrowIfNullOrEmpty("Statements")
                    .ToArray(),
                MinMfn = minMfn,
                MaxMfn = maxMfn
            };
            connection.ExecuteCommand(command);

            return command.Result
                .ThrowIfNull("command.Result");

        }


        // ========================================================

        /// <summary>
        /// Retrieve history for given record.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static MarcRecord[] RecordHistory
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string database,
                int mfn,
                [CanBeNull] string format
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(database, "database");
            Code.Positive(mfn, "mfn");

            List<MarcRecord> result = new List<MarcRecord>();
            MarcRecord record = connection.ReadRecord
                (
                    database,
                    mfn,
                    false,
                    format
                );
            result.Add(record);

            for (int version = 2; version < int.MaxValue; version++)
            {
                record = connection.ReadRecord
                    (
                        database,
                        mfn,
                        version,
                        format
                    );
                if (ReferenceEquals(record, null))
                {
                    break;
                }
                result.Add(record);
            }

            return result.ToArray();
        }

        // ========================================================

        /// <summary>
        /// Чтение записи.
        /// </summary>
        [NotNull]
        public static MarcRecord ReadRecord
            (
                [NotNull] this IrbisConnection connection,
                int mfn
            )
        {
            Code.NotNull(connection, "connection");

            MarcRecord result = connection.ReadRecord
                (
                    connection.Database,
                    mfn,
                    false,
                    null
                );

            return result;
        }

        // ========================================================

        /// <summary>
        /// Read text file from the server.
        /// </summary>
        [CanBeNull]
        public static string ReadTextFile
            (
                [NotNull] this IrbisConnection connection,
                IrbisPath path,
                [NotNull] string fileName
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(fileName, "fileName");

            FileSpecification fileSpecification = new FileSpecification
                (
                    path,
                    connection.Database,
                    fileName
                );

            string result = connection.ReadTextFile(fileSpecification);

            return result;
        }

        // ========================================================

        /// <summary>
        /// Require minimal server version.
        /// </summary>
        public static bool RequireServerVersion
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string minimalVersion,
                bool throwException
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(minimalVersion, "minimalVersion");

            IrbisVersion actualVersion
                = connection.GetServerVersion();
            bool result = string.CompareOrdinal
                (
                    actualVersion.Version,
                    "64." + minimalVersion
                ) >= 0;

            if (!result
                 && throwException)
            {
                string message = string.Format
                    (
                        "Required server version {0}, found version {1}",
                        minimalVersion,
                        actualVersion.Version
                    );
                throw new IrbisException(message);
            }

            return result;
        }

        // ========================================================

        /// <summary>
        /// Search for records with formattable expression.
        /// </summary>
        [NotNull]
        [StringFormatMethod("format")]
        public static int[] Search
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string format,
                params object[] args
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(format, "format");

            string expression = string.Format
                (
                    format,
                    args
                );

            return connection.Search(expression);
        }

        // ========================================================

        /// <summary>
        /// Определение количества записей, которые будут
        /// найдены по указанному выражению.
        /// </summary>
        public static int SearchCount
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string searchExpression
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(searchExpression, "searchExpression");

            SearchCommand command = new SearchCommand(connection)
            {
                Database = connection.Database,
                SearchExpression = searchExpression
            };
            connection.ExecuteCommand(command);
            int result = command.FoundCount;

            return result;
        }

        // ========================================================

        /// <summary>
        /// Поиск с одновременным расформатированием.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static FoundItem[] SearchFormat
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string searchExpression,
                [NotNull] string formatSpecification
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(searchExpression, "searchExpression");
            Code.NotNullNorEmpty(formatSpecification, "formatSpecification");

            SearchCommand command = new SearchCommand(connection)
            {
                Database = connection.Database,
                SearchExpression = searchExpression,
                FormatSpecification = formatSpecification
            };
            connection.ExecuteCommand(command);
            FoundItem[] result = command.Found
                .ThrowIfNull("command.Found")
                .ToArray();

            return result;
        }

        // ========================================================

        /// <summary>
        /// Загрузка записей по результатам поиска.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        [StringFormatMethod("format")]
        public static MarcRecord[] SearchRead
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string format,
                params object[] args
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(format, "format");

            string expression = string.Format
                (
                    format,
                    args
                );

            SearchReadCommand command = new SearchReadCommand(connection)
            {
                Database = connection.Database,
                SearchExpression = expression
            };
            connection.ExecuteCommand(command);
            MarcRecord[] result = command.Records
                .ThrowIfNull("command.Records");

            return result;
        }

        // ========================================================

        /// <summary>
        /// Загрузка одной записи по результатам поиска.
        /// </summary>
        [CanBeNull]
        [StringFormatMethod("format")]
        public static MarcRecord SearchReadOneRecord
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string format,
                params object[] args
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(format, "format");

            string expression = string.Format
                (
                    format,
                    args
                );
            SearchReadCommand command = new SearchReadCommand(connection)
            {
                Database = connection.Database,
                SearchExpression = expression,
                NumberOfRecords = 1
            };
            connection.ExecuteCommand(command);
            MarcRecord result = command.Records
                .ThrowIfNull("command.Records")
                .GetItem(0);

            return result;
        }

        /// <summary>
        /// Unlock record through E command.
        /// </summary>
        public static bool UnlockRecordAlternative
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] string databaseName,
                int mfn
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(databaseName, "databaseName");

            ServerResponse response = connection.ExecuteArbitraryCommand
                (
                    "E",
                    databaseName,
                    mfn
                );
            bool result = response.GetReturnCode() >= 0;

            return result;
        }

        // ========================================================

        /// <summary>
        /// Create or update existing record in the database.
        /// </summary>
        [NotNull]
        public static MarcRecord WriteRecord
            (
                [NotNull] this IrbisConnection connection,
                [NotNull] MarcRecord record
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNull(record, "record");

            return connection.WriteRecord
                (
                    record,
                    false,
                    true
                );
        }

        #endregion
    }
}
