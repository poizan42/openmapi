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
%%

%{
		
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

%}
%eofval{
  return new Symbol(sym.EOF);
%eofval}
%cup
%unicode
%ignorecase
%state commandbase, commanddetail, commandsequence, commandfetchsequence, commandfetch, commandfetchheaderlist, commandstoresequence, commandstoreflags, commandstatus, commandstatuslist, commandsearch, commandsearchheader, commandsearchastring, commandsearchnumber, commandsearchsequence, commandsearchuidsequence

SP=" "
CR=\x0d
LF=\x0a
CRLF=(\x0d\x0a)
CTL=\000|\x0d|\x0a
ATOM_CHAR =[^{atom-specials}]
atom = ({ATOM_CHAR})+
atom-specials=\(|\)|\{|{SP}|{CTL}|{list-wildcards}|{quoted-specials}|{resp-specials}
astring=(({ASTRING_CHAR})+)
ASTRING_CHAR_WITHOUT_PLUS=[^{atom-specials}|\+]|{resp-specials}
ASTRING_CHAR={ATOM_CHAR}|{resp-specials}
charset=CHARSET
date={date-day-fixed}-{date-month}-{date-year}
date-time=\"{date-day-fixed}-{date-month}-{date-year}{SP}{time}{SP}{zone}\"
date-day-fixed=({SP}{DIGIT}|{DIGIT}{DIGIT})
date-month=(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)
date-year=({DIGIT}{DIGIT}{DIGIT}{DIGIT})
DIGIT=[0-9]
digit-nz=[1-9]
fetch_arg_prim=ALL|FULL|FAST
fetch-att=ENVELOPE|FLAGS|INTERNALDATE|RFC822(\.HEADER|\.SIZE|\.TEXT)?|BODYSTRUCTURE|UID
fetch-att-BODY=BODY
fetch-att-BODY-PEEK=BODY\.PEEK
fetch-msgtext-single=HEADER|TEXT
fetch-msgtext-headerlist-key=HEADER\.FIELDS(\.NOT)?
flag=(\\Answered|\\Flagged|\\Deleted|\\Seen|\\Draft|{flag-keyword}|{flag-extension})
flag-extension=\\{atom}
flag-keyword={atom}
flag-sign=\+|\-
flag-key=FLAGS(\.SILENT)?
list-char=({ATOM_CHAR}|{list-wildcards}|{resp-specials})
list-mailbox=({list-char}+)
list-wildcards  = %|\*
literal=(\{{number}\})
mailbox=(INBOX|{astring})
number={DIGIT}+
nz-number=({digit-nz}({DIGIT})*)
quoted=(\"({QUOTED_CHAR})*\")
QUOTED_CHAR=[^{quoted-specials}|{CR}|{LF}]|\\{quoted-specials}
quoted-specials=\"|\\
resp-specials   = \]
search-keyword-sole=ALL|ANSWERED|DELETED|DRAFT|FLAGGED|NEW|OLD|RECENT|SEEN|UNANSWERED|UNDELETED|UNDRAFT|UNFLAGGED|UNSEEN|UNDRAFT
search-keyword-date=BEFORE|ON|SINCE|SENTBEFORE|SENTON|SENTSINCE
search-keyword-number=LARGER|SMALLER
search-keyword-astring=BCC|BODY|CC|FROM|KEYWORD|SUBJECT|TEXT|TO|UNKEYWORD
search-keyword-uid=UID
search-keyword-not=NOT
search-keyword-or=OR
search-keyword-header=HEADER
search-keyword-lparent=\(
search-keyword-rparent=\)
status-att=MESSAGES|RECENT|UIDNEXT|UIDVALIDITY|UNSEEN
section-text-mime=MIME        
tag=({ASTRING_CHAR_WITHOUT_PLUS})+
time=({DIGIT}{DIGIT}:{DIGIT}{DIGIT}:{DIGIT}{DIGIT})
x-command = (X){atom}
zone=((\+|-){DIGIT}{DIGIT}{DIGIT}{DIGIT})

%% 

<YYINITIAL> {tag} { yybegin(commandbase); return new Symbol(sym.TAG,yytext()); }   
<commandbase> {SP}(CAPABILITY) { yybegin(commanddetail); return new Symbol(sym.CAPABILITY, yytext().Substring(1)); }   
<commandbase> {SP}(LOGOUT) { yybegin(commanddetail); return new Symbol(sym.LOGOUT, yytext().Substring(1)); }   
<commandbase> {SP}(NOOP) { yybegin(commanddetail); return new Symbol(sym.NOOP, yytext().Substring(1)); }   
<commandbase> {SP}{x-command} { yybegin(commanddetail); return new Symbol(sym.X_COMMAND, yytext().Substring(1)); }   
<commandbase> {SP}(APPEND){SP} { yybegin(commanddetail); return new Symbol(sym.APPEND, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(CREATE){SP} { yybegin(commanddetail); return new Symbol(sym.CREATE, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(DELETE){SP} { yybegin(commanddetail); return new Symbol(sym.DELETE, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(EXAMINE){SP} { yybegin(commanddetail); return new Symbol(sym.EXAMINE, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(LIST){SP} { yybegin(commanddetail); return new Symbol(sym.LIST, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(LSUB){SP} { yybegin(commanddetail); return new Symbol(sym.LSUB, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(RENAME){SP} { yybegin(commanddetail); return new Symbol(sym.RENAME, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(SELECT){SP} { yybegin(commanddetail); return new Symbol(sym.SELECT, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(STATUS){SP} { yybegin(commandstatus); return new Symbol(sym.STATUS, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(SUBSCRIBE){SP} { yybegin(commanddetail); return new Symbol(sym.SUBSCRIBE, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(UNSUBSCRIBE){SP} { yybegin(commanddetail); return new Symbol(sym.UNSUBSCRIBE, yytext().Substring(1, yytext().Length-2)); } 
<commandbase> {SP}(LOGIN){SP} { yybegin(commanddetail); return new Symbol(sym.LOGIN, yytext().Substring(1, yytext().Length-2)); }
<commandbase> {SP}(AUTHENTICATE){SP} { yybegin(commanddetail); return new Symbol(sym.AUTHENTICATE, yytext().Substring(1, yytext().Length-2)); }
<commandbase> {SP}(STARTTLS) { yybegin(commanddetail); return new Symbol(sym.STARTTLS, yytext().Substring(1)); }
<commandbase> {SP}(CHECK) { yybegin(commanddetail); return new Symbol(sym.CHECK, yytext().Substring(1, yytext().Length-1)); }
<commandbase> {SP}(CLOSE) { yybegin(commanddetail); return new Symbol(sym.CLOSE, yytext().Substring(1)); }
<commandbase> {SP}(EXPUNGE) { yybegin(commanddetail); return new Symbol(sym.EXPUNGE, yytext().Substring(1)); }
<commandbase> {SP}(COPY){SP} { yybegin(commandsequence); return new Symbol(sym.COPY, yytext().Substring(1, yytext().Length-2)); }
<commandbase> {SP}(FETCH){SP} { yybegin(commandfetchsequence); return new Symbol(sym.FETCH, yytext().Substring(1, yytext().Length-2)); }
<commandbase> {SP}(STORE){SP} { yybegin(commandstoresequence); return new Symbol(sym.STORE, yytext().Substring(1, yytext().Length-2)); }
<commandbase> {SP}(SEARCH){SP} { yybegin(commandsearch); return new Symbol(sym.SEARCH, yytext().Substring(1, yytext().Length-2)); }
<commandbase> {SP}(UID) { return new Symbol(sym.UID, yytext().Substring(1)); }
<commanddetail> {atom} { return new Symbol(sym.ATOM, yytext()); }
<commanddetail> {astring} { return new Symbol(sym.ASTRING, yytext()); }
<commanddetail> {list-mailbox} { return new Symbol(sym.LIST_MAILBOX, yytext()); }
<commanddetail> {date-time} { return new Symbol(sym.DATE_TIME, yytext()); }
<commanddetail> {quoted} { return new Symbol(sym.QUOTED, yytext()); }
<commanddetail> {literal} { return new Symbol(sym.LITERAL, yytext()); }
<commanddetail> {SP} { return new Symbol(sym.SP); }
<commanddetail> {CRLF} { return new Symbol(sym.CRLF); }
<commanddetail> \\ { return new Symbol(sym.BACKSLASH); }
<commanddetail> \( { return new Symbol(sym.LPARENT); }
<commanddetail> \) { return new Symbol(sym.RPARENT); }
<commandsequence> "*" { return new Symbol(sym.STAR); }
<commandsequence> "," { return new Symbol(sym.COMMA); }
<commandsequence> ":" { return new Symbol(sym.COLON); } 
<commandsequence> {number} { return new Symbol(sym.NUMBER, yytext()); }
<commandsequence> (DUMMY_FOR_NOTHING)? { yybegin(commanddetail); break; }
<commandfetchsequence> "*" { return new Symbol(sym.STAR); }
<commandfetchsequence> "," { return new Symbol(sym.COMMA); }
<commandfetchsequence> ":" { return new Symbol(sym.COLON); } 
<commandfetchsequence> {number} { return new Symbol(sym.NUMBER, yytext()); }
<commandfetchsequence> (DUMMY_FOR_NOTHING)? { yybegin(commandfetch); break; }
<commandfetch> {fetch_arg_prim} { return new Symbol(sym.FETCH_ARG_PRIM, yytext()); }
<commandfetch> {fetch-att} { return new Symbol(sym.FETCH_ATT, yytext()); }
<commandfetch> {fetch-att-BODY} { return new Symbol(sym.FETCH_ATT_BODY, yytext()); }
<commandfetch> {fetch-att-BODY-PEEK} {  return new Symbol(sym.FETCH_ATT_BODY_PEEK, yytext()); }
<commandfetch> {fetch-msgtext-single} { return new Symbol(sym.FETCH_MSG_TEXT_SINGLE, yytext()); }
<commandfetch> {fetch-msgtext-headerlist-key} { yybegin(commandfetchheaderlist);return new Symbol(sym.FETCH_MSG_TEXT_HEADERLIST_KEY, yytext()); }
<commandfetch> {section-text-mime} { return new Symbol(sym.SECTION_TEXT_MIME); }
<commandfetch> {nz-number} { return new Symbol(sym.NZ_NUMBER, yytext()); }
<commandfetch> {number} { return new Symbol(sym.NUMBER, yytext()); }
<commandfetch> \( { return new Symbol(sym.LPARENT); }
<commandfetch> \) { return new Symbol(sym.RPARENT); }
<commandfetch> \[ { return new Symbol(sym.LBRACK); }
<commandfetch> \] { return new Symbol(sym.RBRACK); }
<commandfetch> \< { return new Symbol(sym.LESSTHAN); }
<commandfetch> \> { return new Symbol(sym.GREATERTHAN); }
<commandfetch> \. { return new Symbol(sym.DOT); }
<commandfetch> {SP} { return new Symbol(sym.SP); }
<commandfetch> {CRLF} { return new Symbol(sym.CRLF); }
<commandfetchheaderlist> {atom} { return new Symbol(sym.ATOM, yytext()); }
<commandfetchheaderlist> {astring} { return new Symbol(sym.ASTRING, yytext()); }
<commandfetchheaderlist> {quoted} { return new Symbol(sym.QUOTED, yytext()); }
<commandfetchheaderlist> {literal} { return new Symbol(sym.LITERAL, yytext()); }
<commandfetchheaderlist> {SP} { return new Symbol(sym.SP); }
<commandfetchheaderlist> \( { return new Symbol(sym.LPARENT); }
<commandfetchheaderlist> \) { yybegin(commandfetch); return new Symbol(sym.RPARENT); }
<commandstoresequence> "*" { return new Symbol(sym.STAR); }
<commandstoresequence> "," { return new Symbol(sym.COMMA); }
<commandstoresequence> ":" { return new Symbol(sym.COLON); } 
<commandstoresequence> {number} { return new Symbol(sym.NUMBER, yytext()); }
<commandstoresequence> (DUMMY_FOR_NOTHING)? { yybegin(commandstoreflags); break; }
<commandstoreflags> {SP} { return new Symbol(sym.SP); }
<commandstoreflags> {flag-sign} { return new Symbol(sym.FLAG_SIGN, yytext()); }
<commandstoreflags> {flag-key} { yybegin(commanddetail); return new Symbol(sym.FLAG_KEY, yytext()); }
<commandstatus> {atom} { return new Symbol(sym.ATOM, yytext()); }
<commandstatus> {astring} { return new Symbol(sym.ASTRING, yytext()); }
<commandstatus> {quoted} { return new Symbol(sym.QUOTED, yytext()); }
<commandstatus> {literal} { return new Symbol(sym.LITERAL, yytext()); }
<commandstatus> {SP} { yybegin(commandstatuslist); return new Symbol(sym.SP); }
<commandstatuslist> {CRLF} { return new Symbol(sym.CRLF); }
<commandstatuslist> {SP} { return new Symbol(sym.SP); }
<commandstatuslist> {status-att} { return new Symbol(sym.STATUS_ATT, yytext()); }
<commandstatuslist> \( { return new Symbol(sym.LPARENT); }
<commandstatuslist> \) { return new Symbol(sym.RPARENT); }
<commandsearch> {charset} { yybegin(commandsearchastring); return new Symbol(sym.CHARSET, yytext()); }
<commandsearch> {search-keyword-sole} { return new Symbol(sym.SEARCH_KEYWORD_SOLE, yytext()); }
<commandsearch> {search-keyword-date} { return new Symbol(sym.SEARCH_KEYWORD_DATE, yytext()); }
<commandsearch> {search-keyword-number} { yybegin(commandsearchnumber); return new Symbol(sym.SEARCH_KEYWORD_NUMBER, yytext()); }
<commandsearch> {search-keyword-astring} { yybegin(commandsearchastring); return new Symbol(sym.SEARCH_KEYWORD_ASTRING, yytext()); }
<commandsearch> {search-keyword-header} { yybegin(commandsearchheader); return new Symbol(sym.SEARCH_KEYWORD_HEADER, yytext()); }
<commandsearch> {search-keyword-not} { return new Symbol(sym.SEARCH_KEYWORD_NOT, yytext()); }
<commandsearch> {search-keyword-or} { return new Symbol(sym.SEARCH_KEYWORD_OR, yytext()); }
<commandsearch> {search-keyword-lparent} { return new Symbol(sym.SEARCH_KEYWORD_LPARENT, yytext()); }
<commandsearch> {search-keyword-rparent} { return new Symbol(sym.SEARCH_KEYWORD_RPARENT, yytext()); }
<commandsearch> \"({date})\" { return new Symbol(sym.DATE, yytext().Substring(1, yytext().Length-2)); }
<commandsearch> {date} { return new Symbol(sym.DATE, yytext()); }
<commandsearch> {number} { yybegin(commandsearchsequence); return new Symbol(sym.NUMBER, yytext()); }
<commandsearch> {search-keyword-uid} { yybegin(commandsearchuidsequence); return new Symbol(sym.SEARCH_KEYWORD_UID, yytext()); }
<commandsearch> {SP} { return new Symbol(sym.SP); }
<commandsearch> {CRLF} { return new Symbol(sym.CRLF); }
<commandsearchnumber> {number} { yybegin(commandsearch); return new Symbol(sym.NUMBER, yytext()); }
<commandsearchnumber> {SP} { return new Symbol(sym.SP); }
<commandsearchastring> {astring} { yybegin(commandsearch); return new Symbol(sym.ASTRING, yytext()); }
<commandsearchastring> {quoted} { yybegin(commandsearch); return new Symbol(sym.QUOTED, yytext()); }
<commandsearchastring> {literal} { yybegin(commandsearch); return new Symbol(sym.LITERAL, yytext()); }
<commandsearchastring> {SP} { return new Symbol(sym.SP); }
<commandsearchheader> {astring} { yybegin(commandsearchastring); return new Symbol(sym.ASTRING, yytext()); }
<commandsearchheader> {quoted} { yybegin(commandsearchastring); return new Symbol(sym.QUOTED, yytext()); }
<commandsearchheader> {literal} { yybegin(commandsearchastring); return new Symbol(sym.LITERAL, yytext()); }
<commandsearchheader> {SP} { return new Symbol(sym.SP); }
<commandsearchuidsequence> {SP} { yybegin(commandsearchsequence); return new Symbol(sym.SP); }
<commandsearchsequence> "*" { return new Symbol(sym.STAR); }
<commandsearchsequence> "," { return new Symbol(sym.COMMA); }
<commandsearchsequence> ":" { return new Symbol(sym.COLON); } 
<commandsearchsequence> {number} { return new Symbol(sym.NUMBER, yytext()); }
<commandsearchsequence> (DUMMY_FOR_NOTHING)? { yybegin(commandsearch); break; }
<YYINITIAL> . { return new Symbol(sym.OTHER, yytext()); }

