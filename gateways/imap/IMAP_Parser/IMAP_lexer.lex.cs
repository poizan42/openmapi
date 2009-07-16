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
		116,
		235,
		41,
		46,
		265,
		330,
		76,
		342,
		352,
		357,
		378,
		404
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
		/* 116 */ YY_NOT_ACCEPT,
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
		/* 128 */ YY_NO_ANCHOR,
		/* 129 */ YY_NO_ANCHOR,
		/* 130 */ YY_NOT_ACCEPT,
		/* 131 */ YY_NO_ANCHOR,
		/* 132 */ YY_NOT_ACCEPT,
		/* 133 */ YY_NO_ANCHOR,
		/* 134 */ YY_NOT_ACCEPT,
		/* 135 */ YY_NO_ANCHOR,
		/* 136 */ YY_NOT_ACCEPT,
		/* 137 */ YY_NO_ANCHOR,
		/* 138 */ YY_NOT_ACCEPT,
		/* 139 */ YY_NO_ANCHOR,
		/* 140 */ YY_NOT_ACCEPT,
		/* 141 */ YY_NO_ANCHOR,
		/* 142 */ YY_NOT_ACCEPT,
		/* 143 */ YY_NO_ANCHOR,
		/* 144 */ YY_NOT_ACCEPT,
		/* 145 */ YY_NO_ANCHOR,
		/* 146 */ YY_NOT_ACCEPT,
		/* 147 */ YY_NO_ANCHOR,
		/* 148 */ YY_NOT_ACCEPT,
		/* 149 */ YY_NO_ANCHOR,
		/* 150 */ YY_NOT_ACCEPT,
		/* 151 */ YY_NO_ANCHOR,
		/* 152 */ YY_NOT_ACCEPT,
		/* 153 */ YY_NO_ANCHOR,
		/* 154 */ YY_NOT_ACCEPT,
		/* 155 */ YY_NO_ANCHOR,
		/* 156 */ YY_NOT_ACCEPT,
		/* 157 */ YY_NO_ANCHOR,
		/* 158 */ YY_NOT_ACCEPT,
		/* 159 */ YY_NO_ANCHOR,
		/* 160 */ YY_NOT_ACCEPT,
		/* 161 */ YY_NO_ANCHOR,
		/* 162 */ YY_NOT_ACCEPT,
		/* 163 */ YY_NO_ANCHOR,
		/* 164 */ YY_NOT_ACCEPT,
		/* 165 */ YY_NO_ANCHOR,
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
		/* 408 */ YY_NO_ANCHOR,
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
		/* 447 */ YY_NO_ANCHOR,
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
		/* 464 */ YY_NO_ANCHOR,
		/* 465 */ YY_NOT_ACCEPT,
		/* 466 */ YY_NOT_ACCEPT,
		/* 467 */ YY_NOT_ACCEPT,
		/* 468 */ YY_NOT_ACCEPT,
		/* 469 */ YY_NO_ANCHOR,
		/* 470 */ YY_NOT_ACCEPT,
		/* 471 */ YY_NOT_ACCEPT,
		/* 472 */ YY_NOT_ACCEPT,
		/* 473 */ YY_NOT_ACCEPT,
		/* 474 */ YY_NO_ANCHOR,
		/* 475 */ YY_NOT_ACCEPT,
		/* 476 */ YY_NOT_ACCEPT,
		/* 477 */ YY_NOT_ACCEPT,
		/* 478 */ YY_NO_ANCHOR,
		/* 479 */ YY_NOT_ACCEPT,
		/* 480 */ YY_NOT_ACCEPT,
		/* 481 */ YY_NOT_ACCEPT,
		/* 482 */ YY_NO_ANCHOR,
		/* 483 */ YY_NOT_ACCEPT,
		/* 484 */ YY_NOT_ACCEPT,
		/* 485 */ YY_NO_ANCHOR,
		/* 486 */ YY_NOT_ACCEPT,
		/* 487 */ YY_NOT_ACCEPT,
		/* 488 */ YY_NO_ANCHOR,
		/* 489 */ YY_NOT_ACCEPT,
		/* 490 */ YY_NOT_ACCEPT,
		/* 491 */ YY_NO_ANCHOR,
		/* 492 */ YY_NOT_ACCEPT,
		/* 493 */ YY_NOT_ACCEPT,
		/* 494 */ YY_NO_ANCHOR,
		/* 495 */ YY_NOT_ACCEPT,
		/* 496 */ YY_NOT_ACCEPT,
		/* 497 */ YY_NO_ANCHOR,
		/* 498 */ YY_NOT_ACCEPT,
		/* 499 */ YY_NOT_ACCEPT,
		/* 500 */ YY_NO_ANCHOR,
		/* 501 */ YY_NOT_ACCEPT,
		/* 502 */ YY_NO_ANCHOR,
		/* 503 */ YY_NOT_ACCEPT,
		/* 504 */ YY_NO_ANCHOR,
		/* 505 */ YY_NOT_ACCEPT,
		/* 506 */ YY_NO_ANCHOR,
		/* 507 */ YY_NOT_ACCEPT,
		/* 508 */ YY_NO_ANCHOR,
		/* 509 */ YY_NO_ANCHOR,
		/* 510 */ YY_NO_ANCHOR,
		/* 511 */ YY_NO_ANCHOR,
		/* 512 */ YY_NOT_ACCEPT,
		/* 513 */ YY_NOT_ACCEPT,
		/* 514 */ YY_NOT_ACCEPT,
		/* 515 */ YY_NOT_ACCEPT,
		/* 516 */ YY_NOT_ACCEPT,
		/* 517 */ YY_NOT_ACCEPT,
		/* 518 */ YY_NO_ANCHOR,
		/* 519 */ YY_NO_ANCHOR,
		/* 520 */ YY_NOT_ACCEPT,
		/* 521 */ YY_NOT_ACCEPT,
		/* 522 */ YY_NO_ANCHOR,
		/* 523 */ YY_NOT_ACCEPT,
		/* 524 */ YY_NO_ANCHOR,
		/* 525 */ YY_NO_ANCHOR,
		/* 526 */ YY_NO_ANCHOR,
		/* 527 */ YY_NO_ANCHOR,
		/* 528 */ YY_NO_ANCHOR,
		/* 529 */ YY_NO_ANCHOR,
		/* 530 */ YY_NO_ANCHOR,
		/* 531 */ YY_NO_ANCHOR,
		/* 532 */ YY_NO_ANCHOR,
		/* 533 */ YY_NO_ANCHOR,
		/* 534 */ YY_NO_ANCHOR,
		/* 535 */ YY_NO_ANCHOR,
		/* 536 */ YY_NO_ANCHOR,
		/* 537 */ YY_NO_ANCHOR,
		/* 538 */ YY_NO_ANCHOR,
		/* 539 */ YY_NO_ANCHOR,
		/* 540 */ YY_NOT_ACCEPT,
		/* 541 */ YY_NOT_ACCEPT,
		/* 542 */ YY_NOT_ACCEPT,
		/* 543 */ YY_NOT_ACCEPT,
		/* 544 */ YY_NO_ANCHOR,
		/* 545 */ YY_NO_ANCHOR,
		/* 546 */ YY_NO_ANCHOR,
		/* 547 */ YY_NO_ANCHOR,
		/* 548 */ YY_NO_ANCHOR,
		/* 549 */ YY_NO_ANCHOR,
		/* 550 */ YY_NO_ANCHOR,
		/* 551 */ YY_NO_ANCHOR,
		/* 552 */ YY_NO_ANCHOR,
		/* 553 */ YY_NO_ANCHOR,
		/* 554 */ YY_NO_ANCHOR,
		/* 555 */ YY_NO_ANCHOR,
		/* 556 */ YY_NO_ANCHOR,
		/* 557 */ YY_NO_ANCHOR,
		/* 558 */ YY_NO_ANCHOR,
		/* 559 */ YY_NO_ANCHOR,
		/* 560 */ YY_NOT_ACCEPT,
		/* 561 */ YY_NOT_ACCEPT,
		/* 562 */ YY_NO_ANCHOR,
		/* 563 */ YY_NO_ANCHOR,
		/* 564 */ YY_NO_ANCHOR,
		/* 565 */ YY_NO_ANCHOR,
		/* 566 */ YY_NO_ANCHOR,
		/* 567 */ YY_NO_ANCHOR,
		/* 568 */ YY_NO_ANCHOR,
		/* 569 */ YY_NO_ANCHOR,
		/* 570 */ YY_NO_ANCHOR,
		/* 571 */ YY_NO_ANCHOR,
		/* 572 */ YY_NO_ANCHOR,
		/* 573 */ YY_NO_ANCHOR,
		/* 574 */ YY_NO_ANCHOR,
		/* 575 */ YY_NOT_ACCEPT,
		/* 576 */ YY_NOT_ACCEPT,
		/* 577 */ YY_NO_ANCHOR,
		/* 578 */ YY_NO_ANCHOR,
		/* 579 */ YY_NO_ANCHOR,
		/* 580 */ YY_NO_ANCHOR,
		/* 581 */ YY_NO_ANCHOR,
		/* 582 */ YY_NO_ANCHOR,
		/* 583 */ YY_NO_ANCHOR,
		/* 584 */ YY_NO_ANCHOR,
		/* 585 */ YY_NO_ANCHOR,
		/* 586 */ YY_NO_ANCHOR,
		/* 587 */ YY_NO_ANCHOR,
		/* 588 */ YY_NO_ANCHOR,
		/* 589 */ YY_NO_ANCHOR,
		/* 590 */ YY_NOT_ACCEPT,
		/* 591 */ YY_NOT_ACCEPT,
		/* 592 */ YY_NO_ANCHOR,
		/* 593 */ YY_NOT_ACCEPT,
		/* 594 */ YY_NO_ANCHOR,
		/* 595 */ YY_NOT_ACCEPT,
		/* 596 */ YY_NO_ANCHOR,
		/* 597 */ YY_NOT_ACCEPT,
		/* 598 */ YY_NO_ANCHOR,
		/* 599 */ YY_NOT_ACCEPT,
		/* 600 */ YY_NO_ANCHOR,
		/* 601 */ YY_NOT_ACCEPT,
		/* 602 */ YY_NO_ANCHOR,
		/* 603 */ YY_NOT_ACCEPT,
		/* 604 */ YY_NO_ANCHOR,
		/* 605 */ YY_NOT_ACCEPT,
		/* 606 */ YY_NO_ANCHOR,
		/* 607 */ YY_NOT_ACCEPT,
		/* 608 */ YY_NOT_ACCEPT,
		/* 609 */ YY_NO_ANCHOR,
		/* 610 */ YY_NO_ANCHOR,
		/* 611 */ YY_NOT_ACCEPT,
		/* 612 */ YY_NOT_ACCEPT,
		/* 613 */ YY_NO_ANCHOR,
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
		/* 659 */ YY_NOT_ACCEPT
	};
	private int[] yy_cmap = unpackFromString(1,65538,
"34,1:9,39,1:2,38,1:18,3,1,28,1:2,26,1:2,40,41,27,17,42,30,46,1,29,48,45,48:" +
"5,44,48,33,1,50,1,51,1:2,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12,6,1,20,22," +
"10,14,32,52,16,11,47,49,35,2,1,43,1,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12" +
",6,1,20,22,10,14,32,52,16,11,47,36,53,37,1:65410,0:2")[0];

	private int[] yy_rmap = unpackFromString(1,660,
"0,1,2,1,3,1:25,4,5,1,6,1:7,7,1,8,1:2,9,1,10,1:4,11,1:2,12,1:7,13,1:3,14,15," +
"16,1:5,17,1,18,1:4,19,20,21,1:8,22,23,1,24,22,1:2,22:4,1:2,22:2,1,22:3,1:3," +
"25,26,1:2,27,28,1,29,1:2,30,31,32,22,33,34,35,36,37,38,39,40,41,42,43,44,3," +
"45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69," +
"70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94," +
"95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114," +
"115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133" +
",134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,15" +
"2,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,1" +
"71,172,173,174,175,176,177,178,179,180,181,182,183,184,168,185,186,187,188," +
"189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207" +
",208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,22" +
"6,227,228,229,230,231,29,232,233,234,235,236,237,238,239,240,241,242,243,24" +
"4,245,246,247,248,249,250,251,252,30,253,254,255,256,257,258,259,260,261,26" +
"2,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,2" +
"81,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299," +
"300,301,302,303,304,305,306,307,308,309,310,311,312,313,314,315,316,317,318" +
",319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,33" +
"7,338,339,340,341,342,343,344,345,346,347,348,349,350,351,352,353,354,355,3" +
"56,357,358,359,360,361,362,363,364,365,366,367,368,369,370,371,372,373,374," +
"375,376,377,378,379,380,381,382,383,384,385,386,387,388,389,390,391,392,393" +
",394,395,396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,41" +
"2,413,414,415,416,417,418,419,420,421,422,423,424,425,426,427,428,429,430,4" +
"31,432,433,434,435,436,437,438,439,440,441,442,443,444,445,446,447,448,449," +
"450,451,452,453,454,455,456,457,458,459,460,461,462,463,464,465,466,467,468" +
",469,470,471,472,473,474,475,476,477,478,479,480,481,482,483,484,485,486,48" +
"7,488,489,490,491,492,493,494,495,496,497,498,499,500,501,502,503,504,505,5" +
"06,507,508,509,510,511,512,513,514,515,516,517,518,519,520,521,522,523,524," +
"525,526,32,527,528,529,530,531,532,533,534,535,536,537,538,539,540,541,542," +
"543,544,545,546,547,548,549,550,551,552,553,554,555,556,26")[0];

	private int[][] yy_nxt = unpackFromString(557,54,
"1,2:2,3,2:13,3,2:8,3:3,2:5,3:3,2,-1:2,3:2,2:11,3,-1:55,2:2,-1,2:13,-1,2:8,-" +
"1:3,2:5,-1:3,2,-1:4,2:11,-1:2,4,-1:2,4:22,-1:3,4:5,-1:3,4,-1:4,4:11,-1:2,30" +
",31,-1,30:22,33:2,-1,30:5,-1:3,30,-1:4,30:11,-1:2,31:2,-1,31:22,33:2,-1,31:" +
"5,-1:3,31,-1:4,31:11,-1:2,33:2,-1,33:24,-1,33:5,-1:3,33,-1:4,33:11,-1,1,-1:" +
"18,242,-1:7,42,-1,43,-1:3,44,-1:8,45,-1,43:2,-1:2,43,-1:34,43,-1:14,43:2,-1" +
":2,43,-1:5,1,-1:18,255,-1:7,47,-1,48,-1:3,49,-1:8,50,-1,48:2,-1:2,48,-1:34," +
"48,-1:14,48:2,-1:2,48,-1:34,53,-1:14,53:2,-1:2,53,-1:34,56,-1:14,56:2,-1:2," +
"56,-1:27,297,-1:23,298,-1:53,521,-1:8,69,70,-1,69:22,-1:3,69:5,-1:3,69,-1:4" +
",69:11,-1:2,70:2,-1,70:22,-1:3,70:5,-1:3,70,-1:4,70:11,-1,1,-1:18,515,-1:7," +
"77,-1,78,-1:3,79,-1:8,80,-1,78:2,-1:2,78,-1:34,78,-1:14,78:2,-1:2,78,-1:51," +
"347,-1:8,84,85,-1,84:22,-1:3,84:5,-1:3,84,-1:4,84:11,-1:2,85:2,-1,85:22,-1:" +
"3,85:5,-1:3,85,-1:4,85:11,-1:2,94:2,-1,94:22,-1:3,94:5,-1:3,94,-1:4,94:11,-" +
"1:30,382,-1:14,382:2,-1:2,382,-1:6,94:2,-1,94:22,-1:3,610,94:4,-1:3,94,-1:4" +
",94:2,610:2,94:2,610,94:4,-1,1,-1:2,130,-1:51,659:27,37,659:6,239,659:2,-1:" +
"2,659:13,-1:47,309,-1:53,310,-1:8,331:27,74,331:6,333,331:2,-1:2,331:13,-1:" +
"2,353:27,87,353:6,355,353:2,-1:2,353:13,-1:2,94:2,-1,102,94:18,565,94:2,-1:" +
"3,94:5,-1:3,94,-1:4,94:11,-1:2,628:27,105,628:6,383,628:2,-1:2,628:13,-1:5," +
"132,134,-1:3,136,-1:4,138,140,142,-1,144,146,410,-1,148,-1:2,449,-1:29,94:2" +
",-1,94:8,102,94:5,474,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:6,150,-1:3,409,-" +
"1:2,411,-1:7,466,-1:2,471,-1:31,94:2,-1,94:5,137,94:5,103,94:4,104,94:5,-1:" +
"3,94:5,-1:3,94,-1:4,94:11,-1:7,450,-1:7,152,-1:40,94:2,-1,94:5,107,94:16,-1" +
":3,94:5,-1:3,94,-1:4,94:11,-1:9,154,-1:3,156,-1:9,158,-1:32,94:2,-1,94:15,1" +
"07,94:6,-1:3,94:5,-1:3,94,-1:4,94:11,-1:9,160,-1:6,413,-1:39,94:2,-1,94:6,1" +
"08,94:15,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,448,-1:42,94:2,-1,94:22,-1:3,94" +
":5,-1:3,94,-1:4,94:10,107,-1:2,94:2,-1,94:7,102,94:14,-1:3,94:5,-1:3,94,-1:" +
"4,94:11,-1:17,162,-1:38,94:2,-1,94:6,102,94:15,-1:3,94:5,-1:3,94,-1:4,94:11" +
",-1:19,164,-1:36,94:2,-1,94:11,107,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:11" +
",167,-1:3,168,-1:3,169,-1:36,94:2,-1,94:17,102,94:4,-1:3,94:5,-1:3,94,-1:4," +
"94:11,-1:7,170,-1:48,94:2,-1,94:6,107,94:15,-1:3,94:5,-1:3,94,-1:4,94:11,-1" +
":11,174,-1:44,94:2,-1,94:14,103,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:23,175" +
",-1:32,94:2,-1,94:16,110,94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:14,176,-1:41," +
"94:2,-1,94:11,103,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:15,414,-1:40,94:2,-" +
"1,94:16,111,94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:20,5,-1:35,94:2,-1,94:6,11" +
"2,94:15,-1:3,94:5,-1:3,94,-1:4,94:11,-1:6,179,419,-1:48,94:2,-1,94:15,102,9" +
"4:6,-1:3,94:5,-1:3,94,-1:4,94:11,-1:10,417,-1:45,94:2,-1,94:22,-1:3,129,94:" +
"4,-1:3,94,-1:4,94:2,129:2,94:2,129,94:4,-1:16,452,-1:43,180,-1:6,181,-1:48," +
"182,-1:51,420,-1:3,453,-1:49,183,-1:59,185,-1:46,186,-1:67,187,-1:58,188,-1" +
":40,189,-1:51,422,-1:3,454,-1:55,421,-1:45,6,-1:68,191,-1:42,194,-1:9,425,-" +
"1:53,423,-1:55,195,-1:38,424,-1:64,7,-1:38,8,-1:74,9,-1:44,197,-1:56,198,-1" +
":38,10,-1:53,11,-1:58,428,-1:55,203,-1:64,430,-1:46,429,-1:43,206,-1:72,207" +
",-1:49,210,-1:49,459,-1:41,12,-1:60,13,-1:65,211,-1:44,212,-1:58,213,-1:45," +
"432,-1:46,14,-1:70,216,-1:36,15,-1:59,433,-1:47,16,-1:53,17,-1:54,219,-1:67" +
",18,-1:38,19,-1:53,20,-1:53,21,-1:58,222,-1:48,22,-1:53,23,-1:70,225,-1:36," +
"24,-1:72,25,-1:38,226,-1:56,227,-1:47,228,-1:57,229,-1:63,230,-1:46,26,-1:4" +
"7,231,-1:55,232,-1:49,27,-1:60,434,-1:61,233,-1:38,28,-1:53,29,-1:50,1,30,3" +
"1,32,30:22,33:2,236,30:5,-1,34,237,30,238,-1,35,36,30:11,-1:2,659:2,658,659" +
":24,37,658,659:5,239,659:2,-1:2,659:4,658:2,659:2,658,659:4,-1:30,240,-1:14" +
",240:2,-1:2,240,-1:44,38,-1:15,659:27,117,659:6,239,659:2,-1:2,659:13,-1:30" +
",240,-1:7,39,-1:6,240:2,-1:2,240,-1:6,659:27,40,659:6,239,659:2,-1:2,659:13" +
",-1:15,243,-1:60,436,-1:43,245,-1:85,246,-1:35,465,-1:48,248,-1:76,249,-1:2" +
"5,470,-1:48,251,-1:66,252,-1:38,253,-1:60,254,-1:51,118,-1:54,461,-1:50,257" +
",-1:85,513,-1:30,259,-1:76,514,-1:20,261,-1:66,262,-1:38,263,-1:60,264,-1:5" +
"1,119,-1:40,1,-1,51,52,-1,266,-1,483,267,-1,268,-1:3,269,-1:3,437,-1,270,27" +
"1,-1,438,-1,272,-1:3,53,-1:8,273,-1,54,55,-1:2,56:2,57,-1,56,58,59,60,-1:11" +
",274,-1:59,276,-1:56,277,-1:43,278,-1:70,280,-1:36,472,-1:50,282,-1:3,439,-" +
"1:4,283,-1:78,61,-1:23,62,-1:63,284,-1:44,285,-1:59,286,-1:56,63,-1:66,440," +
"-1:25,287,-1:54,289,-1:70,290,-1:42,64,-1:60,292,-1:45,65,-1:87,294,-1:27,6" +
"6,-1:54,295,-1:44,62,-1:56,296,-1:60,299,-1:42,486,-1:89,300,-1:26,301,-1:5" +
"7,63,-1:41,302,-1:49,303,-1:62,304,-1:83,120,-1:28,121,-1:53,306,-1:51,441," +
"-1:40,307,-1:54,308,-1:61,311,-1:48,313,-1:62,63,-1:45,462,-1:11,314,468,-1" +
":55,315,-1:32,316,-1:73,67,-1:48,317,-1:42,319,-1:53,473,-1:55,320,-1:48,32" +
"1,-1:64,322,-1:84,308,-1:20,325,-1:49,308,-1:53,63,-1:62,477,-1:43,326,-1:6" +
"4,308,-1:52,328,-1:54,63,-1:55,68,-1:41,122,-1:43,1,69,70,71,69:22,-1:2,331" +
",69:5,-1:2,332,69,-1:2,72,73,69:11,-1:30,334,-1:14,334:2,-1:2,334,-1:6,331:" +
"27,123,331:6,333,331:2,-1:2,331:13,-1:30,334,-1:7,75,-1:6,334:2,-1:2,334,-1" +
":16,541,-1:62,542,-1:43,338,-1:66,339,-1:38,340,-1:60,341,-1:51,124,-1:40,1" +
",-1:2,81,-1:13,82,-1:7,343,-1:4,82,-1:32,344,-1:49,345,-1:61,346,-1:62,83,-" +
"1:53,348,-1:39,349,-1:54,481,-1:59,351,-1:48,125,-1:43,1,84,85,86,84:22,-1:" +
"2,353,84:5,-1:2,354,84,-1:4,84:11,-1:30,356,-1:14,356:2,-1:2,356,-1:6,353:2" +
"7,126,353:6,355,353:2,-1:2,353:13,-1:30,356,-1:7,88,-1:6,356:2,-1:2,356,-1:" +
"5,1,-1:2,89,-1:10,358,-1:5,484,487,-1:16,359,-1,90,91,-1:20,360,-1:6,361,-1" +
":77,92,-1:33,364,-1:56,490,-1:35,493,-1:71,443,-1:46,496,-1:16,365,-1:26,36" +
"8,-1:63,370,-1:54,370,-1:46,372,-1:59,93,-1:48,93,-1:56,501,-1:48,373,-1:64" +
",375,-1:56,93,-1:39,376,-1:55,377,-1:54,93,-1:42,1,94:2,95,127,408,94,447,9" +
"4,558,131,94,133,94,562,464,94:3,539,563,94,511,564,574,519,-1,96,379,97,94" +
":3,98,-1:2,380,94,381,-1,99,100,101,94,97:2,94:2,97,94:4,-1:2,628:2,626,628" +
":24,105,626,628:5,383,628:2,-1:2,628:4,626:2,628:2,626,628:4,-1:30,384,-1:1" +
"4,384:2,-1:2,384,-1:44,106,-1:44,385,-1:24,628:27,128,628:6,383,628:2,-1:2," +
"628:13,-1:30,384,-1:7,109,-1:6,384:2,-1:2,384,-1:10,386,-1:6,387,-1:2,498,-" +
"1:3,503,-1,388,505,-1:2,507,-1:5,389,-1:28,390,-1:7,391,-1:43,392,-1:54,395" +
",-1:53,398,-1:8,399,-1:59,400,-1:46,400,-1:50,400,-1:75,400,-1:25,400,-1:60" +
",400,-1:8,400,-1:39,400,-1:54,400,-1:61,400,-1:47,400,-1:5,400,-1:68,401,-1" +
":52,517,-1:14,517:2,-1:2,517,-1:34,113,-1:14,113:2,-1:2,113,-1:6,628:27,114" +
",628:6,383,628:2,-1:2,628:13,-1,1,-1:13,405,-1:47,406,-1:64,463,-1:40,115,-" +
"1:48,94:2,-1,94:5,135,94:5,577,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,451" +
",-1:59,166,-1:41,171,-1:57,416,-1:65,177,-1:38,190,-1:51,418,-1:52,196,-1:6" +
"7,192,-1:45,456,-1:57,455,-1:59,426,-1:40,201,-1:61,199,-1:56,205,-1:43,208" +
",-1:55,204,-1:47,427,-1:72,217,-1:45,458,-1:60,215,-1:49,214,-1:45,218,-1:5" +
"2,221,-1:52,223,-1:63,234,-1:36,659:27,37,241,659:5,239,659:2,-1:2,659:4,24" +
"1:2,659:2,241,659:4,-1:22,244,-1:47,279,-1:56,281,-1:40,291,-1:66,293,-1:53" +
",312,-1:40,323,-1:70,444,-1:36,371,-1:49,628:27,105,403,628:5,383,628:2,-1:" +
"2,628:4,403:2,628:2,403,628:4,-1:30,402,-1:14,402:2,-1:2,402,-1:6,94:2,-1,5" +
"59,94:7,469,94:5,544,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,178,-1:59,412," +
"-1:41,173,-1:69,184,-1:36,193,-1:66,457,-1:49,200,-1:54,202,-1:56,209,-1:39" +
",431,-1:67,220,-1:45,460,-1:51,224,-1:66,467,-1:50,318,-1:57,407,-1:32,94:2" +
",-1,94:8,139,94:5,141,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,247,-1:59,415" +
",-1:56,256,-1:50,442,-1:36,94:2,-1,94:15,143,94:6,-1:3,94:5,-1:3,94,-1:4,94" +
":11,-1:13,250,-1:59,172,-1:56,288,-1:50,324,-1:36,94:2,-1,94:12,145,94:9,-1" +
":3,94:5,-1:3,94,-1:4,94:11,-1:13,258,-1:62,480,-1:50,327,-1:36,94:2,-1,94:1" +
"1,529,94:2,147,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,260,-1:62,335,-1:50," +
"350,-1:36,94:2,-1,94:8,149,94:13,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,275,-1:" +
"59,362,-1:36,94:2,-1,94:21,151,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,305,-1:59" +
",363,-1:36,94:2,-1,153,94:21,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,329,-1:59,4" +
"99,-1:36,94:2,-1,94:16,153,94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,336,-1:5" +
"9,366,-1:36,94:2,-1,94:14,155,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,337,-" +
"1:59,367,-1:36,94:2,-1,94:11,151,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,3" +
"93,-1:59,369,-1:36,94:2,-1,94:3,572,94:4,157,94:9,556,94:3,-1:3,94:5,-1:3,9" +
"4,-1:4,94:11,-1:19,374,-1:36,94:2,-1,94:14,159,94:7,-1:3,94:5,-1:3,94,-1:4," +
"94:11,-1:19,394,-1:36,94:2,-1,94:14,161,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-" +
"1:19,396,-1:36,94:2,-1,94:14,137,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:19,39" +
"7,-1:36,94:2,-1,145,94:21,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:16,1" +
"63,94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,165,94:4,-1:3," +
"94,-1:4,94:2,165:2,94:2,165,94:4,-1:2,94:2,-1,94:4,524,94:5,567,94:3,478,94" +
":2,568,94:4,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,659:27,37,435,659:5,239,659:2" +
",-1:2,659:4,435:2,659:2,435,659:4,-1:26,475,-1:43,479,-1:52,476,-1:40,628:2" +
"7,105,445,628:5,383,628:2,-1:2,628:4,445:2,628:2,445,628:4,-1:30,446,-1:14," +
"446:2,-1:2,446,-1:6,94:2,-1,94:14,147,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:" +
"2,94:2,-1,94:5,570,94:10,482,94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:26,492,-1" +
":43,489,-1:39,94:2,-1,94,485,94:20,-1:3,94:5,-1:3,94,-1:4,94:11,-1:16,495,-" +
"1:39,94:2,-1,94:11,488,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:8" +
",491,94:13,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:9,494,94:12,-1:3,94" +
":5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:14,518,94:7,-1:3,94:5,-1:3,94,-1:4,94" +
":11,-1:2,94:2,-1,94:14,497,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,9" +
"4:6,500,94:15,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:15,502,94:6,-1:3" +
",94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:18,504,94:3,-1:3,94:5,-1:3,94,-1:4" +
",94:11,-1:2,94:2,-1,94:6,506,94:15,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-" +
"1,94:14,508,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:5,494,94:16,-" +
"1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:8,509,94:13,-1:3,94:5,-1:3,94,-" +
"1:4,94:11,-1:2,94:2,-1,94:9,506,94:12,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:" +
"2,-1,94:16,506,94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,51" +
"0,94:4,-1:3,94,-1:4,94:2,510:2,94:2,510,94:4,-1:2,94:2,-1,94:14,566,94,522," +
"94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,659:27,37,512,659:5,239,659:2,-1:2,6" +
"59:4,512:2,659:2,512,659:4,-1:44,520,-1:53,523,-1:11,628:27,105,516,628:5,3" +
"83,628:2,-1:2,628:4,516:2,628:2,516,628:4,-1:2,94:2,-1,94:21,525,-1:3,94:5," +
"-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:16,526,94:5,-1:3,94:5,-1:3,94,-1:4,94:11" +
",-1:2,94:2,-1,94,589,94:13,539,94:2,527,94,574,578,-1:3,94:5,-1:3,94,-1:4,9" +
"4:11,-1:2,94:2,-1,528,94:21,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94,53" +
"0,94:20,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:16,531,94:5,-1:3,94:5," +
"-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:14,532,94:7,-1:3,94:5,-1:3,94,-1:4,94:11" +
",-1:2,94:2,-1,94:22,-1:3,94:2,533,94:2,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:5" +
",534,94:16,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,94:5,-1:3,9" +
"4,-1:4,94:10,535,-1:2,94:2,-1,94:9,536,94:12,-1:3,94:5,-1:3,94,-1:4,94:11,-" +
"1:2,94:2,-1,94:14,537,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:4,5" +
"24,94:17,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,538,94:4,-1:3" +
",94,-1:4,94:2,538:2,94:2,538,94:4,-1:2,94:2,-1,94,545,94:20,-1:3,94:5,-1:3," +
"94,-1:4,94:11,-1:2,94:2,-1,102,94:21,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,659:" +
"27,37,540,659:5,239,659:2,-1:2,659:4,540:2,659:2,540,659:4,-1:2,628:27,105," +
"543,628:5,383,628:2,-1:2,628:4,543:2,628:2,543,628:4,-1:2,94:2,-1,94:11,546" +
",94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:14,547,94:7,-1:3,94:5,-" +
"1:3,94,-1:4,94:11,-1:2,94:2,-1,94:14,548,94:7,-1:3,94:5,-1:3,94,-1:4,94:11," +
"-1:2,94:2,-1,94,549,94:20,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:5,55" +
"0,94:16,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:3,551,94:18,-1:3,94:5," +
"-1:3,94,-1:4,94:11,-1:2,94:2,-1,94,552,94:20,-1:3,94:5,-1:3,94,-1:4,94:11,-" +
"1:2,94:2,-1,94:7,553,94:14,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94,554" +
",94:20,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,94:5,-1:3,94,-1" +
":4,94:10,555,-1:2,94:2,-1,94:14,544,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2," +
"94:2,-1,94:22,-1:3,94,557,94:3,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:14,569,94" +
":7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,659:16,560,659:10,37,659,560,659:4,239" +
",659:2,-1:2,659:13,-1:2,628:27,105,628,561,628:4,383,628:2,-1:2,628:13,-1:2" +
",94:2,-1,94:18,571,94:3,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:5,570," +
"94:16,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:16,573,94:5,-1:3,94:5,-1" +
":3,94,-1:4,94:11,-1:2,94:2,-1,94:9,573,94:12,-1:3,94:5,-1:3,94,-1:4,94:11,-" +
"1:2,94:2,-1,94:6,573,94:15,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22," +
"-1:3,94:3,573,94,-1:3,94,-1:4,94:11,-1:2,94:2,-1,573,94:21,-1:3,94:5,-1:3,9" +
"4,-1:4,94:11,-1:2,94:2,-1,94:7,573,94:8,573,94:5,-1:3,94:5,-1:3,94,-1:4,94:" +
"11,-1:2,94:2,-1,94:2,573,94:19,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94" +
":3,573,94:18,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:11,573,94:10,-1:3" +
",94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:5,573,94:5,573,94:10,-1:3,94:5,-1:" +
"3,94,-1:4,94:11,-1:2,94:2,-1,94:11,577,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-" +
"1:2,659:2,575,659:24,37,659:6,239,659:2,-1:2,659:13,-1:2,628:19,576,628:7,1" +
"05,628:6,383,628:2,-1:2,628:13,-1:2,94:2,-1,94:2,579,94:7,580,94:11,-1:3,94" +
":5,-1:3,94,-1:4,94:11,-1:2,628:12,576,628:14,105,628:6,383,628:2,-1:2,628:1" +
"3,-1:2,94:2,-1,581,94:21,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,628:9,576,628:17" +
",105,628:6,383,628:2,-1:2,628:13,-1:2,94:2,-1,94:8,582,94:13,-1:3,94:5,-1:3" +
",94,-1:4,94:11,-1:2,628:27,105,628:3,576,628:2,383,628:2,-1:2,628:13,-1:2,9" +
"4:2,-1,94:14,583,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,628:3,576,628:23,10" +
"5,628:6,383,628:2,-1:2,628:13,-1:2,94:2,-1,94,584,94:20,-1:3,94:5,-1:3,94,-" +
"1:4,94:11,-1:2,628:10,576,628:8,576,628:7,105,628:6,383,628:2,-1:2,628:13,-" +
"1:2,94:2,-1,94:14,585,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,628:5,576,628:" +
"21,105,628:6,383,628:2,-1:2,628:13,-1:2,94:2,-1,94:14,586,94:7,-1:3,94:5,-1" +
":3,94,-1:4,94:11,-1:2,628:6,576,628:20,105,628:6,383,628:2,-1:2,628:13,-1:2" +
",94:2,-1,94,587,94:8,588,94:11,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,628:14,576" +
",628:12,105,628:6,383,628:2,-1:2,628:13,-1:2,628:8,576,628:5,576,628:12,105" +
",628:6,383,628:2,-1:2,628:13,-1:2,94:2,-1,94,592,94:6,594,94:2,596,94:3,598" +
",94,600,602,94:2,604,-1:3,94:2,606,94:2,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:" +
"22,-1:3,613,609,94:3,-1:3,94,-1:4,94:2,613:2,94:2,613,94:4,-1:2,659:27,37,5" +
"90,659:5,239,659:2,-1:2,659:4,590:2,659:2,590,659:4,-1:2,628:5,591,628:7,59" +
"3,628:13,105,628:6,383,628:2,-1:2,628:13,-1:2,94:2,-1,94:22,-1:3,613,94:4,-" +
"1:3,94,-1:4,94:2,613:2,94:2,613,94:4,-1:2,628:3,595,628:23,105,628:6,383,62" +
"8:2,-1:2,628:13,-1:2,628:11,597,628:15,105,628:6,383,628:2,-1:2,628:13,-1:2" +
",628:17,599,628:9,105,628:6,383,628:2,-1:2,628:13,-1:2,628:4,601,628:22,105" +
",628:6,383,628:2,-1:2,628:13,-1:2,628:17,603,628:9,105,628:6,383,628:2,-1:2" +
",628:13,-1:2,628:17,605,628:9,105,628:6,383,628:2,-1:2,628:13,-1:2,628:4,60" +
"7,628:8,608,628:13,105,628:6,383,628:2,-1:2,628:13,-1:2,659:27,37,611,659:5" +
",239,659:2,-1:2,659:4,611:2,659:2,611,659:4,-1:2,628:4,612,628:6,614,628:2," +
"615,628:3,616,628,617,618,628:2,619,628:2,105,628:2,620,628:3,383,628:2,-1:" +
"2,628:13,-1:2,659:27,37,659:4,621,659,239,659:2,-1:2,659:13,-1:2,628:27,105" +
",628,622,628:4,383,628:2,-1:2,628:13,-1:2,659:27,37,623,659:5,239,659:2,-1:" +
"2,659:4,623:2,659:2,623,659:4,-1:2,628:27,105,624,628:5,383,628:2,-1:2,628:" +
"4,624:2,628:2,624,628:4,-1:2,659:27,37,625,659:5,239,659:2,-1:2,659:4,625:2" +
",659:2,625,659:4,-1:2,659:27,37,659:4,627,659,239,659:2,-1:2,659:13,-1:2,65" +
"9:27,37,629,659:5,239,659:2,-1:2,659:4,629:2,659:2,629,659:4,-1:2,659:27,37" +
",630,659:5,239,659:2,-1:2,659:4,630:2,659:2,630,659:4,-1:2,659:2,631,659:24" +
",37,659:6,239,659:2,-1:2,659:13,-1:2,659:27,37,632,659:5,239,659:2,-1:2,659" +
":4,632:2,659:2,632,659:4,-1:2,659:27,37,633,659:5,239,659:2,-1:2,659:4,633:" +
"2,659:2,633,659:4,-1:2,659:27,37,634,659:5,239,659:2,-1:2,659:4,634:2,659:2" +
",634,659:4,-1:2,659:27,37,635,659:5,239,659:2,-1:2,659:4,635:2,659:2,635,65" +
"9:4,-1:2,659:27,37,659,636,659:4,239,659:2,-1:2,659:13,-1:2,659:19,637,659:" +
"7,37,659:6,239,659:2,-1:2,659:13,-1:2,659:12,637,659:14,37,659:6,239,659:2," +
"-1:2,659:13,-1:2,659:9,637,659:17,37,659:6,239,659:2,-1:2,659:13,-1:2,659:2" +
"7,37,659:3,637,659:2,239,659:2,-1:2,659:13,-1:2,659:3,637,659:23,37,659:6,2" +
"39,659:2,-1:2,659:13,-1:2,659:10,637,659:8,637,659:7,37,659:6,239,659:2,-1:" +
"2,659:13,-1:2,659:5,637,659:21,37,659:6,239,659:2,-1:2,659:13,-1:2,659:6,63" +
"7,659:20,37,659:6,239,659:2,-1:2,659:13,-1:2,659:14,637,659:12,37,659:6,239" +
",659:2,-1:2,659:13,-1:2,659:8,637,659:5,637,659:12,37,659:6,239,659:2,-1:2," +
"659:13,-1:2,659:5,638,659:7,639,659:13,37,659:6,239,659:2,-1:2,659:13,-1:2," +
"659:3,640,659:23,37,659:6,239,659:2,-1:2,659:13,-1:2,659:11,641,659:15,37,6" +
"59:6,239,659:2,-1:2,659:13,-1:2,659:17,642,659:9,37,659:6,239,659:2,-1:2,65" +
"9:13,-1:2,659:4,643,659:22,37,659:6,239,659:2,-1:2,659:13,-1:2,659:17,644,6" +
"59:9,37,659:6,239,659:2,-1:2,659:13,-1:2,659:17,645,659:9,37,659:6,239,659:" +
"2,-1:2,659:13,-1:2,659:4,646,659:8,647,659:13,37,659:6,239,659:2,-1:2,659:1" +
"3,-1:2,659:4,648,659:6,649,659:2,650,659:3,651,659,652,653,659:2,654,659:2," +
"37,659:2,655,659:3,239,659:2,-1:2,659:13,-1:2,659:27,37,659,656,659:4,239,6" +
"59:2,-1:2,659:13,-1:2,659:27,37,657,659:5,239,659:2,-1:2,659:4,657:2,659:2," +
"657,659:4,-1");

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
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -95:
						break;
					case 95:
						{ return new Symbol(sym.SP); }
					case -96:
						break;
					case 96:
						{ return new Symbol(sym.STAR); }
					case -97:
						break;
					case 97:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -98:
						break;
					case 98:
						{ return new Symbol(sym.COLON); }
					case -99:
						break;
					case 99:
						{ return new Symbol(sym.SEARCH_KEYWORD_LPARENT, yytext()); }
					case -100:
						break;
					case 100:
						{ return new Symbol(sym.SEARCH_KEYWORD_RPARENT, yytext()); }
					case -101:
						break;
					case 101:
						{ return new Symbol(sym.COMMA); }
					case -102:
						break;
					case 102:
						{ return new Symbol(sym.SEARCH_KEYWORD_ASTRING, yytext()); }
					case -103:
						break;
					case 103:
						{ return new Symbol(sym.SEARCH_KEYWORD_DATE, yytext()); }
					case -104:
						break;
					case 104:
						{ return new Symbol(sym.SEARCH_KEYWORD_OR, yytext()); }
					case -105:
						break;
					case 105:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -106:
						break;
					case 106:
						{ return new Symbol(sym.CRLF); }
					case -107:
						break;
					case 107:
						{ return new Symbol(sym.SEARCH_KEYWORD_SOLE, yytext()); }
					case -108:
						break;
					case 108:
						{ return new Symbol(sym.SEARCH_KEYWORD_NOT, yytext()); }
					case -109:
						break;
					case 109:
						{ return new Symbol(sym.LITERAL, yytext()); }
					case -110:
						break;
					case 110:
						{ return new Symbol(sym.SEARCH_KEYWORD_NUMBER, yytext()); }
					case -111:
						break;
					case 111:
						{ return new Symbol(sym.SEARCH_KEYWORD_HEADER, yytext()); }
					case -112:
						break;
					case 112:
						{ return new Symbol(sym.CHARSET, yytext()); }
					case -113:
						break;
					case 113:
						{ return new Symbol(sym.DATE, yytext()); }
					case -114:
						break;
					case 114:
						{ return new Symbol(sym.DATE, yytext().Substring(1, yytext().Length-2)); }
					case -115:
						break;
					case 115:
						{ return new Symbol(sym.SEARCH_KEYWORD_UID, yytext().Substring(1, yytext().Length-1)); }
					case -116:
						break;
					case 117:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -117:
						break;
					case 118:
						{ yybegin(commanddetail); break; }
					case -118:
						break;
					case 119:
						{ yybegin(commandfetch); break; }
					case -119:
						break;
					case 120:
						{ return new Symbol(sym.FETCH_ATT, yytext()); }
					case -120:
						break;
					case 121:
						{ return new Symbol(sym.FETCH_MSG_TEXT_SINGLE, yytext()); }
					case -121:
						break;
					case 122:
						{ yybegin(commandfetchheaderlist);return new Symbol(sym.FETCH_MSG_TEXT_HEADERLIST_KEY, yytext()); }
					case -122:
						break;
					case 123:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -123:
						break;
					case 124:
						{ yybegin(commandstoreflags); break; }
					case -124:
						break;
					case 125:
						{ yybegin(commanddetail); return new Symbol(sym.FLAG_KEY, yytext()); }
					case -125:
						break;
					case 126:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -126:
						break;
					case 127:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -127:
						break;
					case 128:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -128:
						break;
					case 129:
						{ return new Symbol(sym.DATE, yytext()); }
					case -129:
						break;
					case 131:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -130:
						break;
					case 133:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -131:
						break;
					case 135:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -132:
						break;
					case 137:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -133:
						break;
					case 139:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -134:
						break;
					case 141:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -135:
						break;
					case 143:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -136:
						break;
					case 145:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -137:
						break;
					case 147:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -138:
						break;
					case 149:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -139:
						break;
					case 151:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -140:
						break;
					case 153:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -141:
						break;
					case 155:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -142:
						break;
					case 157:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -143:
						break;
					case 159:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -144:
						break;
					case 161:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -145:
						break;
					case 163:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -146:
						break;
					case 165:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -147:
						break;
					case 408:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -148:
						break;
					case 447:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -149:
						break;
					case 464:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -150:
						break;
					case 469:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -151:
						break;
					case 474:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -152:
						break;
					case 478:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -153:
						break;
					case 482:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -154:
						break;
					case 485:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -155:
						break;
					case 488:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -156:
						break;
					case 491:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -157:
						break;
					case 494:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -158:
						break;
					case 497:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -159:
						break;
					case 500:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -160:
						break;
					case 502:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -161:
						break;
					case 504:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -162:
						break;
					case 506:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -163:
						break;
					case 508:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -164:
						break;
					case 509:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -165:
						break;
					case 510:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -166:
						break;
					case 511:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -167:
						break;
					case 518:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -168:
						break;
					case 519:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -169:
						break;
					case 522:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -170:
						break;
					case 524:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -171:
						break;
					case 525:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -172:
						break;
					case 526:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -173:
						break;
					case 527:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -174:
						break;
					case 528:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -175:
						break;
					case 529:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -176:
						break;
					case 530:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -177:
						break;
					case 531:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -178:
						break;
					case 532:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -179:
						break;
					case 533:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -180:
						break;
					case 534:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -181:
						break;
					case 535:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -182:
						break;
					case 536:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -183:
						break;
					case 537:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -184:
						break;
					case 538:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -185:
						break;
					case 539:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -186:
						break;
					case 544:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -187:
						break;
					case 545:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -188:
						break;
					case 546:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -189:
						break;
					case 547:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -190:
						break;
					case 548:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -191:
						break;
					case 549:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -192:
						break;
					case 550:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -193:
						break;
					case 551:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -194:
						break;
					case 552:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -195:
						break;
					case 553:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -196:
						break;
					case 554:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -197:
						break;
					case 555:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -198:
						break;
					case 556:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -199:
						break;
					case 557:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -200:
						break;
					case 558:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -201:
						break;
					case 559:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -202:
						break;
					case 562:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -203:
						break;
					case 563:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -204:
						break;
					case 564:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -205:
						break;
					case 565:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -206:
						break;
					case 566:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -207:
						break;
					case 567:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -208:
						break;
					case 568:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -209:
						break;
					case 569:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -210:
						break;
					case 570:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -211:
						break;
					case 571:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -212:
						break;
					case 572:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -213:
						break;
					case 573:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -214:
						break;
					case 574:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -215:
						break;
					case 577:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -216:
						break;
					case 578:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -217:
						break;
					case 579:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -218:
						break;
					case 580:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -219:
						break;
					case 581:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -220:
						break;
					case 582:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -221:
						break;
					case 583:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -222:
						break;
					case 584:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -223:
						break;
					case 585:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -224:
						break;
					case 586:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -225:
						break;
					case 587:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -226:
						break;
					case 588:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -227:
						break;
					case 589:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -228:
						break;
					case 592:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -229:
						break;
					case 594:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -230:
						break;
					case 596:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -231:
						break;
					case 598:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -232:
						break;
					case 600:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -233:
						break;
					case 602:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -234:
						break;
					case 604:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -235:
						break;
					case 606:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -236:
						break;
					case 609:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -237:
						break;
					case 610:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -238:
						break;
					case 613:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -239:
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
