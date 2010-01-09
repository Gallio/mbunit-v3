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

namespace Gallio.Navigator.Native
{
    [ComImport]
    [Guid("A39EE748-6A27-4817-A6F2-13914BEF5890")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IUri
    {
        [PreserveSig]
        int GetPropertyBSTR(
            [In] Uri_PROPERTY uriProp,
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrProperty,
            [In] uint dwFlags);

        [PreserveSig]
        int GetPropertyLength(
            [In] Uri_PROPERTY uriProp,
            [Out] out uint pcchProperty,
            [In] uint dwFlags);

        [PreserveSig]
        int GetPropertyDWORD(
            [In] Uri_PROPERTY uriProp,
            [Out] out uint pdwProperty,
            [In] uint dwFlags);

        [PreserveSig]
        int HasProperty(
            [In] Uri_PROPERTY uriProp,
            [Out] out bool pfHasProperty);

        [PreserveSig]
        int GetAbsoluteUri(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrAbsoluteUri);

        [PreserveSig]
        int GetAuthority(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrAuthority);

        [PreserveSig]
        int GetDisplayUri(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrDisplayString);

        [PreserveSig]
        int GetDomain(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrDomain);

        [PreserveSig]
        int GetExtension(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrExtension);

        [PreserveSig]
        int GetFragment(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrFragment);

        [PreserveSig]
        int GetHost(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrHost);

        [PreserveSig]
        int GetPassword(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrPassword);

        [PreserveSig]
        int GetPath(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrPath);

        [PreserveSig]
        int GetPathAndQuery(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrPathAndQuery);

        [PreserveSig]
        int GetQuery(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrQuery);

        [PreserveSig]
        int GetRawUri(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrRawUri);

        [PreserveSig]
        int GetSchemeName(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrSchemeName);

        [PreserveSig]
        int GetUserInfo(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrUserInfo);

        [PreserveSig]
        int GetUserName(
            [Out] [MarshalAs(UnmanagedType.BStr)] out string pbstrUserName);

        [PreserveSig]
        int GetHostType(
            [Out] out uint pdwHostType);

        [PreserveSig]
        int GetPort(
            [Out] out uint pdwPort);

        [PreserveSig]
        int GetScheme(
            [Out] out uint pdwScheme);

        [PreserveSig]
        int GetZone(
            [Out] out uint pdwZone);

        [PreserveSig]
        int GetProperties(
            [Out] out uint pdwFlags);

        [PreserveSig]
        int IsEqual(
            [In] IUri pUri,
            [Out] out bool pfEqual);
    }
}
