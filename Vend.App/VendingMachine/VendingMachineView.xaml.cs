using System.Windows;
using System.Windows.Controls;
using Vend.App.ServiceNotes;

namespace Vend.App.VendingMachine
{

    public partial class VendingMachineView : UserControl
    {
        public VendingMachineView()
        {
            InitializeComponent();
        }

        private void ShowServiceNotes_Click(object sender, RoutedEventArgs e)
        {
            var serviceNotes = new ServiceNotesView();
            serviceNotes.Show();

        }
    }
}
