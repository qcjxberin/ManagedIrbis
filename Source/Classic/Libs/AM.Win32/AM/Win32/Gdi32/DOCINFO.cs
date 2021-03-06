﻿/* DOCINFO.cs --  
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Runtime.InteropServices;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// Contains the input and output file names and other information used 
	/// by the StartDoc function.
	/// </summary>
	[Serializable]
	[StructLayout ( LayoutKind.Sequential, Size = DOCINFO.Size )]
	public struct DOCINFO
	{
		/// <summary>
		/// Structure size.
		/// </summary>
		public const int Size = 20;

		/// <summary>
		/// Specifies the size, in bytes, of the structure.
		/// </summary>
		public int cbSize;

		/// <summary>
		/// Pointer to a null-terminated string that specifies the name 
		/// of the document.
		/// </summary>
		[MarshalAs ( UnmanagedType.LPTStr )]
		public string lpszDocName;

		/// <summary>
		/// Pointer to a null-terminated string that specifies the name of 
		/// an output file. If this pointer is NULL, the output will be sent 
		/// to the device identified by the device context handle that was 
		/// passed to the StartDoc function.
		/// </summary>
		[MarshalAs ( UnmanagedType.LPTStr )]
		public string lpszOutput;

		/// <summary>
		/// Pointer to a null-terminated string that specifies the type of data, 
		/// such as "raw" or "emf", used to record the print job. This member can 
		/// be NULL. Note that the requested data type might be ignored.
		/// </summary>
		[MarshalAs ( UnmanagedType.LPTStr )]
		public string lpszDatatype;

		/// <summary>
		/// Specifies additional information about the print job.
		/// </summary>
		public DocInfoFlags fwType;
	}
}
