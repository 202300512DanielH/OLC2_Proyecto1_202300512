grammar Language;

program: dcl*;

dcl: varDcl| funcDcl| stmt;

varDcl: 'var' ID '=' expr ';';

funcDcl: 'function' ID '(' params? ')' '{' dcl* '}';
params: ID (',' ID)*;

stmt: 
	expr ';' # ExprStmt 
	| '{' dcl* '}' # BlockStmt
	| 'if' '(' expr ')' stmt ('else' stmt)? # IfStmt
	| 'while' '(' expr ')' stmt # WhileStmt
	| 'for' '(' forInit expr ';' expr ')' stmt	# ForStmt
	| 'break' ';' # BreakStmt
	| 'continue' ';' # ContinueStmt
	| 'return' expr? ';' # ReturnStmt;

forInit: varDcl | expr ';';

expr:
	'-' expr						# Negate
	| expr call+ 				# Callee
	| expr op = ('*' | '/') expr	# MulDiv
	| expr op = ('+' | '-') expr	# AddSub
	| expr op = ('>' | '<' | '>=' | '<=' ) expr # Relational
	| expr op = ('==' | '!=') expr	# Equalitys

	| ID '=' expr					# Assign
	|BOOL							# Boolean
	|FLOAT							# Float
	|STRING							# String
	| INT							# Int 
	| ID							# Identifier
	| '(' expr ')'					# Parens;

call: '(' args? ')';
args: expr (',' expr)*;


INT: [0-9]+;
BOOL: 'true' | 'false';
FLOAT: [0-9]+ '.' [0-9]+;
STRING: '"' .*? '"';

WS: [ \t\r\n]+ -> skip;
ID: [a-zA-Z]+;

COMMENT: '//' ~[\r\n]* -> skip;
ML_COMMENT: '/*' .*? '*/' -> skip;