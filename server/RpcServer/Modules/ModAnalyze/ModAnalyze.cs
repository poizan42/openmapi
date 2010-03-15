//
// openmapi.org - NMapi C# Mapi API - ModAnalyze.cs
//
// Copyright 2010 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//

using System;
using System.IO;

using NMapi;

namespace NMapi.Server {

	/// <summary>
	///  
	/// </summary>
	public class ModAnalyze : IServerModule, IDisposable
	{
		private StreamWriter writer;

		public string Name {
			get { return "NMapi Analyze Module"; }
		}

		public string ShortName {
			get { return "ModAnalyze"; }
		}

		public Version Version {
			get { return new Version (0, 1); }

		}
		
		
		
		public ModAnalyze (LifeCycle lifeCycle)
		{
			
		}
		
		private void AppendLine (string str)
		{
			if (writer == null)
				writer = File.AppendText ("call_stats.log");
			
			// TODO: flex file
			// TODO: catch exceptions ...
				
//			Console.WriteLine ("mod analyze: writing stuff!");
			writer.WriteLine (str);
			writer.Flush ();
		}
		
		
		private bool disposed;
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
		
		~ModAnalyze ()
		{
			if (!disposed)
				Dispose (false);
		}
		
		protected void Dispose (bool disposing)
		{
			try {
				if (writer != null)
					writer.Close ();
			} catch {
				// ignore
			}
			
			disposed = true;
		}
		
		
		// TODO: API out of date -- pass by reference, etc.
		
		[PreCall]
		public void CallStarted (RemoteCall call, object o)
		{
			AppendLine ("SESSION_NUMBER, " + call + ", start, " + DateTime.Now);
		}
		
		[PostCall]
		public void CallStopped (RemoteCall call, object o)
		{
			AppendLine ("SESSION_NUMBER, " + call + ", stop, " + DateTime.Now);
		}
		
	}

}
