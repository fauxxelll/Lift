namespace LiftParser.Lexing
{
    public enum TokenType
    {
        Integer, Real, String,

        Identifier,

        Plus, Minus, Star, Slash, Percent, Equal, Dot, Comma, Less, Greater,
        Ampersand, Bang, QuestionMark, Colon, At, Semicolon, Pipe, Octothorpe,

        EqEq, NEq, LEq, GEq, PlusEq, MinusEq, StarEq, SlashEq, PercentEq,
        PlusPlus, MinusMinus, AmpAmp, PipePipe,

        LParen, RParen, LBrace, RBrace, LBracket, RBracket,

        Function, Class, Const, Private, ErrorKW, For, Return, While, If, Else,
        Null, True, False,
    }
}
