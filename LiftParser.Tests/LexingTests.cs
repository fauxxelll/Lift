using LiftParser.Lexing;

namespace LiftParser.Tests
{
    public class LexingTests
    {
        private readonly Lexer lexer = new();

        [Fact]
        public void IntegerTest()
        {
            Random rng = new((int)DateTime.UtcNow.Ticks);

            const int GEN_LIMIT = 1000;

            for (int _ = 0; _ < GEN_LIMIT; _++)
            {
                int[] values = [
                    rng.Next(),
                    rng.Next(),
                    rng.Next()
                ];

                List<Token> tokens = lexer.Lex($"{values[0]} {values[1]}\n{values[2]}");

                Assert.Equal(3, tokens.Count);

                foreach (Token token in tokens)
                {
                    Assert.Equal(TokenType.Integer, token.Type);
                }

                Assert.Equal(1, tokens[0].Line);
                Assert.Equal(1, tokens[1].Line);
                Assert.Equal(2, tokens[2].Line);

                for (int i = 0; i < 3; i++)
                {
                    Assert.Equal(values[i].ToString(), tokens[i].Lexeme);
                    Assert.Equal((double)values[i], tokens[i].Value);
                }
            }
        }

        [Fact]
        public void DoubleTest()
        {
            Random rng = new((int)DateTime.UtcNow.Ticks);

            const int GEN_LIMIT = 1000;

            for (int _ = 0; _ < GEN_LIMIT; _++)
            {
                double[] values = [
                    rng.NextDouble(),
                    rng.NextDouble(),
                    rng.NextDouble()
                ];

                List<Token> tokens = lexer.Lex($"{values[0]} {values[1]}\n{values[2]}");

                Assert.Equal(3, tokens.Count);

                foreach (Token token in tokens)
                {
                    Assert.Equal(TokenType.Real, token.Type);
                }

                Assert.Equal(1, tokens[0].Line);
                Assert.Equal(1, tokens[1].Line);
                Assert.Equal(2, tokens[2].Line);

                for (int i = 0; i < 3; i++)
                {
                    Assert.Equal(values[i].ToString(), tokens[i].Lexeme);
                    Assert.Equal(values[i], tokens[i].Value);
                }
            }
        }

        [Fact]
        public void StringTest()
        {
            Random rng = new((int)DateTime.UtcNow.Ticks);

            const int GEN_LIMIT = 1000;

            for (int _ = 0; _ < GEN_LIMIT; _++)
            {
                int len = rng.Next(10, 100);

                string code = "\"";

                for (int _2 = 0; _2 < len; _2++)
                {
                    char c = (char)rng.Next(0x20, 0x7A);
                    while (c == '"')
                    {
                        c = (char)rng.Next(0x20, 0x7A);
                    }
                    code += c;
                }

                code += "\"";

                List<Token> tokens = lexer.Lex(code);

                Assert.Equal(len + 2, tokens[0].Lexeme.Length);
                Assert.Equal(1, tokens[0].Line);
                Assert.Equal(code, tokens[0].Lexeme);
                Assert.Equal(code[1..^1], tokens[0].Value);
            }
        }

        [Fact]
        public void IdentifierTest()
        {
            Random rng = new((int)DateTime.UtcNow.Ticks);

            const int GEN_LIMIT = 1000;

            TokenType[] tokenTypes = new TokenType[GEN_LIMIT];
            string[] tokenLexemes = new string[GEN_LIMIT];

            string code = "";

            for (int i = 0; i < GEN_LIMIT; i++)
            {
                int kw = rng.Next(2);

                if (kw == 0)
                {
                    int index = rng.Next(Lexer.KEYWORDS.Count);

                    code += Lexer.KEYWORDS.Keys.ElementAt(index);

                    tokenTypes[i] = Lexer.KEYWORDS.Values.ElementAt(index);
                    tokenLexemes[i] = Lexer.KEYWORDS.Keys.ElementAt(index);
                }
                else
                {
                    int len = rng.Next(10, 100);

                    string identifier = "";

                    char c = (char)rng.Next(0x20, 0x7A);
                    while (!Lexer.IsIdentifierStart(c))
                    {
                        c = (char)rng.Next(0x20, 0x7A);
                    }
                    identifier += c;

                    for (int _ = 1; _ < len; _++)
                    {
                        c = (char)rng.Next(0x20, 0x7A);
                        while (!Lexer.IsIdentifier(c))
                        {
                            c = (char)rng.Next(0x20, 0x7A);
                        }
                        identifier += c;
                    }

                    code += identifier;

                    tokenTypes[i] = TokenType.Identifier;
                    tokenLexemes[i] = identifier;
                }

                code += ' ';
            }

            List<Token> tokens = lexer.Lex(code);

            for (int i = 0; i < GEN_LIMIT; i++)
            {
                Assert.Equal(tokenTypes[i], tokens[i].Type);
                Assert.Equal(tokenLexemes[i], tokens[i].Lexeme);
                Assert.Equal(tokenLexemes[i], tokens[i].Value);
                Assert.Equal(1, tokens[i].Line);
            }
        }
    }
}