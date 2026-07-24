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
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
