using System;
using System.Collections.Generic;
using System.Text;


using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;


namespace MNGDraw
{
    public class NumbersGrammarProgramNode : AstNode
    {
        // プログラム全体は複数の数字を結果として持つ

        public List<AstNode> numList = new List<AstNode>();

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            ParseTreeNodeList nodes = treeNode.GetMappedChildNodes();
            foreach (var node in nodes)
            {
                this.numList.Add(AddChild("numList", node));
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;

            // すべての文を評価した結果得られた整数値の合計を返す
            int result = 0;
            foreach (var num in this.numList)
            {
                result += (int)num.Evaluate(thread);
            }

            thread.CurrentNode = Parent;

            return result;
        }
    }
}
