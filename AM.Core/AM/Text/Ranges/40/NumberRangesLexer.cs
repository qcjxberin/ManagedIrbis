#if FW40

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from NumberRanges.g4 by ANTLR 4.3

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591

namespace AM.Text.Ranges {
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.3")]
[System.CLSCompliant(false)]
public partial class NumberRangesLexer : Lexer {
	public const int
		NUMBER=1, MINUS=2, DELIMITER=3;
	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] tokenNames = {
		"'\\u0000'", "'\\u0001'", "'\\u0002'", "'\\u0003'"
	};
	public static readonly string[] ruleNames = {
		"NUMBER", "MINUS", "DELIMITER"
	};


	public NumberRangesLexer(ICharStream input)
		: base(input)
	{
		_interp = new LexerATNSimulator(this,_ATN);
	}

	public override string GrammarFileName { get { return "NumberRanges.g4"; } }

	public override string[] TokenNames { get { return tokenNames; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return _serializedATN; } }

	public static readonly string _serializedATN =
		"\x3\xAF6F\x8320\x479D\xB75C\x4880\x1605\x191C\xAB37\x2\x5\x14\b\x1\x4"+
		"\x2\t\x2\x4\x3\t\x3\x4\x4\t\x4\x3\x2\x6\x2\v\n\x2\r\x2\xE\x2\f\x3\x3\x3"+
		"\x3\x3\x4\x3\x4\x3\x4\x3\x4\x2\x2\x2\x5\x3\x2\x3\x5\x2\x4\a\x2\x5\x3\x2"+
		"\x4\a\x2\x31<\x43\\\x61\x61\x63|\xB2\x423\a\x2\v\f\xF\xF\"\"..==\x14\x2"+
		"\x3\x3\x2\x2\x2\x2\x5\x3\x2\x2\x2\x2\a\x3\x2\x2\x2\x3\n\x3\x2\x2\x2\x5"+
		"\xE\x3\x2\x2\x2\a\x10\x3\x2\x2\x2\t\v\t\x2\x2\x2\n\t\x3\x2\x2\x2\v\f\x3"+
		"\x2\x2\x2\f\n\x3\x2\x2\x2\f\r\x3\x2\x2\x2\r\x4\x3\x2\x2\x2\xE\xF\a/\x2"+
		"\x2\xF\x6\x3\x2\x2\x2\x10\x11\t\x3\x2\x2\x11\x12\x3\x2\x2\x2\x12\x13\b"+
		"\x4\x2\x2\x13\b\x3\x2\x2\x2\x4\x2\f\x3\b\x2\x2";
	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN.ToCharArray());
}
} // namespace AM.Text.Ranges

#endif