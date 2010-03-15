//
// openmapi.org - NMapi C# Mapi API - ProgressBar.cs
//
// Copyright 2010 Topalis AG
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
using System.Text;
using System.Collections.Generic;

namespace NMapi.Tools.Shell {

	/// <summary>Stub implementation of progressbar.</summary>
	/// <remarks>
	///  This implementation does not output any progress data.
	///  This could be used by the web-interface to ensure the screen is not 
	///  filled with garbage from the default implementation.	
	/// </remarks>
	public sealed class StubProgressBar : ShellProgressBar
	{
		private const ushort PRECISION = 1;
		
		public StubProgressBar () : base (PRECISION, new TimeSpan (0, 0, 0, 0, 0))
		{
		}
		
		protected override void _Progress (ShellProgressBar.ProgressInfo progressInfo, bool isLast)
		{
			// Do nothing.
		}
	}
	
	/// <summary>Default implementation of progressbar.</summary>
	/// <remarks>
	///  Draws a progressbar to the console and updates it. 
	///  Updates are performed by drawing CursorBackwards-Characters (\b). 
	///  The output must support this for this to work and output to the 
	///  Console while the ProgressBar is being called must be avoided.
	/// </remarks>
	public sealed class ConsoleProgressBar : ShellProgressBar
	{
		private const ushort PRECISION = 1;

		private const int INTERVAL_MS = 200;
		private const char PROGRESS_CHAR = '#';
		private const ushort MAX_BARLENGTH = 80;
		
		private const string PREFIX = " ... [";
		private const string POSTFIX = "]";
		private const string DONE = " done.";
		
		private int lastLength;
		private bool drawBar;
		private bool printCaption;
		private bool printItemProgress;
		private string caption;
		
		/// <summary></summary>
		/// <remarks>Default is true.</remarks>
		public bool DrawBar {
			get { return drawBar; }
			set { drawBar = value; }
		}
		
		/// <summary></summary>
		/// <remarks>Default is true.</remarks>
		public bool PrintCaption {
			get { return printCaption; }
			set { printCaption = value; }
		}
		
		/// <summary></summary>
		/// <remarks>Default is true.</remarks>
		public bool PrintItemProgress {
			get { return printItemProgress; }
			set { printItemProgress = value; }
		}
		
		/// <summary>Caption to be displayed, if any.</summary>
		public string Caption {
			get { return caption; }
			set { caption = (value != null) ? value : ""; }
		}

		/// <summary>Creates a new ProgressBar without a caption.</summary>
		public ConsoleProgressBar () : this (null)
		{
		}
		
		/// <summary>Creates a new ProgressBar with a caption.</summary>
		/// <param name="caption">A caption/title to be displayed.</param>
		public ConsoleProgressBar (string caption)
			: base (PRECISION, new TimeSpan (0, 0, 0, 0, INTERVAL_MS))
		{
			this.drawBar = true;
			this.printCaption = true;
			this.printItemProgress = true;
			this.caption = (caption != null) ? caption : String.Empty;
			this.lastLength = -1;
		}
		
		protected override void _Progress (ShellProgressBar.ProgressInfo progressInfo, bool isLast)
		{
			if (lastLength > 0) {
				StringBuilder builder = new StringBuilder ();
				for (int i=0; i<lastLength; i++)
					builder.Append ("\b");
				Console.Write (builder.ToString ());				
			}
			string result = BuildString (progressInfo, isLast);
			lastLength = result.Length;
			Console.Write (result);
		}
		
		private string BuildPercentString (double percent)
		{
			StringBuilder format = new StringBuilder ();
			int spaceNumber = "XXX".Length + 1 + PRECISION;
			format.Append (" {0,");
			format.Append (spaceNumber);
			format.Append (":0");
			if (PRECISION > 0) {
				format.Append (".");
				for (int i=0; i<PRECISION; i++)
					format.Append ("0");
			}
			format.Append ("}% ");
			return String.Format (format.ToString (), Math.Round (percent, PRECISION));
		}
		
		private string BuildItemOfString (int itemOfTotal, int total)
		{
			string tcStr = String.Empty + total;
			string ioStr = String.Empty + itemOfTotal;
			
			if (ioStr.Length > tcStr.Length)
				throw new Exception ("ERROR: ioStr.Length > tcStr.Length");
			
			int spacesCount = tcStr.Length - ioStr.Length;
			if (spacesCount == 0)
				spacesCount = 1;
			
			StringBuilder builder = new StringBuilder ();
			for (int i=0; i<spacesCount; i++)
				builder.Append (" ");
			builder.Append ("(");
			builder.Append (ioStr);
			builder.Append ("/");
			builder.Append (tcStr);
			builder.Append (")");
			return builder.ToString ();
		}
		
		private int CalcBarCharsLength (string itemOfStr)
		{
			int reservedSpace = (PREFIX + " XXX.% " + itemOfStr + POSTFIX + DONE).Length + PRECISION;
			if (printCaption)
				reservedSpace += caption.Length;
			int maxPossible = Console.WindowWidth - reservedSpace;
			if (maxPossible < 0)
				maxPossible = 0; // TODO: in this case, we should probably fall back to displaying JUST a percentage?
			return Math.Min (maxPossible, MAX_BARLENGTH);
		}
		
		private string BuildString (ShellProgressBar.ProgressInfo progressInfo, bool isLast)
		{
			StringBuilder output = new StringBuilder ();

			string itemOfStr = String.Empty;
			if (printItemProgress)
				itemOfStr = BuildItemOfString (progressInfo.ItemOfTotal, progressInfo.Total);

			if (printCaption)
				output.Append (caption);

			if (drawBar) {
				output.Append (PREFIX);

				int barCharsLength = CalcBarCharsLength (itemOfStr);
				int usedChars = Convert.ToInt32 ((progressInfo.Percent/100.0) * barCharsLength);
				int unusedChars = barCharsLength - usedChars;

				int offsetOfBarChars = 0;
				bool percentPut = false;				
				
				Action<bool> putProgressChar = (isProgress) => {
					char c = (isProgress) ? PROGRESS_CHAR : ' ';
					output.Append (c);
					offsetOfBarChars++;
					if (!percentPut && offsetOfBarChars > (barCharsLength / 2)) {
						output.Append (BuildPercentString (progressInfo.Percent));
						percentPut = true;
					}
				};
				
				for (int i=0; i< usedChars; i++)
					putProgressChar (true);
				for (int i=0; i< unusedChars; i++)
					putProgressChar (false);
				output.Append (POSTFIX);
			} else
				output.Append (BuildPercentString (progressInfo.Percent));
			
			if (printItemProgress)
				output.Append (itemOfStr);
					
			if (isLast) {
				output.Append (DONE);
				output.AppendLine ();
			}

			return output.ToString ();
		}
		
	}

	/// <summary>Base-class for MapiShell-ProgressBars.</summary>
	/// <remarks>
	///  As the MapiShell can be embedded in various contexts and controlled 
	///  by different drivers, e.g. a plain console driver or the web interface, 
	///  different means to draw the bar are needed. 
	///  
	///  Thread-safety is required, because the callbacks happen on a different thread!
	/// </remarks>
	public abstract class ShellProgressBar : IMapiProgress
	{
		private readonly TimeSpan interval;
		private readonly ushort precision;
		private volatile bool isClosed;
		private DateTime lastWrite;
		private ProgressInfo lastValue;
		private int flags;
		private int min;
		private int max;
		
		public int Flags { get { return flags; } }
		public int Min { get { return min; } }
		public int Max { get { return max; } }
		
		public struct ProgressInfo
		{
			public double Percent { get; set; }
			public int ItemOfTotal { get; set; }
			public int Total { get; set; }
			
			public ProgressInfo (double percent , int itemOfTotal, int total)
			{
				this.Percent = percent;
				this.ItemOfTotal = itemOfTotal;
				this.Total = total;
			}
		}
		
		public ShellProgressBar (ushort precision, TimeSpan interval)
		{
			this.precision = precision;
			this.interval = interval;
			this.flags = 0;
			this.min = 0;
			this.max = 1000;
			this.lastWrite = DateTime.Now;
			this.lastValue = new ProgressInfo (0, 0, 0);
		}
		
		/// <summary>Advance the progress bar.</summary>
		/// <remarks>
		///  <para>
		///   A call to this method is only made after at least the specified 
		///   interval of time has passed since the previous call.
		///  </para>
		///  <para>The call is guaranteed to be synchronized.</para>
		/// </remarks>
		/// <param name="progressInfo">The progress data (percent, itemOfTotal, total)</param>
		/// <param name="isLast">True if this is the last call and 'the progressbar has been closed.</param>
		protected abstract void _Progress (ProgressInfo progressInfo, bool isLast);

		private void DoProgress (ProgressInfo progressInfo)
		{
			lock (this) {
				if (progressInfo.Equals (lastValue))
					return;
				DateTime now = DateTime.Now;
				if ((now - lastWrite) >= interval) {
					lastWrite = now;
					lastValue = progressInfo;
					_Progress (progressInfo, false);
				}
			}
		}

		/// <summary>Finish drawing the bar. Must be called!</summary>
		public void Close ()
		{
			if (isClosed)
				return;
			isClosed = true;
			lock (this) {
				_Progress (lastValue, true);
			}
		}

		public void SetLimits (int min, int max, int flags)
		{
			if (isClosed)
				return;
			lock (this) {
				this.flags = flags;
				this.min = min;
				if (max < min)
					max = min;
				this.max = max;
			}
		}

		public void Progress (int progressValue, int itemOfTotal, int totalCount)
		{
			if (isClosed)
				return;
			
			int factor = max - min;
			if (factor < 0)
				factor = 0;
			totalCount = Math.Max (1, totalCount);
			double percentValue = Convert.ToDouble (progressValue * factor) / 
							Convert.ToDouble (totalCount);

			// TODO: top-level support?
			double rounded = Math.Round (percentValue, precision);
			
			ProgressInfo progressInfo = new ProgressInfo (rounded, itemOfTotal, totalCount);
			DoProgress (progressInfo);
		}
		
	}
	
}
