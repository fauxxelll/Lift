namespace LiftParser.Lexing
{
    public class Token(TokenType type, int line, string lexeme, object? value)
    {
        public TokenType Type { get; } = type;
        public int Line { get; } = line;
        public string Lexeme { get; } = lexeme;
        public object? Value { get; } = value;
    }
}
