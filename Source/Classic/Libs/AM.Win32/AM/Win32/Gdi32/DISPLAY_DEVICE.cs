﻿/* DISPLAY_DEVICE.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Runtime.InteropServices;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// Receives information about the display device specified 
	/// by the iDevNum parameter of the EnumDisplayDevices function.
	/// </summary>
	/// <remarks>
	/// The four string members are set based on the parameters passed 
	/// to EnumDisplayDevices. If the lpDevice param is NULL, then DISPLAY_DEVICE is filled in with information about the display adapter(s). If it is a valid device name, then it is filled in with information about the monitor(s) for that device.
	/// </remarks>
	// Не фурычит, падла!
	[Serializable]
	[StructLayout ( LayoutKind.Sequential, Size = DISPLAY_DEVICEW.SIZE, 
		CharSet = CharSet.Unicode )]
	public struct DISPLAY_DEVICEW
	{
		/// <summary>
		/// Size of structure in bytes.
		/// </summary>
		public const int SIZE = 840;

		/// <summary>
		/// Size, in bytes, of the DISPLAY_DEVICE structure. 
		/// This must be initialized prior to calling EnumDisplayDevices. 
		/// </summary>
		//[FieldOffset ( 0 )]
		public int cb;

		/// <summary>
		/// An array of characters identifying the device name. 
		/// This is either the adapter device or the monitor device.
		/// </summary>
		//[FieldOffset ( 4 )]
		[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 32 )]
		public string DeviceName;

		/// <summary>
		/// An array of characters containing the device context string. 
		/// This is either a description of the display adapter or of the 
		/// display monitor. 
		/// </summary>
		//[FieldOffset ( 68 )]
		[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 128 )]
		public string DeviceString;
  
        /// <summary>
        /// 
        /// </summary>
		//[FieldOffset ( 324 )]
		public DeviceStateFlags StateFlags;
  
		/// <summary>
		/// Windows 98/Me: A string that uniquely identifies the hardware 
		/// adapter or the monitor. This is the Plug and Play identifier. 
		/// </summary>
		//[FieldOffset ( 328 )]
		[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 128 )]
		public string DeviceID;
  
		/// <summary>
		/// Reserved.
		/// </summary>
		//[FieldOffset ( 584 )]
		[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 128 )]
		public string DeviceKey;
	}

	/// <summary>
	/// Receives information about the display device specified 
	/// by the iDevNum parameter of the EnumDisplayDevices function.
	/// </summary>
	/// <remarks>
	/// The four string members are set based on the parameters passed 
	/// to EnumDisplayDevices. If the lpDevice param is NULL, then DISPLAY_DEVICE is filled in with information about the display adapter(s). If it is a valid device name, then it is filled in with information about the monitor(s) for that device.
	/// </remarks>
	[Serializable]
	[StructLayout ( LayoutKind.Sequential, Size = DISPLAY_DEVICEA.SIZE )]
	public struct DISPLAY_DEVICEA
	{
		/// <summary>
		/// Size of structure in bytes.
		/// </summary>
		public const int SIZE = 424;

		/// <summary>
		/// Size, in bytes, of the DISPLAY_DEVICE structure. 
		/// This must be initialized prior to calling EnumDisplayDevices. 
		/// </summary>
		//[FieldOffset ( 0 )]
		public int cb;

		/// <summary>
		/// An array of characters identifying the device name. 
		/// This is either the adapter device or the monitor device.
		/// </summary>
		//[FieldOffset ( 4 )]
		[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 32 )]
		public string DeviceName;

		/// <summary>
		/// An array of characters containing the device context string. 
		/// This is either a description of the display adapter or of the 
		/// display monitor. 
		/// </summary>
		//[FieldOffset ( 68 )]
		[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 128 )]
		public string DeviceString;

        /// <summary>
        /// 
        /// </summary>
		//[FieldOffset ( 324 )]
		public DeviceStateFlags StateFlags;

		/// <summary>
		/// Windows 98/Me: A string that uniquely identifies the hardware 
		/// adapter or the monitor. This is the Plug and Play identifier. 
		/// </summary>
		//[FieldOffset ( 328 )]
		[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 128 )]
		public string DeviceID;

		/// <summary>
		/// Reserved.
		/// </summary>
		//[FieldOffset ( 584 )]
		[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 128 )]
		public string DeviceKey;
	}
}
