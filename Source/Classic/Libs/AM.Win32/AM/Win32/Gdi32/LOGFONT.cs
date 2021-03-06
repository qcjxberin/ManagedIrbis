﻿/* LOGFONT.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System.Runtime.InteropServices;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// 
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct LOGFONT
	{
		/// <summary>
		/// 
		/// </summary>
		public int lfHeight;

		/// <summary>
		/// 
		/// </summary>
		public int lfWidth;

		/// <summary>
		/// 
		/// </summary>
		public int lfEscapement;

		/// <summary>
		/// 
		/// </summary>
		public int lfOrientation;

		/// <summary>
		/// 
		/// </summary>
		public int lfWeight;

		/// <summary>
		/// 
		/// </summary>
		public byte lfItalic;

		/// <summary>
		/// 
		/// </summary>
		public byte lfUnderline;

		/// <summary>
		/// 
		/// </summary>
		public byte lfStrikeOut;

		/// <summary>
		/// 
		/// </summary>
		public byte lfCharSet;

		/// <summary>
		/// 
		/// </summary>
		public byte lfOutPrecision;

		/// <summary>
		/// 
		/// </summary>
		public byte lfClipPrecision;

		/// <summary>
		/// 
		/// </summary>
		public byte lfQuality;

		/// <summary>
		/// 
		/// </summary>
		public byte lfPitchAndFamily;

		/// <summary>
		/// 
		/// </summary>
		[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 32 )]
		public string lfFaceName;
	}
}