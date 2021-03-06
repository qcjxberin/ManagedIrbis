﻿/* ListViewAlignment.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// ListView items alignment.
	/// </summary>
	public enum ListViewAlignment
	{
		/// <summary>
		/// Aligns items according to the list-view control's current 
		/// alignment styles (the default value).
		/// </summary>
		LVA_DEFAULT = 0x0000,

		/// <summary>
		/// Aligns items along the left edge of the window.
		/// </summary>
		LVA_ALIGNLEFT = 0x0001,

		/// <summary>
		/// Aligns items along the top edge of the window.
		/// </summary>
		LVA_ALIGNTOP = 0x0002,

		/// <summary>
		/// Snaps all icons to the nearest grid position.
		/// </summary>
		LVA_SNAPTOGRID = 0x0005
	}
}
