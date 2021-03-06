﻿/* DRAWITEMSTRUCT.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Drawing;
using System.Runtime.InteropServices;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// 
	/// </summary>
	[StructLayout ( LayoutKind.Sequential )]
	public struct DRAWITEMSTRUCT
	{
		/// <summary>
		/// 
		/// </summary>
		public int CtlType;
		
		/// <summary>
		/// 
		/// </summary>
		public int CtlID;
		
		/// <summary>
		/// 
		/// </summary>
		public int itemID;
		
		/// <summary>
		/// 
		/// </summary>
		public int itemAction;
		
		/// <summary>
		/// 
		/// </summary>
		public int itemState;
		
		/// <summary>
		/// 
		/// </summary>
		public IntPtr hwndItem;
		
		/// <summary>
		/// 
		/// </summary>
		public IntPtr hDC;
		
		/// <summary>
		/// 
		/// </summary>
		public Rectangle rcItem;
		
		/// <summary>
		/// 
		/// </summary>
		public IntPtr itemData;
	}
}
