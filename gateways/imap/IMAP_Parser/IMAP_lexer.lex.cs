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
// This file translates into IMAP_lexer.lex.cs
// To compile this file you need sym.cs, which is generated out of parser.cup
// see http://www.iseclab.org/projects/cuplex/)
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
		public static void do_printout(String[] argv)
		{
			String [] args = Environment.GetCommandLineArgs();
			System.IO.StreamReader f = new System.IO.StreamReader (
				new System.IO.FileStream(args[1], System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, 8192)
				, System.Text.Encoding.GetEncoding("iso-8859-1"));
			Yylex yy = new Yylex(f);
			Symbol t;
			while ((t = yy.next_token()) != null)
				System.Diagnostics.Trace.WriteLine(t);
		}
		public void init(String filePathName)
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
  private static String[] errorMsg = new String[]
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
    System.Diagnostics.Trace.WriteLine(errorMsg[code]);
    }
  }
public class Yytoken : ICloneable  {
  internal Yytoken
    (
    int index,
    String text,
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
  public String m_text;
  public int m_line;
  public int m_charBegin;
  public int m_charEnd;
  public override String ToString() {
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
	// save the unpacking effort done at object creation time
	public void ReInit (System.IO.TextReader yy_reader1)
	 {
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

	private Yylex () {
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
	private const int commandsearch = 11;
	private const int commandstatuslist = 10;
	private const int commandsequence = 3;
	private const int commandfetch = 5;
	private const int commandfetchsequence = 4;
	private const int commanddetail = 2;
	private const int commandstoresequence = 7;
	private const int commandbase = 1;
	private const int YYINITIAL = 0;
	private const int commandsearchsequence = 12;
	private static readonly int[] yy_state_dtrans =new int[] {
		0,
		111,
		211,
		41,
		46,
		241,
		306,
		76,
		318,
		328,
		333,
		354,
		106
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
	private static int[][] unpackFromString(int size1, int size2, string st) {
		int colonIndex = -1;
		string lengthString;
		int sequenceLength = 0;
		int sequenceInteger = 0;

		int commaIndex;
		string workString;

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
				workString = (commaIndex==-1) ? st :
					st.Substring(0, commaIndex);
				st = st.Substring(commaIndex+1);
				colonIndex = workString.IndexOf(':');
				if (colonIndex == -1) {
					res[i][j]=System.Int32.Parse(workString);
					continue;
				}
				lengthString =
					workString.Substring(colonIndex+1);
				sequenceLength=System.Int32.Parse(lengthString);
				workString=workString.Substring(0,colonIndex);
				sequenceInteger=System.Int32.Parse(workString);
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
		/* 94 */ YY_NO_ANCHOR,
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
		/* 105 */ YY_NO_ANCHOR,
		/* 106 */ YY_NO_ANCHOR,
		/* 107 */ YY_NO_ANCHOR,
		/* 108 */ YY_NO_ANCHOR,
		/* 109 */ YY_NO_ANCHOR,
		/* 110 */ YY_NO_ANCHOR,
		/* 111 */ YY_NOT_ACCEPT,
		/* 112 */ YY_NO_ANCHOR,
		/* 113 */ YY_NO_ANCHOR,
		/* 114 */ YY_NO_ANCHOR,
		/* 115 */ YY_NO_ANCHOR,
		/* 116 */ YY_NO_ANCHOR,
		/* 117 */ YY_NO_ANCHOR,
		/* 118 */ YY_NO_ANCHOR,
		/* 119 */ YY_NO_ANCHOR,
		/* 120 */ YY_NO_ANCHOR,
		/* 121 */ YY_NO_ANCHOR,
		/* 122 */ YY_NO_ANCHOR,
		/* 123 */ YY_NO_ANCHOR,
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
		/* 461 */ YY_NOT_ACCEPT,
		/* 462 */ YY_NOT_ACCEPT,
		/* 463 */ YY_NOT_ACCEPT,
		/* 464 */ YY_NOT_ACCEPT,
		/* 465 */ YY_NOT_ACCEPT,
		/* 466 */ YY_NOT_ACCEPT,
		/* 467 */ YY_NOT_ACCEPT,
		/* 468 */ YY_NOT_ACCEPT,
		/* 469 */ YY_NOT_ACCEPT,
		/* 470 */ YY_NOT_ACCEPT,
		/* 471 */ YY_NOT_ACCEPT,
		/* 472 */ YY_NOT_ACCEPT,
		/* 473 */ YY_NOT_ACCEPT,
		/* 474 */ YY_NOT_ACCEPT,
		/* 475 */ YY_NOT_ACCEPT,
		/* 476 */ YY_NOT_ACCEPT,
		/* 477 */ YY_NOT_ACCEPT,
		/* 478 */ YY_NOT_ACCEPT,
		/* 479 */ YY_NOT_ACCEPT,
		/* 480 */ YY_NOT_ACCEPT,
		/* 481 */ YY_NOT_ACCEPT,
		/* 482 */ YY_NOT_ACCEPT,
		/* 483 */ YY_NOT_ACCEPT,
		/* 484 */ YY_NOT_ACCEPT,
		/* 485 */ YY_NOT_ACCEPT,
		/* 486 */ YY_NOT_ACCEPT,
		/* 487 */ YY_NOT_ACCEPT,
		/* 488 */ YY_NOT_ACCEPT,
		/* 489 */ YY_NOT_ACCEPT,
		/* 490 */ YY_NOT_ACCEPT,
		/* 491 */ YY_NOT_ACCEPT,
		/* 492 */ YY_NOT_ACCEPT,
		/* 493 */ YY_NOT_ACCEPT,
		/* 494 */ YY_NOT_ACCEPT,
		/* 495 */ YY_NOT_ACCEPT,
		/* 496 */ YY_NOT_ACCEPT,
		/* 497 */ YY_NOT_ACCEPT,
		/* 498 */ YY_NOT_ACCEPT,
		/* 499 */ YY_NOT_ACCEPT,
		/* 500 */ YY_NOT_ACCEPT,
		/* 501 */ YY_NOT_ACCEPT,
		/* 502 */ YY_NOT_ACCEPT,
		/* 503 */ YY_NOT_ACCEPT,
		/* 504 */ YY_NOT_ACCEPT,
		/* 505 */ YY_NOT_ACCEPT,
		/* 506 */ YY_NOT_ACCEPT,
		/* 507 */ YY_NOT_ACCEPT,
		/* 508 */ YY_NOT_ACCEPT,
		/* 509 */ YY_NOT_ACCEPT,
		/* 510 */ YY_NOT_ACCEPT,
		/* 511 */ YY_NOT_ACCEPT,
		/* 512 */ YY_NOT_ACCEPT,
		/* 513 */ YY_NOT_ACCEPT,
		/* 514 */ YY_NOT_ACCEPT,
		/* 515 */ YY_NOT_ACCEPT,
		/* 516 */ YY_NOT_ACCEPT,
		/* 517 */ YY_NOT_ACCEPT,
		/* 518 */ YY_NOT_ACCEPT,
		/* 519 */ YY_NOT_ACCEPT,
		/* 520 */ YY_NOT_ACCEPT,
		/* 521 */ YY_NOT_ACCEPT,
		/* 522 */ YY_NOT_ACCEPT,
		/* 523 */ YY_NOT_ACCEPT,
		/* 524 */ YY_NOT_ACCEPT,
		/* 525 */ YY_NOT_ACCEPT,
		/* 526 */ YY_NOT_ACCEPT,
		/* 527 */ YY_NOT_ACCEPT,
		/* 528 */ YY_NOT_ACCEPT,
		/* 529 */ YY_NOT_ACCEPT,
		/* 530 */ YY_NOT_ACCEPT,
		/* 531 */ YY_NOT_ACCEPT,
		/* 532 */ YY_NOT_ACCEPT,
		/* 533 */ YY_NOT_ACCEPT,
		/* 534 */ YY_NOT_ACCEPT,
		/* 535 */ YY_NOT_ACCEPT,
		/* 536 */ YY_NOT_ACCEPT,
		/* 537 */ YY_NOT_ACCEPT,
		/* 538 */ YY_NOT_ACCEPT,
		/* 539 */ YY_NOT_ACCEPT,
		/* 540 */ YY_NOT_ACCEPT,
		/* 541 */ YY_NOT_ACCEPT,
		/* 542 */ YY_NOT_ACCEPT,
		/* 543 */ YY_NOT_ACCEPT,
		/* 544 */ YY_NOT_ACCEPT,
		/* 545 */ YY_NOT_ACCEPT,
		/* 546 */ YY_NOT_ACCEPT,
		/* 547 */ YY_NOT_ACCEPT,
		/* 548 */ YY_NOT_ACCEPT,
		/* 549 */ YY_NOT_ACCEPT,
		/* 550 */ YY_NOT_ACCEPT,
		/* 551 */ YY_NOT_ACCEPT,
		/* 552 */ YY_NOT_ACCEPT,
		/* 553 */ YY_NOT_ACCEPT,
		/* 554 */ YY_NOT_ACCEPT,
		/* 555 */ YY_NOT_ACCEPT,
		/* 556 */ YY_NOT_ACCEPT,
		/* 557 */ YY_NOT_ACCEPT,
		/* 558 */ YY_NOT_ACCEPT,
		/* 559 */ YY_NOT_ACCEPT,
		/* 560 */ YY_NOT_ACCEPT,
		/* 561 */ YY_NOT_ACCEPT,
		/* 562 */ YY_NOT_ACCEPT,
		/* 563 */ YY_NOT_ACCEPT,
		/* 564 */ YY_NOT_ACCEPT,
		/* 565 */ YY_NOT_ACCEPT,
		/* 566 */ YY_NOT_ACCEPT,
		/* 567 */ YY_NOT_ACCEPT,
		/* 568 */ YY_NOT_ACCEPT,
		/* 569 */ YY_NOT_ACCEPT,
		/* 570 */ YY_NOT_ACCEPT,
		/* 571 */ YY_NOT_ACCEPT,
		/* 572 */ YY_NOT_ACCEPT,
		/* 573 */ YY_NOT_ACCEPT,
		/* 574 */ YY_NOT_ACCEPT,
		/* 575 */ YY_NOT_ACCEPT,
		/* 576 */ YY_NOT_ACCEPT,
		/* 577 */ YY_NOT_ACCEPT,
		/* 578 */ YY_NOT_ACCEPT,
		/* 579 */ YY_NOT_ACCEPT,
		/* 580 */ YY_NOT_ACCEPT,
		/* 581 */ YY_NOT_ACCEPT,
		/* 582 */ YY_NOT_ACCEPT,
		/* 583 */ YY_NOT_ACCEPT,
		/* 584 */ YY_NOT_ACCEPT,
		/* 585 */ YY_NOT_ACCEPT,
		/* 586 */ YY_NOT_ACCEPT,
		/* 587 */ YY_NOT_ACCEPT,
		/* 588 */ YY_NOT_ACCEPT,
		/* 589 */ YY_NOT_ACCEPT,
		/* 590 */ YY_NOT_ACCEPT,
		/* 591 */ YY_NOT_ACCEPT,
		/* 592 */ YY_NOT_ACCEPT,
		/* 593 */ YY_NOT_ACCEPT,
		/* 594 */ YY_NOT_ACCEPT,
		/* 595 */ YY_NOT_ACCEPT,
		/* 596 */ YY_NOT_ACCEPT,
		/* 597 */ YY_NOT_ACCEPT,
		/* 598 */ YY_NOT_ACCEPT,
		/* 599 */ YY_NOT_ACCEPT,
		/* 600 */ YY_NOT_ACCEPT,
		/* 601 */ YY_NOT_ACCEPT,
		/* 602 */ YY_NOT_ACCEPT,
		/* 603 */ YY_NOT_ACCEPT,
		/* 604 */ YY_NOT_ACCEPT,
		/* 605 */ YY_NOT_ACCEPT,
		/* 606 */ YY_NOT_ACCEPT,
		/* 607 */ YY_NOT_ACCEPT,
		/* 608 */ YY_NOT_ACCEPT,
		/* 609 */ YY_NOT_ACCEPT,
		/* 610 */ YY_NOT_ACCEPT,
		/* 611 */ YY_NOT_ACCEPT,
		/* 612 */ YY_NOT_ACCEPT,
		/* 613 */ YY_NOT_ACCEPT,
		/* 614 */ YY_NOT_ACCEPT,
		/* 615 */ YY_NOT_ACCEPT,
		/* 616 */ YY_NOT_ACCEPT,
		/* 617 */ YY_NOT_ACCEPT
	};
	private int[] yy_cmap = unpackFromString(1,65538,
"34,1:9,39,1:2,38,1:18,3,1,28,1:2,26,1:2,40,41,27,17,42,30,46,1,29,48,45,48:" +
"5,44,48,33,1,50,1,51,1:2,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12,6,1,20,22," +
"10,14,32,52,16,11,47,49,35,2,1,43,1,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12" +
",6,1,20,22,10,14,32,52,16,11,47,36,53,37,1:65410,0:2")[0];

	private int[] yy_rmap = unpackFromString(1,618,
"0,1,2,1,3,1:25,4,5,1,6,1:7,7,1,8,1:2,9,1,10,1:4,11,1:2,12,1:7,13,1:3,14,15," +
"16,1:5,17,1,18,1:4,19,20,21,1:20,22,1,23,1:2,24,25,1:2,26,27,1,28,1:2,29,30" +
",1,31,32,33,34,35,36,3,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,5" +
"4,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,7" +
"9,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,1" +
"03,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121," +
"122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140" +
",141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,15" +
"9,160,161,162,163,164,148,165,166,167,168,169,170,171,172,173,174,175,176,1" +
"77,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195," +
"196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,28,212,213," +
"214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232" +
",29,233,234,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,250" +
",251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,26" +
"9,270,271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,2" +
"88,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306," +
"307,308,309,310,311,312,293,313,314,315,316,317,318,319,320,317,294,321,302" +
",322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,34" +
"0,341,342,343,344,345,346,347,348,349,350,351,352,353,354,355,356,357,358,3" +
"59,360,361,362,363,364,365,366,367,368,369,370,371,372,373,374,375,376,377," +
"378,379,380,381,382,305,383,384,385,386,387,388,389,390,391,392,393,394,395" +
",396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,412,413,41" +
"4,415,416,417,418,419,420,421,422,423,424,425,426,427,428,429,430,431,432,4" +
"33,434,435,436,437,438,439,440,441,442,443,444,445,446,447,448,449,450,451," +
"452,453,454,455,456,457,458,459,460,461,462,463,464,465,466,467,468,469,470" +
",471,472,473,474,475,476,477,478,30,479,480,481,482,483,484,485,486,487,488" +
",489,490,491,492,493,494,495,496,497,498,499,500,501,502,503,504,505,506,50" +
"7,508,509,510,511,512,513,25")[0];

	private int[][] yy_nxt = unpackFromString(514,54,
"1,2:2,3,2:13,3,2:8,3:3,2:5,3:3,2,-1:2,3:2,2:11,3,-1:55,2:2,-1,2:13,-1,2:8,-" +
"1:3,2:5,-1:3,2,-1:4,2:11,-1:2,4,-1:2,4:22,-1:3,4:5,-1:3,4,-1:4,4:11,-1:2,30" +
",31,-1,30:22,33:2,-1,30:5,-1:3,30,-1:4,30:11,-1:2,31:2,-1,31:22,33:2,-1,31:" +
"5,-1:3,31,-1:4,31:11,-1:2,33:2,-1,33:24,-1,33:5,-1:3,33,-1:4,33:11,-1,1,-1:" +
"18,218,-1:7,42,-1,43,-1:3,44,-1:8,45,-1,43:2,-1:2,43,-1:34,43,-1:14,43:2,-1" +
":2,43,-1:5,1,-1:18,231,-1:7,47,-1,48,-1:3,49,-1:8,50,-1,48:2,-1:2,48,-1:34," +
"48,-1:14,48:2,-1:2,48,-1:34,53,-1:14,53:2,-1:2,53,-1:34,56,-1:14,56:2,-1:2," +
"56,-1:27,273,-1:23,274,-1:53,550,-1:8,69,70,-1,69:22,-1:3,69:5,-1:3,69,-1:4" +
",69:11,-1:2,70:2,-1,70:22,-1:3,70:5,-1:3,70,-1:4,70:11,-1,1,-1:18,541,-1:7," +
"77,-1,78,-1:3,79,-1:8,80,-1,78:2,-1:2,78,-1:34,78,-1:14,78:2,-1:2,78,-1:51," +
"323,-1:8,84,85,-1,84:22,-1:3,84:5,-1:3,84,-1:4,84:11,-1:2,85:2,-1,85:22,-1:" +
"3,85:5,-1:3,85,-1:4,85:11,-1,1,-1:18,551,-1:7,107,-1,108,-1:3,109,-1:8,110," +
"-1,108:2,-1:2,108,-1:34,108,-1:14,108:2,-1:2,108,-1:5,1,-1:2,124,-1:51,617:" +
"27,37,617:6,215,617:2,-1:2,617:13,-1:47,285,-1:53,286,-1:8,307:27,74,307:6," +
"309,307:2,-1:2,307:13,-1:2,329:27,87,329:6,331,329:2,-1:2,329:13,-1:2,581:2" +
"7,99,581:6,381,581:2,-1:2,581:13,-1:5,125,126,-1:3,127,-1:4,128,129,130,-1," +
"131,132,429,-1,133,-1:2,468,-1:33,134,-1:3,428,-1:2,430,-1:7,486,-1:2,491,-" +
"1:36,469,-1:7,135,-1:47,136,-1:3,137,-1:9,138,-1:39,139,-1:6,432,-1:50,467," +
"-1:57,140,-1:55,141,-1:45,143,-1:3,144,-1:3,145,-1:41,146,-1:57,150,-1:65,1" +
"51,-1:44,152,-1:54,433,-1:58,5,-1:39,155,438,-1:56,436,-1:59,471,-1:43,156," +
"-1:6,157,-1:48,158,-1:51,439,-1:3,472,-1:49,159,-1:59,161,-1:46,162,-1:67,1" +
"63,-1:58,164,-1:40,165,-1:51,441,-1:3,473,-1:55,440,-1:45,6,-1:68,167,-1:42" +
",170,-1:9,444,-1:53,442,-1:55,171,-1:38,443,-1:64,7,-1:38,8,-1:74,9,-1:44,1" +
"73,-1:56,174,-1:38,10,-1:53,11,-1:58,447,-1:55,179,-1:64,449,-1:46,448,-1:4" +
"3,182,-1:72,183,-1:49,186,-1:49,478,-1:41,12,-1:60,13,-1:65,187,-1:44,188,-" +
"1:58,189,-1:45,451,-1:46,14,-1:70,192,-1:36,15,-1:59,452,-1:47,16,-1:53,17," +
"-1:54,195,-1:67,18,-1:38,19,-1:53,20,-1:53,21,-1:58,198,-1:48,22,-1:53,23,-" +
"1:70,201,-1:36,24,-1:72,25,-1:38,202,-1:56,203,-1:47,204,-1:57,205,-1:63,20" +
"6,-1:46,26,-1:47,207,-1:55,208,-1:49,27,-1:60,453,-1:61,209,-1:38,28,-1:53," +
"29,-1:50,1,30,31,32,30:22,33:2,212,30:5,-1,34,213,30,214,-1,35,36,30:11,-1:" +
"2,617:2,616,617:24,37,616,617:5,215,617:2,-1:2,617:4,616:2,617:2,616,617:4," +
"-1:30,216,-1:14,216:2,-1:2,216,-1:44,38,-1:15,617:27,112,617:6,215,617:2,-1" +
":2,617:13,-1:30,216,-1:7,39,-1:6,216:2,-1:2,216,-1:6,617:27,40,617:6,215,61" +
"7:2,-1:2,617:13,-1:15,219,-1:60,455,-1:43,221,-1:85,222,-1:35,485,-1:48,224" +
",-1:76,225,-1:25,490,-1:48,227,-1:66,228,-1:38,229,-1:60,230,-1:51,113,-1:5" +
"4,480,-1:50,233,-1:85,539,-1:30,235,-1:76,540,-1:20,237,-1:66,238,-1:38,239" +
",-1:60,240,-1:51,114,-1:40,1,-1,51,52,-1,242,-1,503,243,-1,244,-1:3,245,-1:" +
"3,456,-1,246,247,-1,457,-1,248,-1:3,53,-1:8,249,-1,54,55,-1:2,56:2,57,-1,56" +
",58,59,60,-1:11,250,-1:59,252,-1:56,253,-1:43,254,-1:70,256,-1:36,492,-1:50" +
",258,-1:3,458,-1:4,259,-1:78,61,-1:23,62,-1:63,260,-1:44,261,-1:59,262,-1:5" +
"6,63,-1:66,459,-1:25,263,-1:54,265,-1:70,266,-1:42,64,-1:60,268,-1:45,65,-1" +
":87,270,-1:27,66,-1:54,271,-1:44,62,-1:56,272,-1:60,275,-1:42,507,-1:89,276" +
",-1:26,277,-1:57,63,-1:41,278,-1:49,279,-1:62,280,-1:83,115,-1:28,116,-1:53" +
",282,-1:51,460,-1:40,283,-1:54,284,-1:61,287,-1:48,289,-1:62,63,-1:45,481,-" +
"1:11,290,488,-1:55,291,-1:32,292,-1:73,67,-1:48,293,-1:42,295,-1:53,493,-1:" +
"55,296,-1:48,297,-1:64,298,-1:84,284,-1:20,301,-1:49,284,-1:53,63,-1:62,497" +
",-1:43,302,-1:64,284,-1:52,304,-1:54,63,-1:55,68,-1:41,117,-1:43,1,69,70,71" +
",69:22,-1:2,307,69:5,-1:2,308,69,-1:2,72,73,69:11,-1:30,310,-1:14,310:2,-1:" +
"2,310,-1:6,307:27,118,307:6,309,307:2,-1:2,307:13,-1:30,310,-1:7,75,-1:6,31" +
"0:2,-1:2,310,-1:16,566,-1:62,567,-1:43,314,-1:66,315,-1:38,316,-1:60,317,-1" +
":51,119,-1:40,1,-1:2,81,-1:13,82,-1:7,319,-1:4,82,-1:32,320,-1:49,321,-1:61" +
",322,-1:62,83,-1:53,324,-1:39,325,-1:54,501,-1:59,327,-1:48,120,-1:43,1,84," +
"85,86,84:22,-1:2,329,84:5,-1:2,330,84,-1:4,84:11,-1:30,332,-1:14,332:2,-1:2" +
",332,-1:6,329:27,121,329:6,331,329:2,-1:2,329:13,-1:30,332,-1:7,88,-1:6,332" +
":2,-1:2,332,-1:5,1,-1:2,89,-1:10,334,-1:5,505,508,-1:16,335,-1,90,91,-1:20," +
"336,-1:6,337,-1:77,92,-1:33,340,-1:56,511,-1:35,514,-1:71,462,-1:46,517,-1:" +
"16,341,-1:26,344,-1:63,346,-1:54,346,-1:46,348,-1:59,93,-1:48,93,-1:56,521," +
"-1:48,349,-1:64,351,-1:56,93,-1:39,352,-1:55,353,-1:54,93,-1:42,1,-1:2,94,3" +
"55,356,-1,357,-1,358,359,-1,360,-1,361,523,-1:3,362,548,-1,363,-1,525,364,-" +
"1:2,365,-1:7,366,-1,367,-1,95,96,-1:16,97,-1:18,464,-1:39,368,-1:5,482,-1:4" +
"2,369,-1:7,370,-1:5,555,-1:40,371,-1:60,97,-1:5,372,-1:44,373,-1:5,98,-1:53" +
",374,-1:56,542,-1,376,-1:41,377,-1:5,378,-1:3,379,-1:2,466,-1:41,484,-1:10," +
"518,-1:34,581:2,578,581:24,99,578,581:5,381,581:2,-1:2,581:4,578:2,581:2,57" +
"8,581:4,-1:30,382,-1:14,382:2,-1:2,382,-1:44,100,-1:23,101,-1:48,97,-1:68,3" +
"85,-1:54,545,-1:49,386,-1:56,101,-1:39,387,-1:13,388,-1:2,527,-1,529,389,-1" +
":80,101,-1:6,390,-1:63,391,-1:45,392,-1:61,393,-1:2,394,-1:46,396,-1:43,581" +
":27,122,581:6,381,581:2,-1:2,581:13,-1:30,382,-1:7,102,-1:6,382:2,-1:2,382," +
"-1:25,543,-1:85,532,-1:12,97,-1:52,97,-1:58,482,-1:56,542,-1,400,-1:42,402," +
"-1:69,404,-1:32,406,-1:80,535,-1:32,407,-1:58,101,-1:47,552,-1:96,522,-1:14" +
",553,-1:61,97,-1:52,406,-1:38,410,-1:59,547,-1:47,397,-1:58,537,-1:53,101,-" +
"1:58,404,-1:56,98,-1:42,412,-1:4,413,-1:9,414,-1:51,537,-1:53,103,-1:37,386" +
",-1:67,558,-1:50,98,-1:46,417,-1:65,418,-1:43,104,-1:58,420,-1:57,97,-1:35," +
"581:27,105,581:6,381,581:2,-1:2,581:13,-1:11,424,-1:66,425,-1:38,426,-1:60," +
"427,-1:51,123,-1:52,470,-1:59,142,-1:41,147,-1:57,435,-1:65,153,-1:38,166,-" +
"1:51,437,-1:52,172,-1:67,168,-1:45,475,-1:57,474,-1:59,445,-1:40,177,-1:61," +
"175,-1:56,181,-1:43,184,-1:55,180,-1:47,446,-1:72,193,-1:45,477,-1:60,191,-" +
"1:49,190,-1:45,194,-1:52,197,-1:52,199,-1:63,210,-1:36,617:27,37,217,617:5," +
"215,617:2,-1:2,617:4,217:2,617:2,217,617:4,-1:22,220,-1:47,255,-1:56,257,-1" +
":40,267,-1:66,269,-1:53,288,-1:40,299,-1:70,463,-1:36,347,-1:53,383,-1:49,5" +
"81:19,421,581:7,99,581:6,381,581:2,-1:2,581:13,-1:6,395,-1:60,154,-1:59,431" +
",-1:41,149,-1:69,160,-1:36,169,-1:66,476,-1:49,176,-1:54,178,-1:56,185,-1:3" +
"9,450,-1:67,196,-1:45,479,-1:51,200,-1:66,487,-1:50,294,-1:57,384,-1:32,581" +
":12,421,581:14,99,581:6,381,581:2,-1:2,581:13,-1:13,223,-1:59,434,-1:56,232" +
",-1:50,461,-1:36,581:9,421,581:17,99,581:6,381,581:2,-1:2,581:13,-1:13,226," +
"-1:59,148,-1:56,264,-1:50,300,-1:36,581:27,99,581:3,421,581:2,381,581:2,-1:" +
"2,581:13,-1:13,234,-1:62,500,-1:50,303,-1:36,581:3,421,581:23,99,581:6,381," +
"581:2,-1:2,581:13,-1:13,236,-1:62,311,-1:50,326,-1:36,581:10,421,581:8,421," +
"581:7,99,581:6,381,581:2,-1:2,581:13,-1:13,251,-1:62,579,-1:50,338,-1:36,58" +
"1:5,421,581:21,99,581:6,381,581:2,-1:2,581:13,-1:13,281,-1:59,339,-1:36,581" +
":6,421,581:20,99,581:6,381,581:2,-1:2,581:13,-1:13,305,-1:59,519,-1:36,581:" +
"14,421,581:12,99,581:6,381,581:2,-1:2,581:13,-1:13,312,-1:59,342,-1:36,581:" +
"8,421,581:5,421,581:12,99,581:6,381,581:2,-1:2,581:13,-1:13,313,-1:59,343,-" +
"1:47,398,-1:59,345,-1:47,399,-1:59,350,-1:47,415,-1:59,375,-1:47,419,-1:59," +
"380,-1:47,422,-1:59,534,-1:47,423,-1:59,401,-1:53,403,-1:53,405,-1:53,408,-" +
"1:53,409,-1:53,394,-1:53,411,-1:53,416,-1:53,373,-1:36,617:27,37,454,617:5," +
"215,617:2,-1:2,617:4,454:2,617:2,454,617:4,-1:26,495,-1:43,499,-1:52,496,-1" +
":48,530,-1:66,536,-1:35,531,-1:62,533,-1:41,581:5,465,581:7,483,581:13,99,5" +
"81:6,381,581:2,-1:2,581:13,-1:53,524,-1:19,544,-1:60,513,-1:43,510,-1:52,50" +
"4,-1:48,533,-1:57,537,-1:41,581:3,489,581:23,99,581:6,381,581:2,-1:2,581:13" +
",-1:26,520,-1:43,516,-1:39,581:11,494,581:15,99,581:6,381,581:2,-1:2,581:13" +
",-1:26,526,-1:43,528,-1:39,581:17,498,581:9,99,581:6,381,581:2,-1:2,581:13," +
"-1:2,581:4,502,581:22,99,581:6,381,581:2,-1:2,581:13,-1:2,581:17,506,581:9," +
"99,581:6,381,581:2,-1:2,581:13,-1:2,581:17,509,581:9,99,581:6,381,581:2,-1:" +
"2,581:13,-1:2,581:4,512,581:8,515,581:13,99,581:6,381,581:2,-1:2,581:13,-1:" +
"2,617:27,37,538,617:5,215,617:2,-1:2,617:4,538:2,617:2,538,617:4,-1:44,549," +
"-1:53,556,-1:11,581:4,546,581:6,554,581:2,557,581:3,560,581,561,562,581:2,5" +
"63,581:2,99,581:2,564,581:3,381,581:2,-1:2,581:13,-1:44,559,-1:11,617:27,37" +
",565,617:5,215,617:2,-1:2,617:4,565:2,617:2,565,617:4,-1:21,569,-1:34,581:2" +
"7,99,581,568,581:4,381,581:2,-1:2,581:13,-1:13,571,-1:66,573,-1:71,574,-1:1" +
"1,617:16,570,617:10,37,617,570,617:4,215,617:2,-1:2,617:13,-1:12,575,-1:43," +
"581:27,99,572,581:5,381,581:2,-1:2,581:4,572:2,581:2,572,581:4,-1:22,577,-1" +
":33,617:2,576,617:24,37,617:6,215,617:2,-1:2,617:13,-1:2,617:27,37,580,617:" +
"5,215,617:2,-1:2,617:4,580:2,617:2,580,617:4,-1:2,617:27,37,582,617:5,215,6" +
"17:2,-1:2,617:4,582:2,617:2,582,617:4,-1:2,617:27,37,617:4,583,617,215,617:" +
"2,-1:2,617:13,-1:2,617:27,37,584,617:5,215,617:2,-1:2,617:4,584:2,617:2,584" +
",617:4,-1:2,617:27,37,585,617:5,215,617:2,-1:2,617:4,585:2,617:2,585,617:4," +
"-1:2,617:27,37,617:4,586,617,215,617:2,-1:2,617:13,-1:2,617:27,37,587,617:5" +
",215,617:2,-1:2,617:4,587:2,617:2,587,617:4,-1:2,617:27,37,588,617:5,215,61" +
"7:2,-1:2,617:4,588:2,617:2,588,617:4,-1:2,617:2,589,617:24,37,617:6,215,617" +
":2,-1:2,617:13,-1:2,617:27,37,590,617:5,215,617:2,-1:2,617:4,590:2,617:2,59" +
"0,617:4,-1:2,617:27,37,591,617:5,215,617:2,-1:2,617:4,591:2,617:2,591,617:4" +
",-1:2,617:27,37,592,617:5,215,617:2,-1:2,617:4,592:2,617:2,592,617:4,-1:2,6" +
"17:27,37,593,617:5,215,617:2,-1:2,617:4,593:2,617:2,593,617:4,-1:2,617:27,3" +
"7,617,594,617:4,215,617:2,-1:2,617:13,-1:2,617:19,595,617:7,37,617:6,215,61" +
"7:2,-1:2,617:13,-1:2,617:12,595,617:14,37,617:6,215,617:2,-1:2,617:13,-1:2," +
"617:9,595,617:17,37,617:6,215,617:2,-1:2,617:13,-1:2,617:27,37,617:3,595,61" +
"7:2,215,617:2,-1:2,617:13,-1:2,617:3,595,617:23,37,617:6,215,617:2,-1:2,617" +
":13,-1:2,617:10,595,617:8,595,617:7,37,617:6,215,617:2,-1:2,617:13,-1:2,617" +
":5,595,617:21,37,617:6,215,617:2,-1:2,617:13,-1:2,617:6,595,617:20,37,617:6" +
",215,617:2,-1:2,617:13,-1:2,617:14,595,617:12,37,617:6,215,617:2,-1:2,617:1" +
"3,-1:2,617:8,595,617:5,595,617:12,37,617:6,215,617:2,-1:2,617:13,-1:2,617:5" +
",596,617:7,597,617:13,37,617:6,215,617:2,-1:2,617:13,-1:2,617:3,598,617:23," +
"37,617:6,215,617:2,-1:2,617:13,-1:2,617:11,599,617:15,37,617:6,215,617:2,-1" +
":2,617:13,-1:2,617:17,600,617:9,37,617:6,215,617:2,-1:2,617:13,-1:2,617:4,6" +
"01,617:22,37,617:6,215,617:2,-1:2,617:13,-1:2,617:17,602,617:9,37,617:6,215" +
",617:2,-1:2,617:13,-1:2,617:17,603,617:9,37,617:6,215,617:2,-1:2,617:13,-1:" +
"2,617:4,604,617:8,605,617:13,37,617:6,215,617:2,-1:2,617:13,-1:2,617:4,606," +
"617:6,607,617:2,608,617:3,609,617,610,611,617:2,612,617:2,37,617:2,613,617:" +
"3,215,617:2,-1:2,617:13,-1:2,617:27,37,617,614,617:4,215,617:2,-1:2,617:13," +
"-1:2,617:27,37,615,617:5,215,617:2,-1:2,617:4,615:2,617:2,615,617:4,-1");

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
						{ yybegin(commanddetail); return new Symbol(sym.CHECK, yytext().Substring(1, yytext().Length-1)); }
					case -10:
						break;
					case 10:
						{ yybegin(commanddetail); return new Symbol(sym.LIST, yytext().Substring(1, yytext().Length-2)); }
					case -11:
						break;
					case 11:
						{ yybegin(commanddetail); return new Symbol(sym.LSUB, yytext().Substring(1, yytext().Length-2)); }
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
						{ yybegin(commandstatus); return new Symbol(sym.STATUS, yytext().Substring(1, yytext().Length-2)); }
					case -22:
						break;
					case 22:
						{ yybegin(commandsearch); return new Symbol(sym.SEARCH, yytext().Substring(1, yytext().Length-2)); }
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
						{ return new Symbol(sym.ASTRING, yytext()); }
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
						{ return new Symbol(sym.ASTRING, yytext()); }
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
						{ return new Symbol(sym.ASTRING, yytext()); }
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
					case 94:
						{ return new Symbol(sym.SP); }
					case -95:
						break;
					case 95:
						{ return new Symbol(sym.LPARENT); }
					case -96:
						break;
					case 96:
						{ return new Symbol(sym.RPARENT); }
					case -97:
						break;
					case 97:
						{ return new Symbol(sym.SEARCH_KEYWORD_ASTRING, yytext()); }
					case -98:
						break;
					case 98:
						{ return new Symbol(sym.SEARCH_KEYWORD_DATE, yytext()); }
					case -99:
						break;
					case 99:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -100:
						break;
					case 100:
						{ return new Symbol(sym.CRLF); }
					case -101:
						break;
					case 101:
						{ return new Symbol(sym.SEARCH_KEYWORD_SOLE, yytext()); }
					case -102:
						break;
					case 102:
						{ return new Symbol(sym.LITERAL, yytext()); }
					case -103:
						break;
					case 103:
						{ return new Symbol(sym.SEARCH_KEYWORD_NUMBER, yytext()); }
					case -104:
						break;
					case 104:
						{ return new Symbol(sym.CHARSET, yytext()); }
					case -105:
						break;
					case 105:
						{ return new Symbol(sym.DATE, yytext()); }
					case -106:
						break;
					case 106:
						{ yybegin(commandsearch); break; }
					case -107:
						break;
					case 107:
						{ return new Symbol(sym.STAR); }
					case -108:
						break;
					case 108:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -109:
						break;
					case 109:
						{ return new Symbol(sym.COLON); }
					case -110:
						break;
					case 110:
						{ return new Symbol(sym.COMMA); }
					case -111:
						break;
					case 112:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -112:
						break;
					case 113:
						{ yybegin(commanddetail); break; }
					case -113:
						break;
					case 114:
						{ yybegin(commandfetch); break; }
					case -114:
						break;
					case 115:
						{ return new Symbol(sym.FETCH_ATT, yytext()); }
					case -115:
						break;
					case 116:
						{ return new Symbol(sym.FETCH_MSG_TEXT_SINGLE, yytext()); }
					case -116:
						break;
					case 117:
						{ yybegin(commandfetchheaderlist);return new Symbol(sym.FETCH_MSG_TEXT_HEADERLIST_KEY, yytext()); }
					case -117:
						break;
					case 118:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -118:
						break;
					case 119:
						{ yybegin(commandstoreflags); break; }
					case -119:
						break;
					case 120:
						{ yybegin(commanddetail); return new Symbol(sym.FLAG_KEY, yytext()); }
					case -120:
						break;
					case 121:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -121:
						break;
					case 122:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -122:
						break;
					case 123:
						{ yybegin(commandsearch); break; }
					case -123:
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
