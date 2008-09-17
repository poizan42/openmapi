//
// openmapi.org - NMapi C# Mapi API - URestriction.cs
//
// Copyright 2008 VipCom AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//

using System.Runtime.Serialization;

namespace NMapi.Table {

	/// <summary>
	///  A helper for the SRestriction structure.
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class URestriction
	{
		private SComparePropsRestriction resCompareProps;
		private SAndRestriction          resAnd;
		private SOrRestriction           resOr;
		private SNotRestriction          resNot;
		private SContentRestriction      resContent;
		private SPropertyRestriction     resProperty;
		private SBitMaskRestriction      resBitMask;
		private SSizeRestriction         resSize;
		private SExistRestriction        resExist;
		private SSubRestriction          resSub;
		private SCommentRestriction      resComment;

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResCompareProps")]
		public SComparePropsRestriction ResCompareProps {
			get { return resCompareProps; }
			set { resCompareProps = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResAnd")]
		public SAndRestriction ResAnd {
			get { return resAnd; }
			set { resAnd = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResOr")]
		public SOrRestriction ResOr {
			get { return resOr; }
			set { resOr = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResNot")]
		public SNotRestriction ResNot {
			get { return resNot; }
			set { resNot = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResContent")]
		public SContentRestriction ResContent {
			get { return resContent; }
			set { resContent = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResProperty")]
		public SPropertyRestriction ResProperty {
			get { return resProperty; }
			set { resProperty = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResBitMask")]
		public SBitMaskRestriction ResBitMask {
			get { return resBitMask; }
			set { resBitMask = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResSize")]
		public SSizeRestriction ResSize {
			get { return resSize; }
			set { resSize = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResExist")]
		public SExistRestriction ResExist {
			get { return resExist; }
			set { resExist = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResSub")]
		public SSubRestriction ResSub {
			get { return resSub; }
			set { resSub = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ResComment")]
		public SCommentRestriction ResComment {
			get { return resComment; }
			set { resComment = value; }
		}

		internal URestriction()
		{
		}
	
	}

}
