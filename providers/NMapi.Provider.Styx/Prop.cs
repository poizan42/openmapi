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
using System.Text;
using System.Runtime.InteropServices;

using NMapi;
using NMapi.Properties;
using NMapi.Provider.Styx.Interop;

namespace NMapi.Provider.Styx
{
    public class Prop : Unknown, IMapiProp
    {        
        public Prop (IntPtr cobj) : base (cobj) {

        }

        #region IMapiProp Members

        public MapiError GetLastError (int hresult, int flags) {
            IntPtr ErrorHandle;
            int hr = CMapi_Prop_GetLastError (cobj, hresult, (uint) flags, out ErrorHandle);
            Transmogrify.CheckHResult (hr);
            MapiError error = Transmogrify.PtrToMapiError (ErrorHandle);
            CMapi.FreeBuffer(ErrorHandle);
            return error;
        }

        public PropertyValue[] GetProps (PropertyTag[] Tags, int flags) {

            PropertyValue[] PropArray;
            using (MemContext MemCtx = new MemContext ()) {

                IntPtr TagArrayHandle = Transmogrify.TagArrayToPtr (Tags, MemCtx);
                IntPtr PropArrayHandle;
                uint count;
                int hr = CMapi_Prop_GetProps (cobj, TagArrayHandle, (uint) flags, out count, out PropArrayHandle);

                //FIXME: hack!
                if (hr != 0x40380)
                    Transmogrify.CheckHResult (hr);

                PropArray = Transmogrify.PtrToPropArray (PropArrayHandle, count);
                CMapi.FreeBuffer(PropArrayHandle);
            }

            return PropArray;
        }

        public IBase OpenProperty (int propTag) {
            /* XXX how to get interFace here? NULL is not allowed */
            /* TXC: return OpenProperty(propTag, null, 0, 0); */
            throw new NotImplementedException ();
        }

        public IBase OpenProperty (int propTag, NMapiGuid interFace, int interfaceOptions, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr ifHandle = Transmogrify.GuidToPtr (interFace, MemCtx);
                IntPtr UnkHandle;
                int hr = CMapi_Prop_OpenProperty (cobj, (uint) propTag, ifHandle, (uint) interfaceOptions, (uint) flags, out UnkHandle);
                Transmogrify.CheckHResult (hr);

                Unknown unk = new Unknown (UnkHandle);
                return unk;
            }
        }

        public void SaveChanges (int flags) {

            int hr = CMapi_Prop_SaveChanges (cobj, (uint) flags);

            if (hr != 0) {
                Transmogrify.CheckHResult (hr);
            }
        }

        public PropertyProblem[] DeleteProps (PropertyTag[] propTagArray) {

            using (MemContext MemCtx = new MemContext ()) {

                IntPtr TagArrayHandle = Transmogrify.TagArrayToPtr (propTagArray, MemCtx);
                IntPtr ProblemHandle;
                int hr = CMapi_Prop_DeleteProps (cobj, TagArrayHandle, out ProblemHandle);
                Transmogrify.CheckHResult (hr);
                PropertyProblem[] problems = Transmogrify.PtrToProblemArray (ProblemHandle);
                CMapi.FreeBuffer(ProblemHandle);
                return problems;
            }

        }

        public PropertyTag[] GetIDsFromNames (MapiNameId[] propNames, int flags) {
            PropertyTag[] tags;

            using (MemContext MemCtx = new MemContext ()) {
                IntPtr NamesHandle = Transmogrify.MapiNameIdsToPtr (propNames, MemCtx);
                IntPtr TagHandle;
                int hr = CMapi_Prop_GetIDsFromNames (cobj, (uint) propNames.Length, NamesHandle, (uint) flags, out TagHandle);

                //FIXME
                if (hr != 0x40380)
                    Transmogrify.CheckHResult (hr);

                tags = Transmogrify.PtrToTagArray (TagHandle);
            }

            return tags;
        }

        public GetNamesFromIDsResult GetNamesFromIDs (PropertyTag[] propTags, NMapiGuid propSetGuid, int flags) {
            throw new NotImplementedException ();
        }

        public PropertyTag[] GetPropList (int flags) {
            IntPtr TagArrayHandle;
            int hr = CMapi_Prop_GetPropList (cobj, (uint) flags, out TagArrayHandle);
            Transmogrify.CheckHResult (hr);
            PropertyTag[] tags = Transmogrify.PtrToTagArray (TagArrayHandle);
            CMapi.FreeBuffer(TagArrayHandle);

            return tags;
        }

        public PropertyProblem[] SetProps (PropertyValue[] propArray) {

            using (MemContext MemCtx = new MemContext ()) {

                uint count;
                IntPtr ProblemHandle;
                IntPtr PropArrayHandle = Transmogrify.PropArrayToPtr (propArray, out count, MemCtx);

                int hr = CMapi_Prop_SetProps (cobj, count, PropArrayHandle, out ProblemHandle);
                Transmogrify.CheckHResult (hr);

                PropertyProblem[] problems = Transmogrify.PtrToProblemArray (ProblemHandle);
                CMapi.FreeBuffer(ProblemHandle);

                return problems;
            }
        }

        #endregion

        #region C-Glue
        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_GetLastError (IntPtr prop,
                                    int hResult,
                                    uint ulFlags,
                                    out IntPtr lppMAPIError);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_SaveChanges (IntPtr prop, uint Flags);

        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_GetProps (IntPtr prop, IntPtr PropTagArray, uint Flags, out uint cValues, out IntPtr PropArray);

        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_GetPropList (IntPtr prop,
                                    uint ulFlags,
                                    out IntPtr lppPropTagArray /* LPSPropTagArray* */);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_OpenProperty (IntPtr prop,
                                    uint ulPropTag,
                                    IntPtr lpiid,
                                    uint ulInterfaceOptions,
                                    uint ulFlags,
                                    out IntPtr lppUnk);

        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_SetProps (IntPtr prop, uint Values, IntPtr PropArray, out IntPtr Problems);

        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_DeleteProps (IntPtr prop,
                                    IntPtr lpPropTagArray, //LPSPropTagArray
                                    out IntPtr lppProblems); //LPSPropProblemArray*
        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_CopyTo (IntPtr prop,
                                    uint ciidExclude,
                                    IntPtr rgiidExclude,
                                    IntPtr lpExcludeProps, //LPSPropTagArray
                                    uint ulUIParam,
                                    IntPtr lpProgress,
                                    IntPtr lpInterface,
                                    IntPtr lpDestObj,
                                    uint ulFlags,
                                    out IntPtr lppProblems); //LPSPropProblemArray*
        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_CopyProps (IntPtr prop,
                                    IntPtr lpIncludeProps /* LPSPropTagArray */,
                                    uint ulUIParam,
                                    IntPtr lpProgress,
                                    IntPtr lpInterface,
                                    IntPtr lpDestObj,
                                    uint ulFlags,
                                    out IntPtr lppProblems /* LPSPropProblemArray* */);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_GetNamesFromIDs (IntPtr prop,
                                    IntPtr lppPropTags /* LPSPropTagArray* */,
                                    IntPtr lpPropSetGuid /* LPGUID */,
                                    uint ulFlags,
                                    out uint lpcPropNames,
                                    out IntPtr lpppPropNames /* LPMAPINAMEID** */);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Prop_GetIDsFromNames (IntPtr prop,
                                    uint cPropNames,
                                    IntPtr lppPropNames  /* LPMAPINAMEID* */,
                                    uint ulFlags,
                                    out IntPtr lppPropTags  /* LPSPropTagArray* */);

        #endregion
    }
}
