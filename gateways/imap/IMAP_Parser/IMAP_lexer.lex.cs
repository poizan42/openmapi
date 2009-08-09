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
		127,
		230,
		42,
		47,
		260,
		325,
		77,
		337,
		347,
		352,
		373,
		476,
		480,
		484,
		121,
		490
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
		/* 127 */ YY_NOT_ACCEPT,
		/* 128 */ YY_NO_ANCHOR,
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
		/* 142 */ YY_NOT_ACCEPT,
		/* 143 */ YY_NO_ANCHOR,
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
		/* 681 */ YY_NOT_ACCEPT
	};
	private int[] yy_cmap = unpackFromString(1,65538,
"34,1:9,39,1:2,38,1:18,3,1,28,1:2,26,1:2,40,41,27,17,42,30,46,1,29,48,45,48:" +
"5,44,48,33,1,50,1,51,1:2,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12,6,1,20,22," +
"10,14,32,52,16,11,47,49,35,2,1,43,1,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12" +
",6,1,20,22,10,14,32,52,16,11,47,36,53,37,1:65410,0:2")[0];

	private int[] yy_rmap = unpackFromString(1,682,
"0,1,2,1,3,1:13,4,1:12,5,6,1,7,1:7,8,1,9,1:2,10,1,11,1:4,12,1:2,13,1:7,14,1:" +
"3,15,16,17,1:5,18,1,19,1:4,20,21,22,1:8,23,24,1:14,25,1:3,26,1:4,27,28,1,29" +
",1:3,30,31,1:2,32,33,1,34,1:2,35,36,37,38,1,39,40,41,42,43,44,45,3,46,47,48" +
",49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73" +
",74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98" +
",99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117" +
",118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,13" +
"6,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,1" +
"55,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173," +
"157,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191" +
",192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,21" +
"0,211,212,213,214,215,216,217,218,219,220,34,221,222,223,224,225,226,227,22" +
"8,229,230,231,232,233,234,235,236,237,238,239,240,241,35,242,243,244,245,24" +
"6,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,2" +
"65,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,282,283," +
"284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302" +
",303,304,305,306,307,308,309,310,311,312,313,314,315,316,317,318,319,320,32" +
"1,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,306,3" +
"39,340,341,342,343,344,345,346,347,348,349,350,351,352,353,354,355,356,357," +
"358,359,360,361,344,307,362,319,363,364,365,37,366,367,368,38,369,370,371,3" +
"72,373,374,375,376,377,378,379,380,381,382,383,384,385,386,387,388,389,390," +
"391,392,393,394,395,396,397,398,399,400,401,402,403,404,405,406,407,408,409" +
",410,411,412,413,414,415,416,417,418,419,420,421,422,423,424,425,426,427,42" +
"8,429,430,431,432,433,434,435,436,437,321,438,439,440,441,442,443,444,445,4" +
"46,447,448,449,450,451,452,453,454,455,456,457,458,459,460,461,462,463,464," +
"465,466,467,468,469,470,471,472,473,474,475,476,477,478,479,480,481,482,483" +
",484,485,486,487,488,489,490,491,492,493,494,495,496,497,498,499,500,501,50" +
"2,503,504,505,506,507,508,509,510,511,512,513,514,515,516,517,518,519,520,5" +
"21,522,523,524,525,526,527,528,529,530,531,532,533,534,535,536,537,538,539," +
"540,541,542,543,544,545,546,547,548,549,550,551,552,553,554,555,556,557,558" +
",559,560,561,562,563,564,565,566,31")[0];

	private int[][] yy_nxt = unpackFromString(567,54,
"1,2:2,3,2:13,3,2:8,3:3,2:5,3:3,2,-1:2,3:2,2:11,3,-1:55,2:2,-1,2:13,-1,2:8,-" +
"1:3,2:5,-1:3,2,-1:4,2:11,-1:2,4,-1:2,4:22,-1:3,4:5,-1:3,4,-1:4,4:11,-1:4,25" +
",-1:51,31,32,-1,31:22,34:2,-1,31:5,-1:3,31,-1:4,31:11,-1:2,32:2,-1,32:22,34" +
":2,-1,32:5,-1:3,32,-1:4,32:11,-1:2,34:2,-1,34:24,-1,34:5,-1:3,34,-1:4,34:11" +
",-1,1,-1:18,237,-1:7,43,-1,44,-1:3,45,-1:8,46,-1,44:2,-1:2,44,-1:34,44,-1:1" +
"4,44:2,-1:2,44,-1:5,1,-1:18,250,-1:7,48,-1,49,-1:3,50,-1:8,51,-1,49:2,-1:2," +
"49,-1:34,49,-1:14,49:2,-1:2,49,-1:34,54,-1:14,54:2,-1:2,54,-1:34,57,-1:14,5" +
"7:2,-1:2,57,-1:27,292,-1:23,293,-1:53,622,-1:8,70,71,-1,70:22,-1:3,70:5,-1:" +
"3,70,-1:4,70:11,-1:2,71:2,-1,71:22,-1:3,71:5,-1:3,71,-1:4,71:11,-1,1,-1:18," +
"612,-1:7,78,-1,79,-1:3,80,-1:8,81,-1,79:2,-1:2,79,-1:34,79,-1:14,79:2,-1:2," +
"79,-1:51,342,-1:8,85,86,-1,85:22,-1:3,85:5,-1:3,85,-1:4,85:11,-1:2,86:2,-1," +
"86:22,-1:3,86:5,-1:3,86,-1:4,86:11,-1:30,387,-1:14,387:2,-1:2,387,-1:34,138" +
",-1:14,138:2,-1:2,138,-1:6,111:2,-1,111:22,-1:3,111:5,-1:3,111,-1:4,111:11," +
"-1:2,115:2,-1,115:22,-1:3,115:5,-1:3,115,-1:4,115:11,-1:30,120,-1:14,120:2," +
"-1:2,120,-1:5,1,-1:18,623,-1:7,122,-1,123,-1:3,124,-1:8,125,-1,123:2,-1:2,1" +
"23,-1:34,123,-1:14,123:2,-1:2,123,-1:5,1,-1:2,142,-1:51,681:27,38,681:6,234" +
",681:2,-1:2,681:13,-1:47,304,-1:53,305,-1:8,326:27,75,326:6,328,326:2,-1:2," +
"326:13,-1:2,348:27,88,348:6,350,348:2,-1:2,348:13,-1:30,143,405,-1:13,143:2" +
",-1:2,143,-1:6,477:27,113,477:6,478,477:2,-1:2,477:13,-1:2,481:27,117,481:6" +
",482,481:2,-1:2,481:13,-1:5,144,145,-1:3,146,-1:4,147,148,149,-1,150,151,49" +
"2,-1,152,-1:2,536,-1:57,143,-1:14,143:2,-1:2,143,-1:10,153,-1:3,491,-1:2,49" +
"3,-1:7,554,-1:2,559,-1:36,537,-1:7,154,-1:47,155,-1:3,156,-1:9,157,-1:39,15" +
"8,-1:6,495,-1:50,535,-1:57,159,-1:55,160,-1:45,162,-1:3,163,-1:3,164,-1:41," +
"165,-1:57,169,-1:65,170,-1:44,171,-1:54,496,-1:58,5,-1:39,174,501,-1:56,499" +
",-1:59,539,-1:43,175,-1:6,176,-1:48,177,-1:51,502,-1:3,540,-1:49,178,-1:59," +
"180,-1:46,181,-1:67,182,-1:58,183,-1:40,184,-1:51,504,-1:3,541,-1:55,503,-1" +
":45,6,-1:68,186,-1:42,189,-1:9,507,-1:53,505,-1:55,190,-1:38,506,-1:64,7,-1" +
":38,8,-1:74,9,-1:44,192,-1:56,193,-1:38,10,-1:53,11,-1:58,510,-1:55,198,-1:" +
"64,512,-1:46,511,-1:43,201,-1:72,202,-1:49,205,-1:49,546,-1:41,12,-1:60,13," +
"-1:65,206,-1:44,207,-1:58,208,-1:45,514,-1:46,14,-1:70,211,-1:36,15,-1:59,5" +
"15,-1:47,16,-1:53,17,-1:54,214,-1:67,18,-1:38,19,-1:53,20,-1:53,21,-1:58,21" +
"7,-1:48,22,-1:53,23,-1:70,220,-1:36,24,-1:72,26,-1:38,221,-1:56,222,-1:47,2" +
"23,-1:57,224,-1:63,225,-1:46,27,-1:47,226,-1:55,227,-1:49,28,-1:60,516,-1:6" +
"1,228,-1:38,29,-1:53,30,-1:50,1,31,32,33,31:22,34:2,231,31:5,-1,35,232,31,2" +
"33,-1,36,37,31:11,-1:2,681:2,680,681:24,38,680,681:5,234,681:2,-1:2,681:4,6" +
"80:2,681:2,680,681:4,-1:30,235,-1:14,235:2,-1:2,235,-1:44,39,-1:15,681:27,1" +
"28,681:6,234,681:2,-1:2,681:13,-1:30,235,-1:7,40,-1:6,235:2,-1:2,235,-1:6,6" +
"81:27,41,681:6,234,681:2,-1:2,681:13,-1:15,238,-1:60,518,-1:43,240,-1:85,24" +
"1,-1:35,553,-1:48,243,-1:76,244,-1:25,558,-1:48,246,-1:66,247,-1:38,248,-1:" +
"60,249,-1:51,129,-1:54,548,-1:50,252,-1:85,610,-1:30,254,-1:76,611,-1:20,25" +
"6,-1:66,257,-1:38,258,-1:60,259,-1:51,130,-1:40,1,-1,52,53,-1,261,-1,569,26" +
"2,-1,263,-1:3,264,-1:3,519,-1,265,266,-1,520,-1,267,-1:3,54,-1:8,268,-1,55," +
"56,-1:2,57:2,58,-1,57,59,60,61,-1:11,269,-1:59,271,-1:56,272,-1:43,273,-1:7" +
"0,275,-1:36,560,-1:50,277,-1:3,521,-1:4,278,-1:78,62,-1:23,63,-1:63,279,-1:" +
"44,280,-1:59,281,-1:56,64,-1:66,522,-1:25,282,-1:54,284,-1:70,285,-1:42,65," +
"-1:60,287,-1:45,66,-1:87,289,-1:27,67,-1:54,290,-1:44,63,-1:56,291,-1:60,29" +
"4,-1:42,572,-1:89,295,-1:26,296,-1:57,64,-1:41,297,-1:49,298,-1:62,299,-1:8" +
"3,131,-1:28,132,-1:53,301,-1:51,523,-1:40,302,-1:54,303,-1:61,306,-1:48,308" +
",-1:62,64,-1:45,549,-1:11,309,556,-1:55,310,-1:32,311,-1:73,68,-1:48,312,-1" +
":42,314,-1:53,561,-1:55,315,-1:48,316,-1:64,317,-1:84,303,-1:20,320,-1:49,3" +
"03,-1:53,64,-1:62,565,-1:43,321,-1:64,303,-1:52,323,-1:54,64,-1:55,69,-1:41" +
",133,-1:43,1,70,71,72,70:22,-1:2,326,70:5,-1:2,327,70,-1:2,73,74,70:11,-1:3" +
"0,329,-1:14,329:2,-1:2,329,-1:6,326:27,134,326:6,328,326:2,-1:2,326:13,-1:3" +
"0,329,-1:7,76,-1:6,329:2,-1:2,329,-1:16,633,-1:62,634,-1:43,333,-1:66,334,-" +
"1:38,335,-1:60,336,-1:51,135,-1:40,1,-1:2,82,-1:13,83,-1:7,338,-1:4,83,-1:3" +
"2,339,-1:49,340,-1:61,341,-1:62,84,-1:53,343,-1:39,344,-1:54,568,-1:59,346," +
"-1:48,136,-1:43,1,85,86,87,85:22,-1:2,348,85:5,-1:2,349,85,-1:4,85:11,-1:30" +
",351,-1:14,351:2,-1:2,351,-1:6,348:27,137,348:6,350,348:2,-1:2,348:13,-1:30" +
",351,-1:7,89,-1:6,351:2,-1:2,351,-1:5,1,-1:2,90,-1:10,353,-1:5,571,573,-1:1" +
"6,354,-1,91,92,-1:20,355,-1:6,356,-1:77,93,-1:33,359,-1:56,575,-1:35,577,-1" +
":71,525,-1:46,579,-1:16,360,-1:26,363,-1:63,365,-1:54,365,-1:46,367,-1:59,9" +
"4,-1:48,94,-1:56,583,-1:48,368,-1:64,370,-1:56,94,-1:39,371,-1:55,372,-1:54" +
",94,-1:42,1,-1:2,95,374,375,-1,376,-1,377,378,-1,379,-1,380,381,-1:3,382,62" +
"0,-1,383,585,587,384,-1:2,385,96,-1:8,386,-1,97,98,-1:2,96:2,-1:2,96,-1:9,9" +
"9,-1:18,527,-1:39,388,-1:5,550,-1:42,389,-1:7,390,-1:5,627,-1:40,391,-1:60," +
"99,-1:5,392,-1:44,393,-1:5,100,-1:4,101,-1:41,394,-1:6,395,-1:50,396,-1:5,3" +
"97,-1:53,614,-1,398,-1:41,399,-1:5,400,-1:3,401,-1:2,528,-1:41,551,-1:10,58" +
"0,-1:36,404,-1:25,404,-1:14,404:2,-1:2,404,-1:44,102,-1:44,405,-1:32,103,-1" +
":48,99,-1:68,408,-1:54,617,-1:49,409,-1:56,103,-1:53,104,-1:39,410,-1:13,41" +
"1,-1:2,589,-1,591,412,-1:38,105,-1:95,103,-1:6,413,-1:63,414,-1:45,415,-1:6" +
"1,416,-1:2,417,-1:40,613,-1:59,419,-1:71,422,-1:14,422:2,-1:2,422,-1:10,423" +
",-1:6,529,-1:2,584,-1:3,596,-1,424,597,-1:2,598,-1:5,425,-1:42,615,-1:85,59" +
"9,-1:12,99,-1:52,99,-1:58,550,-1:56,614,-1,530,-1:42,428,-1:69,430,-1:32,43" +
"2,-1:80,602,-1:32,433,-1:58,103,-1:47,624,-1:96,586,-1:14,625,-1:61,99,-1:6" +
"2,434,-1:29,531,-1:7,435,-1:44,439,-1:53,442,-1:8,443,-1:59,432,-1:44,618,-" +
"1:47,420,-1:58,605,-1:53,103,-1:58,430,-1:56,100,-1:42,448,-1:4,449,-1:9,45" +
"0,-1:36,453,-1:6,532,-1:2,588,-1:3,606,-1,454,607,-1:2,608,-1:5,455,-1:35,4" +
"56,-1:50,456,-1:75,456,-1:25,456,-1:60,456,-1:8,456,-1:39,456,-1:54,456,-1:" +
"61,456,-1:47,456,-1:5,456,-1:58,605,-1:53,106,-1:37,409,-1:67,630,-1:50,100" +
",-1:46,458,-1:65,107,-1:53,459,-1:39,460,-1:7,461,-1:44,465,-1:53,533,-1:8," +
"468,-1:69,469,-1:33,108,-1:58,471,-1:57,99,-1:54,472,-1:46,472,-1:50,472,-1" +
":75,472,-1:25,472,-1:60,472,-1:8,472,-1:39,472,-1:54,472,-1:55,472,-1:5,472" +
",-1:67,619,-1:14,619:2,-1:2,619,-1:35,626,-1:52,109,-1:14,109:2,-1:2,109,-1" +
":33,110,-1:25,1,111:2,112,111:22,-1:2,477,111:5,-1:2,557,111,-1:4,111:11,-1" +
":2,477:27,139,477:6,478,477:2,-1:2,477:13,-1:30,479,-1:7,114,-1:6,479:2,-1:" +
"2,479,-1:5,1,115:2,116,115:22,-1:2,481,115:5,-1:2,562,115,-1:4,115:11,-1:2," +
"481:27,140,481:6,482,481:2,-1:2,481:13,-1:30,483,-1:7,118,-1:6,483:2,-1:2,4" +
"83,-1:5,1,-1:2,119,-1:25,120,-1:14,120:2,-1:2,120,-1:15,486,-1:66,487,-1:38" +
",488,-1:60,489,-1:51,141,-1:40,1,-1:2,126,-1:62,538,-1:59,161,-1:41,166,-1:" +
"57,498,-1:65,172,-1:38,185,-1:51,500,-1:52,191,-1:67,187,-1:45,543,-1:57,54" +
"2,-1:59,508,-1:40,196,-1:61,194,-1:56,200,-1:43,203,-1:55,199,-1:47,509,-1:" +
"72,212,-1:45,545,-1:60,210,-1:49,209,-1:45,213,-1:52,216,-1:52,218,-1:63,22" +
"9,-1:36,681:27,38,236,681:5,234,681:2,-1:2,681:4,236:2,681:2,236,681:4,-1:2" +
"2,239,-1:47,274,-1:56,276,-1:40,286,-1:66,288,-1:53,307,-1:40,318,-1:70,526" +
",-1:36,366,-1:53,406,-1:53,418,-1:52,436,-1:54,446,-1:68,456,-1:37,462,-1:6" +
"4,472,-1:67,474,-1:14,474:2,-1:2,474,-1:17,173,-1:59,494,-1:41,168,-1:69,17" +
"9,-1:36,188,-1:66,544,-1:49,195,-1:54,197,-1:56,204,-1:39,513,-1:67,215,-1:" +
"45,547,-1:51,219,-1:66,555,-1:50,313,-1:57,407,-1:60,475,-1:14,475:2,-1:2,4" +
"75,-1:17,242,-1:59,497,-1:56,251,-1:50,524,-1:64,479,-1:14,479:2,-1:2,479,-" +
"1:17,245,-1:59,167,-1:56,283,-1:50,319,-1:64,483,-1:14,483:2,-1:2,483,-1:17" +
",253,-1:62,567,-1:50,322,-1:47,255,-1:62,330,-1:50,345,-1:47,270,-1:62,644," +
"-1:50,357,-1:47,300,-1:59,358,-1:47,324,-1:59,581,-1:47,331,-1:59,361,-1:47" +
",332,-1:59,362,-1:47,421,-1:59,364,-1:47,426,-1:59,369,-1:47,437,-1:59,402," +
"-1:47,452,-1:59,403,-1:47,463,-1:59,601,-1:47,470,-1:59,427,-1:47,473,-1:59" +
",429,-1:47,485,-1:59,431,-1:53,438,-1:53,440,-1:53,441,-1:53,444,-1:53,445," +
"-1:53,417,-1:53,447,-1:53,451,-1:53,457,-1:53,393,-1:53,464,-1:53,466,-1:53" +
",467,-1:36,681:27,38,517,681:5,234,681:2,-1:2,681:4,517:2,681:2,517,681:4,-" +
"1:26,563,-1:43,566,-1:52,564,-1:58,603,-1:43,593,-1:66,604,-1:35,595,-1:62," +
"600,-1:92,590,-1:30,534,-1:14,534:2,-1:2,534,-1:23,616,-1:60,576,-1:43,574," +
"-1:52,570,-1:48,600,-1:57,605,-1:69,635,-1:14,635:2,-1:2,635,-1:30,582,-1:4" +
"3,578,-1:67,552,-1:14,552:2,-1:2,552,-1:30,592,-1:43,594,-1:39,681:27,38,60" +
"9,681:5,234,681:2,-1:2,681:4,609:2,681:2,609,681:4,-1:44,621,-1:53,628,-1:3" +
"9,629,-1:14,629:2,-1:2,629,-1:48,631,-1:11,681:27,38,632,681:5,234,681:2,-1" +
":2,681:4,632:2,681:2,632,681:4,-1:21,636,-1:45,638,-1:66,639,-1:71,640,-1:1" +
"1,681:16,637,681:10,38,681,637,681:4,234,681:2,-1:2,681:13,-1:12,641,-1:63," +
"643,-1:33,681:2,642,681:24,38,681:6,234,681:2,-1:2,681:13,-1:2,681:27,38,64" +
"5,681:5,234,681:2,-1:2,681:4,645:2,681:2,645,681:4,-1:2,681:27,38,646,681:5" +
",234,681:2,-1:2,681:4,646:2,681:2,646,681:4,-1:2,681:27,38,681:4,647,681,23" +
"4,681:2,-1:2,681:13,-1:2,681:27,38,648,681:5,234,681:2,-1:2,681:4,648:2,681" +
":2,648,681:4,-1:2,681:27,38,649,681:5,234,681:2,-1:2,681:4,649:2,681:2,649," +
"681:4,-1:2,681:27,38,681:4,650,681,234,681:2,-1:2,681:13,-1:2,681:27,38,651" +
",681:5,234,681:2,-1:2,681:4,651:2,681:2,651,681:4,-1:2,681:27,38,652,681:5," +
"234,681:2,-1:2,681:4,652:2,681:2,652,681:4,-1:2,681:2,653,681:24,38,681:6,2" +
"34,681:2,-1:2,681:13,-1:2,681:27,38,654,681:5,234,681:2,-1:2,681:4,654:2,68" +
"1:2,654,681:4,-1:2,681:27,38,655,681:5,234,681:2,-1:2,681:4,655:2,681:2,655" +
",681:4,-1:2,681:27,38,656,681:5,234,681:2,-1:2,681:4,656:2,681:2,656,681:4," +
"-1:2,681:27,38,657,681:5,234,681:2,-1:2,681:4,657:2,681:2,657,681:4,-1:2,68" +
"1:27,38,681,658,681:4,234,681:2,-1:2,681:13,-1:2,681:19,659,681:7,38,681:6," +
"234,681:2,-1:2,681:13,-1:2,681:12,659,681:14,38,681:6,234,681:2,-1:2,681:13" +
",-1:2,681:9,659,681:17,38,681:6,234,681:2,-1:2,681:13,-1:2,681:27,38,681:3," +
"659,681:2,234,681:2,-1:2,681:13,-1:2,681:3,659,681:23,38,681:6,234,681:2,-1" +
":2,681:13,-1:2,681:10,659,681:8,659,681:7,38,681:6,234,681:2,-1:2,681:13,-1" +
":2,681:5,659,681:21,38,681:6,234,681:2,-1:2,681:13,-1:2,681:6,659,681:20,38" +
",681:6,234,681:2,-1:2,681:13,-1:2,681:14,659,681:12,38,681:6,234,681:2,-1:2" +
",681:13,-1:2,681:8,659,681:5,659,681:12,38,681:6,234,681:2,-1:2,681:13,-1:2" +
",681:5,660,681:7,661,681:13,38,681:6,234,681:2,-1:2,681:13,-1:2,681:3,662,6" +
"81:23,38,681:6,234,681:2,-1:2,681:13,-1:2,681:11,663,681:15,38,681:6,234,68" +
"1:2,-1:2,681:13,-1:2,681:17,664,681:9,38,681:6,234,681:2,-1:2,681:13,-1:2,6" +
"81:4,665,681:22,38,681:6,234,681:2,-1:2,681:13,-1:2,681:17,666,681:9,38,681" +
":6,234,681:2,-1:2,681:13,-1:2,681:17,667,681:9,38,681:6,234,681:2,-1:2,681:" +
"13,-1:2,681:4,668,681:8,669,681:13,38,681:6,234,681:2,-1:2,681:13,-1:2,681:" +
"4,670,681:6,671,681:2,672,681:3,673,681,674,675,681:2,676,681:2,38,681:2,67" +
"7,681:3,234,681:2,-1:2,681:13,-1:2,681:27,38,681,678,681:4,234,681:2,-1:2,6" +
"81:13,-1:2,681:27,38,679,681:5,234,681:2,-1:2,681:4,679:2,681:2,679,681:4,-" +
"1");

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
						{ yybegin(commandsearchsequence); return new Symbol(sym.NUMBER, yytext()); }
					case -97:
						break;
					case 97:
						{ return new Symbol(sym.SEARCH_KEYWORD_LPARENT, yytext()); }
					case -98:
						break;
					case 98:
						{ return new Symbol(sym.SEARCH_KEYWORD_RPARENT, yytext()); }
					case -99:
						break;
					case 99:
						{ yybegin(commandsearchastring); return new Symbol(sym.SEARCH_KEYWORD_ASTRING, yytext()); }
					case -100:
						break;
					case 100:
						{ return new Symbol(sym.SEARCH_KEYWORD_DATE, yytext()); }
					case -101:
						break;
					case 101:
						{ return new Symbol(sym.SEARCH_KEYWORD_OR, yytext()); }
					case -102:
						break;
					case 102:
						{ return new Symbol(sym.CRLF); }
					case -103:
						break;
					case 103:
						{ return new Symbol(sym.SEARCH_KEYWORD_SOLE, yytext()); }
					case -104:
						break;
					case 104:
						{ yybegin(commandsearchuidsequence); return new Symbol(sym.SEARCH_KEYWORD_UID, yytext()); }
					case -105:
						break;
					case 105:
						{ return new Symbol(sym.SEARCH_KEYWORD_NOT, yytext()); }
					case -106:
						break;
					case 106:
						{ yybegin(commandsearchnumber); return new Symbol(sym.SEARCH_KEYWORD_NUMBER, yytext()); }
					case -107:
						break;
					case 107:
						{ yybegin(commandsearchheader); return new Symbol(sym.SEARCH_KEYWORD_HEADER, yytext()); }
					case -108:
						break;
					case 108:
						{ yybegin(commandsearchastring); return new Symbol(sym.CHARSET, yytext()); }
					case -109:
						break;
					case 109:
						{ return new Symbol(sym.DATE, yytext()); }
					case -110:
						break;
					case 110:
						{ return new Symbol(sym.DATE, yytext().Substring(1, yytext().Length-2)); }
					case -111:
						break;
					case 111:
						{ yybegin(commandsearchastring); return new Symbol(sym.ASTRING, yytext()); }
					case -112:
						break;
					case 112:
						{ return new Symbol(sym.SP); }
					case -113:
						break;
					case 113:
						{ yybegin(commandsearchastring); return new Symbol(sym.QUOTED, yytext()); }
					case -114:
						break;
					case 114:
						{ yybegin(commandsearchastring); return new Symbol(sym.LITERAL, yytext()); }
					case -115:
						break;
					case 115:
						{ yybegin(commandsearch); return new Symbol(sym.ASTRING, yytext()); }
					case -116:
						break;
					case 116:
						{ return new Symbol(sym.SP); }
					case -117:
						break;
					case 117:
						{ yybegin(commandsearch); return new Symbol(sym.QUOTED, yytext()); }
					case -118:
						break;
					case 118:
						{ yybegin(commandsearch); return new Symbol(sym.LITERAL, yytext()); }
					case -119:
						break;
					case 119:
						{ return new Symbol(sym.SP); }
					case -120:
						break;
					case 120:
						{ yybegin(commandsearch); return new Symbol(sym.NUMBER, yytext()); }
					case -121:
						break;
					case 121:
						{ yybegin(commandsearch); break; }
					case -122:
						break;
					case 122:
						{ return new Symbol(sym.STAR); }
					case -123:
						break;
					case 123:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -124:
						break;
					case 124:
						{ return new Symbol(sym.COLON); }
					case -125:
						break;
					case 125:
						{ return new Symbol(sym.COMMA); }
					case -126:
						break;
					case 126:
						{ yybegin(commandsearchsequence); return new Symbol(sym.SP); }
					case -127:
						break;
					case 128:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -128:
						break;
					case 129:
						{ yybegin(commanddetail); break; }
					case -129:
						break;
					case 130:
						{ yybegin(commandfetch); break; }
					case -130:
						break;
					case 131:
						{ return new Symbol(sym.FETCH_ATT, yytext()); }
					case -131:
						break;
					case 132:
						{ return new Symbol(sym.FETCH_MSG_TEXT_SINGLE, yytext()); }
					case -132:
						break;
					case 133:
						{ yybegin(commandfetchheaderlist);return new Symbol(sym.FETCH_MSG_TEXT_HEADERLIST_KEY, yytext()); }
					case -133:
						break;
					case 134:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -134:
						break;
					case 135:
						{ yybegin(commandstoreflags); break; }
					case -135:
						break;
					case 136:
						{ yybegin(commanddetail); return new Symbol(sym.FLAG_KEY, yytext()); }
					case -136:
						break;
					case 137:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -137:
						break;
					case 138:
						{ yybegin(commandsearchsequence); return new Symbol(sym.NUMBER, yytext()); }
					case -138:
						break;
					case 139:
						{ yybegin(commandsearchastring); return new Symbol(sym.QUOTED, yytext()); }
					case -139:
						break;
					case 140:
						{ yybegin(commandsearch); return new Symbol(sym.QUOTED, yytext()); }
					case -140:
						break;
					case 141:
						{ yybegin(commandsearch); break; }
					case -141:
						break;
					case 143:
						{ yybegin(commandsearchsequence); return new Symbol(sym.NUMBER, yytext()); }
					case -142:
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
