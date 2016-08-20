﻿/* PenStyle.cs --  
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// Specifies the pen style.
	/// </summary>
	[Flags]
	public enum PenStyle
	{
		/// <summary>
		/// The pen is solid.
		/// </summary>
		PS_SOLID = 0,

		/// <summary>
		/// The pen is dashed.
		/// </summary>
		PS_DASH = 1,       /* -------  */

		/// <summary>
		/// The pen is dotted.
		/// </summary>
		PS_DOT = 2,       /* .......  */

		/// <summary>
		/// The pen has alternating dashes and dots.
		/// </summary>
		PS_DASHDOT = 3,       /* _._._._  */

		/// <summary>
		/// The pen has dashes and double dots.
		/// </summary>
		PS_DASHDOTDOT = 4,       /* _.._.._  */

		/// <summary>
		/// The pen is invisible.
		/// </summary>
		PS_NULL = 5,

		/// <summary>
		/// The pen is solid. When this pen is used in any GDI 
		/// drawing function that takes a bounding rectangle, 
		/// the dimensions of the figure are shrunk so that it 
		/// fits entirely in the bounding rectangle, taking into 
		/// account the width of the pen. This applies only to 
		/// geometric pens.
		/// </summary>
		PS_INSIDEFRAME = 6,

		/// <summary>
		/// Pen defined by user.
		/// </summary>
		PS_USERSTYLE = 7,

		/// <summary>
		/// ?
		/// </summary>
		PS_ALTERNATE = 8,

		/// <summary>
		/// ?
		/// </summary>
		PS_STYLE_MASK = 0x0000000F,

		/// <summary>
		/// ?
		/// </summary>
		PS_ENDCAP_ROUND = 0x00000000,

		/// <summary>
		/// ?
		/// </summary>
		PS_ENDCAP_SQUARE = 0x00000100,

		/// <summary>
		/// ?
		/// </summary>
		PS_ENDCAP_FLAT = 0x00000200,

		/// <summary>
		/// ?
		/// </summary>
		PS_ENDCAP_MASK = 0x00000F00,

		/// <summary>
		/// ?
		/// </summary>
		PS_JOIN_ROUND = 0x00000000,

		/// <summary>
		/// ?
		/// </summary>
		PS_JOIN_BEVEL = 0x00001000,

		/// <summary>
		/// ?
		/// </summary>
		PS_JOIN_MITER = 0x00002000,

		/// <summary>
		/// ?
		/// </summary>
		PS_JOIN_MASK = 0x0000F000,

		/// <summary>
		/// ?
		/// </summary>
		PS_COSMETIC = 0x00000000,

		/// <summary>
		/// ?
		/// </summary>
		PS_GEOMETRIC = 0x00010000,

		/// <summary>
		/// ?
		/// </summary>
		PS_TYPE_MASK = 0x000F0000
	}
}