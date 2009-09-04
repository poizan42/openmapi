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
// need to have these pragmas to eliminate warnings caused by generated variables that are not used
#pragma warning disable 0414
#pragma warning disable 0169
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
				Console.WriteLine(t);
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
			Symbol t = yy.next_token();
			Console.WriteLine(t);
			return t;
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
    Console.WriteLine(errorMsg[code]);
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
	private const int commandsearchheader = 12;
	private const int commandfetchheaderlist = 6;
	private const int commandsearchastring = 13;
	private const int commandstatus = 9;
	private const int commandsearchuidsequence = 16;
	private const int commandstoreflags = 8;
	private const int commandsearch = 11;
	private const int commandstatuslist = 10;
	private const int commandsequence = 3;
	private const int commandfetch = 5;
	private const int commandfetchsequence = 4;
	private const int commanddetail = 2;
	private const int commandstoresequence = 7;
	private const int commandsearchnumber = 14;
	private const int commandbase = 1;
	private const int YYINITIAL = 0;
	private const int commandsearchsequence = 15;
	private static readonly int[] yy_state_dtrans =new int[] {
		0,
		128,
		231,
		42,
		47,
		261,
		326,
		77,
		338,
		348,
		353,
		374,
		477,
		481,
		485,
		122,
		491
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
		/* 111 */ YY_NO_ANCHOR,
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
		/* 124 */ YY_NO_ANCHOR,
		/* 125 */ YY_NO_ANCHOR,
		/* 126 */ YY_NO_ANCHOR,
		/* 127 */ YY_NO_ANCHOR,
		/* 128 */ YY_NOT_ACCEPT,
		/* 129 */ YY_NO_ANCHOR,
		/* 130 */ YY_NO_ANCHOR,
		/* 131 */ YY_NO_ANCHOR,
		/* 132 */ YY_NO_ANCHOR,
		/* 133 */ YY_NO_ANCHOR,
		/* 134 */ YY_NO_ANCHOR,
		/* 135 */ YY_NO_ANCHOR,
		/* 136 */ YY_NO_ANCHOR,
		/* 137 */ YY_NO_ANCHOR,
		/* 138 */ YY_NO_ANCHOR,
		/* 139 */ YY_NO_ANCHOR,
		/* 140 */ YY_NO_ANCHOR,
		/* 141 */ YY_NO_ANCHOR,
		/* 142 */ YY_NO_ANCHOR,
		/* 143 */ YY_NOT_ACCEPT,
		/* 144 */ YY_NO_ANCHOR,
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
		/* 617 */ YY_NOT_ACCEPT,
		/* 618 */ YY_NOT_ACCEPT,
		/* 619 */ YY_NOT_ACCEPT,
		/* 620 */ YY_NOT_ACCEPT,
		/* 621 */ YY_NOT_ACCEPT,
		/* 622 */ YY_NOT_ACCEPT,
		/* 623 */ YY_NOT_ACCEPT,
		/* 624 */ YY_NOT_ACCEPT,
		/* 625 */ YY_NOT_ACCEPT,
		/* 626 */ YY_NOT_ACCEPT,
		/* 627 */ YY_NOT_ACCEPT,
		/* 628 */ YY_NOT_ACCEPT,
		/* 629 */ YY_NOT_ACCEPT,
		/* 630 */ YY_NOT_ACCEPT,
		/* 631 */ YY_NOT_ACCEPT,
		/* 632 */ YY_NOT_ACCEPT,
		/* 633 */ YY_NOT_ACCEPT,
		/* 634 */ YY_NOT_ACCEPT,
		/* 635 */ YY_NOT_ACCEPT,
		/* 636 */ YY_NOT_ACCEPT,
		/* 637 */ YY_NOT_ACCEPT,
		/* 638 */ YY_NOT_ACCEPT,
		/* 639 */ YY_NOT_ACCEPT,
		/* 640 */ YY_NOT_ACCEPT,
		/* 641 */ YY_NOT_ACCEPT,
		/* 642 */ YY_NOT_ACCEPT,
		/* 643 */ YY_NOT_ACCEPT,
		/* 644 */ YY_NOT_ACCEPT,
		/* 645 */ YY_NOT_ACCEPT,
		/* 646 */ YY_NOT_ACCEPT,
		/* 647 */ YY_NOT_ACCEPT,
		/* 648 */ YY_NOT_ACCEPT,
		/* 649 */ YY_NOT_ACCEPT,
		/* 650 */ YY_NOT_ACCEPT,
		/* 651 */ YY_NOT_ACCEPT,
		/* 652 */ YY_NOT_ACCEPT,
		/* 653 */ YY_NOT_ACCEPT,
		/* 654 */ YY_NOT_ACCEPT,
		/* 655 */ YY_NOT_ACCEPT,
		/* 656 */ YY_NOT_ACCEPT,
		/* 657 */ YY_NOT_ACCEPT,
		/* 658 */ YY_NOT_ACCEPT,
		/* 659 */ YY_NOT_ACCEPT,
		/* 660 */ YY_NOT_ACCEPT,
		/* 661 */ YY_NOT_ACCEPT,
		/* 662 */ YY_NOT_ACCEPT,
		/* 663 */ YY_NOT_ACCEPT,
		/* 664 */ YY_NOT_ACCEPT,
		/* 665 */ YY_NOT_ACCEPT,
		/* 666 */ YY_NOT_ACCEPT,
		/* 667 */ YY_NOT_ACCEPT,
		/* 668 */ YY_NOT_ACCEPT,
		/* 669 */ YY_NOT_ACCEPT,
		/* 670 */ YY_NOT_ACCEPT,
		/* 671 */ YY_NOT_ACCEPT,
		/* 672 */ YY_NOT_ACCEPT,
		/* 673 */ YY_NOT_ACCEPT,
		/* 674 */ YY_NOT_ACCEPT,
		/* 675 */ YY_NOT_ACCEPT,
		/* 676 */ YY_NOT_ACCEPT,
		/* 677 */ YY_NOT_ACCEPT,
		/* 678 */ YY_NOT_ACCEPT,
		/* 679 */ YY_NOT_ACCEPT,
		/* 680 */ YY_NOT_ACCEPT,
		/* 681 */ YY_NOT_ACCEPT,
		/* 682 */ YY_NOT_ACCEPT
	};
	private int[] yy_cmap = unpackFromString(1,65538,
"34,1:9,39,1:2,38,1:18,3,1,28,1:2,26,1:2,40,41,27,17,42,30,46,1,29,48,45,48:" +
"5,44,48,33,1,50,1,51,1:2,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12,6,1,20,22," +
"10,14,32,52,16,11,47,49,35,2,1,43,1,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12" +
",6,1,20,22,10,14,32,52,16,11,47,36,53,37,1:65410,0:2")[0];

	private int[] yy_rmap = unpackFromString(1,683,
"0,1,2,1,3,1:13,4,1:12,5,6,1,7,1:7,8,1,9,1:2,10,1,11,1:4,12,1:2,13,1:7,14,1:" +
"3,15,16,17,1:5,18,1,19,1:4,20,21,22,1:8,23,1,24,1:14,25,1:3,26,1:4,27,28,1," +
"29,1:3,30,31,1:2,32,33,1,34,1:2,35,36,37,38,1,39,40,41,42,43,44,45,3,46,47," +
"48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72," +
"73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97," +
"98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,1" +
"17,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135," +
"136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154" +
",155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,17" +
"3,157,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,1" +
"91,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209," +
"210,211,212,213,214,215,216,217,218,219,220,34,221,222,223,224,225,226,227," +
"228,229,230,231,232,233,234,235,236,237,238,239,240,241,35,242,243,244,245," +
"246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264" +
",265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,282,28" +
"3,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,3" +
"02,303,304,305,306,307,308,309,310,311,312,313,314,315,316,317,318,319,320," +
"321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,306" +
",339,340,341,342,343,344,345,346,347,348,349,350,351,352,353,354,355,356,35" +
"7,358,359,360,361,344,307,362,319,363,364,365,37,366,367,368,38,369,370,371" +
",372,373,374,375,376,377,378,379,380,381,382,383,384,385,386,387,388,389,39" +
"0,391,392,393,394,395,396,397,398,399,400,401,402,403,404,405,406,407,408,4" +
"09,410,411,412,413,414,415,416,417,418,419,420,421,422,423,424,425,426,427," +
"428,429,430,431,432,433,434,435,436,437,321,438,439,440,441,442,443,444,445" +
",446,447,448,449,450,451,452,453,454,455,456,457,458,459,460,461,462,463,46" +
"4,465,466,467,468,469,470,471,472,473,474,475,476,477,478,479,480,481,482,4" +
"83,484,485,486,487,488,489,490,491,492,493,494,495,496,497,498,499,500,501," +
"502,503,504,505,506,507,508,509,510,511,512,513,514,515,516,517,518,519,520" +
",521,522,523,524,525,526,527,528,529,530,531,532,533,534,535,536,537,538,53" +
"9,540,541,542,543,544,545,546,547,548,549,550,551,552,553,554,555,556,557,5" +
"58,559,560,561,562,563,564,565,566,31")[0];

	private int[][] yy_nxt = unpackFromString(567,54,
"1,2:2,3,2:13,3,2:8,3:3,2:5,3:3,2,-1:2,3:2,2:11,3,-1:55,2:2,-1,2:13,-1,2:8,-" +
"1:3,2:5,-1:3,2,-1:4,2:11,-1:2,4,-1:2,4:22,-1:3,4:5,-1:3,4,-1:4,4:11,-1:4,25" +
",-1:51,31,32,-1,31:22,34:2,-1,31:5,-1:3,31,-1:4,31:11,-1:2,32:2,-1,32:22,34" +
":2,-1,32:5,-1:3,32,-1:4,32:11,-1:2,34:2,-1,34:24,-1,34:5,-1:3,34,-1:4,34:11" +
",-1,1,-1:18,238,-1:7,43,-1,44,-1:3,45,-1:8,46,-1,44:2,-1:2,44,-1:34,44,-1:1" +
"4,44:2,-1:2,44,-1:5,1,-1:18,251,-1:7,48,-1,49,-1:3,50,-1:8,51,-1,49:2,-1:2," +
"49,-1:34,49,-1:14,49:2,-1:2,49,-1:34,54,-1:14,54:2,-1:2,54,-1:34,57,-1:14,5" +
"7:2,-1:2,57,-1:27,293,-1:23,294,-1:53,623,-1:8,70,71,-1,70:22,-1:3,70:5,-1:" +
"3,70,-1:4,70:11,-1:2,71:2,-1,71:22,-1:3,71:5,-1:3,71,-1:4,71:11,-1,1,-1:18," +
"613,-1:7,78,-1,79,-1:3,80,-1:8,81,-1,79:2,-1:2,79,-1:34,79,-1:14,79:2,-1:2," +
"79,-1:51,343,-1:8,85,86,-1,85:22,-1:3,85:5,-1:3,85,-1:4,85:11,-1:2,86:2,-1," +
"86:22,-1:3,86:5,-1:3,86,-1:4,86:11,-1:30,388,-1:14,388:2,-1:2,388,-1:34,139" +
",-1:14,139:2,-1:2,139,-1:6,112:2,-1,112:22,-1:3,112:5,-1:3,112,-1:4,112:11," +
"-1:2,116:2,-1,116:22,-1:3,116:5,-1:3,116,-1:4,116:11,-1:30,121,-1:14,121:2," +
"-1:2,121,-1:5,1,-1:18,624,-1:7,123,-1,124,-1:3,125,-1:8,126,-1,124:2,-1:2,1" +
"24,-1:34,124,-1:14,124:2,-1:2,124,-1:5,1,-1:2,143,-1:51,682:27,38,682:6,235" +
",682:2,-1:2,682:13,-1:47,305,-1:53,306,-1:8,327:27,75,327:6,329,327:2,-1:2," +
"327:13,-1:2,349:27,88,349:6,351,349:2,-1:2,349:13,-1:30,144,406,-1:13,144:2" +
",-1:2,144,-1:6,478:27,114,478:6,479,478:2,-1:2,478:13,-1:2,482:27,118,482:6" +
",483,482:2,-1:2,482:13,-1:5,145,146,-1:3,147,-1:4,148,149,150,-1,151,152,49" +
"3,-1,153,-1:2,537,-1:57,144,-1:14,144:2,-1:2,144,-1:10,154,-1:3,492,-1:2,49" +
"4,-1:7,555,-1:2,560,-1:36,538,-1:7,155,-1:47,156,-1:3,157,-1:9,158,-1:39,15" +
"9,-1:6,496,-1:50,536,-1:57,160,-1:55,161,-1:45,163,-1:3,164,-1:3,165,-1:41," +
"166,-1:57,170,-1:65,171,-1:44,172,-1:54,497,-1:58,5,-1:39,175,502,-1:56,500" +
",-1:59,540,-1:43,176,-1:6,177,-1:48,178,-1:51,503,-1:3,541,-1:49,179,-1:59," +
"181,-1:46,182,-1:67,183,-1:58,184,-1:40,185,-1:51,505,-1:3,542,-1:55,504,-1" +
":45,6,-1:68,187,-1:42,190,-1:9,508,-1:53,506,-1:55,191,-1:38,507,-1:64,7,-1" +
":38,8,-1:74,9,-1:44,193,-1:56,194,-1:38,10,-1:53,11,-1:58,511,-1:55,199,-1:" +
"64,513,-1:46,512,-1:43,202,-1:72,203,-1:49,206,-1:49,547,-1:41,12,-1:60,13," +
"-1:65,207,-1:44,208,-1:58,209,-1:45,515,-1:46,14,-1:70,212,-1:36,15,-1:59,5" +
"16,-1:47,16,-1:53,17,-1:54,215,-1:67,18,-1:38,19,-1:53,20,-1:53,21,-1:58,21" +
"8,-1:48,22,-1:53,23,-1:70,221,-1:36,24,-1:72,26,-1:38,222,-1:56,223,-1:47,2" +
"24,-1:57,225,-1:63,226,-1:46,27,-1:47,227,-1:55,228,-1:49,28,-1:60,517,-1:6" +
"1,229,-1:38,29,-1:53,30,-1:50,1,31,32,33,31:22,34:2,232,31:5,-1,35,233,31,2" +
"34,-1,36,37,31:11,-1:2,682:2,681,682:24,38,681,682:5,235,682:2,-1:2,682:4,6" +
"81:2,682:2,681,682:4,-1:30,236,-1:14,236:2,-1:2,236,-1:44,39,-1:15,682:27,1" +
"29,682:6,235,682:2,-1:2,682:13,-1:30,236,-1:7,40,-1:6,236:2,-1:2,236,-1:6,6" +
"82:27,41,682:6,235,682:2,-1:2,682:13,-1:15,239,-1:60,519,-1:43,241,-1:85,24" +
"2,-1:35,554,-1:48,244,-1:76,245,-1:25,559,-1:48,247,-1:66,248,-1:38,249,-1:" +
"60,250,-1:51,130,-1:54,549,-1:50,253,-1:85,611,-1:30,255,-1:76,612,-1:20,25" +
"7,-1:66,258,-1:38,259,-1:60,260,-1:51,131,-1:40,1,-1,52,53,-1,262,-1,570,26" +
"3,-1,264,-1:3,265,-1:3,520,-1,266,267,-1,521,-1,268,-1:3,54,-1:8,269,-1,55," +
"56,-1:2,57:2,58,-1,57,59,60,61,-1:11,270,-1:59,272,-1:56,273,-1:43,274,-1:7" +
"0,276,-1:36,561,-1:50,278,-1:3,522,-1:4,279,-1:78,62,-1:23,63,-1:63,280,-1:" +
"44,281,-1:59,282,-1:56,64,-1:66,523,-1:25,283,-1:54,285,-1:70,286,-1:42,65," +
"-1:60,288,-1:45,66,-1:87,290,-1:27,67,-1:54,291,-1:44,63,-1:56,292,-1:60,29" +
"5,-1:42,573,-1:89,296,-1:26,297,-1:57,64,-1:41,298,-1:49,299,-1:62,300,-1:8" +
"3,132,-1:28,133,-1:53,302,-1:51,524,-1:40,303,-1:54,304,-1:61,307,-1:48,309" +
",-1:62,64,-1:45,550,-1:11,310,557,-1:55,311,-1:32,312,-1:73,68,-1:48,313,-1" +
":42,315,-1:53,562,-1:55,316,-1:48,317,-1:64,318,-1:84,304,-1:20,321,-1:49,3" +
"04,-1:53,64,-1:62,566,-1:43,322,-1:64,304,-1:52,324,-1:54,64,-1:55,69,-1:41" +
",134,-1:43,1,70,71,72,70:22,-1:2,327,70:5,-1:2,328,70,-1:2,73,74,70:11,-1:3" +
"0,330,-1:14,330:2,-1:2,330,-1:6,327:27,135,327:6,329,327:2,-1:2,327:13,-1:3" +
"0,330,-1:7,76,-1:6,330:2,-1:2,330,-1:16,634,-1:62,635,-1:43,334,-1:66,335,-" +
"1:38,336,-1:60,337,-1:51,136,-1:40,1,-1:2,82,-1:13,83,-1:7,339,-1:4,83,-1:3" +
"2,340,-1:49,341,-1:61,342,-1:62,84,-1:53,344,-1:39,345,-1:54,569,-1:59,347," +
"-1:48,137,-1:43,1,85,86,87,85:22,-1:2,349,85:5,-1:2,350,85,-1:4,85:11,-1:30" +
",352,-1:14,352:2,-1:2,352,-1:6,349:27,138,349:6,351,349:2,-1:2,349:13,-1:30" +
",352,-1:7,89,-1:6,352:2,-1:2,352,-1:5,1,-1:2,90,-1:10,354,-1:5,572,574,-1:1" +
"6,355,-1,91,92,-1:20,356,-1:6,357,-1:77,93,-1:33,360,-1:56,576,-1:35,578,-1" +
":71,526,-1:46,580,-1:16,361,-1:26,364,-1:63,366,-1:54,366,-1:46,368,-1:59,9" +
"4,-1:48,94,-1:56,584,-1:48,369,-1:64,371,-1:56,94,-1:39,372,-1:55,373,-1:54" +
",94,-1:42,1,-1:2,95,375,376,-1,377,-1,378,379,-1,380,-1,381,382,-1:3,383,62" +
"1,-1,384,586,588,385,-1,96,386,97,-1:8,387,-1,98,99,-1:2,97:2,-1:2,97,-1:9," +
"100,-1:18,528,-1:39,389,-1:5,551,-1:42,390,-1:7,391,-1:5,628,-1:40,392,-1:6" +
"0,100,-1:5,393,-1:44,394,-1:5,101,-1:4,102,-1:41,395,-1:6,396,-1:50,397,-1:" +
"5,398,-1:53,615,-1,399,-1:41,400,-1:5,401,-1:3,402,-1:2,529,-1:41,552,-1:10" +
",581,-1:36,405,-1:25,405,-1:14,405:2,-1:2,405,-1:44,103,-1:44,406,-1:32,104" +
",-1:48,100,-1:68,409,-1:54,618,-1:49,410,-1:56,104,-1:53,105,-1:39,411,-1:1" +
"3,412,-1:2,590,-1,592,413,-1:38,106,-1:95,104,-1:6,414,-1:63,415,-1:45,416," +
"-1:61,417,-1:2,418,-1:40,614,-1:59,420,-1:71,423,-1:14,423:2,-1:2,423,-1:10" +
",424,-1:6,530,-1:2,585,-1:3,597,-1,425,598,-1:2,599,-1:5,426,-1:42,616,-1:8" +
"5,600,-1:12,100,-1:52,100,-1:58,551,-1:56,615,-1,531,-1:42,429,-1:69,431,-1" +
":32,433,-1:80,603,-1:32,434,-1:58,104,-1:47,625,-1:96,587,-1:14,626,-1:61,1" +
"00,-1:62,435,-1:29,532,-1:7,436,-1:44,440,-1:53,443,-1:8,444,-1:59,433,-1:4" +
"4,619,-1:47,421,-1:58,606,-1:53,104,-1:58,431,-1:56,101,-1:42,449,-1:4,450," +
"-1:9,451,-1:36,454,-1:6,533,-1:2,589,-1:3,607,-1,455,608,-1:2,609,-1:5,456," +
"-1:35,457,-1:50,457,-1:75,457,-1:25,457,-1:60,457,-1:8,457,-1:39,457,-1:54," +
"457,-1:61,457,-1:47,457,-1:5,457,-1:58,606,-1:53,107,-1:37,410,-1:67,631,-1" +
":50,101,-1:46,459,-1:65,108,-1:53,460,-1:39,461,-1:7,462,-1:44,466,-1:53,53" +
"4,-1:8,469,-1:69,470,-1:33,109,-1:58,472,-1:57,100,-1:54,473,-1:46,473,-1:5" +
"0,473,-1:75,473,-1:25,473,-1:60,473,-1:8,473,-1:39,473,-1:54,473,-1:55,473," +
"-1:5,473,-1:67,620,-1:14,620:2,-1:2,620,-1:35,627,-1:52,110,-1:14,110:2,-1:" +
"2,110,-1:33,111,-1:25,1,112:2,113,112:22,-1:2,478,112:5,-1:2,558,112,-1:4,1" +
"12:11,-1:2,478:27,140,478:6,479,478:2,-1:2,478:13,-1:30,480,-1:7,115,-1:6,4" +
"80:2,-1:2,480,-1:5,1,116:2,117,116:22,-1:2,482,116:5,-1:2,563,116,-1:4,116:" +
"11,-1:2,482:27,141,482:6,483,482:2,-1:2,482:13,-1:30,484,-1:7,119,-1:6,484:" +
"2,-1:2,484,-1:5,1,-1:2,120,-1:25,121,-1:14,121:2,-1:2,121,-1:15,487,-1:66,4" +
"88,-1:38,489,-1:60,490,-1:51,142,-1:40,1,-1:2,127,-1:62,539,-1:59,162,-1:41" +
",167,-1:57,499,-1:65,173,-1:38,186,-1:51,501,-1:52,192,-1:67,188,-1:45,544," +
"-1:57,543,-1:59,509,-1:40,197,-1:61,195,-1:56,201,-1:43,204,-1:55,200,-1:47" +
",510,-1:72,213,-1:45,546,-1:60,211,-1:49,210,-1:45,214,-1:52,217,-1:52,219," +
"-1:63,230,-1:36,682:27,38,237,682:5,235,682:2,-1:2,682:4,237:2,682:2,237,68" +
"2:4,-1:22,240,-1:47,275,-1:56,277,-1:40,287,-1:66,289,-1:53,308,-1:40,319,-" +
"1:70,527,-1:36,367,-1:53,407,-1:53,419,-1:52,437,-1:54,447,-1:68,457,-1:37," +
"463,-1:64,473,-1:67,475,-1:14,475:2,-1:2,475,-1:17,174,-1:59,495,-1:41,169," +
"-1:69,180,-1:36,189,-1:66,545,-1:49,196,-1:54,198,-1:56,205,-1:39,514,-1:67" +
",216,-1:45,548,-1:51,220,-1:66,556,-1:50,314,-1:57,408,-1:60,476,-1:14,476:" +
"2,-1:2,476,-1:17,243,-1:59,498,-1:56,252,-1:50,525,-1:64,480,-1:14,480:2,-1" +
":2,480,-1:17,246,-1:59,168,-1:56,284,-1:50,320,-1:64,484,-1:14,484:2,-1:2,4" +
"84,-1:17,254,-1:62,568,-1:50,323,-1:47,256,-1:62,331,-1:50,346,-1:47,271,-1" +
":62,645,-1:50,358,-1:47,301,-1:59,359,-1:47,325,-1:59,582,-1:47,332,-1:59,3" +
"62,-1:47,333,-1:59,363,-1:47,422,-1:59,365,-1:47,427,-1:59,370,-1:47,438,-1" +
":59,403,-1:47,453,-1:59,404,-1:47,464,-1:59,602,-1:47,471,-1:59,428,-1:47,4" +
"74,-1:59,430,-1:47,486,-1:59,432,-1:53,439,-1:53,441,-1:53,442,-1:53,445,-1" +
":53,446,-1:53,418,-1:53,448,-1:53,452,-1:53,458,-1:53,394,-1:53,465,-1:53,4" +
"67,-1:53,468,-1:36,682:27,38,518,682:5,235,682:2,-1:2,682:4,518:2,682:2,518" +
",682:4,-1:26,564,-1:43,567,-1:52,565,-1:58,604,-1:43,594,-1:66,605,-1:35,59" +
"6,-1:62,601,-1:92,591,-1:30,535,-1:14,535:2,-1:2,535,-1:23,617,-1:60,577,-1" +
":43,575,-1:52,571,-1:48,601,-1:57,606,-1:69,636,-1:14,636:2,-1:2,636,-1:30," +
"583,-1:43,579,-1:67,553,-1:14,553:2,-1:2,553,-1:30,593,-1:43,595,-1:39,682:" +
"27,38,610,682:5,235,682:2,-1:2,682:4,610:2,682:2,610,682:4,-1:44,622,-1:53," +
"629,-1:39,630,-1:14,630:2,-1:2,630,-1:48,632,-1:11,682:27,38,633,682:5,235," +
"682:2,-1:2,682:4,633:2,682:2,633,682:4,-1:21,637,-1:45,639,-1:66,640,-1:71," +
"641,-1:11,682:16,638,682:10,38,682,638,682:4,235,682:2,-1:2,682:13,-1:12,64" +
"2,-1:63,644,-1:33,682:2,643,682:24,38,682:6,235,682:2,-1:2,682:13,-1:2,682:" +
"27,38,646,682:5,235,682:2,-1:2,682:4,646:2,682:2,646,682:4,-1:2,682:27,38,6" +
"47,682:5,235,682:2,-1:2,682:4,647:2,682:2,647,682:4,-1:2,682:27,38,682:4,64" +
"8,682,235,682:2,-1:2,682:13,-1:2,682:27,38,649,682:5,235,682:2,-1:2,682:4,6" +
"49:2,682:2,649,682:4,-1:2,682:27,38,650,682:5,235,682:2,-1:2,682:4,650:2,68" +
"2:2,650,682:4,-1:2,682:27,38,682:4,651,682,235,682:2,-1:2,682:13,-1:2,682:2" +
"7,38,652,682:5,235,682:2,-1:2,682:4,652:2,682:2,652,682:4,-1:2,682:27,38,65" +
"3,682:5,235,682:2,-1:2,682:4,653:2,682:2,653,682:4,-1:2,682:2,654,682:24,38" +
",682:6,235,682:2,-1:2,682:13,-1:2,682:27,38,655,682:5,235,682:2,-1:2,682:4," +
"655:2,682:2,655,682:4,-1:2,682:27,38,656,682:5,235,682:2,-1:2,682:4,656:2,6" +
"82:2,656,682:4,-1:2,682:27,38,657,682:5,235,682:2,-1:2,682:4,657:2,682:2,65" +
"7,682:4,-1:2,682:27,38,658,682:5,235,682:2,-1:2,682:4,658:2,682:2,658,682:4" +
",-1:2,682:27,38,682,659,682:4,235,682:2,-1:2,682:13,-1:2,682:19,660,682:7,3" +
"8,682:6,235,682:2,-1:2,682:13,-1:2,682:12,660,682:14,38,682:6,235,682:2,-1:" +
"2,682:13,-1:2,682:9,660,682:17,38,682:6,235,682:2,-1:2,682:13,-1:2,682:27,3" +
"8,682:3,660,682:2,235,682:2,-1:2,682:13,-1:2,682:3,660,682:23,38,682:6,235," +
"682:2,-1:2,682:13,-1:2,682:10,660,682:8,660,682:7,38,682:6,235,682:2,-1:2,6" +
"82:13,-1:2,682:5,660,682:21,38,682:6,235,682:2,-1:2,682:13,-1:2,682:6,660,6" +
"82:20,38,682:6,235,682:2,-1:2,682:13,-1:2,682:14,660,682:12,38,682:6,235,68" +
"2:2,-1:2,682:13,-1:2,682:8,660,682:5,660,682:12,38,682:6,235,682:2,-1:2,682" +
":13,-1:2,682:5,661,682:7,662,682:13,38,682:6,235,682:2,-1:2,682:13,-1:2,682" +
":3,663,682:23,38,682:6,235,682:2,-1:2,682:13,-1:2,682:11,664,682:15,38,682:" +
"6,235,682:2,-1:2,682:13,-1:2,682:17,665,682:9,38,682:6,235,682:2,-1:2,682:1" +
"3,-1:2,682:4,666,682:22,38,682:6,235,682:2,-1:2,682:13,-1:2,682:17,667,682:" +
"9,38,682:6,235,682:2,-1:2,682:13,-1:2,682:17,668,682:9,38,682:6,235,682:2,-" +
"1:2,682:13,-1:2,682:4,669,682:8,670,682:13,38,682:6,235,682:2,-1:2,682:13,-" +
"1:2,682:4,671,682:6,672,682:2,673,682:3,674,682,675,676,682:2,677,682:2,38," +
"682:2,678,682:3,235,682:2,-1:2,682:13,-1:2,682:27,38,682,679,682:4,235,682:" +
"2,-1:2,682:13,-1:2,682:27,38,680,682:5,235,682:2,-1:2,682:4,680:2,682:2,680" +
",682:4,-1");

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
						{ yybegin(commandsequence); return new Symbol(sym.EXPUNGE, yytext().Substring(1, yytext().Length-2)); }
					case -26:
						break;
					case 26:
						{ yybegin(commanddetail); return new Symbol(sym.STARTTLS, yytext().Substring(1)); }
					case -27:
						break;
					case 27:
						{ yybegin(commanddetail); return new Symbol(sym.CAPABILITY, yytext().Substring(1)); }
					case -28:
						break;
					case 28:
						{ yybegin(commanddetail); return new Symbol(sym.SUBSCRIBE, yytext().Substring(1, yytext().Length-2)); }
					case -29:
						break;
					case 29:
						{ yybegin(commanddetail); return new Symbol(sym.UNSUBSCRIBE, yytext().Substring(1, yytext().Length-2)); }
					case -30:
						break;
					case 30:
						{ yybegin(commanddetail); return new Symbol(sym.AUTHENTICATE, yytext().Substring(1, yytext().Length-2)); }
					case -31:
						break;
					case 31:
						{ return new Symbol(sym.ATOM, yytext()); }
					case -32:
						break;
					case 32:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -33:
						break;
					case 33:
						{ return new Symbol(sym.SP); }
					case -34:
						break;
					case 34:
						{ return new Symbol(sym.LIST_MAILBOX, yytext()); }
					case -35:
						break;
					case 35:
						{ return new Symbol(sym.BACKSLASH); }
					case -36:
						break;
					case 36:
						{ return new Symbol(sym.LPARENT); }
					case -37:
						break;
					case 37:
						{ return new Symbol(sym.RPARENT); }
					case -38:
						break;
					case 38:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -39:
						break;
					case 39:
						{ return new Symbol(sym.CRLF); }
					case -40:
						break;
					case 40:
						{ return new Symbol(sym.LITERAL, yytext()); }
					case -41:
						break;
					case 41:
						{ return new Symbol(sym.DATE_TIME, yytext()); }
					case -42:
						break;
					case 42:
						{ yybegin(commanddetail); break; }
					case -43:
						break;
					case 43:
						{ return new Symbol(sym.STAR); }
					case -44:
						break;
					case 44:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -45:
						break;
					case 45:
						{ return new Symbol(sym.COLON); }
					case -46:
						break;
					case 46:
						{ return new Symbol(sym.COMMA); }
					case -47:
						break;
					case 47:
						{ yybegin(commandfetch); break; }
					case -48:
						break;
					case 48:
						{ return new Symbol(sym.STAR); }
					case -49:
						break;
					case 49:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -50:
						break;
					case 50:
						{ return new Symbol(sym.COLON); }
					case -51:
						break;
					case 51:
						{ return new Symbol(sym.COMMA); }
					case -52:
						break;
					case 52:
						{ return new Symbol(sym.RBRACK); }
					case -53:
						break;
					case 53:
						{ return new Symbol(sym.SP); }
					case -54:
						break;
					case 54:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -55:
						break;
					case 55:
						{ return new Symbol(sym.LPARENT); }
					case -56:
						break;
					case 56:
						{ return new Symbol(sym.RPARENT); }
					case -57:
						break;
					case 57:
						{ return new Symbol(sym.NZ_NUMBER, yytext()); }
					case -58:
						break;
					case 58:
						{ return new Symbol(sym.DOT); }
					case -59:
						break;
					case 59:
						{ return new Symbol(sym.LBRACK); }
					case -60:
						break;
					case 60:
						{ return new Symbol(sym.LESSTHAN); }
					case -61:
						break;
					case 61:
						{ return new Symbol(sym.GREATERTHAN); }
					case -62:
						break;
					case 62:
						{ return new Symbol(sym.CRLF); }
					case -63:
						break;
					case 63:
						{ return new Symbol(sym.FETCH_ARG_PRIM, yytext()); }
					case -64:
						break;
					case 64:
						{ return new Symbol(sym.FETCH_ATT, yytext()); }
					case -65:
						break;
					case 65:
						{ return new Symbol(sym.FETCH_ATT_BODY, yytext()); }
					case -66:
						break;
					case 66:
						{ return new Symbol(sym.FETCH_MSG_TEXT_SINGLE, yytext()); }
					case -67:
						break;
					case 67:
						{ return new Symbol(sym.SECTION_TEXT_MIME); }
					case -68:
						break;
					case 68:
						{  return new Symbol(sym.FETCH_ATT_BODY_PEEK, yytext()); }
					case -69:
						break;
					case 69:
						{ yybegin(commandfetchheaderlist);return new Symbol(sym.FETCH_MSG_TEXT_HEADERLIST_KEY, yytext()); }
					case -70:
						break;
					case 70:
						{ return new Symbol(sym.ATOM, yytext()); }
					case -71:
						break;
					case 71:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -72:
						break;
					case 72:
						{ return new Symbol(sym.SP); }
					case -73:
						break;
					case 73:
						{ return new Symbol(sym.LPARENT); }
					case -74:
						break;
					case 74:
						{ yybegin(commandfetch); return new Symbol(sym.RPARENT); }
					case -75:
						break;
					case 75:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -76:
						break;
					case 76:
						{ return new Symbol(sym.LITERAL, yytext()); }
					case -77:
						break;
					case 77:
						{ yybegin(commandstoreflags); break; }
					case -78:
						break;
					case 78:
						{ return new Symbol(sym.STAR); }
					case -79:
						break;
					case 79:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -80:
						break;
					case 80:
						{ return new Symbol(sym.COLON); }
					case -81:
						break;
					case 81:
						{ return new Symbol(sym.COMMA); }
					case -82:
						break;
					case 82:
						{ return new Symbol(sym.SP); }
					case -83:
						break;
					case 83:
						{ return new Symbol(sym.FLAG_SIGN, yytext()); }
					case -84:
						break;
					case 84:
						{ yybegin(commanddetail); return new Symbol(sym.FLAG_KEY, yytext()); }
					case -85:
						break;
					case 85:
						{ return new Symbol(sym.ATOM, yytext()); }
					case -86:
						break;
					case 86:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -87:
						break;
					case 87:
						{ yybegin(commandstatuslist); return new Symbol(sym.SP); }
					case -88:
						break;
					case 88:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -89:
						break;
					case 89:
						{ return new Symbol(sym.LITERAL, yytext()); }
					case -90:
						break;
					case 90:
						{ return new Symbol(sym.SP); }
					case -91:
						break;
					case 91:
						{ return new Symbol(sym.LPARENT); }
					case -92:
						break;
					case 92:
						{ return new Symbol(sym.RPARENT); }
					case -93:
						break;
					case 93:
						{ return new Symbol(sym.CRLF); }
					case -94:
						break;
					case 94:
						{ return new Symbol(sym.STATUS_ATT, yytext()); }
					case -95:
						break;
					case 95:
						{ return new Symbol(sym.SP); }
					case -96:
						break;
					case 96:
						{ yybegin(commandsearchsequence); return new Symbol(sym.STAR); }
					case -97:
						break;
					case 97:
						{ yybegin(commandsearchsequence); return new Symbol(sym.NUMBER, yytext()); }
					case -98:
						break;
					case 98:
						{ return new Symbol(sym.SEARCH_KEYWORD_LPARENT, yytext()); }
					case -99:
						break;
					case 99:
						{ return new Symbol(sym.SEARCH_KEYWORD_RPARENT, yytext()); }
					case -100:
						break;
					case 100:
						{ yybegin(commandsearchastring); return new Symbol(sym.SEARCH_KEYWORD_ASTRING, yytext()); }
					case -101:
						break;
					case 101:
						{ return new Symbol(sym.SEARCH_KEYWORD_DATE, yytext()); }
					case -102:
						break;
					case 102:
						{ return new Symbol(sym.SEARCH_KEYWORD_OR, yytext()); }
					case -103:
						break;
					case 103:
						{ return new Symbol(sym.CRLF); }
					case -104:
						break;
					case 104:
						{ return new Symbol(sym.SEARCH_KEYWORD_SOLE, yytext()); }
					case -105:
						break;
					case 105:
						{ yybegin(commandsearchuidsequence); return new Symbol(sym.SEARCH_KEYWORD_UID, yytext()); }
					case -106:
						break;
					case 106:
						{ return new Symbol(sym.SEARCH_KEYWORD_NOT, yytext()); }
					case -107:
						break;
					case 107:
						{ yybegin(commandsearchnumber); return new Symbol(sym.SEARCH_KEYWORD_NUMBER, yytext()); }
					case -108:
						break;
					case 108:
						{ yybegin(commandsearchheader); return new Symbol(sym.SEARCH_KEYWORD_HEADER, yytext()); }
					case -109:
						break;
					case 109:
						{ yybegin(commandsearchastring); return new Symbol(sym.CHARSET, yytext()); }
					case -110:
						break;
					case 110:
						{ return new Symbol(sym.DATE, yytext()); }
					case -111:
						break;
					case 111:
						{ return new Symbol(sym.DATE, yytext().Substring(1, yytext().Length-2)); }
					case -112:
						break;
					case 112:
						{ yybegin(commandsearchastring); return new Symbol(sym.ASTRING, yytext()); }
					case -113:
						break;
					case 113:
						{ return new Symbol(sym.SP); }
					case -114:
						break;
					case 114:
						{ yybegin(commandsearchastring); return new Symbol(sym.QUOTED, yytext()); }
					case -115:
						break;
					case 115:
						{ yybegin(commandsearchastring); return new Symbol(sym.LITERAL, yytext()); }
					case -116:
						break;
					case 116:
						{ yybegin(commandsearch); return new Symbol(sym.ASTRING, yytext()); }
					case -117:
						break;
					case 117:
						{ return new Symbol(sym.SP); }
					case -118:
						break;
					case 118:
						{ yybegin(commandsearch); return new Symbol(sym.QUOTED, yytext()); }
					case -119:
						break;
					case 119:
						{ yybegin(commandsearch); return new Symbol(sym.LITERAL, yytext()); }
					case -120:
						break;
					case 120:
						{ return new Symbol(sym.SP); }
					case -121:
						break;
					case 121:
						{ yybegin(commandsearch); return new Symbol(sym.NUMBER, yytext()); }
					case -122:
						break;
					case 122:
						{ yybegin(commandsearch); break; }
					case -123:
						break;
					case 123:
						{ return new Symbol(sym.STAR); }
					case -124:
						break;
					case 124:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -125:
						break;
					case 125:
						{ return new Symbol(sym.COLON); }
					case -126:
						break;
					case 126:
						{ return new Symbol(sym.COMMA); }
					case -127:
						break;
					case 127:
						{ yybegin(commandsearchsequence); return new Symbol(sym.SP); }
					case -128:
						break;
					case 129:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -129:
						break;
					case 130:
						{ yybegin(commanddetail); break; }
					case -130:
						break;
					case 131:
						{ yybegin(commandfetch); break; }
					case -131:
						break;
					case 132:
						{ return new Symbol(sym.FETCH_ATT, yytext()); }
					case -132:
						break;
					case 133:
						{ return new Symbol(sym.FETCH_MSG_TEXT_SINGLE, yytext()); }
					case -133:
						break;
					case 134:
						{ yybegin(commandfetchheaderlist);return new Symbol(sym.FETCH_MSG_TEXT_HEADERLIST_KEY, yytext()); }
					case -134:
						break;
					case 135:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -135:
						break;
					case 136:
						{ yybegin(commandstoreflags); break; }
					case -136:
						break;
					case 137:
						{ yybegin(commanddetail); return new Symbol(sym.FLAG_KEY, yytext()); }
					case -137:
						break;
					case 138:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -138:
						break;
					case 139:
						{ yybegin(commandsearchsequence); return new Symbol(sym.NUMBER, yytext()); }
					case -139:
						break;
					case 140:
						{ yybegin(commandsearchastring); return new Symbol(sym.QUOTED, yytext()); }
					case -140:
						break;
					case 141:
						{ yybegin(commandsearch); return new Symbol(sym.QUOTED, yytext()); }
					case -141:
						break;
					case 142:
						{ yybegin(commandsearch); break; }
					case -142:
						break;
					case 144:
						{ yybegin(commandsearchsequence); return new Symbol(sym.NUMBER, yytext()); }
					case -143:
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
