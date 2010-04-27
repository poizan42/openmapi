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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NMapi.Provider.Styx.Interop.Structs;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Provider.Styx.Interop
{
    [Flags]
    enum PropertyType : uint {

        MultiValue   =  0x1000,

        /// <summary> 0x000, (Reserved for interface use) type doesn't matter to caller </summary>
        Unspecified  =  0, 
        
        /// <summary> 0x001, NULL property value </summary>
        Null         =  1, 
        
        /// <summary> 0x002, Signed 16-bit value (C: short int, CLR: Int16) </summary>
        I2           =  2, 
        
        /// <summary> 0x003, Signed 32-bit value </summary>
        Long         =  3, 
        
        /// <summary> 0x004, 4-byte floating point </summary>
        R4           =  4, 
        
        /// <summary> 0x005, Floating point double </summary>
        Double       =  5, 
        
        /// <summary> 0x006, Signed 64-bit int (decimal w/    4 digits right of decimal pt) </summary>
        Currency     =  6, 
        
        /// <summary> 0x007, Application time </summary>
        AppTime      =  7, 
        
        /// <summary> 0x00A, 32-bit error value </summary>
        Error        = 10,
        
         /// <summary> 0x00B, 16-bit boolean (non-zero true) </summary>
        Boolean      = 11,
        
        /// <summary> 0x00D, Embedded object in a property </summary>
        Object       = 13, 
        
        /// <summary> 0x014, 8-byte signed integer </summary>
        I8           = 20,
        
         /// <summary> 0x01E, Null terminated 8-bit character string </summary>
        String8      = 30,
        
        /// <summary> 0x01F, Null terminated Unicode string </summary>
        Unicode      = 31, 
        
        /// <summary> 0x040, FILETIME 64-bit int w/ number of 100ns periods since Jan 1,1601 </summary>
        Systime      = 64, 
        
        /// <summary> 0x048, OLE GUID </summary>
        ClsID        = 72,

        /// <summary> 0x102, </summary>  
        Binary       = 258
    } 

	internal class Transmogrify {
        
        //private static Type GuidAttributeType = typeof (GuidAttribute);

        public static void CheckHResult (int hr) {
            if (hr != 0) {
                System.Diagnostics.Trace.WriteLine (String.Format ("Hr: {0:X}", hr));
                throw MapiException.Make(hr);
            }
        }

        public static SBinary[] PtrToSBinaryArray (IntPtr pointer, uint count) {
            IntPtr iter = pointer;
            SBinary[] BinArray = new SBinary[count];
            for (int i = 0 ; i < count ; i++) {
                NativeBinary bin = (NativeBinary) Marshal.PtrToStructure (iter, typeof (NativeBinary));
                byte[] ba = new byte[bin.cb];
                Marshal.Copy (bin.ptr, ba, 0, (int) bin.cb);
                BinArray[i] = new SBinary (ba);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (bin));
            }

            return BinArray;
        }

        public static PropertyValue StructToManaged (NativePropValue pv) {
            PropertyType type = (PropertyType) (pv.PropTag & ((uint) 0x0000FFFF));
            PropertyValue prop = null;
            bool  MultiValue = (type & PropertyType.MultiValue) != 0;

            if (MultiValue) {

                type = type & ~PropertyType.MultiValue;
                NativeMultiValue mv = pv.mv;

                switch (type) {

                    case PropertyType.Binary:
                        SBinary[] BinArray = PtrToSBinaryArray (mv.ptr, mv.cb);
                        prop = new BinaryArrayProperty (BinArray);
                        break;
                }

            } else {

                switch (type) {

                    case PropertyType.Null:
                        prop = new NullProperty ();
                        break;

                    case PropertyType.I2:
                        prop = new ShortProperty (pv.i);
                        break;

                    case PropertyType.Long:
                        prop = new IntProperty (pv.l);
                        break;

                    case PropertyType.R4:
                        prop = new FloatProperty (pv.flt);
                        break;

                    case PropertyType.Double:
                        prop = new DoubleProperty (pv.dbl);
                        break;

                    case PropertyType.Currency:
                        //prop = new CurrencyProperty (pv.cur);
                        break;

                    case PropertyType.Boolean:
                        prop = new BooleanProperty ((short) pv.b);
                        break;

                    case PropertyType.Object:
                        prop = new ObjectProperty ((int) pv.x);
                        break;

                    case PropertyType.String8:

                        string str = Marshal.PtrToStringAnsi (pv.ptr);
                        if (str == null)
                            str = "";
                        prop = new String8Property (str);
                        break;

                    case PropertyType.Unicode:
                        prop = new UnicodeProperty (Marshal.PtrToStringUni (pv.ptr));
                        break;

                    case PropertyType.Systime:
                        FileTime filetime = new FileTime ();
                        filetime.HighDateTime = (int) pv.ft.dwHighDateTime;
                        filetime.LowDateTime = (int) pv.ft.dwLowDateTime;
                        prop = new FileTimeProperty (filetime);
                        break;

                    case PropertyType.I8:
                        prop = new LongProperty (pv.li);
                        break;

                    case PropertyType.ClsID:
                        prop = new GuidProperty (PtrToGuid (pv.ptr));
                        break;

                    case PropertyType.Binary:
                        byte[] ba = new byte[pv.bin.cb];
                        Marshal.Copy (pv.bin.ptr, ba, 0, (int) pv.bin.cb);
                        SBinary bin = new SBinary (ba);
                        prop = new BinaryProperty (bin);
                        break;

                    case PropertyType.Error:
                        prop = new ErrorProperty (pv.l);
                        break;
                }
            }

            if (prop != null) {
                prop.PropTag = (int) pv.PropTag;
            } else {
                System.Console.WriteLine ("WARNING {0:X} not serialized", pv.PropTag);
                prop = new Properties.XProperty (0);
                prop.PropTag = Property.Null;
            }

            return prop;
        }

        public static PropertyValue PtrToPropValue (IntPtr pointer) {
            NativePropValue pv = (NativePropValue) Marshal.PtrToStructure (pointer, typeof (NativePropValue));
            PropertyValue prop = StructToManaged (pv);
            return prop;
        }

        public static PropertyValue[] PtrToPropArray (IntPtr pointer, uint count) {
            PropertyValue[] PropArray = new PropertyValue[count];

            IntPtr iter = pointer;
            for (int i = 0 ; i < count ; i++) {

                PropArray[i] = PtrToPropValue (iter);
                
                //Pointer Arithmetix, I haz it!
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (typeof (NativePropValue)));
            }

            return PropArray;
        }

        public static PropertyValue[] PtrArrayToPropArray (IntPtr pointer, uint count) {
            PropertyValue[] PropArray = new PropertyValue[count];

            IntPtr iter = pointer;
            for (int i = 0 ; i < count ; i++) {

                PropArray[i] = PtrToPropValue (iter);

                //Pointer Arithmetix, I haz it!
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (typeof (NativePropValue)));
            }

            return PropArray;
        }

        public static Row StructToManaged (NativeRow row) {
            PropertyValue[] values = PtrToPropArray (row.Props, row.cValues);
            return new Row (values);
        }

        public static RowSet PtrToRowSet (IntPtr RowHandle) {
            NativeRowSet RowSet = (NativeRowSet) Marshal.PtrToStructure (RowHandle, typeof (NativeRowSet));

            IntPtr iter = (IntPtr) ((int) RowHandle + Marshal.SizeOf (typeof (UInt32)));

            Row[] rows = new Row[RowSet.cRows];
            for (int i = 0 ; i < RowSet.cRows ; i++) {
                NativeRow row = (NativeRow) Marshal.PtrToStructure (iter, typeof (NativeRow));
                rows[i] = StructToManaged (row);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (typeof (NativeRow)));
            }

            return new RowSet (rows);
        }

        public static IntPtr TagArrayToPtr (PropertyTag[] PropTagArray, MemContext MemCtx) {

            if (PropTagArray == null) {
                return IntPtr.Zero;
            }

            IntPtr RealArray = MemCtx.Alloc<int> (PropTagArray.Length + 1);

            Marshal.WriteInt32 (RealArray, PropTagArray.Length);
            IntPtr iter = (IntPtr) ((int) RealArray + Marshal.SizeOf (typeof (Int32)));

            foreach (PropertyTag tag in PropTagArray) {
                Marshal.WriteInt32 (iter, tag.Tag);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (typeof (Int32)));
            }

            return RealArray;
        }


        internal static PropertyTag[] PtrToTagArray (IntPtr TagArrayHandle) {
            IntPtr iter = TagArrayHandle;
            NativeSizedArray RealArray = PtrToSizedArray (ref iter);
            int[] tags = new int[RealArray.Count];
            Marshal.Copy (iter, tags, 0, (int) RealArray.Count);
            return PropertyTag.ArrayFromIntegers (tags);
        }


        private static IntPtr PropValueToPtr (PropertyValue PropertyValue, MemContext MemCtx) {
            PropertyValue[] PropArray = { PropertyValue };
            uint count;
            IntPtr pointer = PropArrayToPtr (PropArray, out count, MemCtx);
            return pointer;
        }

        internal static IntPtr GuidToPtr (NMapiGuid nguid, MemContext MemCtx) {
            IntPtr pointer;

            if (nguid == null) {
                return IntPtr.Zero;
            }

            Guid guid = new Guid (nguid.Data1, nguid.Data2, nguid.Data3, nguid.Data4);
            int size = Marshal.SizeOf (guid);
            pointer = MemCtx.Alloc<byte> (size);
            Marshal.StructureToPtr (guid, pointer, false);

            return pointer;
        }

        internal static NMapiGuid PtrToGuid (IntPtr ptr) {
            Guid guid = (Guid) Marshal.PtrToStructure (ptr, typeof (Guid));
            NMapiGuid nguid = new NMapiGuid (guid.ToByteArray ());
            return nguid;
        }

        internal static IntPtr PropArrayToPtr (PropertyValue[] PropArray, out uint count, MemContext MemCtx) {
            if (PropArray == null) {
                count = 0;
                return IntPtr.Zero;
            }

            IntPtr PropArrayHandle;
            IntPtr iter = PropArrayHandle = MemCtx.Alloc<NativePropValue> (PropArray.Length);
            count = 0;

            foreach (PropertyValue prop in PropArray) {
                NativePropValue pv = new NativePropValue ();
                bool success = true;

                PropertyType type = (NMapi.Provider.Styx.Interop.PropertyType) PropertyTypeHelper.PROP_TYPE(prop.PropTag);

                pv.PropTag = (uint) prop.PropTag;

                switch (type) {

                    case PropertyType.Null:
                        pv.x = 0;
                        break;

                    case PropertyType.I2:
                        pv.i = (short) prop;
                        break;

                    case PropertyType.Long:
                        pv.l = (int) prop;
                        break;

                    case PropertyType.R4:
                        pv.flt = (float) prop;
                        break;

                    case PropertyType.Double:
                        pv.dbl = (double) prop;
                        break;

                    case PropertyType.Boolean:
                        pv.b = (ushort) (((bool) prop) ? 1 : 0);
                        break;

                    case PropertyType.String8:
                    case PropertyType.Unicode:
                        string str = (string) prop;
                        pv.ptr = MemCtx.StringDup (str, type == PropertyType.String8);
                        break;

                    case PropertyType.Binary:
                        byte[] barray = (byte[]) prop;
                        pv.bin.cb = (uint) barray.Length;
                        pv.bin.ptr = MemCtx.Alloc<byte> (barray.Length);
                        Marshal.Copy (barray, 0, pv.bin.ptr, barray.Length);
                        break;

                    case PropertyType.I8:
                        pv.li = (long) prop;
                        break;

                    case PropertyType.ClsID:
                        pv.ptr = GuidToPtr ((NMapiGuid) prop, MemCtx);
                        break;

                    default:
                        System.Console.WriteLine ("WARNING {0} not serialized", type);
                        success = false;
                        break;
                }

                if (success) {
                    Marshal.StructureToPtr (pv, iter, false);
                    count++;

                    //Pointer Arithmetix, I haz it again!
                    iter = (IntPtr) ((int) iter + Marshal.SizeOf (pv));
                }
                //FIXME: ELSE: Add to Proplem Array somehow?!
            }

            return PropArrayHandle;
        }

        internal static PropertyProblem[] PtrToProblemArray (IntPtr ProblemHandle) {

            if (ProblemHandle == IntPtr.Zero)
                return null;

            IntPtr iter = ProblemHandle;
            NativeSizedArray SizedArray = PtrToSizedArray (ref iter);
            PropertyProblem[] ProblemArray = new PropertyProblem[SizedArray.Count];

            for (int i = 0 ; i < SizedArray.Count ; i++) {
                NativePropProblem problem = new NativePropProblem ();
                Marshal.PtrToStructure (iter, problem);

                PropertyProblem PropProblem = ProblemArray[i] = new PropertyProblem ();
                PropProblem.Index = (int) problem.Index;
                PropProblem.PropTag = (int) problem.PropTag;
                PropProblem.SCode = problem.SCode;

                iter = (IntPtr) ((int) iter + Marshal.SizeOf (typeof (NativePropProblem)));
            }

            return ProblemArray;
        }

        internal static MapiError PtrToMapiError (IntPtr ErrorHandle) {

            if (ErrorHandle == IntPtr.Zero)
                return null;

            IntPtr pointer = Marshal.ReadIntPtr (ErrorHandle);

            if (pointer == IntPtr.Zero)
                return null;

            NativeMapiError NativeError = (NativeMapiError) Marshal.PtrToStructure (pointer, typeof (NativeMapiError));

            MapiError error = new MapiError ();
            error.Component = Marshal.PtrToStringAuto (NativeError.Component);
            error.Context = (int) NativeError.Context;
            error.Error = Marshal.PtrToStringAuto (NativeError.Error);
            error.LowLevelError = (int) NativeError.LowLevelError;
            error.Version = (int) NativeError.Version;

            return error;
        }

        private static NativeSizedArray PtrToSizedArray (ref IntPtr pointer) {
            NativeSizedArray RealArray = (NativeSizedArray) Marshal.PtrToStructure (pointer, typeof (NativeSizedArray));
            pointer = (IntPtr) ((int) pointer + Marshal.SizeOf (typeof (UInt32)));
            return RealArray;
        }


        internal static IntPtr SortOrderSetToPtr (SortOrderSet Criteria, MemContext MemCtx) {

            NativeSortOrderSet Set = new NativeSortOrderSet ();
            Set.cCategories = (uint) Criteria.CCategories;
            Set.cExpanded = (uint) Criteria.CExpanded;
            uint count = Set.cSorts = (uint) (Criteria.ASort != null ? Criteria.ASort.Length : 0);

            int size = (int) (Marshal.SizeOf (Set) + count * Marshal.SizeOf (typeof (NativeSortOrder)));

            IntPtr pointer = MemCtx.Alloc (size);
            Marshal.StructureToPtr (Set, pointer, false);

            IntPtr iter = (IntPtr) ((int) pointer + Marshal.SizeOf (Set));
            foreach (SortOrder order in Criteria.ASort) {
                NativeSortOrder so = new NativeSortOrder ();

                so.ulOrder = (uint) order.Order;
                so.ulPropTag = (uint) order.PropTag;

                Marshal.StructureToPtr (so, iter, false);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (so));
            }

            return pointer;
        }

        /* Restriction */

        enum ResType : uint {

            And = 0x00000000,
            Or = 0x00000001,
            Not = 0x00000002,
            Content = 0x00000003,
            Property = 0x00000004,
            CompareProps = 0x00000005,
            Bitmask = 0x00000006,
            Size = 0x00000007,
            Exist = 0x00000008,
            SubRestriction = 0x00000009,
            Comment = 0x0000000A

        }

        private static IntPtr RestrictionToPointer (MemContext MemCtx, params Restriction[] ResArray) {

            IntPtr iter;
            IntPtr pointer = iter = MemCtx.Alloc<SRestriction> (ResArray.Length);

            foreach (Restriction CurRes in ResArray) {

                SRestriction NativeRes = new SRestriction ();

                if (CurRes is AndRestriction) {
                    AndRestriction and = (AndRestriction) CurRes;

                    NativeRes.rt = (uint) ResType.And;
                    NativeRes.resAnd.cRes = (uint) and.Res.Length;
                    NativeRes.resAnd.lpRes = RestrictionToPointer (MemCtx, and.Res);

                } else if (CurRes is OrRestriction) {
                    OrRestriction or = (OrRestriction) CurRes;

                    NativeRes.rt = (uint) ResType.Or;
                    NativeRes.resOr.cRes = (uint) or.Res.Length;
                    NativeRes.resOr.lpRes = RestrictionToPointer (MemCtx, or.Res);

                } else if (CurRes is NotRestriction) {
                    NotRestriction not = (NotRestriction) CurRes;

                    NativeRes.rt = (uint) ResType.Not;
                    NativeRes.resNot.lpRes = RestrictionToPointer (not.Res, MemCtx);

                } else if (CurRes is ContentRestriction) {
                    ContentRestriction content = (ContentRestriction) CurRes;

                    NativeRes.rt = (uint) ResType.Content;
                    NativeRes.resContent.lpProp = PropValueToPtr (content.Prop, MemCtx);
                    NativeRes.resContent.ulPropTag = (uint) content.PropTag;
                    NativeRes.resContent.ulFuzzyLevel = (uint) content.FuzzyLevel;

                } else if (CurRes is BitMaskRestriction) {

                    BitMaskRestriction bitmask = (BitMaskRestriction) CurRes;

                    NativeRes.rt = (uint) ResType.Bitmask;
                    NativeRes.resBitMask.ulPropTag = (uint) bitmask.PropTag;
                    NativeRes.resBitMask.ulMask = (uint) bitmask.Mask;
                    NativeRes.resBitMask.relBMR = (uint) bitmask.RelBMR;

                } else if (CurRes is PropertyRestriction) {

                    PropertyRestriction property = (PropertyRestriction) CurRes;

                    NativeRes.rt = (uint) ResType.Property;
                    NativeRes.resProperty.lpProp = PropValueToPtr (property.Prop, MemCtx);
                    NativeRes.resProperty.ulPropTag = (uint) property.PropTag;
                    NativeRes.resProperty.relop = (uint) property.RelOp;

                } else if (CurRes is ComparePropsRestriction) {

                    ComparePropsRestriction compare = (ComparePropsRestriction) CurRes;

                    NativeRes.rt = (uint) ResType.CompareProps;
                    NativeRes.resCompareProps.ulPropTag1 = (uint) compare.PropTag1;
                    NativeRes.resCompareProps.ulPropTag2 = (uint) compare.PropTag2;
                    NativeRes.resCompareProps.relop = (uint) compare.RelOp;

                } else if (CurRes is SizeRestriction) {

                    SizeRestriction size = (SizeRestriction) CurRes;

                    NativeRes.rt = (uint) ResType.Size;
                    NativeRes.resSize.cb = (uint) size.Cb;
                    NativeRes.resSize.relop = (uint) size.RelOp;
                    NativeRes.resSize.ulPropTag = (uint) size.PropTag;

                } else if (CurRes is ExistRestriction) {

                    ExistRestriction exist = (ExistRestriction) CurRes;

                    NativeRes.rt = (uint) ResType.Exist;
                    NativeRes.resExist.ulPropTag = (uint) exist.PropTag;

                } else if (CurRes is SubRestriction) {

                    SubRestriction sub = (SubRestriction) CurRes;

                    NativeRes.rt = (uint) ResType.Exist;
                    NativeRes.resSub.lpRes = RestrictionToPointer (MemCtx, sub.Res);
                    NativeRes.resSub.ulSubObject = (uint) sub.SubObject;

                } else if (CurRes is CommentRestriction) {
                    CommentRestriction comment = (CommentRestriction) CurRes;

                    uint count;

                    NativeRes.rt = (uint) ResType.Comment;
                    NativeRes.resComment.lpProp = PropArrayToPtr (comment.Prop, out count, MemCtx);
                    NativeRes.resComment.cValues = count;
                }

                Marshal.StructureToPtr (NativeRes, iter, false);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (typeof (SRestriction)));
            }

            return pointer;
        }

        internal static IntPtr RestrictionToPointer (Restriction res, MemContext MemCtx) {
            return RestrictionToPointer (MemCtx, res);
        }

        internal static Restriction PtrToRestriction (IntPtr RestrictionHandle) {

            SRestriction CurRes = (SRestriction) Marshal.PtrToStructure (RestrictionHandle, typeof (SRestriction));
            RestrictionType type = (RestrictionType) CurRes.rt;
            Restriction res = null;

            switch (type) {

                case RestrictionType.And:
                    AndRestriction And = new AndRestriction ();
                    And.Res = PtrToRestriction (CurRes.resAnd.lpRes, CurRes.resAnd.cRes);
                    res = And;
                    break;

                case RestrictionType.Bitmask:
                    BitMaskRestriction BitMask = new BitMaskRestriction ();
                    BitMask.PropTag = (int) CurRes.resBitMask.ulPropTag;
                    BitMask.Mask = (int) CurRes.resBitMask.ulMask;
                    BitMask.RelBMR = (int) CurRes.resBitMask.relBMR;
                    res = BitMask;
                    break;

                case RestrictionType.Comment:
                    CommentRestriction Comment = new CommentRestriction ();
                    Comment.Prop = PtrToPropArray (CurRes.resComment.lpProp, CurRes.resComment.cValues);
                    Comment.Res = PtrToRestriction (CurRes.resComment.lpRes);
                    res = Comment;
                    break;

                case RestrictionType.CompareProps:
                    ComparePropsRestriction CompareProps = new ComparePropsRestriction ();

                    CompareProps.PropTag1 = (int) CurRes.resCompareProps.ulPropTag1;
                    CompareProps.PropTag2 = (int) CurRes.resCompareProps.ulPropTag2;
                    CompareProps.RelOp = (RelOp) CurRes.resCompareProps.relop;
                    res = CompareProps;
                    break;

                case RestrictionType.Content:
                    ContentRestriction Content = new ContentRestriction ();
                    Content.FuzzyLevel = (FuzzyLevel) CurRes.resContent.ulFuzzyLevel;
                    Content.Prop = PtrToPropValue (CurRes.resContent.lpProp);
                    Content.PropTag = (int) CurRes.resContent.ulPropTag;
                    res = Content;
                    break;

                case RestrictionType.Exist:
                    ExistRestriction Exist = new ExistRestriction ();
                    Exist.PropTag = (int) CurRes.resExist.ulPropTag;
                    res = Exist;
                    break;

                case RestrictionType.Not:
                    NotRestriction Not = new NotRestriction ();
                    Not.Res = PtrToRestriction (CurRes.resNot.lpRes);
                    res = Not;
                    break;

                case RestrictionType.Or:
                    OrRestriction Or = new OrRestriction ();
                    Or.Res = PtrToRestriction (CurRes.resOr.lpRes, CurRes.resOr.cRes);
                    res = Or;
                    break;

                case RestrictionType.Property:
                    PropertyRestriction Property = new PropertyRestriction ();
                    Property.Prop = PtrToPropValue (CurRes.resProperty.lpProp);
                    Property.PropTag = (int) CurRes.resProperty.ulPropTag;
                    Property.RelOp = (RelOp) CurRes.resProperty.relop;
                    res = Property;
                    break;

                case RestrictionType.Size:
                    SizeRestriction Size = new SizeRestriction ();
                    Size.Cb = (int) CurRes.resSize.cb;
                    Size.PropTag = (int) CurRes.resSize.ulPropTag;
                    Size.RelOp = (int) CurRes.resSize.relop;
                    res = Size;
                    break;

                case RestrictionType.SubRestriction:
                    SubRestriction Sub = new SubRestriction ();
                    Sub.Res = PtrToRestriction (CurRes.resSub.lpRes);
                    Sub.SubObject = (int) CurRes.resSub.ulSubObject;
                    res = Sub;
                    break;
            }

            return res;
        }

        private static Restriction[] PtrToRestriction (IntPtr RestrictionHandle, uint count) {
            IntPtr iter = RestrictionHandle;
            Restriction[] Array = new Restriction[count];

            for (int i = 0 ; i < count ; i++) {
                Array[i] = PtrToRestriction (iter);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (typeof (SRestriction)));
            }

            return Array;
        }

        internal static IntPtr MapiNameIdsToPtr (MapiNameId[] ids, MemContext MemCtx) {

            if (ids == null) {
                return IntPtr.Zero;
            }

            IntPtr iter;
            IntPtr pointer = iter = MemCtx.Alloc<IntPtr> (ids.Length);

            foreach (MapiNameId id in ids) {
                SMapiNameId NativeId = new SMapiNameId ();

                switch (id.UlKind) {

                    case NamedPropertyIdKind.String:
                        StringMapiNameId StrId = (StringMapiNameId) id;
                        NativeId.Kind.ptr = Marshal.StringToHGlobalUni (StrId.StrName);
                        break;

                    case NamedPropertyIdKind.Id:
                        NumericMapiNameId NumId = (NumericMapiNameId) id;
                        NativeId.Kind.lID = NumId.ID;
                        break;
                }

                NativeId.ulKind = (uint) id.UlKind;
                NativeId.lpguid = GuidToPtr (id.Guid, MemCtx);

                int size = Marshal.SizeOf (NativeId);
                IntPtr ptr = MemCtx.Alloc (size);
                Marshal.StructureToPtr (NativeId, ptr, false);
                Marshal.WriteIntPtr (iter, ptr);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (iter));
            }

            return pointer;
        }

        internal static MapiNameId[] PtrToMapiNameIds (IntPtr nativeIds, uint numIds) {

            if (numIds == 0 || nativeIds == IntPtr.Zero) {
                return new MapiNameId[0];
            }

            IntPtr iter = nativeIds;
            uint cnt;
            MapiNameId[] ret = new MapiNameId[numIds];

            for(cnt = 0; cnt < numIds; cnt++) {
                IntPtr currentId = Marshal.ReadIntPtr(iter);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (iter));

                SMapiNameId nativeId = (SMapiNameId) Marshal.PtrToStructure(currentId, typeof(SMapiNameId));


                switch ((NamedPropertyIdKind) nativeId.ulKind) {

                    case NamedPropertyIdKind.String:
                        StringMapiNameId StrId = new StringMapiNameId();
                        StrId.Guid = PtrToGuid(nativeId.lpguid);
                        StrId.StrName = Marshal.PtrToStringUni(nativeId.Kind.ptr);
                        ret[cnt] = StrId;
                        break;

                    case NamedPropertyIdKind.Id:
                        NumericMapiNameId NumId = new NumericMapiNameId();
                        NumId.Guid = PtrToGuid(nativeId.lpguid);
                        NumId.ID = nativeId.Kind.lID;
                        ret[cnt] = NumId;
                        break;
                }
            }
            
            return ret;
        }

        public static byte[] PtrToByteArray (IntPtr pointer, uint count) {

            if (pointer == IntPtr.Zero)
                return null;

            int scount = (int) count;
            byte[] ba = new byte[scount];
            Marshal.Copy (pointer, ba, 0, scount);
            return ba;
        }

        public static Notification[] PtrToNotification (IntPtr pointer, uint count) {

            Notification[] events = new Notification[count];

            IntPtr iter = pointer;
            for (uint i = 0 ; i < count ; i++) {
                Notification ev = null;

                SNotification CurNot = (SNotification) Marshal.PtrToStructure (iter, typeof (SNotification));
                NotificationEventType type = (NotificationEventType) CurNot.ulEventType;

                switch (type) {

                    case NotificationEventType.CriticalError:
                        NMapi.Events.ErrorNotification ErrorEvent = new NMapi.Events.ErrorNotification ();
                        ErrorEvent.EntryID = PtrToByteArray (CurNot.err.lpEntryID, CurNot.err.cbEntryID);
                        ErrorEvent.Flags = (int) CurNot.err.ulFlags;
                        ErrorEvent.SCode = (int) CurNot.err.scode;
                        ErrorEvent.MAPIError = PtrToMapiError (CurNot.err.lpMAPIError);
                        ev = ErrorEvent;
                        break;

                    case NotificationEventType.NewMail:
                        NewMailNotification NewMail = new NewMailNotification ();
                        NewMail.EntryID = PtrToByteArray (CurNot.newmail.lpEntryID, CurNot.newmail.cbEntryID);
                        NewMail.ParentID = PtrToByteArray (CurNot.newmail.lpszMessageClass, CurNot.newmail.cbParentID);
                        NewMail.Flags = (int) CurNot.newmail.ulFlags;
                        NewMail.MessageClass = Marshal.PtrToStringAuto (CurNot.newmail.lpszMessageClass);
                        NewMail.MessageFlags = (int) CurNot.newmail.ulMessageFlags;
                        ev = NewMail;
                        break;

                    case NotificationEventType.ObjectCopied:
                    case NotificationEventType.ObjectCreated:
                    case NotificationEventType.ObjectDeleted:
                    case NotificationEventType.ObjectModified:
                    case NotificationEventType.ObjectMoved:
                    case NotificationEventType.SearchComplete:
                        ObjectNotification ObjEvent = new ObjectNotification ();
                        ObjEvent.EntryID = PtrToByteArray (CurNot.obj.lpEntryID, CurNot.obj.cbEntryID);
                        ObjEvent.ObjType = (int) CurNot.obj.ulObjType;
                        ObjEvent.OldID = PtrToByteArray (CurNot.obj.lpOldID, CurNot.obj.cbOldID);
                        ObjEvent.OldParentID = PtrToByteArray (CurNot.obj.lpOldParentID, CurNot.obj.cbOldParentID);
                        ObjEvent.ParentID = PtrToByteArray (CurNot.obj.lpParentID, CurNot.obj.cbOldParentID);
                        ObjEvent.PropTagArray = PtrToTagArray (CurNot.obj.lpPropTagArray);
                        ev = ObjEvent;
                        break;

                    case NotificationEventType.Extended:
                        ExtendedNotification ExtEvent = new ExtendedNotification ();
                        ExtEvent.Event = (int) CurNot.ext.ulEvent;
                        ExtEvent.EventParameters = PtrToByteArray (CurNot.ext.pbEventParameters, CurNot.ext.cb);
                        ev = ExtEvent;
                        break;

                    case NotificationEventType.TableModified:
                        TableNotification TableEvent = new TableNotification ();
                        TableEvent.HResult = CurNot.tab.hResult;
                        TableEvent.PropIndex = StructToManaged (CurNot.tab.propIndex);
                        TableEvent.PropPrior = StructToManaged (CurNot.tab.propPrior);
                        TableEvent.Row = StructToManaged (CurNot.tab.row);
                        ev = TableEvent;
                        break;

                    case NotificationEventType.StatusObjectModified:
                        StatusObjectNotification StatusEvent = new StatusObjectNotification ();
                        StatusEvent.EntryID = PtrToByteArray (CurNot.statobj.lpEntryID, CurNot.statobj.cbEntryID);
                        StatusEvent.PropVals = PtrToPropArray (CurNot.statobj.lpPropVals, CurNot.statobj.cValues);
                        ev = StatusEvent;
                        break;
                }

                events[i] = ev;
            }

            return events;
        }

        public static EntryList PtrToEntryList (IntPtr pointer) {
            NativeSizedArray sa = PtrToSizedArray (ref pointer);
            SBinary[] BinArray = PtrToSBinaryArray (sa.Data, sa.Count);
            return new EntryList (BinArray);;
        }

        public static IntPtr EntryListToPtr (EntryList list, MemContext MemCtx) {
            if (list == null) {
                return IntPtr.Zero;
            }

            /* allocate space for the SBinaryArray and the SBinary entries, but not the
             * SBinary data itself */
            IntPtr RealArray = MemCtx.Alloc<uint> ((list.Bin.Length + 1) * 2);
            Marshal.WriteInt32 (RealArray, list.Bin.Length);
            IntPtr iter = (IntPtr) ((int) RealArray + Marshal.SizeOf (typeof (Int32)));
            IntPtr temp = (IntPtr) ((int) iter + Marshal.SizeOf (typeof (Int32)));
            Marshal.WriteInt32 (iter, (Int32) temp);
            iter = temp;

            /* iter is now the pointer to the first SBinary entry */
            foreach (SBinary entry in list.Bin) {
                Int32 entryLength = entry.lpb.Length;

                /* allocate space for the SBinary data and copy data */
                IntPtr nativeEntry = MemCtx.Alloc<byte>(entryLength);
                Marshal.Copy(entry.lpb, 0, nativeEntry, entryLength);

                /* write SBinary structure for entry */
                Marshal.WriteInt32 (iter, entryLength);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf(entryLength));
                Marshal.WriteInt32 (iter, (Int32) nativeEntry);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf(nativeEntry));

            }

            return RealArray;
        }

        public static IntPtr AdrListToPointer (AdrList list, MemContext MemCtx) {
            if (list == null) {
                return IntPtr.Zero;
            }

            IntPtr RealArray = MemCtx.Alloc<uint> (list.AEntries.Length + 1);

            Marshal.WriteInt32 (RealArray, list.AEntries.Length);
            IntPtr iter = (IntPtr) ((int) RealArray + Marshal.SizeOf (typeof (UInt32)));

            foreach (AdrEntry entry in list.AEntries) {
                NativeSizedArray Array = new NativeSizedArray ();
                Array.Data = PropArrayToPtr (entry.PropVals, out Array.Count, MemCtx);
                Marshal.StructureToPtr (Array, iter, false);
                iter = (IntPtr) ((int) iter + Marshal.SizeOf (Array));
            }

            return RealArray;
        }
    }
}
