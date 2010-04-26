/* 
 *  NMapiTool - Little Helper
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


using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System;
using NMapi.Utility;

using NDesk.Options;
using System.Diagnostics;

namespace NMapi.Dump
{
    public class NMapiDump {

        public static string FillSpace (int count) {
            string space = "";
            for (int i = 0; i < (count * 2); i++) {
                space += " ";
            }

            return space;
        }

        public static void DumpRecipients (IMessage msg, int depth) {

            IMapiTableReader reader = msg.GetRecipientTable (0);

            while (true) {

                RowSet rows = reader.GetRows (1);

                if (rows.Count == 0)
                    break;

                ConsoleColor SavedColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.Write (" Rcp");
                System.Console.ForegroundColor = SavedColor;
                System.Console.WriteLine ("");
            }
            
            if (reader != null)
                reader.Dispose (); 
        }

        public static void DumpMessage (IMessage msg, int depth) {

            PropertyTag[] tags = PropertyTag.ArrayFromIntegers (Property.Subject);
            PropertyValue[] props = msg.GetProps (tags, 0);

            string name;
            try {
                name = (string) props[0];
            } catch(InvalidCastException) {
                name = "";
            }

            System.Console.Write (FillSpace (depth));
            ConsoleColor SavedColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.Write ("[{0}", name);
            DumpRecipients (msg, depth);
            System.Console.Write ("]");
            System.Console.ForegroundColor = SavedColor;
            System.Console.WriteLine ("");
        }

        public static void DumpMessages (IMapiFolder folder, int depth) {
            PropertyTag[] tags = PropertyTag.ArrayFromIntegers (Property.EntryId);

            IMapiTable table = folder.GetContentsTable (0);
            table.SetColumns (tags, 0);

            while (true) {
                RowSet rows = table.QueryRows (1, 0);

                if (rows.Count == 0)
                    break;

                byte[] eid = (byte[]) rows.ARow[0].Props[0];

                IMessage msg = (IMessage) folder.OpenEntry (eid);
                DumpMessage (msg, depth);
                msg.Dispose ();
            }

        }

        public static void DumpFolder (IMsgStore store, byte[] EntryId, int depth) {

            IMapiFolder folder = (IMapiFolder) store.OpenEntry (EntryId);
            MapiPropHelper helper = new MapiPropHelper (folder);
            PropertyValue pv = helper.HrGetOneProp (Property.DisplayName);

            string name  = pv != null ? (string) pv : "[Error: DN]";
            pv = helper.HrGetOneProp (Property.FolderType);
            Flags.FolderType type = pv != null ? (Flags.FolderType) (int) pv : 0;


            System.Console.Write (FillSpace (depth));
            ConsoleColor SavedColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.Write (name);
            System.Console.ForegroundColor = SavedColor;
            System.Console.WriteLine ("/");

            DumpMessages (folder, depth + 1);
            
            if (type != Flags.FolderType.Search) {
                IMapiTableReader tr = folder.GetHierarchyTable (0);

                PropertyTag[] tags = tr.GetTags ();
                
                int IdxEntryId;
                for (IdxEntryId = 0; IdxEntryId < tags.Length; IdxEntryId++) {
                    if (tags[IdxEntryId].Tag == Property.EntryId) {
                        break;
                    }
                }

                while (true) {
                    RowSet rows = tr.GetRows (1);
                    if (rows.Count == 0)
                        break;
              
                    PropertyValue prop = rows.ARow[0].lpProps[IdxEntryId];
                    DumpFolder (store, (byte[]) prop, depth + 1);
                }

                tr.Dispose ();
            }

            folder.Dispose ();
        }

        private static void DumpProps (PropertyValue[] values) {

            foreach (PropertyValue prop in values) {
                System.Console.WriteLine ("{0:X} {1}", prop.PropTag, prop);
            }
        }

        public static void Dump (IMsgStore store) {
            PropertyValue pv = new MapiPropHelper (store).HrGetOneProp (Property.DisplayName);
            string name  = pv != null ? (string) pv : "[Error: DN]";

            ConsoleColor SavedColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.Write (name);
            System.Console.ForegroundColor = SavedColor;
            System.Console.WriteLine ("*");

            DumpFolder (store, null, 1);
        }

        public static void Main(string[] args) {

            if (args.Length < 4) {
                System.Console.WriteLine ("Usage: NMapiDump provider host user pass");
                return;
            }

            Stopwatch watch = null;
            bool loop = false;
            OptionSet p = new OptionSet () {
               { "t|time", v => watch = new Stopwatch () },
               { "l|loop", v => loop = true }
            };

            List<string> extra = p.Parse (args);
            args = extra.ToArray ();

            if (watch != null) {
               watch.Start ();
            }

            string provider = args[0];
            string host = args[1];
            string user = args[2];
            string pass = args[3];

            TypeResolver resolver = new TypeResolver ();
            resolver.AddAssembly ("mscorlib", new Version (2, 0));
            resolver.AddAssembly ("System", new Version (2, 0));
            resolver.AddAssembly ("NMapi", new Version (0, 1));
            PropertyLookup PropertyLookup = new PropertyLookup (resolver);

            System.Console.Write ("Loading Property names ... ");
            PropertyLookup.RegisterClass (typeof (NMapi.Flags.Property).FullName);
            PropertyLookup.RegisterClass (typeof (NMapi.Flags.Outlook).FullName);
            System.Console.Write (" OK\n");

            System.Console.Write ("Loading Providers ... ");
            Dictionary<string, string[]> Providers = ProviderManager.FindProviders ();
            System.Console.Write (" OK\n");

            if (Providers.ContainsKey (provider) == false) {
                System.Console.WriteLine ("Provider {0} not found", provider);  
                return;
            }

            if (watch != null) {
                System.Console.WriteLine ("Timed (Loading done): {0} {1}", watch.Elapsed, watch.ElapsedMilliseconds);
            }

            IMapiFactory factory = ProviderManager.GetFactory (Providers[provider]);
            IMapiSession session;

            do {
                try {
                    System.Console.WriteLine("Creating session");
                    session = factory.CreateMapiSession ();
                    System.Console.WriteLine("Logging on");
                    session.Logon (host, user, pass);
    
                    System.Console.WriteLine("Dumping store(s)");
                    try {
                        using (IMsgStore store = session.PrivateStore) {
                            Dump (store);
                        }
                    } catch(Exception e) {
                        System.Console.WriteLine("***ERROR*** while dumping store, message: " + e.Message);
                    }
    
                    try {
                        using (IMsgStore store = session.PublicStore) {
                            Dump (store);
                        }
                    } catch(NotImplementedException) {}
    
                    System.Console.WriteLine("Disposing session");
                    session.Dispose ();
    
                } catch (MapiException e) {
                    System.Console.WriteLine ("***ERROR***, message: " + e.Message);
                    loop = false;
                }
    
                if (watch != null) {
                    watch.Stop ();
                    System.Console.WriteLine ("Timed: {0} {1}", watch.Elapsed, watch.ElapsedMilliseconds);
                }
            } while(loop);
        }
    }
}
