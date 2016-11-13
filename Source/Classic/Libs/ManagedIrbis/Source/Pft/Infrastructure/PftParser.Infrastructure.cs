﻿/* PftParser.Infrastructure.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using AM;
using AM.Collections;
using AM.IO;
using AM.Text;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Pft.Infrastructure
{
    using Ast;

    partial class PftParser
    {
        //================================================================
        // Service variables
        //================================================================

        private bool inAssignment;

        private PftProcedureManager procedures;


        //================================================================
        // Service routines
        //================================================================

        private void ChangeContext
            (
                [NotNull] NonNullCollection<PftNode> result,
                [NotNull] PftTokenList newTokens
            )
        {
            PftTokenList saveTokens = Tokens;
            Tokens = newTokens;

            try
            {
                while (!Tokens.IsEof)
                {
                    PftNode node = ParseNext();
                    result.Add(node);
                }
            }
            finally
            {
                Tokens = saveTokens;
            }
        }

        [CanBeNull]
        private PftNode ChangeContext
            (
                [NotNull] PftTokenList newTokens,
                [NotNull] Func<PftNode> function
            )
        {
            PftNode result = null;
            PftTokenList saveTokens = Tokens;
            Tokens = newTokens;

            try
            {
                if (!Tokens.IsEof)
                {
                    result = function();
                }
            }
            finally
            {
                Tokens = saveTokens;
            }

            return result;
        }

        /// <summary>
        /// Create next AST node from token list.
        /// </summary>
        [CanBeNull]
        public PftNode Get
            (
                [NotNull] Dictionary<PftTokenKind, Func<PftNode>> map,
                [NotNull] PftTokenKind[] expectedTokens
            )
        {
            PftNode result = null;
            PftToken token = Tokens.Current;

            if (Array.IndexOf(expectedTokens, token.Kind) >= 0)
            {
                Func<PftNode> function;
                if (!map.TryGetValue(token.Kind, out function))
                {
                    throw new PftException
                        (
                            "don't know how to handle token "
                            + token.Kind
                        );
                }
                result = function();
            }

            return result;
        }

        [NotNull]
        private T MoveNext<T>([NotNull] T node)
            where T : PftNode
        {
            Tokens.MoveNext();
            return node;
        }

        private PftNode ParseCall(PftNode result)
        {
            Tokens.RequireNext();
            return ParseCall2(result);
        }

        private PftNode ParseCall2(PftNode result)
        {
            PftToken token = Tokens.Current;
            token.MustBe(PftTokenKind.LeftParenthesis);
            Tokens.RequireNext();
            return ParseCall3(result);
        }

        private PftNode ParseCall3(PftNode result)
        {
            PftTokenList innerTokens = Tokens.Segment
                (
                    _parenthesisOpen,
                    _parenthesisClose,
                    _parenthesisStop
                )
                .ThrowIfNull("innerTokens");

            PftTokenList saveTokens = Tokens;
            Tokens = innerTokens;

            try
            {
                while (!Tokens.IsEof)
                {
                    PftNode node = ParseNext();
                    result.Children.Add(node);
                }
            }
            finally
            {
                Tokens = saveTokens;
                Tokens.MoveNext();
            }

            return result;
        }

        [NotNull]
        private PftNode ParseNext()
        {
            PftNode result = Get(MainMap, SimpleTokens);

            if (!ReferenceEquals(result, null))
            {
                return result;
            }

            throw new PftSyntaxException(Tokens.Current);
        }
    }
}
