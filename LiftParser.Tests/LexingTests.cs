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
    }
}