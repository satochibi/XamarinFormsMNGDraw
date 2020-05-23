using System;
using System.Collections.Generic;
using System.Text;

using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;


namespace MNGDraw
{
    public class NumbersGrammarUnExprNode : AstNode
    {
        public AstNode unOpNode = null;
        public AstNode termNode = null;

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            ParseTreeNodeList nodes = treeNode.GetMappedChildNodes();

            this.unOpNode = AddChild("unOpNode", nodes[0]);
            this.termNode = AddChild("termNode", nodes[1]);
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;

            // "-"なら-1を掛けて負の値にする。それ以外は+とする。
            int result = (int)this.termNode.Evaluate(thread) * (((string)this.unOpNode.Evaluate(thread)).Equals("-") ? -1 : +1);

            thread.CurrentNode = Parent;

            return result;
        }
    }
}
