using System;
using System.Collections.Generic;
using System.Text;

using Irony.Interpreter;
using Irony.Parsing;

namespace MNGDraw
{
    //https://taiyakisun.hatenablog.com/entry/2019/01/04/011829
    [Language("MyNumbersGrammar")]
    class NumbersGrammar : InterpretedLanguageGrammar
    {
        public NumbersGrammar() : base(true)
        {
            // 一行コメント(//)や、複数行コメント(/* ... */)、その他文法上無視するものの指定
            var singleLineComment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
            var delimitedComment = new CommentTerminal("DelimitedComment", "/*", "*/");
            NonGrammarTerminals.Add(singleLineComment);
            NonGrammarTerminals.Add(delimitedComment);

            // 1. Terminals
            NumberLiteral Number = new NumberLiteral("number");
            KeyTerm AddOperator = ToTerm("+");
            KeyTerm SubOperator = ToTerm("-");
            KeyTerm MulOperator = ToTerm("*");
            KeyTerm DivOperator = ToTerm("/");
            KeyTerm PowOperator = ToTerm("**");
            KeyTerm LeftParen = ToTerm("(");
            KeyTerm RightParen = ToTerm(")");
            KeyTerm EndOfSentence = ToTerm(";");

            // 2. Non-Terminals
            var Program = new NonTerminal("Program", typeof(NumbersGrammarProgramNode));

            var Statement = new NonTerminal("Statement", typeof(NumbersGrammarStatementNode));

            var ExprAddSub = new NonTerminal("ExprAddSub", typeof(NumbersGrammarExprNode));
            var ExprMulDiv = new NonTerminal("ExprMulDiv", typeof(NumbersGrammarExprNode));
            var Expr = new NonTerminal("Expr", typeof(NumbersGrammarExprNode));
            var ParenExpr = new NonTerminal("ParenExpr", typeof(NumbersGrammarParenExprNode));

            var UnExpr = new NonTerminal("UnExpr", typeof(NumbersGrammarUnExprNode));
            var UnOp = new NonTerminal("UnOp", typeof(NumbersGrammarUnOpNode));

            // 3. BNF
            Program.Rule = MakeStarRule(Program, Statement);

            Statement.Rule = ExprAddSub + EndOfSentence;

            ExprAddSub.Rule = ExprMulDiv
                    | ExprAddSub + AddOperator + ExprAddSub
                    | ExprAddSub + SubOperator + ExprAddSub
                    ;

            ExprMulDiv.Rule = Expr
                    | ExprMulDiv + MulOperator + ExprMulDiv
                    | ExprMulDiv + DivOperator + ExprMulDiv
                    | ExprMulDiv + PowOperator + ExprMulDiv
                    ;

            Expr.Rule = ParenExpr
                   | UnExpr
                   | Number;

            ParenExpr.Rule = LeftParen + ExprAddSub + RightParen;

            UnExpr.Rule = UnOp + ParenExpr | UnOp + Number;

            UnOp.Rule = ToTerm("+") | "-";

            this.Root = Program;

            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "**", "/");

            LanguageFlags = LanguageFlags.CreateAst;
        }
    }
}
