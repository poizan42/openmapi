//
// openmapi.org - NMapi C# Mapi API - Unmask.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
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

using System;
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;

namespace NMapi.Tools.Shell {

	public sealed class UnmaskCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "unmask";
			}
		}

		public override string[] Aliases {
			get {
				return new string [0];
			}
		}

		public override string Description {
			get {
				return "Unmask an object";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public UnmaskCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			string[] prms = ShellUtil.SplitParams (context.Param);
			if (prms.Length < 2) {
				RequireMsg ("key", "mask_provider");
				return;
			}

			string keyName = prms [0];
			string maskProvider = prms [1];

/*			using (IMapiProp obj = state.OpenPropObj (keyName)) {
				if (obj == null) {
					driver.WriteLine ("Unknown Key ID!");
					return;
				}
				int propTag = -1;
				try {
					string[] tmp = TypeResolver.SplitTypeNameAndProperty (propName);
					string typeName = tmp [0];
					string fieldName = tmp [1];
					propTag = state.PropertyLookup.GetValue (typeName, fieldName);
				} catch (Exception e) {
					driver.WriteLine ("Invalid/unregistered Property!");
					return;
				}
				PropertyValue val = null;
				try {
					// TODO: Depends on type!

					val = new MapiPropHelper (obj).HrGetOneProp (propTag);

					IStream streamsrc = null, streamdst = null;

					try { 
						fileName = Path.GetTempFileName ();

						IStream propStream = (IStream) source.OpenProperty (propTag,
								Guids.IID_IStream, 0, 0);

						MemoryStream memStream = new MemoryStream ();
						propStream.GetData (memStream);

						memStream.Close (); // TODO: Write!

					} catch (IOException e) {
						throw new MapiException(e);
					}
					finally {
						if (propStream != null)
							propStream.Close ();
					}


				} catch (MapiException e) {
					if (e.HResult == Error.NotFound) {
						driver.WriteLine ("Property does not exist on this object!");
						return;
					}
					throw;
				}
				PropertyType type = PropertyTypeHelper.PROP_TYPE (val.PropTag);
				object data = val.GetValueObj ();
*/
				driver.WriteLine ("TODO!");
//			}
		}

	}
}
