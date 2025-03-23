namespace LiftParser.Lexing
{
    public class Lexer()
    {
        public static readonly Dictionary<string, TokenType> KEYWORDS = new()
        {
            { "fn", TokenType.Function },
            { "class", TokenType.Class },
            { "const", TokenType.Const },
            { "private", TokenType.Private },
            { "error", TokenType.ErrorKW },
            { "for", TokenType.For },
            { "return", TokenType.Return },
            { "while", TokenType.While },
            { "if", TokenType.If },
            { "else", TokenType.Else },
            { "null", TokenType.Null },
            { "true", TokenType.True },
            { "false", TokenType.False }
        };

        private int _start = 0, _current = 0;
        private int _line = 1;
        private string _source = "";

        private char Previous => _source[_current - 1];
        private char Peek => _source[_current];
        private bool AtEnd => _current >= _source.Length;

        public List<Token> Lex(string source)
        {
            _start = 0;
            _current = 0;
            _line = 1;
            _source = source;

            List<Token> tokens = [];

            while (!AtEnd)
            {
                _start = _current;

                switch (Advance())
                {
                    case ' ':
                    case '\r':
                    case '\t':
                    case '\f':
                        break;

                    case '\n':
                        _line++;
                        break;

                    case '+':
                        if (Match('='))
                        {
                            tokens.Add(Token(TokenType.PlusEq));
                            break;
                        }
                        if (Match('+'))
                        {
                            tokens.Add(Token(TokenType.PlusPlus));
                            break;
                        }
                        tokens.Add(Token(TokenType.Plus));
                        break;

                    case '-':
                        if (Match('='))
                        {
                            tokens.Add(Token(TokenType.MinusEq));
                            break;
                        }
                        if (Match('-'))
                        {
                            tokens.Add(Token(TokenType.MinusMinus));
                            break;
                        }
                        tokens.Add(Token(TokenType.Minus));
                        break;

                    case '*':
                        if (Match('='))
                        {
                            tokens.Add(Token(TokenType.StarEq));
                            break;
                        }
                        tokens.Add(Token(TokenType.Star));
                        break;

                    case '/':
                        if (Match('/'))
                        {
                            while (Peek != '\n')
                            {
                                Advance();
                            }
                            break;
                        }
                        if (Match('*'))
                        {
                            Advance();

                            while (Previous != '*' || Peek != '/')
                            {
                                Advance();
                            }

                            break;
                        }
                        if (Match('='))
                        {
                            tokens.Add(Token(TokenType.SlashEq));
                            break;
                        }
                        tokens.Add(Token(TokenType.Slash));
                        break;

                    case '%':
                        if (Match('='))
                        {
                            tokens.Add(Token(TokenType.PercentEq));
                            break;
                        }
                        tokens.Add(Token(TokenType.Percent));
                        break;

                    case '<':
                        if (Match('='))
                        {
                            tokens.Add(Token(TokenType.LEq));
                            break;
                        }
                        tokens.Add(Token(TokenType.Less));
                        break;

                    case '>':
                        if (Match('='))
                        {
                            tokens.Add(Token(TokenType.GEq));
                            break;
                        }
                        tokens.Add(Token(TokenType.Greater));
                        break;

                    case '=':
                        if (Match('='))
                        {
                            tokens.Add(Token(TokenType.EqEq));
                            break;
                        }
                        tokens.Add(Token(TokenType.Equal));
                        break;

                    case '!':
                        if (Match('='))
                        {
                            tokens.Add(Token(TokenType.NEq));
                            break;
                        }
                        tokens.Add(Token(TokenType.Bang));
                        break;

                    case '&':
                        if (Match('&'))
                        {
                            tokens.Add(Token(TokenType.AmpAmp));
                            break;
                        }
                        tokens.Add(Token(TokenType.Ampersand));
                        break;

                    case '|':
                        if (Match('|'))
                        {
                            tokens.Add(Token(TokenType.PipePipe));
                            break;
                        }
                        tokens.Add(Token(TokenType.Pipe));
                        break;

                    case '.':
                        tokens.Add(Token(TokenType.Dot));
                        break;

                    case ',':
                        tokens.Add(Token(TokenType.Comma));
                        break;

                    case '?':
                        tokens.Add(Token(TokenType.QuestionMark));
                        break;

                    case ':':
                        tokens.Add(Token(TokenType.Colon));
                        break;

                    case ';':
                        tokens.Add(Token(TokenType.Semicolon));
                        break;

                    case '#':
                        tokens.Add(Token(TokenType.Octothorpe));
                        break;

                    case '@':
                        tokens.Add(Token(TokenType.At));
                        break;

                    case '(':
                        tokens.Add(Token(TokenType.LParen));
                        break;

                    case ')':
                        tokens.Add(Token(TokenType.RParen));
                        break;

                    case '{':
                        tokens.Add(Token(TokenType.LBrace));
                        break;

                    case '}':
                        tokens.Add(Token(TokenType.RBrace));
                        break;

                    case '[':
                        tokens.Add(Token(TokenType.LBracket));
                        break;

                    case ']':
                        tokens.Add(Token(TokenType.RBracket));
                        break;

                    case '"':
                        tokens.Add(StringToken());
                        break;

                    default:
                        if (char.IsAsciiDigit(Previous))
                        {
                            tokens.Add(NumberToken());
                            break;
                        }
                        if (IsIdentifierStart(Previous))
                        {
                            tokens.Add(Identifier());
                            break;
                        }


                        break;
                }
            }

            return tokens;
        }

        private Token StringToken()
        {
            while (Advance() != '"')
            {
                if (Previous == '\n')
                {
                    _line++;
                }
            }

            return new Token(TokenType.String, _line, _source[_start.._current], _source[(_start + 1)..(_current - 1)]);
        }

        private Token NumberToken()
        {
            bool real = false;

            if (!AtEnd && Peek != '.')
            {
                while (!AtEnd && char.IsAsciiDigit(Peek)) Advance();
            }

            if (!AtEnd && Peek == '.')
            {
                Advance();
                real = true;
                while (!AtEnd && char.IsAsciiDigit(Peek)) Advance();
            }

            TokenType type = TokenType.Integer;
            if (real) type = TokenType.Real;

            string num = _source[_start.._current];

            return Token(type, real ? double.Parse(num) : int.Parse(num));
        }

        private Token Identifier()
        {
            while (IsIdentifier(Peek)) Advance();

            string identifier = _source[_start.._current];
            TokenType type = TokenType.Identifier;

            if (KEYWORDS.TryGetValue(identifier, out TokenType value))
            {
                type = value;
            }

            return Token(type, identifier);
        }

        public static bool IsIdentifierStart(char c) => char.IsAsciiLetter(c) || c == '_';

        public static bool IsIdentifier(char c) => IsIdentifierStart(c) || char.IsAsciiDigit(c);

        private bool Match(char c)
        {
            if (Peek == c)
            {
                Advance();
                return true;
            }

            return false;
        }

        private Token Token(TokenType type, object? value = null)
        {
            return new Token(type, _line, _source[_start.._current], value);
        }

        private char Advance()
        {
            if (AtEnd) return Previous;
            return _source[_current++];
        }
    }
}
