// openmapi.org - NMapi C# IMAP Gateway - IMAP_lexer.lex.cs
//
// Copyright 2008 Topalis AG
//
// Author: Andreas Huegel <andreas.huegel@topalis.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//

using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

using TUVienna.CS_CUP.Runtime;
using TUVienna;
using NMapi.Gateways.IMAP;
/*
*/
namespace NMapi.Gateways.IMAP
{
	public class Sample
	{
		Yylex yy;
        System.IO.StreamReader f;
		public static void do_printout(string[] argv)
		{
			string [] args = Environment.GetCommandLineArgs();
			System.IO.StreamReader f = new System.IO.StreamReader (
				new System.IO.FileStream(args[1], System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, 8192)
				, System.Text.Encoding.GetEncoding("iso-8859-1"));
			Yylex yy = new Yylex(f);
			Symbol t;
			while ((t = yy.next_token()) != null)
				Console.WriteLine(t);
		}
		public void init(string filePathName)
		{
			f = new System.IO.StreamReader (
				new System.IO.FileStream(filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, 8192)
				, System.Text.Encoding.GetEncoding("iso-8859-1"));
			yy = new Yylex(f);
		}
		public Symbol nextToken()
		{
			return yy.next_token();
		}
		public void close()
		{
		    f.Close();
		}
		~Sample()
		{
		    close();
		}
	}
class Utility {
  public static void assert
   (
   bool expr
   )
    { 
    if (false == expr)
      throw new ApplicationException("Error: Assertion failed.");
    }
  private static string[] errorMsg = new string[]
    {
    "Error: Unmatched end-of-comment punctuation.",
    "Error: Unmatched start-of-comment punctuation.",
    "Error: Unclosed string.",
    "Error: Illegal character."
    };
  public const int E_ENDCOMMENT = 0; 
  public const int E_STARTCOMMENT = 1; 
  public const int E_UNCLOSEDSTR = 2; 
  public const int E_UNMATCHED = 3; 
  public static void error
    (
    int code
    )
    {
    Console.WriteLine(errorMsg[code]);
    }
  }
public class Yytoken : ICloneable  {
  internal Yytoken
    (
    int index,
    string text,
    int line,
    int charBegin,
    int charEnd
    )
    {
    m_index = index;
    m_text = text;
    m_line = line;
    m_charBegin = charBegin;
    m_charEnd = charEnd;
    }
  public int m_index;
  public string m_text;
  public int m_line;
  public int m_charBegin;
  public int m_charEnd;
  public override string ToString() {
    return "Token #"+ m_index + ": " + m_text  + " (line "+ m_line + ")";
    }
  public object Clone() {
    return MemberwiseClone();
    }
  }
}


class Yylex : TUVienna.CS_CUP.Runtime.Scanner {
	private const int YY_BUFFER_SIZE = 512;
	private const int YY_F = -1;
	private const int YY_NO_STATE = -1;
	private const int YY_NOT_ACCEPT = 0;
	private const int YY_START = 1;
	private const int YY_END = 2;
	private const int YY_NO_ANCHOR = 4;
	private const int YY_BOL = 65536;
	private const int YY_EOF = 65537;

		private int tokenbase;
		private int yytokenbase()
		{
			return tokenbase;
		}
		public void newReader (System.IO.TextReader yy_reader1)
		{
			if (null == yy_reader1) {
				throw (new System.Exception("Error: Bad input stream initializer."));
			}
			yy_reader = yy_reader1;
		}

	public void ReInit (System.IO.TextReader yy_reader1) {
		yy_buffer = new char[YY_BUFFER_SIZE];
		yy_buffer_read = 0;
		yy_buffer_index = 0;
		yy_buffer_start = 0;
		yy_buffer_end = 0;
		yy_at_bol = true;
		yy_lexical_state = YYINITIAL;

		if (null == yy_reader1) {
			throw (new System.Exception("Error: Bad input stream initializer."));
		}
		yy_reader = yy_reader1;
	}

	private System.IO.TextReader yy_reader;
	private int yy_buffer_index;
	private int yy_buffer_read;
	private int yy_buffer_start;
	private int yy_buffer_end;
	private char[] yy_buffer;
	private bool yy_at_bol;
	private int yy_lexical_state;

	public Yylex (System.IO.TextReader yy_reader1) : this() {
		if (null == yy_reader1) {
			throw (new System.Exception("Error: Bad input stream initializer."));
		}
		yy_reader = yy_reader1;
	}

	public Yylex () {
		yy_buffer = new char[YY_BUFFER_SIZE];
		yy_buffer_read = 0;
		yy_buffer_index = 0;
		yy_buffer_start = 0;
		yy_buffer_end = 0;
		yy_at_bol = true;
		yy_lexical_state = YYINITIAL;
	}

	private bool yy_eof_done = false;
	private const int commandfetchheaderlist = 6;
	private const int commandstatus = 9;
	private const int commandstoreflags = 8;
	private const int commandstatuslist = 10;
	private const int commandsequence = 3;
	private const int commandfetch = 5;
	private const int commandfetchsequence = 4;
	private const int commanddetail = 2;
	private const int commandstoresequence = 7;
	private const int commandbase = 1;
	private const int YYINITIAL = 0;
	private static readonly int[] yy_state_dtrans =new int[] {
		0,
		94,
		193,
		41,
		46,
		223,
		288,
		76,
		300,
		310,
		315
	};
	private void yybegin (int state) {
		yy_lexical_state = state;
	}
	private int yy_advance ()
	{
		int next_read;
		int i;
		int j;

		if (yy_buffer_index < yy_buffer_read) {
			return yy_buffer[yy_buffer_index++];
		}

		if (0 != yy_buffer_start) {
			i = yy_buffer_start;
			j = 0;
			while (i < yy_buffer_read) {
				yy_buffer[j] = yy_buffer[i];
				++i;
				++j;
			}
			yy_buffer_end = yy_buffer_end - yy_buffer_start;
			yy_buffer_start = 0;
			yy_buffer_read = j;
			yy_buffer_index = j;
			next_read = yy_reader.Read(yy_buffer,
					yy_buffer_read,
					yy_buffer.Length - yy_buffer_read);
			if ( next_read<=0) {
				return YY_EOF;
			}
			yy_buffer_read = yy_buffer_read + next_read;
		}

		while (yy_buffer_index >= yy_buffer_read) {
			if (yy_buffer_index >= yy_buffer.Length) {
				yy_buffer = yy_double(yy_buffer);
			}
			next_read = yy_reader.Read(yy_buffer,
					yy_buffer_read,
					yy_buffer.Length - yy_buffer_read);
			if ( next_read<=0) {
				return YY_EOF;
			}
			yy_buffer_read = yy_buffer_read + next_read;
		}
		return yy_buffer[yy_buffer_index++];
	}
	private void yy_move_end () {
		if (yy_buffer_end > yy_buffer_start &&
		    '\n' == yy_buffer[yy_buffer_end-1])
			yy_buffer_end--;
		if (yy_buffer_end > yy_buffer_start &&
		    '\r' == yy_buffer[yy_buffer_end-1])
			yy_buffer_end--;
	}
	private bool yy_last_was_cr=false;
	private void yy_mark_start () {
		yy_buffer_start = yy_buffer_index;
	}
	private void yy_mark_end () {
		yy_buffer_end = yy_buffer_index;
	}
	private void yy_to_mark () {
		yy_buffer_index = yy_buffer_end;
		yy_at_bol = (yy_buffer_end > yy_buffer_start) &&
		            ('\r' == yy_buffer[yy_buffer_end-1] ||
		             '\n' == yy_buffer[yy_buffer_end-1] ||
		             2028/*LS*/ == yy_buffer[yy_buffer_end-1] ||
		             2029/*PS*/ == yy_buffer[yy_buffer_end-1]);
	}
	private string yytext () {
		return (new string(yy_buffer,
			yy_buffer_start,
			yy_buffer_end - yy_buffer_start));
	}
	private int yylength () {
		return yy_buffer_end - yy_buffer_start;
	}
	private char[] yy_double (char[] buf) {
		int i;
		char[] newbuf;
		newbuf = new char[2*buf.Length];
		for (i = 0; i < buf.Length; ++i) {
			newbuf[i] = buf[i];
		}
		return newbuf;
	}
	private const int YY_E_INTERNAL = 0;
	private const int YY_E_MATCH = 1;
	private string[] yy_error_string = {
		"Error: Internal error.\n",
		"Error: Unmatched input.\n"
	};
	private void yy_error (int code,bool fatal) {
		 System.Console.Write(yy_error_string[code]);
		 System.Console.Out.Flush();
		if (fatal) {
			throw new System.Exception("Fatal Error.\n");
		}
	}
	private static int[][] unpackFromstring(int size1, int size2, string st) {
		int colonIndex = -1;
		string lengthstring;
		int sequenceLength = 0;
		int sequenceInteger = 0;

		int commaIndex;
		string workstring;

		int[][] res = new int[size1][];
		for(int i=0;i<size1;i++) res[i]=new int[size2];
		for (int i= 0; i < size1; i++) {
			for (int j= 0; j < size2; j++) {
				if (sequenceLength != 0) {
					res[i][j] = sequenceInteger;
					sequenceLength--;
					continue;
				}
				commaIndex = st.IndexOf(',');
				workstring = (commaIndex==-1) ? st :
					st.Substring(0, commaIndex);
				st = st.Substring(commaIndex+1);
				colonIndex = workstring.IndexOf(':');
				if (colonIndex == -1) {
					res[i][j]=System.Int32.Parse(workstring);
					continue;
				}
				lengthstring =
					workstring.Substring(colonIndex+1);
				sequenceLength=System.Int32.Parse(lengthstring);
				workstring=workstring.Substring(0,colonIndex);
				sequenceInteger=System.Int32.Parse(workstring);
				res[i][j] = sequenceInteger;
				sequenceLength--;
			}
		}
		return res;
	}
	private int[] yy_acpt = {
		/* 0 */ YY_NOT_ACCEPT,
		/* 1 */ YY_NO_ANCHOR,
		/* 2 */ YY_NO_ANCHOR,
		/* 3 */ YY_NO_ANCHOR,
		/* 4 */ YY_NO_ANCHOR,
		/* 5 */ YY_NO_ANCHOR,
		/* 6 */ YY_NO_ANCHOR,
		/* 7 */ YY_NO_ANCHOR,
		/* 8 */ YY_NO_ANCHOR,
		/* 9 */ YY_NO_ANCHOR,
		/* 10 */ YY_NO_ANCHOR,
		/* 11 */ YY_NO_ANCHOR,
		/* 12 */ YY_NO_ANCHOR,
		/* 13 */ YY_NO_ANCHOR,
		/* 14 */ YY_NO_ANCHOR,
		/* 15 */ YY_NO_ANCHOR,
		/* 16 */ YY_NO_ANCHOR,
		/* 17 */ YY_NO_ANCHOR,
		/* 18 */ YY_NO_ANCHOR,
		/* 19 */ YY_NO_ANCHOR,
		/* 20 */ YY_NO_ANCHOR,
		/* 21 */ YY_NO_ANCHOR,
		/* 22 */ YY_NO_ANCHOR,
		/* 23 */ YY_NO_ANCHOR,
		/* 24 */ YY_NO_ANCHOR,
		/* 25 */ YY_NO_ANCHOR,
		/* 26 */ YY_NO_ANCHOR,
		/* 27 */ YY_NO_ANCHOR,
		/* 28 */ YY_NO_ANCHOR,
		/* 29 */ YY_NO_ANCHOR,
		/* 30 */ YY_NO_ANCHOR,
		/* 31 */ YY_NO_ANCHOR,
		/* 32 */ YY_NO_ANCHOR,
		/* 33 */ YY_NO_ANCHOR,
		/* 34 */ YY_NO_ANCHOR,
		/* 35 */ YY_NO_ANCHOR,
		/* 36 */ YY_NO_ANCHOR,
		/* 37 */ YY_NO_ANCHOR,
		/* 38 */ YY_NO_ANCHOR,
		/* 39 */ YY_NO_ANCHOR,
		/* 40 */ YY_NO_ANCHOR,
		/* 41 */ YY_NO_ANCHOR,
		/* 42 */ YY_NO_ANCHOR,
		/* 43 */ YY_NO_ANCHOR,
		/* 44 */ YY_NO_ANCHOR,
		/* 45 */ YY_NO_ANCHOR,
		/* 46 */ YY_NO_ANCHOR,
		/* 47 */ YY_NO_ANCHOR,
		/* 48 */ YY_NO_ANCHOR,
		/* 49 */ YY_NO_ANCHOR,
		/* 50 */ YY_NO_ANCHOR,
		/* 51 */ YY_NO_ANCHOR,
		/* 52 */ YY_NO_ANCHOR,
		/* 53 */ YY_NO_ANCHOR,
		/* 54 */ YY_NO_ANCHOR,
		/* 55 */ YY_NO_ANCHOR,
		/* 56 */ YY_NO_ANCHOR,
		/* 57 */ YY_NO_ANCHOR,
		/* 58 */ YY_NO_ANCHOR,
		/* 59 */ YY_NO_ANCHOR,
		/* 60 */ YY_NO_ANCHOR,
		/* 61 */ YY_NO_ANCHOR,
		/* 62 */ YY_NO_ANCHOR,
		/* 63 */ YY_NO_ANCHOR,
		/* 64 */ YY_NO_ANCHOR,
		/* 65 */ YY_NO_ANCHOR,
		/* 66 */ YY_NO_ANCHOR,
		/* 67 */ YY_NO_ANCHOR,
		/* 68 */ YY_NO_ANCHOR,
		/* 69 */ YY_NO_ANCHOR,
		/* 70 */ YY_NO_ANCHOR,
		/* 71 */ YY_NO_ANCHOR,
		/* 72 */ YY_NO_ANCHOR,
		/* 73 */ YY_NO_ANCHOR,
		/* 74 */ YY_NO_ANCHOR,
		/* 75 */ YY_NO_ANCHOR,
		/* 76 */ YY_NO_ANCHOR,
		/* 77 */ YY_NO_ANCHOR,
		/* 78 */ YY_NO_ANCHOR,
		/* 79 */ YY_NO_ANCHOR,
		/* 80 */ YY_NO_ANCHOR,
		/* 81 */ YY_NO_ANCHOR,
		/* 82 */ YY_NO_ANCHOR,
		/* 83 */ YY_NO_ANCHOR,
		/* 84 */ YY_NO_ANCHOR,
		/* 85 */ YY_NO_ANCHOR,
		/* 86 */ YY_NO_ANCHOR,
		/* 87 */ YY_NO_ANCHOR,
		/* 88 */ YY_NO_ANCHOR,
		/* 89 */ YY_NO_ANCHOR,
		/* 90 */ YY_NO_ANCHOR,
		/* 91 */ YY_NO_ANCHOR,
		/* 92 */ YY_NO_ANCHOR,
		/* 93 */ YY_NO_ANCHOR,
		/* 94 */ YY_NOT_ACCEPT,
		/* 95 */ YY_NO_ANCHOR,
		/* 96 */ YY_NO_ANCHOR,
		/* 97 */ YY_NO_ANCHOR,
		/* 98 */ YY_NO_ANCHOR,
		/* 99 */ YY_NO_ANCHOR,
		/* 100 */ YY_NO_ANCHOR,
		/* 101 */ YY_NO_ANCHOR,
		/* 102 */ YY_NO_ANCHOR,
		/* 103 */ YY_NO_ANCHOR,
		/* 104 */ YY_NO_ANCHOR,
		/* 105 */ YY_NOT_ACCEPT,
		/* 106 */ YY_NOT_ACCEPT,
		/* 107 */ YY_NOT_ACCEPT,
		/* 108 */ YY_NOT_ACCEPT,
		/* 109 */ YY_NOT_ACCEPT,
		/* 110 */ YY_NOT_ACCEPT,
		/* 111 */ YY_NOT_ACCEPT,
		/* 112 */ YY_NOT_ACCEPT,
		/* 113 */ YY_NOT_ACCEPT,
		/* 114 */ YY_NOT_ACCEPT,
		/* 115 */ YY_NOT_ACCEPT,
		/* 116 */ YY_NOT_ACCEPT,
		/* 117 */ YY_NOT_ACCEPT,
		/* 118 */ YY_NOT_ACCEPT,
		/* 119 */ YY_NOT_ACCEPT,
		/* 120 */ YY_NOT_ACCEPT,
		/* 121 */ YY_NOT_ACCEPT,
		/* 122 */ YY_NOT_ACCEPT,
		/* 123 */ YY_NOT_ACCEPT,
		/* 124 */ YY_NOT_ACCEPT,
		/* 125 */ YY_NOT_ACCEPT,
		/* 126 */ YY_NOT_ACCEPT,
		/* 127 */ YY_NOT_ACCEPT,
		/* 128 */ YY_NOT_ACCEPT,
		/* 129 */ YY_NOT_ACCEPT,
		/* 130 */ YY_NOT_ACCEPT,
		/* 131 */ YY_NOT_ACCEPT,
		/* 132 */ YY_NOT_ACCEPT,
		/* 133 */ YY_NOT_ACCEPT,
		/* 134 */ YY_NOT_ACCEPT,
		/* 135 */ YY_NOT_ACCEPT,
		/* 136 */ YY_NOT_ACCEPT,
		/* 137 */ YY_NOT_ACCEPT,
		/* 138 */ YY_NOT_ACCEPT,
		/* 139 */ YY_NOT_ACCEPT,
		/* 140 */ YY_NOT_ACCEPT,
		/* 141 */ YY_NOT_ACCEPT,
		/* 142 */ YY_NOT_ACCEPT,
		/* 143 */ YY_NOT_ACCEPT,
		/* 144 */ YY_NOT_ACCEPT,
		/* 145 */ YY_NOT_ACCEPT,
		/* 146 */ YY_NOT_ACCEPT,
		/* 147 */ YY_NOT_ACCEPT,
		/* 148 */ YY_NOT_ACCEPT,
		/* 149 */ YY_NOT_ACCEPT,
		/* 150 */ YY_NOT_ACCEPT,
		/* 151 */ YY_NOT_ACCEPT,
		/* 152 */ YY_NOT_ACCEPT,
		/* 153 */ YY_NOT_ACCEPT,
		/* 154 */ YY_NOT_ACCEPT,
		/* 155 */ YY_NOT_ACCEPT,
		/* 156 */ YY_NOT_ACCEPT,
		/* 157 */ YY_NOT_ACCEPT,
		/* 158 */ YY_NOT_ACCEPT,
		/* 159 */ YY_NOT_ACCEPT,
		/* 160 */ YY_NOT_ACCEPT,
		/* 161 */ YY_NOT_ACCEPT,
		/* 162 */ YY_NOT_ACCEPT,
		/* 163 */ YY_NOT_ACCEPT,
		/* 164 */ YY_NOT_ACCEPT,
		/* 165 */ YY_NOT_ACCEPT,
		/* 166 */ YY_NOT_ACCEPT,
		/* 167 */ YY_NOT_ACCEPT,
		/* 168 */ YY_NOT_ACCEPT,
		/* 169 */ YY_NOT_ACCEPT,
		/* 170 */ YY_NOT_ACCEPT,
		/* 171 */ YY_NOT_ACCEPT,
		/* 172 */ YY_NOT_ACCEPT,
		/* 173 */ YY_NOT_ACCEPT,
		/* 174 */ YY_NOT_ACCEPT,
		/* 175 */ YY_NOT_ACCEPT,
		/* 176 */ YY_NOT_ACCEPT,
		/* 177 */ YY_NOT_ACCEPT,
		/* 178 */ YY_NOT_ACCEPT,
		/* 179 */ YY_NOT_ACCEPT,
		/* 180 */ YY_NOT_ACCEPT,
		/* 181 */ YY_NOT_ACCEPT,
		/* 182 */ YY_NOT_ACCEPT,
		/* 183 */ YY_NOT_ACCEPT,
		/* 184 */ YY_NOT_ACCEPT,
		/* 185 */ YY_NOT_ACCEPT,
		/* 186 */ YY_NOT_ACCEPT,
		/* 187 */ YY_NOT_ACCEPT,
		/* 188 */ YY_NOT_ACCEPT,
		/* 189 */ YY_NOT_ACCEPT,
		/* 190 */ YY_NOT_ACCEPT,
		/* 191 */ YY_NOT_ACCEPT,
		/* 192 */ YY_NOT_ACCEPT,
		/* 193 */ YY_NOT_ACCEPT,
		/* 194 */ YY_NOT_ACCEPT,
		/* 195 */ YY_NOT_ACCEPT,
		/* 196 */ YY_NOT_ACCEPT,
		/* 197 */ YY_NOT_ACCEPT,
		/* 198 */ YY_NOT_ACCEPT,
		/* 199 */ YY_NOT_ACCEPT,
		/* 200 */ YY_NOT_ACCEPT,
		/* 201 */ YY_NOT_ACCEPT,
		/* 202 */ YY_NOT_ACCEPT,
		/* 203 */ YY_NOT_ACCEPT,
		/* 204 */ YY_NOT_ACCEPT,
		/* 205 */ YY_NOT_ACCEPT,
		/* 206 */ YY_NOT_ACCEPT,
		/* 207 */ YY_NOT_ACCEPT,
		/* 208 */ YY_NOT_ACCEPT,
		/* 209 */ YY_NOT_ACCEPT,
		/* 210 */ YY_NOT_ACCEPT,
		/* 211 */ YY_NOT_ACCEPT,
		/* 212 */ YY_NOT_ACCEPT,
		/* 213 */ YY_NOT_ACCEPT,
		/* 214 */ YY_NOT_ACCEPT,
		/* 215 */ YY_NOT_ACCEPT,
		/* 216 */ YY_NOT_ACCEPT,
		/* 217 */ YY_NOT_ACCEPT,
		/* 218 */ YY_NOT_ACCEPT,
		/* 219 */ YY_NOT_ACCEPT,
		/* 220 */ YY_NOT_ACCEPT,
		/* 221 */ YY_NOT_ACCEPT,
		/* 222 */ YY_NOT_ACCEPT,
		/* 223 */ YY_NOT_ACCEPT,
		/* 224 */ YY_NOT_ACCEPT,
		/* 225 */ YY_NOT_ACCEPT,
		/* 226 */ YY_NOT_ACCEPT,
		/* 227 */ YY_NOT_ACCEPT,
		/* 228 */ YY_NOT_ACCEPT,
		/* 229 */ YY_NOT_ACCEPT,
		/* 230 */ YY_NOT_ACCEPT,
		/* 231 */ YY_NOT_ACCEPT,
		/* 232 */ YY_NOT_ACCEPT,
		/* 233 */ YY_NOT_ACCEPT,
		/* 234 */ YY_NOT_ACCEPT,
		/* 235 */ YY_NOT_ACCEPT,
		/* 236 */ YY_NOT_ACCEPT,
		/* 237 */ YY_NOT_ACCEPT,
		/* 238 */ YY_NOT_ACCEPT,
		/* 239 */ YY_NOT_ACCEPT,
		/* 240 */ YY_NOT_ACCEPT,
		/* 241 */ YY_NOT_ACCEPT,
		/* 242 */ YY_NOT_ACCEPT,
		/* 243 */ YY_NOT_ACCEPT,
		/* 244 */ YY_NOT_ACCEPT,
		/* 245 */ YY_NOT_ACCEPT,
		/* 246 */ YY_NOT_ACCEPT,
		/* 247 */ YY_NOT_ACCEPT,
		/* 248 */ YY_NOT_ACCEPT,
		/* 249 */ YY_NOT_ACCEPT,
		/* 250 */ YY_NOT_ACCEPT,
		/* 251 */ YY_NOT_ACCEPT,
		/* 252 */ YY_NOT_ACCEPT,
		/* 253 */ YY_NOT_ACCEPT,
		/* 254 */ YY_NOT_ACCEPT,
		/* 255 */ YY_NOT_ACCEPT,
		/* 256 */ YY_NOT_ACCEPT,
		/* 257 */ YY_NOT_ACCEPT,
		/* 258 */ YY_NOT_ACCEPT,
		/* 259 */ YY_NOT_ACCEPT,
		/* 260 */ YY_NOT_ACCEPT,
		/* 261 */ YY_NOT_ACCEPT,
		/* 262 */ YY_NOT_ACCEPT,
		/* 263 */ YY_NOT_ACCEPT,
		/* 264 */ YY_NOT_ACCEPT,
		/* 265 */ YY_NOT_ACCEPT,
		/* 266 */ YY_NOT_ACCEPT,
		/* 267 */ YY_NOT_ACCEPT,
		/* 268 */ YY_NOT_ACCEPT,
		/* 269 */ YY_NOT_ACCEPT,
		/* 270 */ YY_NOT_ACCEPT,
		/* 271 */ YY_NOT_ACCEPT,
		/* 272 */ YY_NOT_ACCEPT,
		/* 273 */ YY_NOT_ACCEPT,
		/* 274 */ YY_NOT_ACCEPT,
		/* 275 */ YY_NOT_ACCEPT,
		/* 276 */ YY_NOT_ACCEPT,
		/* 277 */ YY_NOT_ACCEPT,
		/* 278 */ YY_NOT_ACCEPT,
		/* 279 */ YY_NOT_ACCEPT,
		/* 280 */ YY_NOT_ACCEPT,
		/* 281 */ YY_NOT_ACCEPT,
		/* 282 */ YY_NOT_ACCEPT,
		/* 283 */ YY_NOT_ACCEPT,
		/* 284 */ YY_NOT_ACCEPT,
		/* 285 */ YY_NOT_ACCEPT,
		/* 286 */ YY_NOT_ACCEPT,
		/* 287 */ YY_NOT_ACCEPT,
		/* 288 */ YY_NOT_ACCEPT,
		/* 289 */ YY_NOT_ACCEPT,
		/* 290 */ YY_NOT_ACCEPT,
		/* 291 */ YY_NOT_ACCEPT,
		/* 292 */ YY_NOT_ACCEPT,
		/* 293 */ YY_NOT_ACCEPT,
		/* 294 */ YY_NOT_ACCEPT,
		/* 295 */ YY_NOT_ACCEPT,
		/* 296 */ YY_NOT_ACCEPT,
		/* 297 */ YY_NOT_ACCEPT,
		/* 298 */ YY_NOT_ACCEPT,
		/* 299 */ YY_NOT_ACCEPT,
		/* 300 */ YY_NOT_ACCEPT,
		/* 301 */ YY_NOT_ACCEPT,
		/* 302 */ YY_NOT_ACCEPT,
		/* 303 */ YY_NOT_ACCEPT,
		/* 304 */ YY_NOT_ACCEPT,
		/* 305 */ YY_NOT_ACCEPT,
		/* 306 */ YY_NOT_ACCEPT,
		/* 307 */ YY_NOT_ACCEPT,
		/* 308 */ YY_NOT_ACCEPT,
		/* 309 */ YY_NOT_ACCEPT,
		/* 310 */ YY_NOT_ACCEPT,
		/* 311 */ YY_NOT_ACCEPT,
		/* 312 */ YY_NOT_ACCEPT,
		/* 313 */ YY_NOT_ACCEPT,
		/* 314 */ YY_NOT_ACCEPT,
		/* 315 */ YY_NOT_ACCEPT,
		/* 316 */ YY_NOT_ACCEPT,
		/* 317 */ YY_NOT_ACCEPT,
		/* 318 */ YY_NOT_ACCEPT,
		/* 319 */ YY_NOT_ACCEPT,
		/* 320 */ YY_NOT_ACCEPT,
		/* 321 */ YY_NOT_ACCEPT,
		/* 322 */ YY_NOT_ACCEPT,
		/* 323 */ YY_NOT_ACCEPT,
		/* 324 */ YY_NOT_ACCEPT,
		/* 325 */ YY_NOT_ACCEPT,
		/* 326 */ YY_NOT_ACCEPT,
		/* 327 */ YY_NOT_ACCEPT,
		/* 328 */ YY_NOT_ACCEPT,
		/* 329 */ YY_NOT_ACCEPT,
		/* 330 */ YY_NOT_ACCEPT,
		/* 331 */ YY_NOT_ACCEPT,
		/* 332 */ YY_NOT_ACCEPT,
		/* 333 */ YY_NOT_ACCEPT,
		/* 334 */ YY_NOT_ACCEPT,
		/* 335 */ YY_NOT_ACCEPT,
		/* 336 */ YY_NOT_ACCEPT,
		/* 337 */ YY_NOT_ACCEPT,
		/* 338 */ YY_NOT_ACCEPT,
		/* 339 */ YY_NOT_ACCEPT,
		/* 340 */ YY_NOT_ACCEPT,
		/* 341 */ YY_NOT_ACCEPT,
		/* 342 */ YY_NOT_ACCEPT,
		/* 343 */ YY_NOT_ACCEPT,
		/* 344 */ YY_NOT_ACCEPT,
		/* 345 */ YY_NOT_ACCEPT,
		/* 346 */ YY_NOT_ACCEPT,
		/* 347 */ YY_NOT_ACCEPT,
		/* 348 */ YY_NOT_ACCEPT,
		/* 349 */ YY_NOT_ACCEPT,
		/* 350 */ YY_NOT_ACCEPT,
		/* 351 */ YY_NOT_ACCEPT,
		/* 352 */ YY_NOT_ACCEPT,
		/* 353 */ YY_NOT_ACCEPT,
		/* 354 */ YY_NOT_ACCEPT,
		/* 355 */ YY_NOT_ACCEPT,
		/* 356 */ YY_NOT_ACCEPT,
		/* 357 */ YY_NOT_ACCEPT,
		/* 358 */ YY_NOT_ACCEPT,
		/* 359 */ YY_NOT_ACCEPT,
		/* 360 */ YY_NOT_ACCEPT,
		/* 361 */ YY_NOT_ACCEPT,
		/* 362 */ YY_NOT_ACCEPT,
		/* 363 */ YY_NOT_ACCEPT,
		/* 364 */ YY_NOT_ACCEPT,
		/* 365 */ YY_NOT_ACCEPT,
		/* 366 */ YY_NOT_ACCEPT,
		/* 367 */ YY_NOT_ACCEPT,
		/* 368 */ YY_NOT_ACCEPT,
		/* 369 */ YY_NOT_ACCEPT,
		/* 370 */ YY_NOT_ACCEPT,
		/* 371 */ YY_NOT_ACCEPT,
		/* 372 */ YY_NOT_ACCEPT,
		/* 373 */ YY_NOT_ACCEPT,
		/* 374 */ YY_NOT_ACCEPT,
		/* 375 */ YY_NOT_ACCEPT,
		/* 376 */ YY_NOT_ACCEPT,
		/* 377 */ YY_NOT_ACCEPT,
		/* 378 */ YY_NOT_ACCEPT,
		/* 379 */ YY_NOT_ACCEPT,
		/* 380 */ YY_NOT_ACCEPT,
		/* 381 */ YY_NOT_ACCEPT,
		/* 382 */ YY_NOT_ACCEPT,
		/* 383 */ YY_NOT_ACCEPT,
		/* 384 */ YY_NOT_ACCEPT,
		/* 385 */ YY_NOT_ACCEPT,
		/* 386 */ YY_NOT_ACCEPT,
		/* 387 */ YY_NOT_ACCEPT,
		/* 388 */ YY_NOT_ACCEPT,
		/* 389 */ YY_NOT_ACCEPT,
		/* 390 */ YY_NOT_ACCEPT,
		/* 391 */ YY_NOT_ACCEPT,
		/* 392 */ YY_NOT_ACCEPT,
		/* 393 */ YY_NOT_ACCEPT,
		/* 394 */ YY_NOT_ACCEPT,
		/* 395 */ YY_NOT_ACCEPT,
		/* 396 */ YY_NOT_ACCEPT,
		/* 397 */ YY_NOT_ACCEPT,
		/* 398 */ YY_NOT_ACCEPT,
		/* 399 */ YY_NOT_ACCEPT,
		/* 400 */ YY_NOT_ACCEPT,
		/* 401 */ YY_NOT_ACCEPT,
		/* 402 */ YY_NOT_ACCEPT,
		/* 403 */ YY_NOT_ACCEPT,
		/* 404 */ YY_NOT_ACCEPT,
		/* 405 */ YY_NOT_ACCEPT,
		/* 406 */ YY_NOT_ACCEPT,
		/* 407 */ YY_NOT_ACCEPT,
		/* 408 */ YY_NOT_ACCEPT,
		/* 409 */ YY_NOT_ACCEPT,
		/* 410 */ YY_NOT_ACCEPT,
		/* 411 */ YY_NOT_ACCEPT,
		/* 412 */ YY_NOT_ACCEPT,
		/* 413 */ YY_NOT_ACCEPT,
		/* 414 */ YY_NOT_ACCEPT,
		/* 415 */ YY_NOT_ACCEPT,
		/* 416 */ YY_NOT_ACCEPT,
		/* 417 */ YY_NOT_ACCEPT,
		/* 418 */ YY_NOT_ACCEPT,
		/* 419 */ YY_NOT_ACCEPT,
		/* 420 */ YY_NOT_ACCEPT,
		/* 421 */ YY_NOT_ACCEPT,
		/* 422 */ YY_NOT_ACCEPT,
		/* 423 */ YY_NOT_ACCEPT,
		/* 424 */ YY_NOT_ACCEPT,
		/* 425 */ YY_NOT_ACCEPT,
		/* 426 */ YY_NOT_ACCEPT,
		/* 427 */ YY_NOT_ACCEPT,
		/* 428 */ YY_NOT_ACCEPT,
		/* 429 */ YY_NOT_ACCEPT,
		/* 430 */ YY_NOT_ACCEPT,
		/* 431 */ YY_NOT_ACCEPT,
		/* 432 */ YY_NOT_ACCEPT,
		/* 433 */ YY_NOT_ACCEPT,
		/* 434 */ YY_NOT_ACCEPT,
		/* 435 */ YY_NOT_ACCEPT,
		/* 436 */ YY_NOT_ACCEPT,
		/* 437 */ YY_NOT_ACCEPT,
		/* 438 */ YY_NOT_ACCEPT,
		/* 439 */ YY_NOT_ACCEPT,
		/* 440 */ YY_NOT_ACCEPT,
		/* 441 */ YY_NOT_ACCEPT,
		/* 442 */ YY_NOT_ACCEPT,
		/* 443 */ YY_NOT_ACCEPT,
		/* 444 */ YY_NOT_ACCEPT,
		/* 445 */ YY_NOT_ACCEPT,
		/* 446 */ YY_NOT_ACCEPT,
		/* 447 */ YY_NOT_ACCEPT,
		/* 448 */ YY_NOT_ACCEPT,
		/* 449 */ YY_NOT_ACCEPT,
		/* 450 */ YY_NOT_ACCEPT,
		/* 451 */ YY_NOT_ACCEPT,
		/* 452 */ YY_NOT_ACCEPT,
		/* 453 */ YY_NOT_ACCEPT,
		/* 454 */ YY_NOT_ACCEPT,
		/* 455 */ YY_NOT_ACCEPT,
		/* 456 */ YY_NOT_ACCEPT,
		/* 457 */ YY_NOT_ACCEPT,
		/* 458 */ YY_NOT_ACCEPT,
		/* 459 */ YY_NOT_ACCEPT,
		/* 460 */ YY_NOT_ACCEPT,
		/* 461 */ YY_NOT_ACCEPT
	};
	private int[] yy_cmap = unpackFromstring(1,65538,
"34,1:9,39,1:2,38,1:18,3,1,28,1:2,26,1:2,40,41,27,17,42,30,46,1,29,48,45,48:" +
"5,44,48,33,1,50,1,51,1:2,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12,6,1,20,22," +
"10,14,32,1,16,11,47,49,35,2,1,43,1,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12," +
"6,1,20,22,10,14,32,1,16,11,47,36,52,37,1:65410,0:2")[0];

	private int[] yy_rmap = unpackFromstring(1,462,
"0,1,2,1,3,1:25,4,5,1,6,1:7,7,1,8,1:2,9,1,10,1:4,11,1:2,12,1:7,13,1:3,14,15," +
"16,1:5,17,1,18,1:4,19,20,21,1:8,22,23,1:2,24,25,1,26,1:2,27,28,29,30,31,32," +
"33,3,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,5" +
"7,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,8" +
"2,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,10" +
"5,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,1" +
"24,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142," +
"143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161" +
",162,146,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,17" +
"9,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,1" +
"98,199,200,201,202,203,204,205,206,207,208,209,26,210,211,212,213,214,215,2" +
"16,217,218,219,220,221,222,223,224,225,226,227,228,229,230,27,231,232,233,2" +
"34,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,252," +
"253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271" +
",272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,29" +
"0,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306,307,308,3" +
"09,310,311,312,313,314,315,316,317,318,319,320,321,322,323,324,325,326,327," +
"328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,346" +
",347,348,349,350,351,352,353,354,355,356,357,358,359,360,361,362,363,364,36" +
"5,366,367,368,369,370,371,372,373,374,375,376,377,378,379,23")[0];

	private int[][] yy_nxt = unpackFromstring(380,53,
"1,2:2,3,2:13,3,2:8,3:3,2:5,3:3,2,-1:2,3:2,2:10,3,-1:54,2:2,-1,2:13,-1,2:8,-" +
"1:3,2:5,-1:3,2,-1:4,2:10,-1:2,4,-1:2,4:22,-1:3,4:5,-1:3,4,-1:4,4:10,-1:2,30" +
",31,-1,30:22,33:2,-1,30:5,-1:3,30,-1:4,30:10,-1:2,31:2,-1,31:22,33:2,-1,31:" +
"5,-1:3,31,-1:4,31:10,-1:2,33:2,-1,33:24,-1,33:5,-1:3,33,-1:4,33:10,-1,1,-1:" +
"18,200,-1:7,42,-1,43,-1:3,44,-1:8,45,-1,43:2,-1:2,43,-1:33,43,-1:14,43:2,-1" +
":2,43,-1:4,1,-1:18,213,-1:7,47,-1,48,-1:3,49,-1:8,50,-1,48:2,-1:2,48,-1:33," +
"48,-1:14,48:2,-1:2,48,-1:33,53,-1:14,53:2,-1:2,53,-1:33,56,-1:14,56:2,-1:2," +
"56,-1:26,255,-1:23,256,-1:52,418,-1:7,69,70,-1,69:22,-1:3,69:5,-1:3,69,-1:4" +
",69:10,-1:2,70:2,-1,70:22,-1:3,70:5,-1:3,70,-1:4,70:10,-1,1,-1:18,416,-1:7," +
"77,-1,78,-1:3,79,-1:8,80,-1,78:2,-1:2,78,-1:33,78,-1:14,78:2,-1:2,78,-1:50," +
"305,-1:7,84,85,-1,84:22,-1:3,84:5,-1:3,84,-1:4,84:10,-1:2,85:2,-1,85:22,-1:" +
"3,85:5,-1:3,85,-1:4,85:10,-1,1,-1:2,105,-1:50,461:27,37,461:6,197,461:2,-1:" +
"2,461:12,-1:47,267,-1:52,268,-1:7,289:27,74,289:6,291,289:2,-1:2,289:12,-1:" +
"2,311:27,87,311:6,313,311:2,-1:2,311:12,-1:5,106,107,-1:3,108,-1:4,109,110," +
"111,-1,112,113,337,-1,114,-1:2,373,-1:32,115,-1:3,336,-1:2,338,-1:7,388,-1:" +
"2,392,-1:35,374,-1:7,116,-1:46,117,-1:3,118,-1:9,119,-1:38,120,-1:6,340,-1:" +
"49,372,-1:56,121,-1:54,122,-1:44,124,-1:3,125,-1:3,126,-1:40,127,-1:56,131," +
"-1:64,132,-1:43,133,-1:53,341,-1:57,5,-1:38,136,346,-1:55,344,-1:58,376,-1:" +
"42,137,-1:6,138,-1:47,139,-1:50,347,-1:3,377,-1:48,140,-1:58,142,-1:45,143," +
"-1:66,144,-1:57,145,-1:39,146,-1:50,349,-1:3,378,-1:54,348,-1:44,6,-1:67,14" +
"8,-1:41,151,-1:9,352,-1:52,350,-1:54,152,-1:37,351,-1:63,7,-1:37,8,-1:73,15" +
"4,-1:43,155,-1:55,156,-1:37,9,-1:52,10,-1:57,355,-1:54,161,-1:63,357,-1:45," +
"356,-1:42,164,-1:71,165,-1:32,11,-1:68,168,-1:48,383,-1:40,12,-1:59,13,-1:6" +
"4,169,-1:43,170,-1:57,171,-1:44,359,-1:45,14,-1:69,174,-1:35,15,-1:58,360,-" +
"1:46,16,-1:52,17,-1:53,177,-1:66,18,-1:37,19,-1:52,20,-1:52,21,-1:57,180,-1" +
":47,22,-1:52,23,-1:69,183,-1:35,24,-1:71,25,-1:37,184,-1:55,185,-1:46,186,-" +
"1:56,187,-1:62,188,-1:45,26,-1:46,189,-1:54,190,-1:48,27,-1:59,361,-1:60,19" +
"1,-1:37,28,-1:52,29,-1:49,1,30,31,32,30:22,33:2,194,30:5,-1,34,195,30,196,-" +
"1,35,36,30:10,-1:2,461:2,460,461:24,37,460,461:5,197,461:2,-1:2,461:4,460:2" +
",461:2,460,461:3,-1:30,198,-1:14,198:2,-1:2,198,-1:43,38,-1:14,461:27,95,46" +
"1:6,197,461:2,-1:2,461:12,-1:30,198,-1:7,39,-1:6,198:2,-1:2,198,-1:5,461:27" +
",40,461:6,197,461:2,-1:2,461:12,-1:15,201,-1:59,363,-1:42,203,-1:84,204,-1:" +
"34,387,-1:47,206,-1:75,207,-1:24,391,-1:47,209,-1:65,210,-1:37,211,-1:59,21" +
"2,-1:50,96,-1:53,385,-1:49,215,-1:84,414,-1:29,217,-1:75,415,-1:19,219,-1:6" +
"5,220,-1:37,221,-1:59,222,-1:50,97,-1:39,1,-1,51,52,-1,224,-1,401,225,-1,22" +
"6,-1:3,227,-1:3,364,-1,228,229,-1,365,-1,230,-1:3,53,-1:8,231,-1,54,55,-1:2" +
",56:2,57,-1,56,58,59,60,-1:10,232,-1:58,234,-1:55,235,-1:42,236,-1:69,238,-" +
"1:35,393,-1:49,240,-1:3,366,-1:4,241,-1:77,61,-1:22,62,-1:62,242,-1:43,243," +
"-1:58,244,-1:55,63,-1:65,367,-1:24,245,-1:53,247,-1:69,248,-1:41,64,-1:59,2" +
"50,-1:44,65,-1:86,252,-1:26,66,-1:53,253,-1:43,62,-1:55,254,-1:59,257,-1:41" +
",403,-1:88,258,-1:25,259,-1:56,63,-1:40,260,-1:48,261,-1:61,262,-1:82,98,-1" +
":27,99,-1:52,264,-1:50,368,-1:39,265,-1:53,266,-1:60,269,-1:47,271,-1:61,63" +
",-1:44,386,-1:11,272,390,-1:54,273,-1:31,274,-1:72,67,-1:47,275,-1:41,277,-" +
"1:52,394,-1:54,278,-1:47,279,-1:63,280,-1:83,266,-1:19,283,-1:48,266,-1:52," +
"63,-1:61,397,-1:42,284,-1:63,266,-1:51,286,-1:53,63,-1:54,68,-1:40,100,-1:4" +
"2,1,69,70,71,69:22,-1:2,289,69:5,-1:2,290,69,-1:2,72,73,69:10,-1:30,292,-1:" +
"14,292:2,-1:2,292,-1:5,289:27,101,289:6,291,289:2,-1:2,289:12,-1:30,292,-1:" +
"7,75,-1:6,292:2,-1:2,292,-1:15,421,-1:61,422,-1:42,296,-1:65,297,-1:37,298," +
"-1:59,299,-1:50,102,-1:39,1,-1:2,81,-1:13,82,-1:7,301,-1:4,82,-1:31,302,-1:" +
"48,303,-1:60,304,-1:61,83,-1:52,306,-1:38,307,-1:53,400,-1:58,309,-1:47,103" +
",-1:42,1,84,85,86,84:22,-1:2,311,84:5,-1:2,312,84,-1:4,84:10,-1:30,314,-1:1" +
"4,314:2,-1:2,314,-1:5,311:27,104,311:6,313,311:2,-1:2,311:12,-1:30,314,-1:7" +
",88,-1:6,314:2,-1:2,314,-1:4,1,-1:2,89,-1:10,316,-1:5,402,404,-1:16,317,-1," +
"90,91,-1:19,318,-1:6,319,-1:76,92,-1:32,322,-1:55,406,-1:34,408,-1:70,370,-" +
"1:45,410,-1:16,323,-1:25,326,-1:62,328,-1:53,328,-1:45,330,-1:58,93,-1:47,9" +
"3,-1:55,412,-1:47,331,-1:63,333,-1:55,93,-1:38,334,-1:54,335,-1:53,93,-1:53" +
",375,-1:58,123,-1:40,128,-1:56,343,-1:64,134,-1:37,147,-1:50,345,-1:51,153," +
"-1:66,149,-1:44,380,-1:56,379,-1:58,353,-1:39,159,-1:60,157,-1:55,163,-1:42" +
",166,-1:54,162,-1:46,354,-1:71,175,-1:44,382,-1:59,173,-1:48,172,-1:44,176," +
"-1:51,179,-1:51,181,-1:62,192,-1:35,461:27,37,199,461:5,197,461:2,-1:2,461:" +
"4,199:2,461:2,199,461:3,-1:22,202,-1:46,237,-1:55,239,-1:39,249,-1:65,251,-" +
"1:52,270,-1:39,281,-1:69,371,-1:35,329,-1:59,135,-1:58,339,-1:40,130,-1:68," +
"141,-1:35,150,-1:65,381,-1:48,158,-1:53,160,-1:55,167,-1:38,358,-1:66,178,-" +
"1:44,384,-1:50,182,-1:65,389,-1:49,276,-1:46,205,-1:58,342,-1:55,214,-1:49," +
"369,-1:46,208,-1:58,129,-1:55,246,-1:49,282,-1:46,216,-1:61,399,-1:49,285,-" +
"1:46,218,-1:61,293,-1:49,308,-1:46,233,-1:58,320,-1:46,263,-1:58,321,-1:46," +
"287,-1:58,411,-1:46,294,-1:58,324,-1:46,295,-1:58,325,-1:52,327,-1:52,332,-" +
"1:35,461:27,37,362,461:5,197,461:2,-1:2,461:4,362:2,461:2,362,461:3,-1:26,3" +
"95,-1:42,398,-1:51,396,-1:63,407,-1:42,405,-1:52,409,-1:38,461:27,37,413,46" +
"1:5,197,461:2,-1:2,461:4,413:2,461:2,413,461:3,-1:44,417,-1:52,419,-1:10,46" +
"1:27,37,420,461:5,197,461:2,-1:2,461:4,420:2,461:2,420,461:3,-1:2,461:16,42" +
"3,461:10,37,461,423,461:4,197,461:2,-1:2,461:12,-1:2,461:2,424,461:24,37,46" +
"1:6,197,461:2,-1:2,461:12,-1:2,461:27,37,425,461:5,197,461:2,-1:2,461:4,425" +
":2,461:2,425,461:3,-1:2,461:27,37,426,461:5,197,461:2,-1:2,461:4,426:2,461:" +
"2,426,461:3,-1:2,461:27,37,461:4,427,461,197,461:2,-1:2,461:12,-1:2,461:27," +
"37,428,461:5,197,461:2,-1:2,461:4,428:2,461:2,428,461:3,-1:2,461:27,37,429," +
"461:5,197,461:2,-1:2,461:4,429:2,461:2,429,461:3,-1:2,461:27,37,461:4,430,4" +
"61,197,461:2,-1:2,461:12,-1:2,461:27,37,431,461:5,197,461:2,-1:2,461:4,431:" +
"2,461:2,431,461:3,-1:2,461:27,37,432,461:5,197,461:2,-1:2,461:4,432:2,461:2" +
",432,461:3,-1:2,461:2,433,461:24,37,461:6,197,461:2,-1:2,461:12,-1:2,461:27" +
",37,434,461:5,197,461:2,-1:2,461:4,434:2,461:2,434,461:3,-1:2,461:27,37,435" +
",461:5,197,461:2,-1:2,461:4,435:2,461:2,435,461:3,-1:2,461:27,37,436,461:5," +
"197,461:2,-1:2,461:4,436:2,461:2,436,461:3,-1:2,461:27,37,437,461:5,197,461" +
":2,-1:2,461:4,437:2,461:2,437,461:3,-1:2,461:27,37,461,438,461:4,197,461:2," +
"-1:2,461:12,-1:2,461:19,439,461:7,37,461:6,197,461:2,-1:2,461:12,-1:2,461:1" +
"2,439,461:14,37,461:6,197,461:2,-1:2,461:12,-1:2,461:9,439,461:17,37,461:6," +
"197,461:2,-1:2,461:12,-1:2,461:27,37,461:3,439,461:2,197,461:2,-1:2,461:12," +
"-1:2,461:3,439,461:23,37,461:6,197,461:2,-1:2,461:12,-1:2,461:10,439,461:8," +
"439,461:7,37,461:6,197,461:2,-1:2,461:12,-1:2,461:5,439,461:21,37,461:6,197" +
",461:2,-1:2,461:12,-1:2,461:6,439,461:20,37,461:6,197,461:2,-1:2,461:12,-1:" +
"2,461:14,439,461:12,37,461:6,197,461:2,-1:2,461:12,-1:2,461:8,439,461:5,439" +
",461:12,37,461:6,197,461:2,-1:2,461:12,-1:2,461:5,440,461:7,441,461:13,37,4" +
"61:6,197,461:2,-1:2,461:12,-1:2,461:3,442,461:23,37,461:6,197,461:2,-1:2,46" +
"1:12,-1:2,461:11,443,461:15,37,461:6,197,461:2,-1:2,461:12,-1:2,461:17,444," +
"461:9,37,461:6,197,461:2,-1:2,461:12,-1:2,461:4,445,461:22,37,461:6,197,461" +
":2,-1:2,461:12,-1:2,461:17,446,461:9,37,461:6,197,461:2,-1:2,461:12,-1:2,46" +
"1:17,447,461:9,37,461:6,197,461:2,-1:2,461:12,-1:2,461:4,448,461:8,449,461:" +
"13,37,461:6,197,461:2,-1:2,461:12,-1:2,461:4,450,461:6,451,461:2,452,461:3," +
"453,461,454,455,461:2,456,461:2,37,461:2,457,461:3,197,461:2,-1:2,461:12,-1" +
":2,461:27,37,461,458,461:4,197,461:2,-1:2,461:12,-1:2,461:27,37,459,461:5,1" +
"97,461:2,-1:2,461:4,459:2,461:2,459,461:3,-1");

	public TUVienna.CS_CUP.Runtime.Symbol next_token ()
 {
		int yy_lookahead;
		int yy_anchor = YY_NO_ANCHOR;
		int yy_state = yy_state_dtrans[yy_lexical_state];
		int yy_next_state = YY_NO_STATE;
		int yy_last_accept_state = YY_NO_STATE;
		bool yy_initial = true;
		int yy_this_accept;

		yy_mark_start();
		yy_this_accept = yy_acpt[yy_state];
		if (YY_NOT_ACCEPT != yy_this_accept) {
			yy_last_accept_state = yy_state;
			yy_mark_end();
		}
		while (true) {
			if (yy_initial && yy_at_bol) yy_lookahead = YY_BOL;
			else yy_lookahead = yy_advance();
			yy_next_state = YY_F;
			yy_next_state = yy_nxt[yy_rmap[yy_state]][yy_cmap[yy_lookahead]];
			if (YY_EOF == yy_lookahead && true == yy_initial) {

  return new Symbol(sym.EOF);
			}
			if (YY_F != yy_next_state) {
				yy_state = yy_next_state;
				yy_initial = false;
				yy_this_accept = yy_acpt[yy_state];
				if (YY_NOT_ACCEPT != yy_this_accept) {
					yy_last_accept_state = yy_state;
					yy_mark_end();
				}
			}
			else {
				if (YY_NO_STATE == yy_last_accept_state) {
					throw (new System.Exception("Lexical Error: Unmatched Input."));
				}
				else {
					yy_anchor = yy_acpt[yy_last_accept_state];
					if (0 != (YY_END & yy_anchor)) {
						yy_move_end();
					}
					yy_to_mark();
					switch (yy_last_accept_state) {
					case 1:
						break;
					case -2:
						break;
					case 2:
						{ yybegin(commandbase); return new Symbol(sym.TAG,yytext()); }
					case -3:
						break;
					case 3:
						{ return new Symbol(sym.OTHER, yytext()); }
					case -4:
						break;
					case 4:
						{ yybegin(commanddetail); return new Symbol(sym.X_COMMAND, yytext().Substring(1)); }
					case -5:
						break;
					case 5:
						{ return new Symbol(sym.UID, yytext().Substring(1)); }
					case -6:
						break;
					case 6:
						{ yybegin(commanddetail); return new Symbol(sym.NOOP, yytext().Substring(1)); }
					case -7:
						break;
					case 7:
						{ yybegin(commanddetail); return new Symbol(sym.CLOSE, yytext().Substring(1)); }
					case -8:
						break;
					case 8:
						{ yybegin(commandsequence); return new Symbol(sym.COPY, yytext().Substring(1, yytext().Length-2)); }
					case -9:
						break;
					case 9:
						{ yybegin(commanddetail); return new Symbol(sym.LIST, yytext().Substring(1, yytext().Length-2)); }
					case -10:
						break;
					case 10:
						{ yybegin(commanddetail); return new Symbol(sym.LSUB, yytext().Substring(1, yytext().Length-2)); }
					case -11:
						break;
					case 11:
						{ yybegin(commanddetail); return new Symbol(sym.CHECK, yytext().Substring(1, yytext().Length-2)); }
					case -12:
						break;
					case 12:
						{ yybegin(commanddetail); return new Symbol(sym.LOGIN, yytext().Substring(1, yytext().Length-2)); }
					case -13:
						break;
					case 13:
						{ yybegin(commanddetail); return new Symbol(sym.LOGOUT, yytext().Substring(1)); }
					case -14:
						break;
					case 14:
						{ yybegin(commandstoresequence); return new Symbol(sym.STORE, yytext().Substring(1, yytext().Length-2)); }
					case -15:
						break;
					case 15:
						{ yybegin(commandfetchsequence); return new Symbol(sym.FETCH, yytext().Substring(1, yytext().Length-2)); }
					case -16:
						break;
					case 16:
						{ yybegin(commanddetail); return new Symbol(sym.CREATE, yytext().Substring(1, yytext().Length-2)); }
					case -17:
						break;
					case 17:
						{ yybegin(commanddetail); return new Symbol(sym.APPEND, yytext().Substring(1, yytext().Length-2)); }
					case -18:
						break;
					case 18:
						{ yybegin(commanddetail); return new Symbol(sym.EXPUNGE, yytext().Substring(1)); }
					case -19:
						break;
					case 19:
						{ yybegin(commanddetail); return new Symbol(sym.DELETE, yytext().Substring(1, yytext().Length-2)); }
					case -20:
						break;
					case 20:
						{ yybegin(commanddetail); return new Symbol(sym.RENAME, yytext().Substring(1, yytext().Length-2)); }
					case -21:
						break;
					case 21:
						{ yybegin(commandstatus); return new Symbol(sym.STATUS, yytext().Substring(1)); }
					case -22:
						break;
					case 22:
						{ yybegin(commanddetail); return new Symbol(sym.SEARCH, yytext().Substring(1, yytext().Length-2)); }
					case -23:
						break;
					case 23:
						{ yybegin(commanddetail); return new Symbol(sym.SELECT, yytext().Substring(1, yytext().Length-2)); }
					case -24:
						break;
					case 24:
						{ yybegin(commanddetail); return new Symbol(sym.EXAMINE, yytext().Substring(1, yytext().Length-2)); }
					case -25:
						break;
					case 25:
						{ yybegin(commanddetail); return new Symbol(sym.STARTTLS, yytext().Substring(1)); }
					case -26:
						break;
					case 26:
						{ yybegin(commanddetail); return new Symbol(sym.CAPABILITY, yytext().Substring(1)); }
					case -27:
						break;
					case 27:
						{ yybegin(commanddetail); return new Symbol(sym.SUBSCRIBE, yytext().Substring(1, yytext().Length-2)); }
					case -28:
						break;
					case 28:
						{ yybegin(commanddetail); return new Symbol(sym.UNSUBSCRIBE, yytext().Substring(1, yytext().Length-2)); }
					case -29:
						break;
					case 29:
						{ yybegin(commanddetail); return new Symbol(sym.AUTHENTICATE, yytext().Substring(1, yytext().Length-2)); }
					case -30:
						break;
					case 30:
						{ return new Symbol(sym.ATOM, yytext()); }
					case -31:
						break;
					case 31:
						{ return new Symbol(sym.Astring, yytext()); }
					case -32:
						break;
					case 32:
						{ return new Symbol(sym.SP); }
					case -33:
						break;
					case 33:
						{ return new Symbol(sym.LIST_MAILBOX, yytext()); }
					case -34:
						break;
					case 34:
						{ return new Symbol(sym.BACKSLASH); }
					case -35:
						break;
					case 35:
						{ return new Symbol(sym.LPARENT); }
					case -36:
						break;
					case 36:
						{ return new Symbol(sym.RPARENT); }
					case -37:
						break;
					case 37:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -38:
						break;
					case 38:
						{ return new Symbol(sym.CRLF); }
					case -39:
						break;
					case 39:
						{ return new Symbol(sym.LITERAL, yytext()); }
					case -40:
						break;
					case 40:
						{ return new Symbol(sym.DATE_TIME, yytext()); }
					case -41:
						break;
					case 41:
						{ yybegin(commanddetail); break; }
					case -42:
						break;
					case 42:
						{ return new Symbol(sym.STAR); }
					case -43:
						break;
					case 43:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -44:
						break;
					case 44:
						{ return new Symbol(sym.COLON); }
					case -45:
						break;
					case 45:
						{ return new Symbol(sym.COMMA); }
					case -46:
						break;
					case 46:
						{ yybegin(commandfetch); break; }
					case -47:
						break;
					case 47:
						{ return new Symbol(sym.STAR); }
					case -48:
						break;
					case 48:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -49:
						break;
					case 49:
						{ return new Symbol(sym.COLON); }
					case -50:
						break;
					case 50:
						{ return new Symbol(sym.COMMA); }
					case -51:
						break;
					case 51:
						{ return new Symbol(sym.RBRACK); }
					case -52:
						break;
					case 52:
						{ return new Symbol(sym.SP); }
					case -53:
						break;
					case 53:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -54:
						break;
					case 54:
						{ return new Symbol(sym.LPARENT); }
					case -55:
						break;
					case 55:
						{ return new Symbol(sym.RPARENT); }
					case -56:
						break;
					case 56:
						{ return new Symbol(sym.NZ_NUMBER, yytext()); }
					case -57:
						break;
					case 57:
						{ return new Symbol(sym.DOT); }
					case -58:
						break;
					case 58:
						{ return new Symbol(sym.LBRACK); }
					case -59:
						break;
					case 59:
						{ return new Symbol(sym.LESSTHAN); }
					case -60:
						break;
					case 60:
						{ return new Symbol(sym.GREATERTHAN); }
					case -61:
						break;
					case 61:
						{ return new Symbol(sym.CRLF); }
					case -62:
						break;
					case 62:
						{ return new Symbol(sym.FETCH_ARG_PRIM, yytext()); }
					case -63:
						break;
					case 63:
						{ return new Symbol(sym.FETCH_ATT, yytext()); }
					case -64:
						break;
					case 64:
						{ return new Symbol(sym.FETCH_ATT_BODY, yytext()); }
					case -65:
						break;
					case 65:
						{ return new Symbol(sym.FETCH_MSG_TEXT_SINGLE, yytext()); }
					case -66:
						break;
					case 66:
						{ return new Symbol(sym.SECTION_TEXT_MIME); }
					case -67:
						break;
					case 67:
						{  return new Symbol(sym.FETCH_ATT_BODY_PEEK, yytext()); }
					case -68:
						break;
					case 68:
						{ yybegin(commandfetchheaderlist);return new Symbol(sym.FETCH_MSG_TEXT_HEADERLIST_KEY, yytext()); }
					case -69:
						break;
					case 69:
						{ return new Symbol(sym.ATOM, yytext()); }
					case -70:
						break;
					case 70:
						{ return new Symbol(sym.Astring, yytext()); }
					case -71:
						break;
					case 71:
						{ return new Symbol(sym.SP); }
					case -72:
						break;
					case 72:
						{ return new Symbol(sym.LPARENT); }
					case -73:
						break;
					case 73:
						{ yybegin(commandfetch); return new Symbol(sym.RPARENT); }
					case -74:
						break;
					case 74:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -75:
						break;
					case 75:
						{ return new Symbol(sym.LITERAL, yytext()); }
					case -76:
						break;
					case 76:
						{ yybegin(commandstoreflags); break; }
					case -77:
						break;
					case 77:
						{ return new Symbol(sym.STAR); }
					case -78:
						break;
					case 78:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -79:
						break;
					case 79:
						{ return new Symbol(sym.COLON); }
					case -80:
						break;
					case 80:
						{ return new Symbol(sym.COMMA); }
					case -81:
						break;
					case 81:
						{ return new Symbol(sym.SP); }
					case -82:
						break;
					case 82:
						{ return new Symbol(sym.FLAG_SIGN, yytext()); }
					case -83:
						break;
					case 83:
						{ yybegin(commanddetail); return new Symbol(sym.FLAG_KEY, yytext()); }
					case -84:
						break;
					case 84:
						{ return new Symbol(sym.ATOM, yytext()); }
					case -85:
						break;
					case 85:
						{ return new Symbol(sym.Astring, yytext()); }
					case -86:
						break;
					case 86:
						{ yybegin(commandstatuslist); return new Symbol(sym.SP); }
					case -87:
						break;
					case 87:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -88:
						break;
					case 88:
						{ return new Symbol(sym.LITERAL, yytext()); }
					case -89:
						break;
					case 89:
						{ return new Symbol(sym.SP); }
					case -90:
						break;
					case 90:
						{ return new Symbol(sym.LPARENT); }
					case -91:
						break;
					case 91:
						{ return new Symbol(sym.RPARENT); }
					case -92:
						break;
					case 92:
						{ return new Symbol(sym.CRLF); }
					case -93:
						break;
					case 93:
						{ return new Symbol(sym.STATUS_ATT, yytext()); }
					case -94:
						break;
					case 95:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -95:
						break;
					case 96:
						{ yybegin(commanddetail); break; }
					case -96:
						break;
					case 97:
						{ yybegin(commandfetch); break; }
					case -97:
						break;
					case 98:
						{ return new Symbol(sym.FETCH_ATT, yytext()); }
					case -98:
						break;
					case 99:
						{ return new Symbol(sym.FETCH_MSG_TEXT_SINGLE, yytext()); }
					case -99:
						break;
					case 100:
						{ yybegin(commandfetchheaderlist);return new Symbol(sym.FETCH_MSG_TEXT_HEADERLIST_KEY, yytext()); }
					case -100:
						break;
					case 101:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -101:
						break;
					case 102:
						{ yybegin(commandstoreflags); break; }
					case -102:
						break;
					case 103:
						{ yybegin(commanddetail); return new Symbol(sym.FLAG_KEY, yytext()); }
					case -103:
						break;
					case 104:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -104:
						break;
					default:
						yy_error(YY_E_INTERNAL,false);break;
					}
					yy_initial = true;
					yy_state = yy_state_dtrans[yy_lexical_state];
					yy_next_state = YY_NO_STATE;
					yy_last_accept_state = YY_NO_STATE;
					yy_mark_start();
					yy_this_accept = yy_acpt[yy_state];
					if (YY_NOT_ACCEPT != yy_this_accept) {
						yy_last_accept_state = yy_state;
						yy_mark_end();
					}
				}
			}
		}
	}
}
