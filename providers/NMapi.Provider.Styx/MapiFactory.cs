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
using System.Linq;
using System.Text;

using NMapi;
using NMapi.Admin;
using NMapi.Events;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Provider.Styx {

    [MapiFactory ("org.openmapi.styx")]
    class MapiFactory : IMapiFactory {

        public bool SupportsNotifications {
            get { return false; }
        }

        public IMapiSession CreateMapiSession () {
            CMapi.Initialize (CMapi.InitFlags.MultithreadNotifications | CMapi.InitFlags.NoCoInit);
            return new Session ();
        }

        public IMapiAdmin CreateMapiAdmin(string host) {
            throw new NotImplementedException ();
        }
    }
}
