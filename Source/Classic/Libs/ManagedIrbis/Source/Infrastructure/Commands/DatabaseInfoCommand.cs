﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* DatabaseInfoCommand.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis.Infrastructure.Commands
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public class DatabaseInfoCommand
        : AbstractCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// Result.
        /// </summary>
        [CanBeNull]
        public DatabaseInfo Result { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public DatabaseInfoCommand
            (
                [NotNull] IrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region AbstractCommand members

        /// <inheritdoc />
        public override ClientQuery CreateQuery()
        {
            ClientQuery result = base.CreateQuery();
            result.CommandCode = CommandCode.RecordList;

            string database = Database ?? Connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                throw new IrbisException("database not specified");
            }
            result.AddAnsi(database);

            return result;
        }

        /// <inheritdoc />
        public override ServerResponse Execute
            (
                ClientQuery query
            )
        {
            ServerResponse response = base.Execute(query);
            Result = DatabaseInfo.ParseServerResponse(response);
            Result.Name = Database;

            return response;
        }

        #endregion

        #region IVerifiable members

        /// <summary>
        /// Verify object state.
        /// </summary>
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<DatabaseInfoCommand> verifier
                = new Verifier<DatabaseInfoCommand>
                (
                    this,
                    throwOnError
                );

            verifier
                .NotNullNorEmpty(Database, "Database");

            return verifier.Result;
        }

        #endregion
    }
}
