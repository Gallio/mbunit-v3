grammar GallioFilter;

options
{
    //output=AST;
    //ASTLabelType=CommonTree;
    language=CSharp;
}

@members
{
    /*public class InvalidInputException : Exception
    {
    	public InvalidInputException(string msg) : base(msg) {}
    }
    public override void EmitErrorMessage(string msg)
    {
        throw new InvalidInputException(msg);
    }*/
}

@header
{
	using System.Collections.Generic;
}

@lexer::namespace {
	Gallio.Model.Filters
}

@parser::namespace {
	Gallio.Model.Filters
}

filter	:
		orFilter { Console.WriteLine($orFilter.value); }
	;

orFilter  returns [string value]
	:	a1=andFilter (('|' | 'or')  a2=andFilter)* 
		{
			if ($a2.value != null)
				 $value = "new OrFilter(" + $a1.value + "," + $a2.value + ")"; 
			else
				$value = $a1.value;
		}
	;
		
andFilter returns [string value]
	:	n1=negationFilter (('&' | 'and') n2=negationFilter)* 
		{ 
			if ($n2.value != null)
				$value = "new AndFilter<T>(" + $n1.value + "," + $n2.value + ")"; 
			else
				$value = $n1.value;
		}
	;

negationFilter returns [string value]
	:	negation=('!' | 'not')* parenthesizedFilter 
		{
			if ($negation != null)
				$value = "new NotFilter<T>(" + $parenthesizedFilter.value + ")"; 
			else
				$value = $parenthesizedFilter.value;
		}
	;

parenthesizedFilter returns [string value]
	: 	
		key ':' matchSequence 
		{
			$value = "new AndFilter<T>((new List<string>(){";
			foreach(string s in $matchSequence.expVal)
			{
				$value += "BuildFilter<T>(" + $key.text + "," + s + ")";
				if (s != $matchSequence.expVal[$matchSequence.expVal.Count - 1])
					$value += ',';
			}
			$value += "}).ToArray())";
		}
	|	'(' orFilter ')' 
		{
			$value = $orFilter.value;
		}
	;

matchSequence returns [List<string> expVal]
@init
{
	$expVal = new List<string>();
}
	:	v1=value (',' v2=value { $expVal.Add($v2.text); })* { $expVal.Add($v1.text); }
	;

key	:	word 
	;

value	:	word
	|	'~' word
	;

word	:	STRING
	|	'"' (~'"')* '"' // Quoted String
	|	'\'' (~'\'')* '\'' // Quoted String
	;

STRING	:	('0'..'9' | 'a'..'z' | 'A'..'Z' | '*' | '_' | '-' | '+' | '.')+
	;
NEWLINE :	'\r'? '\n' 
	; 
WS	:	(' ' |'\t' |'\n' |'\r' )+ { Skip(); } ;
