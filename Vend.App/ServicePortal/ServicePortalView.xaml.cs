using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vend.App.ServiceNotes;

namespace Vend.App.ServicePortal
{
    /// <summary>
    /// Interaction logic for ServicePortalView.xaml
    /// </summary>
    public partial class ServicePortalView : UserControl
    {
        public ServicePortalView()
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
