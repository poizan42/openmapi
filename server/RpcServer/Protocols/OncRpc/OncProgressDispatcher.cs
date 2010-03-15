//
// openmapi.org - NMapi C# Mapi API - OncProgressDispatcher.cs
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
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

using NMapi;
using NMapi.Events;
using NMapi.Flags;

using NMapi.Interop.MapiRPC;

namespace NMapi.Server {

	/// <summary></summary>
	public class OncProgressDispatcher
	{
		private OncProxySession session;
		
		private static object globalProgressSync = new object ();
		private static Queue<VirtualProgress> waiting = new Queue<VirtualProgress> ();
		
/*		private static void LazyDeliveryThread ()
		{
			while (waiting.Count > 0) {
				VirtualProgress progress = waiting.Dequeue ();
				progress.SendLatest ();
			}
			
			// TODO: never ending thread?
			
		}
*/

		public class VirtualProgress : IMapiProgress
		{			
			private OncProxySession session;
			private DateTime lastCall;

			private int connectionId;
			private int flags;
			private int min;
			private int max;
			private int lastValue;
			
			private volatile bool isWaiting;
			private int lazyValue = -1;
			private int lazyCount = -1;
			private int lazyTotal = -1;
			
			public int Flags { get { return flags; } }
			public int Min { get { return min; } }
			public int Max { get { return max; } }
			
			public VirtualProgress (OncProxySession session, int connectionId, int min, int max, int flags)
			{
				this.session = session;
				this.connectionId = connectionId;
				this.flags = flags;
				this.min = min;
				this.max = max;
				this.lastValue = -1;
				this.lastCall = DateTime.Today;

/*				if ((flags & NMAPI.MAPI_TOP_LEVEL) != 0) {
	                min = 0;
	                max = m_ulItems;
					SendSetLimits
	                m_pSession->ProgressSetLimits(m_pBar, m_ulMin, m_ulMax, m_pBar->ulFlags);
	            }
*/	
				Console.WriteLine ("progressbar - Ok!");
			}
			
			private void SendData (ProgressType type, int val1, int val2, int val3)
			{				
				ClEvProgress mapiProgress = new ClEvProgress ();
				mapiProgress.type = type;
				mapiProgress.ulID = connectionId;
				mapiProgress.ul1 = val1;
				mapiProgress.ul2 = val2;
				mapiProgress.ul3 = val3;
				session.ReverseEventConnectionServer.PushProgress (mapiProgress);
			}
			
			private void SendSetLimits (int min, int max, int flags)
			{
				SendData (ProgressType.PROGRESS_SETLIMITS, min, max, flags);
			}
			/*
			private void SendLatest ()
			{
				// lock?
				lock (globalProgressSync) {
					if (lazySend) {
						 = value;
						 = count;
						 = total;	
						isWaiting = false;
					}
				}
			}
			*/
			private void SendProgress (int value, int count, int total)
			{
				SendData (ProgressType.PROGRESS_UPDATE, value, count, total);
			}
			
			public void SetLimits (int min, int max, int flags)
			{
				this.flags = flags;
				this.min = min;
				this.max = max;
			
				SendSetLimits (min, max, flags);
			}
			
			
				// TODO: we could actually send the average of the waiting period ...
				
				// have a custom thread for this.
/*
	            // delay notification
	            MyGetSystemTimeAsFileTime(&ft);
	            FileTime2LargeInteger(&ft, &li);
	            if (li.QuadPart < m_nexttime.QuadPart)
	                return S_OK;

	            m_nexttime.QuadPart = li.QuadPart + 5000000; //500msec, 10000 tics/msec
*/

			public void Progress (int progressValue, int itemOfTotal, int totalCount)
			{
				totalCount = Math.Max (1, totalCount);

//				int factor = progressValue > 0 ? progressValue : 1;
				int factor = max - min;
				double val = Convert.ToDouble (progressValue * factor) / Convert.ToDouble (totalCount);
				int scaled = Convert.ToInt32 (Math.Ceiling (val));


				Console.WriteLine ("max: " + max);
				Console.WriteLine ("min: " + min);
				Console.WriteLine ("totalCount: " + totalCount);
				Console.WriteLine ("factor: " + factor);
				Console.WriteLine ("val: " + val);
				Console.WriteLine ("progressing: " + scaled + " for " + itemOfTotal + "th item.");

	            if (scaled == lastValue)
	                return;

/*


				if (DateTime.Now - lastCall > xxx) {
					// TODO: lock with queue ...
					if (lazySend) {
						lazyValue = value;
						lazyCount = count;
						lazyTotal = total;

						if (!isWaiting) {
							waitQueue.Add (this);
							isWaiting = true;
						}
					}
				}

				// TODO always lock!
*/



//bool isTopLevel = ((flags & NMAPI.MAPI_TOP_LEVEL) != 0);
				bool isTopLevel = true; // TODO: hack! FIXME		TODO!
	
				if (isTopLevel)
					SendProgress (scaled, itemOfTotal, totalCount);
	            else
					SendProgress (scaled, 0, 0);

	            lastValue = scaled;
				lastCall = DateTime.Now;
			}
		}

		public OncProgressDispatcher (OncProxySession session)
		{
			this.session = session;
		}
		
		public IMapiProgress Build (ProgressBar progressBar)
		{
			if (progressBar != null)
				return Build (progressBar.ID, progressBar.Min, progressBar.Max, progressBar.Flags);
			return null;
		}

		public IMapiProgress Build (int connectionId, int min, int max, int flags)
		{
			return new VirtualProgress (session, connectionId, min, max, flags);
		}
		
	}

}
