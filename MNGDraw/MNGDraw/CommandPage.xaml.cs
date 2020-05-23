using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Irony.Interpreter;
using Irony.Parsing;

namespace MNGDraw
{
    

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommandPage : ContentPage
    {

        NumbersGrammar grammar = new NumbersGrammar();
        LanguageData language;


        public CommandPage()
        {
            InitializeComponent();
            language = new LanguageData(grammar);

        }

        private void SubmitButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var app = new ScriptApp(language);
                var command = this.CommandEntry.Text;

                if (string.IsNullOrEmpty(command))
                {
                    this.StdOut.Text = "null";
                    return;
                }

                int result = (int)app.Evaluate(command);
                this.StdOut.Text = result.ToString();
            }
            catch (ScriptException ex)
            {
                this.StdOut.Text = ex.Location + " " + ex.Message;
            }

        }
    }
}