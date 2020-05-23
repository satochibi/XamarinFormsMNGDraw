using System;
using System.Collections.Generic;
using System.Text;


using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;


namespace MNGDraw
{
    public class NumbersGrammarExprNode : AstNode
    {
        public AstNode[] numberNodes = null;

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            ParseTreeNodeList nodes = treeNode.GetMappedChildNodes();

            this.numberNodes = new AstNode[nodes.Count];

            for (int i = 0; i < nodes.Count; ++i)
            {
                this.numberNodes[i] = AddChild("numberNode", nodes[i]);
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;

            int number = 0;
            if (this.numberNodes.Length == 1)
            {
                // 要素1つの場合
                number = (int)this.numberNodes[0].Evaluate(thread);
            }
            else
            {
                // 要素3つの場合(と仮定)

                int nLeft = (int)this.numberNodes[0].Evaluate(thread);
                int nRight = (int)this.numberNodes[2].Evaluate(thread);

                // 演算子はこれ以上評価できないので、取得した文字列を直接参照する。
                string strBinOp = this.numberNodes[1].Term.Name;
                if (strBinOp.Equals("+"))
                {
                    number = nLeft + nRight;
                }
                else if (strBinOp.Equals("-"))
                {
                    number = nLeft - nRight;
                }
                else if (strBinOp.Equals("*"))
                {
                    number = nLeft * nRight;
                }
                else if (strBinOp.Equals("/"))
                {
                    number = nLeft / nRight;
                }
                else
                {
                    // **(べき乗)と仮定する。
                    number = (int)Math.Pow(nLeft, nRight);
                }

            }

            thread.CurrentNode = Parent;

            return number;
        }
    }
}
