﻿/* FlashWindowFlags.cs --  
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// Flags for FlashWindowEx function.
	/// </summary>
	[Flags]
	public enum FlashWindowFlags
	{
		/// <summary>
		/// Stop flashing. The system restores the window to its 
		/// original state.
		/// </summary>
		FLASHW_STOP = 0,

		/// <summary>
		/// Flash the window caption.
		/// </summary>
		FLASHW_CAPTION = 0x00000001,

		/// <summary>
		/// Flash the taskbar button.
		/// </summary>
		FLASHW_TRAY = 0x00000002,

		/// <summary>
		/// Flash both the window caption and taskbar button. 
		/// This is equivalent to setting the 
		/// FLASHW_CAPTION | FLASHW_TRAY flags.
		/// </summary>
		FLASHW_ALL = FLASHW_CAPTION | FLASHW_TRAY,

		/// <summary>
		/// Flash continuously, until the FLASHW_STOP flag is set.
		/// </summary>
		FLASHW_TIMER = 0x00000004,

		/// <summary>
		/// Flash continuously until the window comes to the foreground.
		/// </summary>
		FLASHW_TIMERNOFG = 0x0000000C
	}
}
