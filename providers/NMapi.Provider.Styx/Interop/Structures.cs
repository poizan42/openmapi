/* 
 *  NMapy.Styx - The Border between C and C#
 *
 *  Copyright (C) Christian Kellner <christian.kellner@topalis.com> 
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as
 *  published by the Free Software Foundation, either version 3 of the
 *  License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */



using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace NMapi.Provider.Styx.Interop.Structs {
    
    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    [StructLayout (LayoutKind.Explicit)]
    internal struct NativeBinary {
        [FieldOffset (0)]
        public UInt32 cb;
        
        [FieldOffset (4)]
        public IntPtr ptr;        
    }

    [StructLayout (LayoutKind.Explicit)]
    internal struct NativeMultiValue {
        [FieldOffset (0)]
        public UInt32 cb;

        [FieldOffset (4)]
        public IntPtr ptr;
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct NativeFileTime {
        public UInt32 dwLowDateTime;
        public UInt32 dwHighDateTime;
    }

    [StructLayout (LayoutKind.Explicit, Size=16)]
    internal struct NativePropValue {
        [FieldOffset (0)]
        public UInt32 PropTag;
        
        [FieldOffset (4)]
        public UInt32 padding;

        /// <summary> C: short int, Type: PT_I2, CLR: Int16 </summary>
        [FieldOffset (8)]
        public Int16 i; 

        /// <summary>C: LONG (long), Type PT_LONG </summary>
        [FieldOffset (8)]
        public Int32 l;

        /// <summary> alias for PT_LONG </summary>
        [FieldOffset (8)]
        public UInt32 ul;

        /// <summary>C: float, Type: PT_R4 </summary>
        [FieldOffset (8)]
        public Single flt; 

        /// <summary>  C: double, Type: PT_DOUBLE </summary>
        [FieldOffset (8)]
        public Double dbl;

        /// <summary> C: unsigned short int, Type: PT_BOOLEAN </summary>
        [FieldOffset (8)]
        public UInt16 b;

        /// <summary> C: CURRENCY (struct), Type: PT_CURRENCY </summary>
        //[FieldOffset (8), MarshalAs (UnmanagedType.Currency)]
        //public Decimal cur;

        /// <summary> C: FILETIME (struct), Type: PT_SYSTIME </summary>
        [FieldOffset (8)]
        public NativeFileTime ft;

        /// <summary> C: SBinary (struct), Type: PT_BINARY </summary>
        [FieldOffset (8)]
        public NativeBinary bin;

        /// <summary> C: LONGLONG (_int64), Type: PT_I8 </summary>
        [FieldOffset (8)]
        public Int64 li;

        /// <summary> C: LONG (LONG), Type: PT_NULL, PT_OBJECT </summary>
        [FieldOffset (8)]
        public Int32 x;

        /// <summary> Pointer, used for everything else </summary>
        [FieldOffset (8)]
        public IntPtr ptr;

        [FieldOffset (8)]
        public  NativeMultiValue mv;
    };
    
    [StructLayout (LayoutKind.Explicit)]
    internal struct NativePropProblem {
        [FieldOffset (0)]
        public UInt32 Index;
        
        [FieldOffset (4)]
        public UInt32 PropTag;    
        
        [FieldOffset (8)]
        public Int32  SCode;
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct NativeSizedArray {
        public UInt32 Count;
        public IntPtr Data;
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct NativeMapiError {
        public UInt32 Version;
        public IntPtr Error;
        public IntPtr Component;
        public UInt32 LowLevelError;
        public UInt32 Context;
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct NativeRowSet {
        public UInt32 cRows;
        public IntPtr aRows;
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct NativeRow {
        public UInt32 EntryPad;
        public UInt32 cValues;
        public IntPtr Props;
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct NativeSortOrderSet {
        public UInt32 cSorts;
        public UInt32 cCategories;
        public UInt32 cExpanded;

        // IntPtr aSort; /* SSortOrder, Array! */
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct NativeSortOrder{

        public UInt32 ulPropTag;
        public UInt32 ulOrder;

    };

    /* Restrictions */

    [StructLayout (LayoutKind.Sequential)]
    internal struct SAndRestriction {
        public uint           cRes;
        public IntPtr         lpRes; /* LPSRestriction */
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct SOrRestriction {
        public uint           cRes;
        public IntPtr         lpRes; /* LPSRestriction */
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct SNotRestriction {
        public uint           ulReserved;
        public IntPtr         lpRes; /* LPSRestriction */
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct SContentRestriction {
        public uint           ulFuzzyLevel;
        public uint           ulPropTag;
        public IntPtr         lpProp; /* LPSPropValue */
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct SBitMaskRestriction {
        public uint           relBMR;
        public uint           ulPropTag;
        public uint           ulMask;
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct SPropertyRestriction {
        public uint           relop;
        public uint           ulPropTag;
        public IntPtr         lpProp; /* LPSPropValue */
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct SComparePropsRestriction {
        public uint           relop;
        public uint           ulPropTag1;
        public uint           ulPropTag2;
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct SSizeRestriction {
        public uint           relop;
        public uint           ulPropTag;
        public uint           cb;
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct SExistRestriction {
        public uint           ulReserved1;
        public uint           ulPropTag;
        public uint           ulReserved2;
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct SSubRestriction {
        public uint           ulSubObject;
        public IntPtr         lpRes; /* LPSRestriction */
    }

    [StructLayout (LayoutKind.Sequential)]
    internal struct SCommentRestriction {
        public uint            cValues; /* # of properties in lpProp */
        public IntPtr          lpRes; /* LPSRestriction */
        public IntPtr          lpProp; /* LPSPropValue */
    }

    [StructLayout (LayoutKind.Explicit)]
    internal struct SRestriction {

        [FieldOffset (0)]
        public uint   rt;         /* Restriction type */

        [FieldOffset (4)]
        public SComparePropsRestriction    resCompareProps;    /* first */

        [FieldOffset (4)]
        public SAndRestriction             resAnd;

        [FieldOffset (4)]
        public SOrRestriction              resOr;

        [FieldOffset (4)]
        public SNotRestriction             resNot;

        [FieldOffset (4)]
        public SContentRestriction         resContent;

        [FieldOffset (4)]
        public SPropertyRestriction        resProperty;

        [FieldOffset (4)]
        public SBitMaskRestriction         resBitMask;

        [FieldOffset (4)]
        public SSizeRestriction            resSize;

        [FieldOffset (4)]
        public SExistRestriction           resExist;

        [FieldOffset (4)]
        public SSubRestriction             resSub;

        [FieldOffset (4)]
        public SCommentRestriction         resComment;
    }

    [StructLayout (LayoutKind.Explicit)]
    public struct SMapiNameIdKind {

        [FieldOffset (0)]
        public int lID;

        [FieldOffset (0)]
        public IntPtr ptr; ///lpwstrName
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct SMapiNameId {

        public IntPtr          lpguid;
        public uint            ulKind;
        public SMapiNameIdKind Kind;
    }

    /* Notifications */

    [StructLayout (LayoutKind.Sequential)]
    public struct ERROR_NOTIFICATION {
        public uint       cbEntryID;
        public IntPtr   lpEntryID;
        public int       scode;
        public uint       ulFlags;
        public IntPtr lpMAPIError;
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct NEWMAIL_NOTIFICATION {
        public uint    cbEntryID;
        public IntPtr  lpEntryID;
        public uint    cbParentID;
        public IntPtr  lpParentID;
        public uint    ulFlags;
        public IntPtr  lpszMessageClass;
        public uint    ulMessageFlags;
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct OBJECT_NOTIFICATION {
        public uint    cbEntryID;
        public IntPtr  lpEntryID;
        public uint    ulObjType;
        public uint    cbParentID;
        public IntPtr  lpParentID;
        public uint    cbOldID;
        public IntPtr  lpOldID;
        public uint    cbOldParentID;
        public IntPtr  lpOldParentID;
        public IntPtr  lpPropTagArray;
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct TABLE_NOTIFICATION {
        public uint             ulTableEvent;
        public int              hResult;
        internal NativePropValue  propIndex;
        internal NativePropValue  propPrior;
        internal NativeRow        row;
        public uint             ulPad;
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct EXTENDED_NOTIFICATION {
        public uint   ulEvent;
        public uint   cb;
        public IntPtr pbEventParameters;
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct STATUS_OBJECT_NOTIFICATION {
        public uint    cbEntryID;
        public IntPtr  lpEntryID;
        public uint    cValues;
        public IntPtr  lpPropVals;
    }

    [StructLayout (LayoutKind.Explicit)]
    public struct SNotification {

        [FieldOffset (0)]
        public uint   ulEventType;

        [FieldOffset (4)]
        public uint   ulAlignPad;

        [FieldOffset (8)]
        public ERROR_NOTIFICATION          err;

        [FieldOffset (8)]
        public NEWMAIL_NOTIFICATION        newmail;

        [FieldOffset (8)]
        public OBJECT_NOTIFICATION         obj;

        [FieldOffset (8)]
        public TABLE_NOTIFICATION          tab;

        [FieldOffset (8)]
        public EXTENDED_NOTIFICATION       ext;

        [FieldOffset (8)]
        public STATUS_OBJECT_NOTIFICATION  statobj;
    };
}
