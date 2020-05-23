using System;
using System.Collections.Generic;
using System.Text;


using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;


namespace MNGDraw
{
    public class NumbersGrammarUnOpNode : AstNode
    {
        public AstNode unOpNode = null;

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            ParseTreeNodeList nodes = treeNode.GetMappedChildNodes();

            this.unOpNode = AddChild("unOpNode", nodes[0]);
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;

            string result = (string)this.unOpNode.Term.ToString();

            thread.CurrentNode = Parent;

            return result;
        }
    }
}
