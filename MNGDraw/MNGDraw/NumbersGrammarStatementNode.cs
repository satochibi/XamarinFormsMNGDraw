using System;
using System.Collections.Generic;
using System.Text;


using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;


namespace MNGDraw
{
    public class NumbersGrammarStatementNode : AstNode
    {
        public AstNode exprNode = null;

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            ParseTreeNodeList nodes = treeNode.GetMappedChildNodes();
            this.exprNode = AddChild("exprNode", nodes[0]);  // 数字のみ取り出す。nodes[1]の";"は無視。
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;

            // 単一式を評価して、整数を取得する
            int number = (int)exprNode.Evaluate(thread);

            thread.CurrentNode = Parent;

            return number;
        }
    }
}
