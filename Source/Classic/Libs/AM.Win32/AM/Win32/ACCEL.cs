﻿/* ACCEL.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System.Runtime.InteropServices;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// 
	/// </summary>
	[StructLayout ( LayoutKind.Sequential )]
	public class ACCEL
	{
		/// <summary>
		/// 
		/// </summary>
		public byte fVirt;
		
		/// <summary>
		/// 
		/// </summary>
		public short key;
		
		/// <summary>
		/// 
		/// </summary>
		public short cmd;
	}
}
