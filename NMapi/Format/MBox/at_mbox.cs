// Copyright 2009 Alp Toker <alp@atoker.com>

// I just check this in, so we don't loose it;
// TODO: integrate properly + uncomment.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using System.Threading;

// namespace NDesk.Mail
/*

 namespace NMapi.Format.MBox
{
	public class Mailbox
	{
		public string filename;
		public FileInfo fileInfo;

		public Mailbox (string filename)
		{
			this.filename = filename;
			fileInfo = new FileInfo (filename);
		}

		// TODO: Locking with eg. flock()

		public long Size
		{
			get {
				fileInfo.Refresh ();
				return fileInfo.Length;
			}
		}

		public IEnumerable<Message> Parse ()
		{
			return MboxParser.Parse (filename);
		}

		public IEnumerable<Message> Watch ()
		{
			return Watch (false);
		}

		public IEnumerable<Message> Watch (bool includeExisting)
		{
			FileStream fs = new FileStream (filename, FileMode.Open, FileAccess.Read);

			if (fs.CanSeek && !includeExisting) {
				fs.Position = fs.Length;
				// TODO: Ensure that we're at the start of a new line
			} else {
				foreach (Message msg in MboxParser.Parse (fs))
					if (includeExisting)
						yield return msg;
			}

			string path = Path.GetFullPath (fs.Name);
			FileSystemWatcher fsw = new FileSystemWatcher (Path.GetDirectoryName (path), Path.GetFileName (path));

			while (true) {
				WaitForChangedResult result = fsw.WaitForChanged (WatcherChangeTypes.Changed);
				if (result.ChangeType != WatcherChangeTypes.Changed)
					break;

				foreach (Message msg in MboxParser.Parse (fs))
					yield return msg;
			}

			fs.Close ();
		}

		public void Append (IEnumerable<Message> msgs)
		{
			using (FileStream fs = new FileStream (filename, FileMode.Append, FileAccess.Write)) {
				MboxParser.Serialize (fs, msgs);
			}
		}

		protected Thread watcherThread = null;
		public bool WatchActive
		{
			get {
				return watcherThread != null;
			} set {
				if (true && watcherThread == null)
					watcherThread = SpawnWatcher ();
				else {
					watcherThread.Abort ();
					watcherThread = null;
				}
			}
		}

		protected Thread SpawnWatcher ()
		{
			Thread t = new Thread (new ThreadStart (RunParse));
			t.Start ();
			return t;
		}

		void RunParse ()
		{
			foreach (Message msg in Watch ()) {
				if (NewMessages != null)
					NewMessages (this, new MailEventArgs (new Message[] {msg}));
			}
		}

		public event EventHandler<MailEventArgs> NewMessages;
	}

	public class MailEventArgs : EventArgs
	{
		public MailEventArgs (params Message[] messages)
		{
			this.Messages = messages;
		}

		public MailEventArgs (IEnumerable<Message> messages)
		{
			this.Messages = messages;
		}

		public readonly IEnumerable<Message> Messages;
	}

	// Note that mbox files are not fully standardized.
	// Specifications used:
	//   http://qmail.org/qmail-manual-html/man5/mbox.html
	//   http://www.postfix.org/local.8.html
	public class MboxParser
	{
		public bool TrustContentLength = false;
		const string fromString = "From ";

		public static IEnumerable<Message> Parse (string fname)
		{
			FileStream fs = new FileStream (fname, FileMode.Open, FileAccess.Read);
			return Parse (fs);
		}

		public static IEnumerable<Message> Parse (Stream stream)
		{
			TextReader reader = new StreamReader (stream, Encoding.ASCII);

			Message msg = null;
			StringBuilder sb = null;

			while (true) {
				string ln = reader.ReadLine ();

				if (ln == null || ln.StartsWith (fromString)) {
					if (msg != null) {
						if (sb.Length > 0 && sb[sb.Length - 1] == '\n')
							msg.Body = sb.ToString (0, sb.Length - 1);
						else {
							//if (sb.Length > 0)
							//	Console.Error.WriteLine ("mbox inconsistency");
							msg.Body = sb.ToString ();
						}

						yield return msg;

						sb = null;
						msg = null;
					}

					if (ln == null)
						yield break;

					msg = new Message ();
					sb = new StringBuilder ();

					// Parse the From_ line
					ParseFromLine (msg, ln);

					// Strictly speaking, ParseHeader() should observe From_ but currently doesn't
					ParseHeader (msg, reader);
					continue;
				}

				if (msg == null)
					continue;

				// From_ unescape the body
				ln = Regex.Replace (ln, "^>(>*" + fromString + ")", "$1");
				sb.AppendLine (ln);
			}
		}

		static bool ParseFromLine (Message msg, string ln)
		{
			int pos = fromString.Length;
			// Mandatory field: 'Envelope sender'
			int nextPos = ln.IndexOf (' ', pos);
			if (nextPos == -1)
				throw new Exception ("mbox parsing error: malformed From_ line");
			string envsender = ln.Substring (pos, nextPos - pos);
			msg.EnvelopeSender = envsender;
			pos = nextPos + 1;

			// Skip trailing spaces, greedily to be on the safe side.
			while (ln[pos] == ' ')
				pos++;

			// Mandatory field: 'Delivery date'
			// Exactly 24 characters, in asctime format
			string deliverydate = ln.Substring (pos, 24);
			pos += 24;
			DateTime deliveryDT;
			if (DateTime.TryParseExact (deliverydate, RfcDate.asctime_date, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out deliveryDT))
				msg.DeliveryDate = deliveryDT.ToUniversalTime ();
			else
				throw new Exception ("mbox parsing error: invalid From_ delivery date");

			// Optional field: 'More info'
			if (pos < ln.Length) {
				string moreinfo = ln.Substring (pos, ln.Length - pos);
				Console.Error.WriteLine ("moreinfo: '{0}'", moreinfo);
			}

			return true;
		}

		[Flags]
		enum MozMsgFlags
		{
			// X-Mozilla-Status
			Read = 0x0001,
			Replied = 0x0002,
			Marked = 0x0004,
			Expunged = 0x0008,
			HasRe = 0x0010,
			Elided = 0x0020,
			Offline = 0x0080,
			Watched = 0x0100,
			SenderAuthed = 0x0200,
			Partial = 0x0400,
			Queued = 0x0800,
			Forwarded = 0x1000,
			Priorities = 0xe000,

			// X-Mozilla-Status2
			New = 0x00010000,
			Ignored = 0x00040000,
			ImapDeleted = 0x00200000,
			MdnReportNeeded = 0x00400000,
			MdnReportSent = 0x00800000,
			Template = 0x01000000,
			Labels = 0x0e000000,
			Attachment = 0x10000000,
		}

		// http://mxr.mozilla.org/seamonkey/source/mailnews/base/public/nsMsgMessageFlags.h
		public static void ParseMozHeaders (Account account, Message msg)
		{
			string str;
			int val;
			MozMsgFlags flags;

			if (msg.Headers.TryGetValue ("x-mozilla-draft-info", out str))
				msg.AddLabel (account.DraftLabel);

			if (!msg.Headers.TryGetValue ("x-mozilla-status", out str))
				return;

			if (!Int32.TryParse (str, NumberStyles.HexNumber, null, out val))
				return;

			flags = (MozMsgFlags)val;

			if ((flags & MozMsgFlags.Read) != 0)
				msg.AddLabel (account.ReadLabel);

			if ((flags & MozMsgFlags.Replied) != 0)
				msg.AddLabel (account.GetLabel ("moz-replied"));

			if ((flags & MozMsgFlags.Marked) != 0)
				msg.AddLabel (account.StarredLabel);

			if ((flags & MozMsgFlags.Expunged) != 0)
				msg.AddLabel (account.GetLabel ("moz-expunged"));

			//if ((flags & MozMsgFlags.HasRe) != 0)
			//	msg.Headers["subject"] = "Re: " + msg.Headers["subject"];

			if ((flags & MozMsgFlags.Partial) != 0)
				msg.AddLabel (account.GetLabel ("moz-partial"));

			if ((flags & MozMsgFlags.Queued) != 0)
				msg.AddLabel (account.GetLabel ("moz-queued"));

			if ((flags & MozMsgFlags.Forwarded) != 0)
				msg.AddLabel (account.GetLabel ("moz-forwarded"));

			if (!msg.Headers.TryGetValue ("x-mozilla-status2", out str))
				return;

			if (!Int32.TryParse (str, NumberStyles.HexNumber, null, out val))
				return;

			flags = (MozMsgFlags)val;

			if ((flags & MozMsgFlags.Ignored) != 0)
				msg.AddLabel (account.MutedLabel);

			if ((flags & MozMsgFlags.MdnReportSent) != 0)
				msg.AddLabel (account.GetLabel ("moz-mdn-report-sent"));

			if ((flags & MozMsgFlags.Template) != 0)
				msg.AddLabel (account.GetLabel ("moz-template"));
		}

		public static void SerializeMozHeaders (Account account, Message msg)
		{
			MozMsgFlags flags = 0;

			if (msg.HasLabel (account.DraftLabel))
				msg.Headers["x-mozilla-draft-info"] = "internal/draft; vcard=0; receipt=0; uuencode=0";

			if (msg.HasLabel (account.ReadLabel))
				flags |= MozMsgFlags.Read;

			if (msg.HasLabel (account.GetLabel ("moz-replied")))
				flags |= MozMsgFlags.Replied;

			if (msg.HasLabel (account.StarredLabel))
				flags |= MozMsgFlags.Marked;

			msg.Headers["x-mozilla-status"] = ((int)flags).ToString ("x4");


			flags = 0;

			if (msg.HasLabel (account.MutedLabel))
				flags |= MozMsgFlags.Ignored;

			msg.Headers["x-mozilla-status2"] = ((int)flags).ToString ("x8");
		}

		public static IEnumerable<Message> ParseEmls (string path)
		{
			string searchPattern = "*.eml";
			foreach (string fname in Directory.GetFiles (path, searchPattern))
				yield return ParseEml (fname);
		}

		public static Message ParseEml (string fname)
		{
			TextReader reader = new StreamReader (fname, Encoding.ASCII);
			Message msg = new Message ();
			ParseHeader (msg, reader);
			// TODO: Consider CR/LF etc.
			msg.Body = reader.ReadToEnd ();
			return msg;
		}

		static bool ParseHeader (Message msg, TextReader reader)
		{
			string key = null;
			StringBuilder sb = null;

			while (true) {
				string ln = reader.ReadLine ();

				// TODO: Clean this up!

				if (ln == null)
					//return false;
					break;

				// Newline in the header marks the start of the body
				if (ln == String.Empty) {
					if (key != null)
						msg.Headers[key] = sb.ToString ();
					return true;
				}

				// Supports header "unfolding"
				char[] delim = {':', ' ', '\t'};
				int pos = ln.IndexOfAny (delim);
				if (pos != -1 && ln[pos] == ':') {
					if (key != null)
						msg.Headers[key] = sb.ToString ();

					key = ln.Substring (0, pos).ToLower ();
					pos++;
					sb = new StringBuilder ();
					sb.Append (ln.Substring (pos, ln.Length - pos).Trim ());
					continue;
				}

				if (key != null) {
					sb.Append (' ');
					sb.Append (ln.Trim ());
				}
			}

			return false;
		}

		static void SerializeHeaders (Stream stream, Message msg)
		{
			StreamWriter writer = new StreamWriter (stream, Encoding.ASCII);

			foreach (KeyValuePair<string, string> header in msg.Headers) {
				writer.Write (header.Key);
				writer.Write (": ");
				writer.Write (header.Value);
				writer.WriteLine ();
			}

			writer.WriteLine ();
			writer.Flush ();
		}

		public static void Serialize (Stream stream, IEnumerable<Message> msgs)
		{
			foreach (Message msg in msgs)
				Serialize (stream, msg);
		}

		public static void Serialize (Stream stream, Message msg)
		{
			string envsender = "-";
			DateTime deliveryDate = msg.DeliveryDate;
			string deliveryDateString = deliveryDate.ToString (RfcDate.asctime_date);
			if (deliveryDateString.Length != 24)
				throw new Exception ("mbox: Date formatting error");
			string moreinfo = "";

			StreamWriter writer = new StreamWriter (stream, Encoding.ASCII);
			writer.Write (fromString);
			writer.Write (envsender);
			writer.Write (' ');
			writer.Write (deliveryDateString);
			writer.Write (moreinfo);
			writer.WriteLine ();

			writer.Flush ();
			SerializeHeaders (stream, msg);

			// From_ escape the body
			string body = msg.Body;
			body = Regex.Replace (body, "^(>*" + fromString + ")", ">$1", RegexOptions.Multiline);
			writer.Write (body);
			writer.WriteLine ();

			writer.Flush ();
		}

		public static void SerializeEml (Stream stream, Message msg)
		{
			StreamWriter writer = new StreamWriter (stream, Encoding.ASCII);

			SerializeHeaders (stream, msg);

			string body = msg.Body;
			writer.Write (body);

			writer.Flush ();
		}
	}

	// Inspired by mcs/class/System/System.Net/MonoHttpDate.cs
	// TODO: Validate correctness of parsing, especially re. timezone preservation etc.
	class RfcDate
	{
		public const string rfc1123_date = "r";
		public const string rfc850_date = "dddd, dd-MMM-yy HH:mm:ss G\\MT";
		public const string asctime_date = "ddd MMM dd HH:mm:ss yyyy";
		public static readonly string[] formats = new string[] {rfc1123_date, rfc850_date, asctime_date};

		public static DateTime Parse (string dateStr)
		{
			return DateTime.ParseExact (dateStr,
					formats,
					CultureInfo.InvariantCulture,
					DateTimeStyles.AllowWhiteSpaces).ToLocalTime ();
		}
	}
}
*/