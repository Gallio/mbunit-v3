// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Gallio.Icarus.Utilities
{
    // Lifted from the Windows API Code Pack
    // http://code.msdn.microsoft.com/WindowsAPICodePack
    [ComImportAttribute()]
    [GuidAttribute("c43dc798-95d1-4bea-9030-bb99e2983a1a")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITaskbarList4
    {
        // ITaskbarList
        [PreserveSig]
        void HrInit();
        [PreserveSig]
        void AddTab(IntPtr hwnd);
        [PreserveSig]
        void DeleteTab(IntPtr hwnd);
        [PreserveSig]
        void ActivateTab(IntPtr hwnd);
        [PreserveSig]
        void SetActiveAlt(IntPtr hwnd);

        // ITaskbarList2
        [PreserveSig]
        void MarkFullscreenWindow(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        // ITaskbarList3
        [PreserveSig]
        void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
        [PreserveSig]
        void SetProgressState(IntPtr hwnd, TBPFLAG tbpFlags);
        [PreserveSig]
        void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
        [PreserveSig]
        void UnregisterTab(IntPtr hwndTab);
        [PreserveSig]
        void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
        [PreserveSig]
        void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, uint dwReserved);
        [PreserveSig]
        HRESULT ThumbBarAddButtons(
            IntPtr hwnd,
            uint cButtons,
            [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
        [PreserveSig]
        HRESULT ThumbBarUpdateButtons(
            IntPtr hwnd,
            uint cButtons,
            [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
        [PreserveSig]
        void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
        [PreserveSig]
        void SetOverlayIcon(
          IntPtr hwnd,
          IntPtr hIcon,
          [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
        [PreserveSig]
        void SetThumbnailTooltip(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
        [PreserveSig]
        void SetThumbnailClip(
            IntPtr hwnd,
            ref RECT prcClip);

        // ITaskbarList4
        void SetTabProperties(IntPtr hwndTab, STPFLAG stpFlags);
    }

    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    };

    public enum STPFLAG
    {
        STPF_NONE = 0x0,
        STPF_USEAPPTHUMBNAILALWAYS = 0x1,
        STPF_USEAPPTHUMBNAILWHENACTIVE = 0x2,
        STPF_USEAPPPEEKALWAYS = 0x4,
        STPF_USEAPPPEEKWHENACTIVE = 0x8
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct THUMBBUTTON
    {
        /// <summary>
        /// WPARAM value for a THUMBBUTTON being clicked.
        /// </summary>
        internal const int THBN_CLICKED = 0x1800;

        [MarshalAs(UnmanagedType.U4)]
        internal THBMASK dwMask;
        internal uint iId;
        internal uint iBitmap;
        internal IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string szTip;
        [MarshalAs(UnmanagedType.U4)]
        internal THBFLAGS dwFlags;
    }

    public enum THBMASK
    {
        THB_BITMAP = 0x1,
        THB_ICON = 0x2,
        THB_TOOLTIP = 0x4,
        THB_FLAGS = 0x8
    }

    [Flags]
    public enum THBFLAGS
    {
        THBF_ENABLED = 0x00000000,
        THBF_DISABLED = 0x00000001,
        THBF_DISMISSONCLICK = 0x00000002,
        THBF_NOBACKGROUND = 0x00000004,
        THBF_HIDDEN = 0x00000008,
        THBF_NONINTERACTIVE = 0x00000010
    }

    public enum HRESULT : uint
    {
        S_FALSE = 0x0001,
        S_OK = 0x0000,
        E_INVALIDARG = 0x80070057,
        E_OUTOFMEMORY = 0x8007000E,
        E_NOINTERFACE = 0x80004002,
        E_FAIL = 0x80004005,
        E_ELEMENTNOTFOUND = 0x80070490,
        TYPE_E_ELEMENTNOTFOUND = 0x8002802B,
        NO_OBJECT = 0x800401E5,
        ERROR_CANCELLED = 1223,
        RESOURCE_IN_USE = 0x800700AA,
    }
}
