﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PftRef.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using AM;
using AM.Collections;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis.Pft.Infrastructure.Ast
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class PftRef
        : PftNode
    {
        #region Properties

        /// <summary>
        /// MFN.
        /// </summary>
        [CanBeNull]
        public PftNumeric Mfn { get; set; }

        /// <summary>
        /// Format.
        /// </summary>
        [NotNull]
        public NonNullCollection<PftNode> Format { get; private set; }

        /// <inheritdoc />
        public override IList<PftNode> Children
        {
            get
            {
                if (ReferenceEquals(_virtualChildren, null))
                {

                    _virtualChildren = new VirtualChildren();
                    List<PftNode> nodes = new List<PftNode>();
                    if (!ReferenceEquals(Mfn, null))
                    {
                        nodes.Add(Mfn);
                    }
                    nodes.AddRange(Format);
                    _virtualChildren.SetChildren(nodes);
                }

                return _virtualChildren;
            }
            protected set
            {
                // Nothing to do here
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftRef()
        {
            Format = new NonNullCollection<PftNode>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftRef
            (
                [NotNull] PftToken token
            )
            : base(token)
        {
            Code.NotNull(token, "token");
            token.MustBe(PftTokenKind.Ref);

            Format = new NonNullCollection<PftNode>();
        }

        #endregion

        #region Private members

        private VirtualChildren _virtualChildren;

        #endregion

        #region Public methods

        #endregion

        #region ICloneable members

        /// <inheritdoc/>
        public override object Clone()
        {
            PftRef result = (PftRef)base.Clone();

            if (!ReferenceEquals(Mfn, null))
            {
                result.Mfn = (PftNumeric)Mfn.Clone();
            }

            result.Format = Format.CloneNodes().ThrowIfNull();

            return result;
        }

        #endregion

        #region PftNode members

        /// <inheritdoc />
        public override void Execute
            (
                PftContext context
            )
        {
            OnBeforeExecution(context);

            if (!ReferenceEquals(Mfn, null))
            {
                context.Evaluate(Mfn);
                int mfn = (int)Mfn.Value;
                MarcRecord record = context.Environment.ReadRecord(mfn);
                if (!ReferenceEquals(record, null))
                {
                    using (new PftContextSaver(context, true))
                    {
                        context.Record = record;
                        context.Execute(Format);
                    }
                }
            }

            OnAfterExecution(context);
        }

        #endregion
    }
}
