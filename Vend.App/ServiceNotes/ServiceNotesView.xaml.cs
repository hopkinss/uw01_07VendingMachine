using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Linq;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Vend.App.ServiceNotes
{
    /// <summary>
    /// Interaction logic for ServiceNotesView.xaml
    /// </summary>
    public partial class ServiceNotesView : Window
    {

        private List<ServiceNote> serviceNotes;
        private Thickness margin = new Thickness(2, 0, 2, 0);
        private Brush borderBrush = Brushes.DeepSkyBlue;


        public List<ServiceNote> ServiceNotes
        {
            get { return serviceNotes; }
            set { serviceNotes = value; }
        }


        public ServiceNotesView()
        {
            InitializeComponent();
            this.serviceNotes = new List<ServiceNote>();
        }

        private StackPanel CreateServiceNote(string text = null)
        {
            var sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };

            var cb = new CheckBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = margin,
                Width=80,               
                BorderBrush = borderBrush,
                IsChecked=true,
               
            };

            cb.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(EnableSave_Checked));
            cb.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(EnableSave_Checked));

            var dd = new ComboBox()
            {
                ItemsSource = ServiceNote.GetIssueTypes(),
                SelectedValuePath = "Key",
                DisplayMemberPath = "Value",
                BorderBrush= borderBrush,
                Background=Brushes.AliceBlue,
                Width=120,
                SelectedIndex=0
            };

            var tb = new  TextBox()
            {
                BorderBrush = borderBrush,
                Padding = new Thickness(2),
                Margin = margin,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                Text=text,
                Width=500
            };           
            sp.Children.Add(cb);
            sp.Children.Add(dd);
            sp.Children.Add(tb);
            return sp;
        }

        // If any of the service note are checked, then enable save
        private void EnableSave_Checked(object s, RoutedEventArgs e)
        {
            var canSave = false;
            foreach (var c in sPanServiceNotes.Children)
            {
                foreach(var cc in ((StackPanel)c).Children)
                {
                    if (cc.GetType()==typeof(CheckBox))
                    {
                        if (((CheckBox)cc).IsChecked == true)
                            canSave = true;
                    }
                }
            }
            menuItemSave.IsEnabled = canSave ;
        }

        private void MnuItmNew_Click(object sender, RoutedEventArgs e)
        {
            sPanServiceNotes.Children.Add(CreateServiceNote());
        }

        private void MnuItmOpen_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Title = "Select a file to load into a comment"
            };
            if (ofd.ShowDialog()==true)
            {
                sPanServiceNotes.Children
                    .Add(CreateServiceNote(string.Join("\n", File.ReadAllLines(ofd.FileName))));
            }
        }

        private void MnuItmSave_Click(object sender, RoutedEventArgs e)
        {
            GetContent();
            if (this.serviceNotes.Count==0)
            {
                MessageBox.Show("Select at least one record to add to the database");
                return;
            }
            var sfd = new SaveFileDialog()
            {
                Title = "Save service notes to JSON file",
                Filter = "JSON | *.json",
                FileName=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"service_notes"),
                DefaultExt="json",
                CreatePrompt=false,
                OverwritePrompt=false,
            };
            if (sfd.ShowDialog() == true)
            {                
                SaveAppendJson(sfd.FileName);
                if( MessageBox.Show($"Updated data file: {sfd.FileName}.\n\nDo you with to open the file?",
                    "File saved",MessageBoxButton.YesNo,MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    Process.Start(sfd.FileName);
                }
            }
            else
            {
                this.serviceNotes.Clear();
            }
        }

        private void MnuItmExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }

        private void GetContent()
        {
            foreach (var c in sPanServiceNotes.Children)
            {
                if (((CheckBox)((StackPanel)c).Children[0]).IsChecked ?? false)
                {
                    var issue = new ServiceNote()
                    {
                        IssueCategory = (ServiceNote.IssueType)(int)((ComboBox)((StackPanel)c).Children[1]).SelectedValue,
                        Note = ((TextBox)((StackPanel)c).Children[2]).Text,
                        Logged = DateTime.Now,
                        LoggedBy = Environment.UserName
                    };

                    this.serviceNotes.Add(issue);
                }
            }
        }

        public void SaveAppendJson(string fn)
        {
            if (this.serviceNotes.Count>0)
            {
                try
                {
                    
                    if (File.Exists(fn))
                    {
                        var existingData = JsonConvert.DeserializeObject<List<ServiceNote>>(File.ReadAllText(fn));
                        this.serviceNotes.AddRange(existingData);
                    }
                    using (var sw = new StreamWriter(fn))
                    {
                        sw.Write(JsonConvert.SerializeObject(this.serviceNotes));
                    }
                    sPanServiceNotes.Children.Clear();
                }
                catch(Exception ee)
                {
                    MessageBox.Show($"Error encountered updating the database: {ee.Message}");
                }
            }

        }




    }
}
