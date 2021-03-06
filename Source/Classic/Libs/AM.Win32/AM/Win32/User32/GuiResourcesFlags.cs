﻿/* GuiResourcesFlags.cs --  
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// GUI object type.
	/// </summary>
	public enum GuiResourcesFlags
	{
		/// <summary>
		/// Count of GDI objects.
		/// </summary>
		GR_GDIOBJECTS = 0,

		/// <summary>
		/// Count of USER objects.
		/// </summary>
		GR_USEROBJECTS = 1
	}
}
