using System;
using System.Collections.Generic;
using System.Text;

using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace MNGDraw
{
    public class NumbersGrammarParenExprNode : AstNode
    {
        public AstNode middleExprNode = null;

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            ParseTreeNodeList nodes = treeNode.GetMappedChildNodes();

            this.middleExprNode = AddChild("middleExprNode", nodes[1]);   // 両端の(と)は無視して、間だけを評価する
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;

            int result = (int)this.middleExprNode.Evaluate(thread);

            thread.CurrentNode = Parent;

            return result;
        }
    }
}
