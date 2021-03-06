﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PftWith.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public sealed class PftWith
        : PftNode
    {
        #region Properties

        /// <summary>
        /// Variable reference.
        /// </summary>
        [CanBeNull]
        public PftVariableReference Variable { get; set; }

        /// <summary>
        /// Fields.
        /// </summary>
        [NotNull]
        public NonNullCollection<FieldSpecification> Fields { get; private set; }

        /// <inheritdoc/>
        public override bool ExtendedSyntax
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override IList<PftNode> Children
        {
            get
            {
                if (ReferenceEquals(_virtualChildren, null))
                {

                    _virtualChildren = new VirtualChildren();
                    List<PftNode> nodes = new List<PftNode>();
                    if (!ReferenceEquals(Variable, null))
                    {
                        nodes.Add(Variable);
                    }
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
        public PftWith()
        {
            Fields = new NonNullCollection<FieldSpecification>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftWith
            (
                [NotNull] PftToken token
            )
            : base(token)
        {
            Code.NotNull(token, "token");
            token.MustBe(PftTokenKind.With);

            Fields = new NonNullCollection<FieldSpecification>();
        }

        #endregion

        #region Private members

        private VirtualChildren _virtualChildren;

        #endregion

        #region Public methods

        #endregion

        #region ICloneable members

        /// <inheritdoc />
        public override object Clone()
        {
            PftWith result = (PftWith) base.Clone();

            result._virtualChildren = null;

            if (!ReferenceEquals(Variable, null))
            {
                result.Variable = (PftVariableReference) Variable.Clone();
            }

            result.Fields = new NonNullCollection<FieldSpecification>();
            foreach (FieldSpecification field in Fields)
            {
                //result.Fields.Add(field.Clone());
            }

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

            if (ReferenceEquals(Variable, null))
            {
                throw new PftException("Variable");
            }

            string name = Variable.Name
                .ThrowIfNull("Variable.Name");

            PftContext localContext = context.Push();
            localContext.Output = context.Output;

            PftVariableManager localManager
                = new PftVariableManager(context.Variables);
            
            localContext.SetVariables(localManager);
            
            PftVariable variable = new PftVariable
                (
                    name,
                    false
                );
            
            localManager.Registry.Add
                (
                    name,
                    variable
                );

            foreach (FieldSpecification field in Fields)
            {
                string tag = field.Tag.ThrowIfNull("field.Tag");
                string[] lines = field.SubField == SubField.NoCode
                    ? PftUtility.GetFieldValue
                        (
                            context,
                            tag,
                            field.FieldRepeat
                        )
                    : PftUtility.GetSubFieldValue
                    (
                        context,
                        tag,
                        field.FieldRepeat,
                        field.SubField,
                        field.SubFieldRepeat
                    );

                List<string> lines2 = new List<string>();
                foreach (string line in lines)
                {
                    variable.StringValue = line;

                    localContext.Execute(Children);

                    string value = variable.StringValue;
                    if (!string.IsNullOrEmpty(value))
                    {
                        lines2.Add(value);
                    }
                }

                bool flag = lines2.Count != lines.Length;
                if (!flag)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i] != lines2[i])
                        {
                            flag = true;
                            break;
                        }
                    }
                }

                if (flag)
                {
                    string value = string.Join
                        (
                            Environment.NewLine,
                            lines2.ToArray()
                        );

                    if (field.SubField == SubField.NoCode)
                    {
                        PftUtility.AssignField
                            (
                                context,
                                tag,
                                field.FieldRepeat,
                                value
                            );
                    }
                    else
                    {
                        PftUtility.AssignSubField
                            (
                                context,
                                tag,
                                field.FieldRepeat,
                                field.SubField,
                                field.SubFieldRepeat,
                                value
                            );
                    }
                }
            }

            OnAfterExecution(context);
        }

        #endregion
    }
}
