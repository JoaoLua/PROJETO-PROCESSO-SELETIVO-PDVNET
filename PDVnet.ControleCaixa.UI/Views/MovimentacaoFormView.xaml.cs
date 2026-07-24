using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace PDVnet.ControleCaixa.UI.Views
{
    public partial class MovimentacaoFormView : UserControl
    {
        public MovimentacaoFormView()
        {
            InitializeComponent();
        }

        private void ValidarEntradaNumerica(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var textoAtual = textBox?.Text ?? "";

            if (!Regex.IsMatch(e.Text, @"^[0-9,.]$"))
            {
                e.Handled = true;
                return;
            }

            if ((e.Text == "," || e.Text == ".") && (textoAtual.Contains(',') || textoAtual.Contains('.')))
            {
                e.Handled = true;
            }
        }
    }
}
