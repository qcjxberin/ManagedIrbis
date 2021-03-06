﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BatchRecordFormatter.cs -- batch record formatter
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AM;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Batch
{
    /// <summary>
    /// Batch formatter for <see cref="MarcRecord"/>.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class BatchRecordFormatter
        : IEnumerable<string>
    {
        #region Events

        /// <summary>
        /// Raised on batch reading.
        /// </summary>
        public event EventHandler BatchRead;

        /// <summary>
        /// Raised when exception occurs.
        /// </summary>
#if !WINMOBILE && !PocketPC
        [CanBeNull]
#endif
        public event EventHandler<ExceptionEventArgs<Exception>> Exception;

        #endregion

        #region Properties

        /// <summary>
        /// Batch size.
        /// </summary>
        public int BatchSize { get; private set; }

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IrbisConnection Connection { get; private set; }

        /// <summary>
        /// Database name.
        /// </summary>
        [NotNull]
        public string Database { get; private set; }

        /// <summary>
        /// Format.
        /// </summary>
        [NotNull]
        public string Format { get; private set; }

        /// <summary>
        /// Total number of records formatted.
        /// </summary>
        public int RecordsFormatted { get; private set; }

        /// <summary>
        /// Number of records to format.
        /// </summary>
        public int TotalRecords { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public BatchRecordFormatter
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string database,
                [NotNull] string format,
                int batchSize,
                [NotNull] IEnumerable<int> range
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(database, "database");
            Code.NotNullNorEmpty(format, "format");
            Code.NotNull(range, "range");
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException("batchSize");
            }

            Connection = connection;
            Database = database;
            BatchSize = batchSize;
            Format = format;
            //_syncRoot = new object();

            _packages = range.Slice(batchSize).ToArray();
            TotalRecords = _packages.Sum(p => p.Length);
        }

        #endregion

        #region Private members

        private readonly int[][] _packages;

        private bool _HandleException
            (
                Exception exception
            )
        {
            EventHandler<ExceptionEventArgs<Exception>> handler
                = Exception;

            if (handler == null)
            {
                return false;
            }

            ExceptionEventArgs<Exception> arguments
                = new ExceptionEventArgs<Exception>(exception);
            handler(this, arguments);

            return arguments.Handled;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Read interval of records
        /// </summary>
        [NotNull]
        public static IEnumerable<string> Interval
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string database,
                [NotNull] string format,
                int firstMfn,
                int lastMfn,
                int batchSize
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(database, "database");
            Code.NotNullNorEmpty(format, "format");
            Code.Positive(firstMfn, "firstMfn");
            Code.Positive(lastMfn, "lastMfn");
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException("batchSize");
            }

            int maxMfn = connection.GetMaxMfn(database) - 1;
            if (maxMfn == 0)
            {
                return new string[0];
            }

            lastMfn = Math.Min(lastMfn, maxMfn);
            if (firstMfn > lastMfn)
            {
                return new string[0];
            }

            BatchRecordFormatter result = new BatchRecordFormatter
                (
                    connection,
                    database,
                    format,
                    batchSize,
                    Enumerable.Range
                    (
                        firstMfn,
                        lastMfn - firstMfn + 1
                    )
                );

            return result;
        }

        /// <summary>
        /// Считывает все записи сразу.
        /// </summary>
        [NotNull]
        public List<string> FormatAll()
        {
            List<string> result
                = new List<string>(TotalRecords);

            foreach (string record in this)
            {
                result.Add(record);
            }

            return result;
        }

        /// <summary>
        /// Search and format records.
        /// </summary>
        [NotNull]
        public IEnumerable<string> Search
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string database,
                [NotNull] string format,
                [NotNull] string searchExpression,
                int batchSize
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(database, "database");
            Code.NotNullNorEmpty(format, "format");
            Code.NotNullNorEmpty(searchExpression, "searchExpression");
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException("batchSize");
            }

            int[] found = connection.Search(searchExpression);
            if (found.Length == 0)
            {
                return new string[0];
            }

            BatchRecordFormatter reader = new BatchRecordFormatter
                (
                    connection,
                    database,
                    format,
                    batchSize,
                    found
                );

            return reader;
        }

        /// <summary>
        /// Format whole database
        /// </summary>
        [NotNull]
        public static IEnumerable<string> WholeDatabase
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string database,
                [NotNull] string format,
                int batchSize
            )
        {
            Code.NotNull(connection, "connection");
            Code.NotNullNorEmpty(database, "database");
            Code.NotNullNorEmpty(format, "format");
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException("batchSize");
            }

            int maxMfn = connection.GetMaxMfn(database) - 1;
            if (maxMfn == 0)
            {
                return new string[0];
            }

            BatchRecordFormatter result = new BatchRecordFormatter
                (
                    connection,
                    database,
                    format,
                    batchSize,
                    Enumerable.Range(1, maxMfn)
                );

            return result;
        }

        #endregion

        #region IEnumerable members

        /// <summary>
        /// Get enumerator.
        /// </summary>
        public IEnumerator<string> GetEnumerator()
        {
            foreach (int[] package in _packages)
            {
                string[] records = Connection.FormatRecords
                    (
                        Database,
                        Format,
                        package
                    );

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (!ReferenceEquals(records, null))
                {
                    RecordsFormatted += records.Length;
                    BatchRead.Raise(this);
                    foreach (string record in records)
                    {
                        yield return record;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}
