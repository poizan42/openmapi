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
		114,
		232,
		41,
		46,
		262,
		327,
		76,
		339,
		349,
		354,
		375,
		109
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
		/* 114 */ YY_NOT_ACCEPT,
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
		/* 128 */ YY_NO_ANCHOR,
		/* 129 */ YY_NOT_ACCEPT,
		/* 130 */ YY_NO_ANCHOR,
		/* 131 */ YY_NOT_ACCEPT,
		/* 132 */ YY_NO_ANCHOR,
		/* 133 */ YY_NOT_ACCEPT,
		/* 134 */ YY_NO_ANCHOR,
		/* 135 */ YY_NOT_ACCEPT,
		/* 136 */ YY_NO_ANCHOR,
		/* 137 */ YY_NOT_ACCEPT,
		/* 138 */ YY_NO_ANCHOR,
		/* 139 */ YY_NOT_ACCEPT,
		/* 140 */ YY_NO_ANCHOR,
		/* 141 */ YY_NOT_ACCEPT,
		/* 142 */ YY_NO_ANCHOR,
		/* 143 */ YY_NOT_ACCEPT,
		/* 144 */ YY_NO_ANCHOR,
		/* 145 */ YY_NOT_ACCEPT,
		/* 146 */ YY_NO_ANCHOR,
		/* 147 */ YY_NOT_ACCEPT,
		/* 148 */ YY_NO_ANCHOR,
		/* 149 */ YY_NOT_ACCEPT,
		/* 150 */ YY_NO_ANCHOR,
		/* 151 */ YY_NOT_ACCEPT,
		/* 152 */ YY_NO_ANCHOR,
		/* 153 */ YY_NOT_ACCEPT,
		/* 154 */ YY_NO_ANCHOR,
		/* 155 */ YY_NOT_ACCEPT,
		/* 156 */ YY_NO_ANCHOR,
		/* 157 */ YY_NOT_ACCEPT,
		/* 158 */ YY_NO_ANCHOR,
		/* 159 */ YY_NOT_ACCEPT,
		/* 160 */ YY_NO_ANCHOR,
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
		/* 406 */ YY_NO_ANCHOR,
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
		/* 445 */ YY_NO_ANCHOR,
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
		/* 461 */ YY_NO_ANCHOR,
		/* 462 */ YY_NOT_ACCEPT,
		/* 463 */ YY_NOT_ACCEPT,
		/* 464 */ YY_NOT_ACCEPT,
		/* 465 */ YY_NOT_ACCEPT,
		/* 466 */ YY_NO_ANCHOR,
		/* 467 */ YY_NOT_ACCEPT,
		/* 468 */ YY_NOT_ACCEPT,
		/* 469 */ YY_NOT_ACCEPT,
		/* 470 */ YY_NOT_ACCEPT,
		/* 471 */ YY_NO_ANCHOR,
		/* 472 */ YY_NOT_ACCEPT,
		/* 473 */ YY_NOT_ACCEPT,
		/* 474 */ YY_NOT_ACCEPT,
		/* 475 */ YY_NO_ANCHOR,
		/* 476 */ YY_NOT_ACCEPT,
		/* 477 */ YY_NOT_ACCEPT,
		/* 478 */ YY_NOT_ACCEPT,
		/* 479 */ YY_NO_ANCHOR,
		/* 480 */ YY_NOT_ACCEPT,
		/* 481 */ YY_NOT_ACCEPT,
		/* 482 */ YY_NOT_ACCEPT,
		/* 483 */ YY_NO_ANCHOR,
		/* 484 */ YY_NOT_ACCEPT,
		/* 485 */ YY_NOT_ACCEPT,
		/* 486 */ YY_NO_ANCHOR,
		/* 487 */ YY_NOT_ACCEPT,
		/* 488 */ YY_NOT_ACCEPT,
		/* 489 */ YY_NO_ANCHOR,
		/* 490 */ YY_NOT_ACCEPT,
		/* 491 */ YY_NOT_ACCEPT,
		/* 492 */ YY_NO_ANCHOR,
		/* 493 */ YY_NOT_ACCEPT,
		/* 494 */ YY_NOT_ACCEPT,
		/* 495 */ YY_NO_ANCHOR,
		/* 496 */ YY_NOT_ACCEPT,
		/* 497 */ YY_NOT_ACCEPT,
		/* 498 */ YY_NO_ANCHOR,
		/* 499 */ YY_NOT_ACCEPT,
		/* 500 */ YY_NOT_ACCEPT,
		/* 501 */ YY_NO_ANCHOR,
		/* 502 */ YY_NOT_ACCEPT,
		/* 503 */ YY_NO_ANCHOR,
		/* 504 */ YY_NOT_ACCEPT,
		/* 505 */ YY_NO_ANCHOR,
		/* 506 */ YY_NOT_ACCEPT,
		/* 507 */ YY_NO_ANCHOR,
		/* 508 */ YY_NO_ANCHOR,
		/* 509 */ YY_NO_ANCHOR,
		/* 510 */ YY_NOT_ACCEPT,
		/* 511 */ YY_NOT_ACCEPT,
		/* 512 */ YY_NOT_ACCEPT,
		/* 513 */ YY_NOT_ACCEPT,
		/* 514 */ YY_NOT_ACCEPT,
		/* 515 */ YY_NOT_ACCEPT,
		/* 516 */ YY_NO_ANCHOR,
		/* 517 */ YY_NO_ANCHOR,
		/* 518 */ YY_NOT_ACCEPT,
		/* 519 */ YY_NOT_ACCEPT,
		/* 520 */ YY_NOT_ACCEPT,
		/* 521 */ YY_NO_ANCHOR,
		/* 522 */ YY_NOT_ACCEPT,
		/* 523 */ YY_NO_ANCHOR,
		/* 524 */ YY_NOT_ACCEPT,
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
		/* 539 */ YY_NOT_ACCEPT,
		/* 540 */ YY_NOT_ACCEPT,
		/* 541 */ YY_NOT_ACCEPT,
		/* 542 */ YY_NOT_ACCEPT,
		/* 543 */ YY_NO_ANCHOR,
		/* 544 */ YY_NOT_ACCEPT,
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
		/* 559 */ YY_NOT_ACCEPT,
		/* 560 */ YY_NOT_ACCEPT,
		/* 561 */ YY_NOT_ACCEPT,
		/* 562 */ YY_NOT_ACCEPT,
		/* 563 */ YY_NOT_ACCEPT,
		/* 564 */ YY_NOT_ACCEPT,
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
		/* 575 */ YY_NO_ANCHOR,
		/* 576 */ YY_NO_ANCHOR,
		/* 577 */ YY_NOT_ACCEPT,
		/* 578 */ YY_NOT_ACCEPT,
		/* 579 */ YY_NOT_ACCEPT,
		/* 580 */ YY_NOT_ACCEPT,
		/* 581 */ YY_NO_ANCHOR,
		/* 582 */ YY_NO_ANCHOR,
		/* 583 */ YY_NO_ANCHOR,
		/* 584 */ YY_NO_ANCHOR,
		/* 585 */ YY_NO_ANCHOR,
		/* 586 */ YY_NO_ANCHOR,
		/* 587 */ YY_NO_ANCHOR,
		/* 588 */ YY_NO_ANCHOR,
		/* 589 */ YY_NO_ANCHOR,
		/* 590 */ YY_NO_ANCHOR,
		/* 591 */ YY_NO_ANCHOR,
		/* 592 */ YY_NO_ANCHOR,
		/* 593 */ YY_NO_ANCHOR,
		/* 594 */ YY_NOT_ACCEPT,
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
		/* 608 */ YY_NO_ANCHOR,
		/* 609 */ YY_NOT_ACCEPT,
		/* 610 */ YY_NO_ANCHOR,
		/* 611 */ YY_NOT_ACCEPT,
		/* 612 */ YY_NOT_ACCEPT,
		/* 613 */ YY_NO_ANCHOR,
		/* 614 */ YY_NO_ANCHOR,
		/* 615 */ YY_NOT_ACCEPT,
		/* 616 */ YY_NOT_ACCEPT,
		/* 617 */ YY_NO_ANCHOR,
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
		/* 663 */ YY_NOT_ACCEPT
	};
	private int[] yy_cmap = unpackFromString(1,65538,
"34,1:9,39,1:2,38,1:18,3,1,28,1:2,26,1:2,40,41,27,17,42,30,46,1,29,48,45,48:" +
"5,44,48,33,1,50,1,51,1:2,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12,6,1,20,22," +
"10,14,32,52,16,11,47,49,35,2,1,43,1,5,7,4,19,18,25,13,23,8,31,24,9,21,15,12" +
",6,1,20,22,10,14,32,52,16,11,47,36,53,37,1:65410,0:2")[0];

	private int[] yy_rmap = unpackFromString(1,664,
"0,1,2,1,3,1:25,4,5,1,6,1:7,7,1,8,1:2,9,1,10,1:4,11,1:2,12,1:7,13,1:3,14,15," +
"16,1:5,17,1,18,1:4,19,20,21,1:8,22,23,24,1:2,22:2,1:2,22,1,22:2,1:2,25,1,26" +
",1:2,27,28,1:2,29,30,1,31,1:2,32,33,34,22,1,35,36,37,38,39,40,41,42,43,44,4" +
"5,46,3,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69" +
",70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94" +
",95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114" +
",115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,13" +
"3,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,1" +
"52,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170," +
"171,172,173,174,175,176,177,178,179,180,181,182,183,184,168,185,186,187,188" +
",189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,20" +
"7,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,2" +
"26,227,228,229,230,231,31,232,233,234,235,236,237,238,239,240,241,242,243,2" +
"44,245,246,247,248,249,250,251,252,32,253,254,255,256,257,258,259,260,261,2" +
"62,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280," +
"281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299" +
",300,301,302,303,304,305,306,307,308,309,310,311,312,313,314,315,316,317,31" +
"8,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,3" +
"37,338,339,340,341,342,343,344,345,346,347,348,349,350,351,352,353,354,355," +
"356,357,358,359,360,361,362,363,364,365,366,367,368,369,370,371,372,373,374" +
",375,376,377,378,379,380,381,382,383,384,385,386,387,388,389,390,391,392,39" +
"3,394,395,396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,4" +
"12,413,414,415,416,417,418,419,420,421,422,423,424,425,426,427,428,429,430," +
"431,432,433,434,435,436,437,438,439,440,441,442,443,444,445,446,447,448,449" +
",450,451,452,453,454,455,456,457,458,459,460,461,462,463,464,465,466,467,46" +
"8,469,470,471,472,473,474,475,476,477,478,479,480,481,482,483,484,485,486,4" +
"87,488,489,490,491,492,493,494,495,496,497,498,499,500,501,502,503,504,505," +
"506,507,508,509,510,511,512,513,514,515,516,517,518,519,520,521,522,523,524" +
",525,526,527,528,529,530,531,532,533,34,534,535,536,537,538,539,540,541,542" +
",543,544,545,546,547,548,549,550,551,552,553,554,555,556,557,558,559,560,56" +
"1,562,563,28")[0];

	private int[][] yy_nxt = unpackFromString(564,54,
"1,2:2,3,2:13,3,2:8,3:3,2:5,3:3,2,-1:2,3:2,2:11,3,-1:55,2:2,-1,2:13,-1,2:8,-" +
"1:3,2:5,-1:3,2,-1:4,2:11,-1:2,4,-1:2,4:22,-1:3,4:5,-1:3,4,-1:4,4:11,-1:2,30" +
",31,-1,30:22,33:2,-1,30:5,-1:3,30,-1:4,30:11,-1:2,31:2,-1,31:22,33:2,-1,31:" +
"5,-1:3,31,-1:4,31:11,-1:2,33:2,-1,33:24,-1,33:5,-1:3,33,-1:4,33:11,-1,1,-1:" +
"18,239,-1:7,42,-1,43,-1:3,44,-1:8,45,-1,43:2,-1:2,43,-1:34,43,-1:14,43:2,-1" +
":2,43,-1:5,1,-1:18,252,-1:7,47,-1,48,-1:3,49,-1:8,50,-1,48:2,-1:2,48,-1:34," +
"48,-1:14,48:2,-1:2,48,-1:34,53,-1:14,53:2,-1:2,53,-1:34,56,-1:14,56:2,-1:2," +
"56,-1:27,294,-1:23,295,-1:53,519,-1:8,69,70,-1,69:22,-1:3,69:5,-1:3,69,-1:4" +
",69:11,-1:2,70:2,-1,70:22,-1:3,70:5,-1:3,70,-1:4,70:11,-1,1,-1:18,513,-1:7," +
"77,-1,78,-1:3,79,-1:8,80,-1,78:2,-1:2,78,-1:34,78,-1:14,78:2,-1:2,78,-1:51," +
"344,-1:8,84,85,-1,84:22,-1:3,84:5,-1:3,84,-1:4,84:11,-1:2,85:2,-1,85:22,-1:" +
"3,85:5,-1:3,85,-1:4,85:11,-1:2,94:2,-1,94:22,-1:3,94:5,-1:3,94,-1:4,94:11,-" +
"1:30,379,-1:14,379:2,-1:2,379,-1:6,94:2,-1,94:22,-1:3,614,94:4,-1:3,94,-1:4" +
",94:2,614:2,94:2,614,94:4,-1,1,-1:18,520,-1:7,110,-1,111,-1:3,112,-1:8,113," +
"-1,111:2,-1:2,111,-1:34,111,-1:14,111:2,-1:2,111,-1:5,1,-1:2,129,-1:51,663:" +
"27,37,663:6,236,663:2,-1:2,663:13,-1:47,306,-1:53,307,-1:8,328:27,74,328:6," +
"330,328:2,-1:2,328:13,-1:2,350:27,87,350:6,352,350:2,-1:2,350:13,-1:2,94:2," +
"-1,99,94:18,567,94:2,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,632:27,101,632:6,380" +
",632:2,-1:2,632:13,-1:5,131,133,-1:3,135,-1:4,137,139,141,-1,143,145,408,-1" +
",147,-1:2,447,-1:29,94:2,-1,94:8,99,94:5,471,94:7,-1:3,94:5,-1:3,94,-1:4,94" +
":11,-1:6,149,-1:3,407,-1:2,409,-1:7,463,-1:2,468,-1:31,94:2,-1,94:5,136,94:" +
"5,100,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:7,448,-1:7,151,-1:40,94:2,-1,94" +
":5,103,94:16,-1:3,94:5,-1:3,94,-1:4,94:11,-1:9,153,-1:3,155,-1:9,157,-1:32," +
"94:2,-1,94:15,103,94:6,-1:3,94:5,-1:3,94,-1:4,94:11,-1:9,159,-1:6,411,-1:39" +
",94:2,-1,94:22,-1:3,94:5,-1:3,94,-1:4,94:10,103,-1:13,446,-1:42,94:2,-1,94:" +
"7,99,94:14,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:6,99,94:15,-1:3,94:" +
"5,-1:3,94,-1:4,94:11,-1:17,161,-1:38,94:2,-1,94:11,103,94:10,-1:3,94:5,-1:3" +
",94,-1:4,94:11,-1:19,162,-1:36,94:2,-1,94:17,99,94:4,-1:3,94:5,-1:3,94,-1:4" +
",94:11,-1:11,164,-1:3,165,-1:3,166,-1:36,94:2,-1,94:6,103,94:15,-1:3,94:5,-" +
"1:3,94,-1:4,94:11,-1:7,167,-1:48,94:2,-1,94:14,100,94:7,-1:3,94:5,-1:3,94,-" +
"1:4,94:11,-1:11,171,-1:44,94:2,-1,94:16,105,94:5,-1:3,94:5,-1:3,94,-1:4,94:" +
"11,-1:23,172,-1:32,94:2,-1,94:11,100,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:" +
"14,173,-1:41,94:2,-1,94:6,106,94:15,-1:3,94:5,-1:3,94,-1:4,94:11,-1:15,412," +
"-1:40,94:2,-1,94:15,99,94:6,-1:3,94:5,-1:3,94,-1:4,94:11,-1:20,5,-1:35,94:2" +
",-1,94:22,-1:3,127,94:4,-1:3,94,-1:4,94:2,127:2,94:2,127,94:4,-1:6,176,417," +
"-1:56,415,-1:59,450,-1:43,177,-1:6,178,-1:48,179,-1:51,418,-1:3,451,-1:49,1" +
"80,-1:59,182,-1:46,183,-1:67,184,-1:58,185,-1:40,186,-1:51,420,-1:3,452,-1:" +
"55,419,-1:45,6,-1:68,188,-1:42,191,-1:9,423,-1:53,421,-1:55,192,-1:38,422,-" +
"1:64,7,-1:38,8,-1:74,9,-1:44,194,-1:56,195,-1:38,10,-1:53,11,-1:58,426,-1:5" +
"5,200,-1:64,428,-1:46,427,-1:43,203,-1:72,204,-1:49,207,-1:49,457,-1:41,12," +
"-1:60,13,-1:65,208,-1:44,209,-1:58,210,-1:45,430,-1:46,14,-1:70,213,-1:36,1" +
"5,-1:59,431,-1:47,16,-1:53,17,-1:54,216,-1:67,18,-1:38,19,-1:53,20,-1:53,21" +
",-1:58,219,-1:48,22,-1:53,23,-1:70,222,-1:36,24,-1:72,25,-1:38,223,-1:56,22" +
"4,-1:47,225,-1:57,226,-1:63,227,-1:46,26,-1:47,228,-1:55,229,-1:49,27,-1:60" +
",432,-1:61,230,-1:38,28,-1:53,29,-1:50,1,30,31,32,30:22,33:2,233,30:5,-1,34" +
",234,30,235,-1,35,36,30:11,-1:2,663:2,662,663:24,37,662,663:5,236,663:2,-1:" +
"2,663:4,662:2,663:2,662,663:4,-1:30,237,-1:14,237:2,-1:2,237,-1:44,38,-1:15" +
",663:27,115,663:6,236,663:2,-1:2,663:13,-1:30,237,-1:7,39,-1:6,237:2,-1:2,2" +
"37,-1:6,663:27,40,663:6,236,663:2,-1:2,663:13,-1:15,240,-1:60,434,-1:43,242" +
",-1:85,243,-1:35,462,-1:48,245,-1:76,246,-1:25,467,-1:48,248,-1:66,249,-1:3" +
"8,250,-1:60,251,-1:51,116,-1:54,459,-1:50,254,-1:85,511,-1:30,256,-1:76,512" +
",-1:20,258,-1:66,259,-1:38,260,-1:60,261,-1:51,117,-1:40,1,-1,51,52,-1,263," +
"-1,480,264,-1,265,-1:3,266,-1:3,435,-1,267,268,-1,436,-1,269,-1:3,53,-1:8,2" +
"70,-1,54,55,-1:2,56:2,57,-1,56,58,59,60,-1:11,271,-1:59,273,-1:56,274,-1:43" +
",275,-1:70,277,-1:36,469,-1:50,279,-1:3,437,-1:4,280,-1:78,61,-1:23,62,-1:6" +
"3,281,-1:44,282,-1:59,283,-1:56,63,-1:66,438,-1:25,284,-1:54,286,-1:70,287," +
"-1:42,64,-1:60,289,-1:45,65,-1:87,291,-1:27,66,-1:54,292,-1:44,62,-1:56,293" +
",-1:60,296,-1:42,484,-1:89,297,-1:26,298,-1:57,63,-1:41,299,-1:49,300,-1:62" +
",301,-1:83,118,-1:28,119,-1:53,303,-1:51,439,-1:40,304,-1:54,305,-1:61,308," +
"-1:48,310,-1:62,63,-1:45,460,-1:11,311,465,-1:55,312,-1:32,313,-1:73,67,-1:" +
"48,314,-1:42,316,-1:53,470,-1:55,317,-1:48,318,-1:64,319,-1:84,305,-1:20,32" +
"2,-1:49,305,-1:53,63,-1:62,474,-1:43,323,-1:64,305,-1:52,325,-1:54,63,-1:55" +
",68,-1:41,120,-1:43,1,69,70,71,69:22,-1:2,328,69:5,-1:2,329,69,-1:2,72,73,6" +
"9:11,-1:30,331,-1:14,331:2,-1:2,331,-1:6,328:27,121,328:6,330,328:2,-1:2,32" +
"8:13,-1:30,331,-1:7,75,-1:6,331:2,-1:2,331,-1:16,540,-1:62,541,-1:43,335,-1" +
":66,336,-1:38,337,-1:60,338,-1:51,122,-1:40,1,-1:2,81,-1:13,82,-1:7,340,-1:" +
"4,82,-1:32,341,-1:49,342,-1:61,343,-1:62,83,-1:53,345,-1:39,346,-1:54,478,-" +
"1:59,348,-1:48,123,-1:43,1,84,85,86,84:22,-1:2,350,84:5,-1:2,351,84,-1:4,84" +
":11,-1:30,353,-1:14,353:2,-1:2,353,-1:6,350:27,124,350:6,352,350:2,-1:2,350" +
":13,-1:30,353,-1:7,88,-1:6,353:2,-1:2,353,-1:5,1,-1:2,89,-1:10,355,-1:5,482" +
",485,-1:16,356,-1,90,91,-1:20,357,-1:6,358,-1:77,92,-1:33,361,-1:56,488,-1:" +
"35,491,-1:71,441,-1:46,494,-1:16,362,-1:26,365,-1:63,367,-1:54,367,-1:46,36" +
"9,-1:59,93,-1:48,93,-1:56,500,-1:48,370,-1:64,372,-1:56,93,-1:39,373,-1:55," +
"374,-1:54,93,-1:42,1,94:2,95,125,406,94,445,94,557,130,94,132,94,565,461,94" +
":3,538,566,94,509,94,576,517,-1:2,376,96,94:4,-1:2,377,94,378,-1,97,98,94:2" +
",96:2,94:2,96,94:4,-1:2,632:2,630,632:24,101,630,632:5,380,632:2,-1:2,632:4" +
",630:2,632:2,630,632:4,-1:30,381,-1:14,381:2,-1:2,381,-1:44,102,-1:44,382,-" +
"1:24,632:27,126,632:6,380,632:2,-1:2,632:13,-1:30,381,-1:7,104,-1:6,381:2,-" +
"1:2,381,-1:10,383,-1:6,384,-1:2,496,-1:3,502,-1,385,504,-1:2,506,-1:5,386,-" +
"1:28,387,-1:7,388,-1:43,389,-1:54,392,-1:53,395,-1:8,396,-1:59,397,-1:46,39" +
"7,-1:50,397,-1:75,397,-1:25,397,-1:60,397,-1:8,397,-1:39,397,-1:54,397,-1:6" +
"1,397,-1:47,397,-1:5,397,-1:68,398,-1:52,515,-1:14,515:2,-1:2,515,-1:34,107" +
",-1:14,107:2,-1:2,107,-1:6,632:27,108,632:6,380,632:2,-1:2,632:13,-1:11,402" +
",-1:66,403,-1:38,404,-1:60,405,-1:51,128,-1:41,94:2,-1,94:5,134,94:5,581,94" +
":10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,449,-1:59,163,-1:41,168,-1:57,414,-1" +
":65,174,-1:38,187,-1:51,416,-1:52,193,-1:67,189,-1:45,454,-1:57,453,-1:59,4" +
"24,-1:40,198,-1:61,196,-1:56,202,-1:43,205,-1:55,201,-1:47,425,-1:72,214,-1" +
":45,456,-1:60,212,-1:49,211,-1:45,215,-1:52,218,-1:52,220,-1:63,231,-1:36,6" +
"63:27,37,238,663:5,236,663:2,-1:2,663:4,238:2,663:2,238,663:4,-1:22,241,-1:" +
"47,276,-1:56,278,-1:40,288,-1:66,290,-1:53,309,-1:40,320,-1:70,442,-1:36,36" +
"8,-1:49,632:27,101,400,632:5,380,632:2,-1:2,632:4,400:2,632:2,400,632:4,-1:" +
"30,399,-1:14,399:2,-1:2,399,-1:6,94:2,-1,558,94:7,466,94:5,543,94:7,-1:3,94" +
":5,-1:3,94,-1:4,94:11,-1:13,175,-1:59,410,-1:41,170,-1:69,181,-1:36,190,-1:" +
"66,455,-1:49,197,-1:54,199,-1:56,206,-1:39,429,-1:67,217,-1:45,458,-1:51,22" +
"1,-1:66,464,-1:50,315,-1:36,94:2,-1,94:14,138,94:7,-1:3,94:5,-1:3,94,-1:4,9" +
"4:11,-1:13,244,-1:59,413,-1:56,253,-1:50,440,-1:36,94:2,-1,94:15,140,94:6,-" +
"1:3,94:5,-1:3,94,-1:4,94:11,-1:13,247,-1:59,169,-1:56,285,-1:50,321,-1:36,9" +
"4:2,-1,94:12,142,94:9,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,255,-1:62,477,-1:5" +
"0,324,-1:36,94:2,-1,94:11,529,94:2,144,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1" +
":13,257,-1:62,332,-1:50,347,-1:36,94:2,-1,94:8,146,94:13,-1:3,94:5,-1:3,94," +
"-1:4,94:11,-1:13,272,-1:62,580,-1:50,359,-1:36,94:2,-1,94:21,148,-1:3,94:5," +
"-1:3,94,-1:4,94:11,-1:13,302,-1:59,360,-1:36,94:2,-1,150,94:21,-1:3,94:5,-1" +
":3,94,-1:4,94:11,-1:13,326,-1:59,497,-1:36,94:2,-1,94:16,150,94:5,-1:3,94:5" +
",-1:3,94,-1:4,94:11,-1:13,333,-1:59,363,-1:36,94:2,-1,94:14,152,94:7,-1:3,9" +
"4:5,-1:3,94,-1:4,94:11,-1:13,334,-1:59,364,-1:36,94:2,-1,94:11,148,94:10,-1" +
":3,94:5,-1:3,94,-1:4,94:11,-1:13,390,-1:59,366,-1:36,94:2,-1,94:3,574,94:4," +
"154,94:9,555,94:3,-1:3,94:5,-1:3,94,-1:4,94:11,-1:13,401,-1:59,371,-1:36,94" +
":2,-1,94:14,156,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:19,391,-1:36,94:2,-1,9" +
"4:14,136,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:19,393,-1:36,94:2,-1,142,94:2" +
"1,-1:3,94:5,-1:3,94,-1:4,94:11,-1:19,394,-1:36,94:2,-1,94:16,158,94:5,-1:3," +
"94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,160,94:4,-1:3,94,-1:4,94:2," +
"160:2,94:2,160,94:4,-1:2,94:2,-1,94:4,523,94:5,569,94:3,475,94:2,570,94:4,-" +
"1:3,94:5,-1:3,94,-1:4,94:11,-1:2,663:27,37,433,663:5,236,663:2,-1:2,663:4,4" +
"33:2,663:2,433,663:4,-1:26,472,-1:43,476,-1:52,473,-1:40,632:27,101,443,632" +
":5,380,632:2,-1:2,632:4,443:2,632:2,443,632:4,-1:30,444,-1:14,444:2,-1:2,44" +
"4,-1:6,94:2,-1,94:14,144,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:" +
"5,572,94:10,479,94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:26,490,-1:43,487,-1:52" +
",481,-1:40,94:2,-1,94,483,94:20,-1:3,94:5,-1:3,94,-1:4,94:11,-1:16,493,-1:3" +
"9,94:2,-1,94:11,486,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:16,499,-1:39,94:2" +
",-1,94:8,489,94:13,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:9,492,94:12" +
",-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:14,516,94:7,-1:3,94:5,-1:3,94" +
",-1:4,94:11,-1:2,94:2,-1,94:14,495,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,9" +
"4:2,-1,94:6,498,94:15,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:18,501,9" +
"4:3,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:6,503,94:15,-1:3,94:5,-1:3" +
",94,-1:4,94:11,-1:2,94:2,-1,94:14,505,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:" +
"2,94:2,-1,94:5,492,94:16,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:8,507" +
",94:13,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:9,503,94:12,-1:3,94:5,-" +
"1:3,94,-1:4,94:11,-1:2,94:2,-1,94:16,503,94:5,-1:3,94:5,-1:3,94,-1:4,94:11," +
"-1:2,94:2,-1,94:22,-1:3,508,94:4,-1:3,94,-1:4,94:2,508:2,94:2,508,94:4,-1:2" +
",94:2,-1,94:14,568,94,521,94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,663:27,37," +
"510,663:5,236,663:2,-1:2,663:4,510:2,663:2,510,663:4,-1:44,518,-1:53,522,-1" +
":11,632:27,101,514,632:5,380,632:2,-1:2,632:4,514:2,632:2,514,632:4,-1:2,94" +
":2,-1,94:21,525,-1:3,94:5,-1:3,94,-1:4,94:11,-1:44,524,-1:11,94:2,-1,94:16," +
"526,94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94,593,94:13,538,94:2,52" +
"7,94,576,582,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,528,94:21,-1:3,94:5," +
"-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:16,530,94:5,-1:3,94:5,-1:3,94,-1:4,94:11" +
",-1:2,94:2,-1,94:14,531,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:2" +
"2,-1:3,94:2,532,94:2,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:5,533,94:16,-1:3,94" +
":5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,94:5,-1:3,94,-1:4,94:10,534,-" +
"1:2,94:2,-1,94:9,535,94:12,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:14," +
"536,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:4,523,94:17,-1:3,94:5" +
",-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,537,94:4,-1:3,94,-1:4,94:2,537:" +
"2,94:2,537,94:4,-1:2,94:2,-1,94,545,94:20,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2" +
",94:2,-1,99,94:21,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,663:27,37,539,663:5,236" +
",663:2,-1:2,663:4,539:2,663:2,539,663:4,-1:21,544,-1:34,632:27,101,542,632:" +
"5,380,632:2,-1:2,632:4,542:2,632:2,542,632:4,-1:13,560,-1:66,562,-1:71,563," +
"-1:11,94:2,-1,94:11,546,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:" +
"14,547,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94,548,94:20,-1:3,94:" +
"5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:5,549,94:16,-1:3,94:5,-1:3,94,-1:4,94:" +
"11,-1:2,94:2,-1,94:3,550,94:18,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94" +
",551,94:20,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:7,552,94:14,-1:3,94" +
":5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94,553,94:20,-1:3,94:5,-1:3,94,-1:4,94:1" +
"1,-1:2,94:2,-1,94:22,-1:3,94:5,-1:3,94,-1:4,94:10,554,-1:2,94:2,-1,94:14,54" +
"3,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,94,556,94:3,-1:" +
"3,94,-1:4,94:11,-1:2,94:2,-1,94:14,571,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1" +
":2,663:16,559,663:10,37,663,559,663:4,236,663:2,-1:2,663:13,-1:12,564,-1:43" +
",632:27,101,632,561,632:4,380,632:2,-1:2,632:13,-1:22,578,-1:33,94:2,-1,94:" +
"18,573,94:3,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:5,572,94:16,-1:3,9" +
"4:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:16,575,94:5,-1:3,94:5,-1:3,94,-1:4,9" +
"4:11,-1:2,94:2,-1,94:9,575,94:12,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1," +
"94:6,575,94:15,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,94:3,57" +
"5,94,-1:3,94,-1:4,94:11,-1:2,94:2,-1,575,94:21,-1:3,94:5,-1:3,94,-1:4,94:11" +
",-1:2,94:2,-1,94:7,575,94:8,575,94:5,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2" +
",-1,94:2,575,94:19,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:3,575,94:18" +
",-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:11,575,94:10,-1:3,94:5,-1:3,9" +
"4,-1:4,94:11,-1:2,94:2,-1,94:5,575,94:5,575,94:10,-1:3,94:5,-1:3,94,-1:4,94" +
":11,-1:2,94:2,-1,94:11,581,94:10,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,663:2,57" +
"7,663:24,37,663:6,236,663:2,-1:2,663:13,-1:2,632:19,579,632:7,101,632:6,380" +
",632:2,-1:2,632:13,-1:2,94:2,-1,94:2,583,94:7,584,94:11,-1:3,94:5,-1:3,94,-" +
"1:4,94:11,-1:2,632:12,579,632:14,101,632:6,380,632:2,-1:2,632:13,-1:2,94:2," +
"-1,585,94:21,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,632:9,579,632:17,101,632:6,3" +
"80,632:2,-1:2,632:13,-1:2,94:2,-1,94:8,586,94:13,-1:3,94:5,-1:3,94,-1:4,94:" +
"11,-1:2,632:27,101,632:3,579,632:2,380,632:2,-1:2,632:13,-1:2,94:2,-1,94:14" +
",587,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,632:3,579,632:23,101,632:6,380," +
"632:2,-1:2,632:13,-1:2,94:2,-1,94,588,94:20,-1:3,94:5,-1:3,94,-1:4,94:11,-1" +
":2,632:10,579,632:8,579,632:7,101,632:6,380,632:2,-1:2,632:13,-1:2,94:2,-1," +
"94:14,589,94:7,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,632:5,579,632:21,101,632:6" +
",380,632:2,-1:2,632:13,-1:2,94:2,-1,94:14,590,94:7,-1:3,94:5,-1:3,94,-1:4,9" +
"4:11,-1:2,632:6,579,632:20,101,632:6,380,632:2,-1:2,632:13,-1:2,94:2,-1,94," +
"591,94:8,592,94:11,-1:3,94:5,-1:3,94,-1:4,94:11,-1:2,632:14,579,632:12,101," +
"632:6,380,632:2,-1:2,632:13,-1:2,632:8,579,632:5,579,632:12,101,632:6,380,6" +
"32:2,-1:2,632:13,-1:2,94:2,-1,94,596,94:6,598,94:2,600,94:3,602,94,604,606," +
"94:2,608,-1:3,94:2,610,94:2,-1:3,94,-1:4,94:11,-1:2,94:2,-1,94:22,-1:3,617," +
"613,94:3,-1:3,94,-1:4,94:2,617:2,94:2,617,94:4,-1:2,663:27,37,594,663:5,236" +
",663:2,-1:2,663:4,594:2,663:2,594,663:4,-1:2,632:5,595,632:7,597,632:13,101" +
",632:6,380,632:2,-1:2,632:13,-1:2,94:2,-1,94:22,-1:3,617,94:4,-1:3,94,-1:4," +
"94:2,617:2,94:2,617,94:4,-1:2,632:3,599,632:23,101,632:6,380,632:2,-1:2,632" +
":13,-1:2,632:11,601,632:15,101,632:6,380,632:2,-1:2,632:13,-1:2,632:17,603," +
"632:9,101,632:6,380,632:2,-1:2,632:13,-1:2,632:4,605,632:22,101,632:6,380,6" +
"32:2,-1:2,632:13,-1:2,632:17,607,632:9,101,632:6,380,632:2,-1:2,632:13,-1:2" +
",632:17,609,632:9,101,632:6,380,632:2,-1:2,632:13,-1:2,632:4,611,632:8,612," +
"632:13,101,632:6,380,632:2,-1:2,632:13,-1:2,663:27,37,615,663:5,236,663:2,-" +
"1:2,663:4,615:2,663:2,615,663:4,-1:2,632:4,616,632:6,618,632:2,619,632:3,62" +
"0,632,621,622,632:2,623,632:2,101,632:2,624,632:3,380,632:2,-1:2,632:13,-1:" +
"2,663:27,37,663:4,625,663,236,663:2,-1:2,663:13,-1:2,632:27,101,632,626,632" +
":4,380,632:2,-1:2,632:13,-1:2,663:27,37,627,663:5,236,663:2,-1:2,663:4,627:" +
"2,663:2,627,663:4,-1:2,632:27,101,628,632:5,380,632:2,-1:2,632:4,628:2,632:" +
"2,628,632:4,-1:2,663:27,37,629,663:5,236,663:2,-1:2,663:4,629:2,663:2,629,6" +
"63:4,-1:2,663:27,37,663:4,631,663,236,663:2,-1:2,663:13,-1:2,663:27,37,633," +
"663:5,236,663:2,-1:2,663:4,633:2,663:2,633,663:4,-1:2,663:27,37,634,663:5,2" +
"36,663:2,-1:2,663:4,634:2,663:2,634,663:4,-1:2,663:2,635,663:24,37,663:6,23" +
"6,663:2,-1:2,663:13,-1:2,663:27,37,636,663:5,236,663:2,-1:2,663:4,636:2,663" +
":2,636,663:4,-1:2,663:27,37,637,663:5,236,663:2,-1:2,663:4,637:2,663:2,637," +
"663:4,-1:2,663:27,37,638,663:5,236,663:2,-1:2,663:4,638:2,663:2,638,663:4,-" +
"1:2,663:27,37,639,663:5,236,663:2,-1:2,663:4,639:2,663:2,639,663:4,-1:2,663" +
":27,37,663,640,663:4,236,663:2,-1:2,663:13,-1:2,663:19,641,663:7,37,663:6,2" +
"36,663:2,-1:2,663:13,-1:2,663:12,641,663:14,37,663:6,236,663:2,-1:2,663:13," +
"-1:2,663:9,641,663:17,37,663:6,236,663:2,-1:2,663:13,-1:2,663:27,37,663:3,6" +
"41,663:2,236,663:2,-1:2,663:13,-1:2,663:3,641,663:23,37,663:6,236,663:2,-1:" +
"2,663:13,-1:2,663:10,641,663:8,641,663:7,37,663:6,236,663:2,-1:2,663:13,-1:" +
"2,663:5,641,663:21,37,663:6,236,663:2,-1:2,663:13,-1:2,663:6,641,663:20,37," +
"663:6,236,663:2,-1:2,663:13,-1:2,663:14,641,663:12,37,663:6,236,663:2,-1:2," +
"663:13,-1:2,663:8,641,663:5,641,663:12,37,663:6,236,663:2,-1:2,663:13,-1:2," +
"663:5,642,663:7,643,663:13,37,663:6,236,663:2,-1:2,663:13,-1:2,663:3,644,66" +
"3:23,37,663:6,236,663:2,-1:2,663:13,-1:2,663:11,645,663:15,37,663:6,236,663" +
":2,-1:2,663:13,-1:2,663:17,646,663:9,37,663:6,236,663:2,-1:2,663:13,-1:2,66" +
"3:4,647,663:22,37,663:6,236,663:2,-1:2,663:13,-1:2,663:17,648,663:9,37,663:" +
"6,236,663:2,-1:2,663:13,-1:2,663:17,649,663:9,37,663:6,236,663:2,-1:2,663:1" +
"3,-1:2,663:4,650,663:8,651,663:13,37,663:6,236,663:2,-1:2,663:13,-1:2,663:4" +
",652,663:6,653,663:2,654,663:3,655,663,656,657,663:2,658,663:2,37,663:2,659" +
",663:3,236,663:2,-1:2,663:13,-1:2,663:27,37,663,660,663:4,236,663:2,-1:2,66" +
"3:13,-1:2,663:27,37,661,663:5,236,663:2,-1:2,663:4,661:2,663:2,661,663:4,-1");

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
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -97:
						break;
					case 97:
						{ return new Symbol(sym.LPARENT); }
					case -98:
						break;
					case 98:
						{ return new Symbol(sym.RPARENT); }
					case -99:
						break;
					case 99:
						{ return new Symbol(sym.SEARCH_KEYWORD_ASTRING, yytext()); }
					case -100:
						break;
					case 100:
						{ return new Symbol(sym.SEARCH_KEYWORD_DATE, yytext()); }
					case -101:
						break;
					case 101:
						{ return new Symbol(sym.QUOTED, yytext()); }
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
						{ return new Symbol(sym.LITERAL, yytext()); }
					case -105:
						break;
					case 105:
						{ return new Symbol(sym.SEARCH_KEYWORD_NUMBER, yytext()); }
					case -106:
						break;
					case 106:
						{ return new Symbol(sym.CHARSET, yytext()); }
					case -107:
						break;
					case 107:
						{ return new Symbol(sym.DATE, yytext()); }
					case -108:
						break;
					case 108:
						{ return new Symbol(sym.DATE, yytext().Substring(1, yytext().Length-2)); }
					case -109:
						break;
					case 109:
						{ yybegin(commandsearch); break; }
					case -110:
						break;
					case 110:
						{ return new Symbol(sym.STAR); }
					case -111:
						break;
					case 111:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -112:
						break;
					case 112:
						{ return new Symbol(sym.COLON); }
					case -113:
						break;
					case 113:
						{ return new Symbol(sym.COMMA); }
					case -114:
						break;
					case 115:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -115:
						break;
					case 116:
						{ yybegin(commanddetail); break; }
					case -116:
						break;
					case 117:
						{ yybegin(commandfetch); break; }
					case -117:
						break;
					case 118:
						{ return new Symbol(sym.FETCH_ATT, yytext()); }
					case -118:
						break;
					case 119:
						{ return new Symbol(sym.FETCH_MSG_TEXT_SINGLE, yytext()); }
					case -119:
						break;
					case 120:
						{ yybegin(commandfetchheaderlist);return new Symbol(sym.FETCH_MSG_TEXT_HEADERLIST_KEY, yytext()); }
					case -120:
						break;
					case 121:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -121:
						break;
					case 122:
						{ yybegin(commandstoreflags); break; }
					case -122:
						break;
					case 123:
						{ yybegin(commanddetail); return new Symbol(sym.FLAG_KEY, yytext()); }
					case -123:
						break;
					case 124:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -124:
						break;
					case 125:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -125:
						break;
					case 126:
						{ return new Symbol(sym.QUOTED, yytext()); }
					case -126:
						break;
					case 127:
						{ return new Symbol(sym.DATE, yytext()); }
					case -127:
						break;
					case 128:
						{ yybegin(commandsearch); break; }
					case -128:
						break;
					case 130:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -129:
						break;
					case 132:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -130:
						break;
					case 134:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -131:
						break;
					case 136:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -132:
						break;
					case 138:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -133:
						break;
					case 140:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -134:
						break;
					case 142:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -135:
						break;
					case 144:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -136:
						break;
					case 146:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -137:
						break;
					case 148:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -138:
						break;
					case 150:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -139:
						break;
					case 152:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -140:
						break;
					case 154:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -141:
						break;
					case 156:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -142:
						break;
					case 158:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -143:
						break;
					case 160:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -144:
						break;
					case 406:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -145:
						break;
					case 445:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -146:
						break;
					case 461:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -147:
						break;
					case 466:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -148:
						break;
					case 471:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -149:
						break;
					case 475:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -150:
						break;
					case 479:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -151:
						break;
					case 483:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -152:
						break;
					case 486:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -153:
						break;
					case 489:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -154:
						break;
					case 492:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -155:
						break;
					case 495:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -156:
						break;
					case 498:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -157:
						break;
					case 501:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -158:
						break;
					case 503:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -159:
						break;
					case 505:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -160:
						break;
					case 507:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -161:
						break;
					case 508:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -162:
						break;
					case 509:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -163:
						break;
					case 516:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -164:
						break;
					case 517:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -165:
						break;
					case 521:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -166:
						break;
					case 523:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -167:
						break;
					case 525:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -168:
						break;
					case 526:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -169:
						break;
					case 527:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -170:
						break;
					case 528:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -171:
						break;
					case 529:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -172:
						break;
					case 530:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -173:
						break;
					case 531:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -174:
						break;
					case 532:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -175:
						break;
					case 533:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -176:
						break;
					case 534:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -177:
						break;
					case 535:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -178:
						break;
					case 536:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -179:
						break;
					case 537:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -180:
						break;
					case 538:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -181:
						break;
					case 543:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -182:
						break;
					case 545:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -183:
						break;
					case 546:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -184:
						break;
					case 547:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -185:
						break;
					case 548:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -186:
						break;
					case 549:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -187:
						break;
					case 550:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -188:
						break;
					case 551:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -189:
						break;
					case 552:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -190:
						break;
					case 553:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -191:
						break;
					case 554:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -192:
						break;
					case 555:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -193:
						break;
					case 556:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -194:
						break;
					case 557:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -195:
						break;
					case 558:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -196:
						break;
					case 565:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -197:
						break;
					case 566:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -198:
						break;
					case 567:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -199:
						break;
					case 568:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -200:
						break;
					case 569:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -201:
						break;
					case 570:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -202:
						break;
					case 571:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -203:
						break;
					case 572:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -204:
						break;
					case 573:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -205:
						break;
					case 574:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -206:
						break;
					case 575:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -207:
						break;
					case 576:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -208:
						break;
					case 581:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -209:
						break;
					case 582:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -210:
						break;
					case 583:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -211:
						break;
					case 584:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -212:
						break;
					case 585:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -213:
						break;
					case 586:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -214:
						break;
					case 587:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -215:
						break;
					case 588:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -216:
						break;
					case 589:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -217:
						break;
					case 590:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -218:
						break;
					case 591:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -219:
						break;
					case 592:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -220:
						break;
					case 593:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -221:
						break;
					case 596:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -222:
						break;
					case 598:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -223:
						break;
					case 600:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -224:
						break;
					case 602:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -225:
						break;
					case 604:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -226:
						break;
					case 606:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -227:
						break;
					case 608:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -228:
						break;
					case 610:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -229:
						break;
					case 613:
						{ return new Symbol(sym.ASTRING, yytext()); }
					case -230:
						break;
					case 614:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -231:
						break;
					case 617:
						{ return new Symbol(sym.NUMBER, yytext()); }
					case -232:
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
