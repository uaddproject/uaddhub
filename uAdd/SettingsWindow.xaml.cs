using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using BencodeLibrary;

namespace uAdd
{

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {

        private string host;
        private string user;        
        private bool   _FailedRetries;        
        private bool   _NotAllowEmptyCategory;
        private bool   _BoldFontCategory;
        private bool   _BigFontCategory;
        private bool   _NotAddDefaultCategory;

        private TorrentAPI TServerConnector=null;
        private bool? Connected = null;

        ObservableCollection<СategoriesDataRow> _CategoriesDataSet = new ObservableCollection<СategoriesDataRow>();
                
        public string Host {get { return host; }  set { host = value; OnPropertyChanged("Host"); } }
        public string User { get { return user; } set { user = value; OnPropertyChanged("User"); } }                        
        public bool FailedRetries { get { return _FailedRetries; } set { _FailedRetries = value; OnPropertyChanged("FailedRetries"); } }        
        public bool NotAllowEmptyCategory { get { return _NotAllowEmptyCategory; } set { _NotAllowEmptyCategory = value; OnPropertyChanged("NotAllowEmptyCategory"); } }
        public bool BoldFontCategory { get { return _BoldFontCategory; } set { _BoldFontCategory = value; OnPropertyChanged("BoldFontCategory"); } }
        public bool BigFontCategory { get { return _BigFontCategory; } set { _BigFontCategory = value; OnPropertyChanged("BigFontCategory"); } }
        public bool NotAddDefaultCategory { get { return _NotAddDefaultCategory; } set { _NotAddDefaultCategory = value; OnPropertyChanged("NotAddDefaultCategory"); } }
        

        #region Implementation of INotifyPropertyChanged

        public ObservableCollection<СategoriesDataRow> CategoriesData { get { return _CategoriesDataSet; } set { _CategoriesDataSet = value; OnPropertyChanged("CategoriesData"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion        

        
        public SettingsWindow()
        {
            // set current culture for debug
            //System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            InitializeComponent();
            
            this.DataContext = this;

            // read settings data            
            ReadSettingData();
            
            TServerConnector = new TorrentAPI();

        }

        ~SettingsWindow()
        {
            
        }

        private void ReadSettingData()
        {
            Host = Properties.Settings.Default.Host;
            User = Properties.Settings.Default.User;
            SettingsPasswordBox.Password = Properties.Settings.Default.Password;            
            FailedRetries  = Properties.Settings.Default.FailedRetries;            
            NotAllowEmptyCategory = Properties.Settings.Default.NotAllowEmptyCategory;
            BoldFontCategory = Properties.Settings.Default.BoldFontCategory;
            BigFontCategory = Properties.Settings.Default.BigFontCategory;
            NotAddDefaultCategory = Properties.Settings.Default.NotAddDefaultCategory;

            if (Properties.Settings.Default.CategoriesTable != "")
            {                
                CategoriesData = uAdd.App.DeserializeString<ObservableCollection<СategoriesDataRow>>(Properties.Settings.Default.CategoriesTable);
            }

        }
        private void WriteSettingData()
        {
            Properties.Settings.Default.Host = Host;
            Properties.Settings.Default.User = User;
            Properties.Settings.Default.Password = SettingsPasswordBox.Password;            
            Properties.Settings.Default.FailedRetries  = FailedRetries;            
            Properties.Settings.Default.NotAllowEmptyCategory = NotAllowEmptyCategory;            
            Properties.Settings.Default.CategoriesTable = uAdd.App.SerializeToString(CategoriesData);
            Properties.Settings.Default.BoldFontCategory = BoldFontCategory;
            Properties.Settings.Default.BigFontCategory = BigFontCategory;
            Properties.Settings.Default.NotAddDefaultCategory = NotAddDefaultCategory;

            Properties.Settings.Default.Save();
        }

        private void SettingsTestBtn_Click(object sender, RoutedEventArgs e)
        {
            bool Tresult = TServerConnector.Connect(Host, User, SettingsPasswordBox.Password);

            if (Tresult == true && TServerConnector.IsConnected() == true)
            {
                MessageBox.Show(uAdd.Properties.Resources.MsgServerOK + "\"" + TServerConnector.GetLastHost() + "\"", uAdd.Properties.Resources.MsgServerConnection);
            }
            else
            {
                MessageBox.Show(uAdd.Properties.Resources.MsgServerError + "\"" + TServerConnector.GetLastHost() + "\"", uAdd.Properties.Resources.MsgServerConnection);
            }
            
        }

        private void SettingsOKBtn_Click(object sender, RoutedEventArgs e)
        {
            WriteSettingData();
            if (this.Owner != null && Owner.GetType().FullName == "uAdd.MainWindow")
            {
                ((uAdd.MainWindow)Owner).LoadCatageriesList();
                ((uAdd.MainWindow)Owner).UpdateVisibleControl();
            }
            this.Close();
        }

        private void SettingsCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void CategoriesGridCommandAdd(object sender, RoutedEventArgs e)
        {
            СategoriesDataRow NewRowObj = new СategoriesDataRow();

            CategoriesData.Add(NewRowObj);
            CategoriesDataGrid.SelectedItem = NewRowObj;                                               
        }

        private void CategoriesGridCommandDelete(object sender, RoutedEventArgs e)
        {
            if(CategoriesDataGrid.CurrentItem !=null)
            {
                CategoriesData.Remove((uAdd.СategoriesDataRow) CategoriesDataGrid.CurrentItem);
            }

        }

        private void CategoriesGridCommandUp(object sender, RoutedEventArgs e)
        {
            CategoriesGridMoveRow(true);
        }

        private void CategoriesGridCommandDown(object sender, RoutedEventArgs e)
        {
            CategoriesGridMoveRow(false);
        }

        private void CategoriesGridMoveRow(bool DirectionUp)
        {
            if (CategoriesDataGrid.CurrentItem != null)
            {
                СategoriesDataRow SelRow = (uAdd.СategoriesDataRow)CategoriesDataGrid.CurrentItem;
                int SelIndex = CategoriesDataGrid.SelectedIndex;
                int NewIndex = SelIndex;

                if (DirectionUp == true && SelIndex > 0 )
                {
                    NewIndex = SelIndex-1;
                }
                else if (DirectionUp == false && SelIndex < (CategoriesData.Count()-1))
                {
                    NewIndex = SelIndex+1;
                }
                
                CategoriesData.Remove((uAdd.СategoriesDataRow)CategoriesDataGrid.CurrentItem);
                
                CategoriesData.Insert(NewIndex, SelRow);

                CategoriesDataGrid.SelectedItem = SelRow;
            }

        }
        
        private void CategoriesGridCommandLoadFromLink(object sender, RoutedEventArgs e)
        {
            if (CategoriesData.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show(uAdd.Properties.Resources.MsgNotEmptyCategorisData, uAdd.Properties.Resources.MsgLoadCategoriesCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        CategoriesData.Clear();
                        break;
                    case MessageBoxResult.No:
                        break;
                }

            }

            bool Tresult = TServerConnector.Connect(Host, User, SettingsPasswordBox.Password);
            
            if (Tresult == true && TServerConnector.IsConnected() == true)
            {
                Connected = true;
                                
                bool flag = TServerConnector.GetTorrentsWebDirectory(CategoriesData);                

            }
            else
            {
                Connected = false;
                MessageBox.Show(uAdd.Properties.Resources.MsgServerError + "\"" + TServerConnector.GetLastHost() + "\"", uAdd.Properties.Resources.MsgServerError);
            }

        }

        private void CategoriesGridCommandLoadFromFile(object sender, RoutedEventArgs e)
        {
            if (CategoriesData.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show(uAdd.Properties.Resources.MsgNotEmptyCategorisData, uAdd.Properties.Resources.MsgLoadCategoriesCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        CategoriesData.Clear();
                        break;
                    case MessageBoxResult.No:
                        break;
                }

            }

            string SettingsFilePath="";
            OpenFileDialog OfdDlg = new OpenFileDialog();           
            OfdDlg.DefaultExt = ".dat";
            OfdDlg.Filter = "uTorrent Settings file (.dat)|*.dat";

            Nullable<bool> DlgResult = OfdDlg.ShowDialog();

            if (DlgResult == true)
            {
                SettingsFilePath = OfdDlg.FileName;
            }
            
            if (SettingsFilePath.Length > 0)
            {
                
                IBencodingType BEObject = BencodeLibrary.BencodingUtils.DecodeFile(SettingsFilePath);
              
                BDict BTree = (BDict)BEObject;
                

                if (BTree.ContainsKey("webui.download_folders") == true)
                {
   
                    BList CatalogList = (BList)BTree["webui.download_folders"];

                    if (CatalogList.Count()>0)
                    {
                                                
                        foreach (BString CatalogRow in CatalogList)
                        {
                            if (CatalogRow !=null)
                            {
                                СategoriesDataRow NewRow = new СategoriesDataRow();
                                NewRow.CategoryName = "";
                                NewRow.TorrentDirectoryListNumber = CatalogList.IndexOf(CatalogRow)+1;
                                NewRow.Comment = CatalogRow.Value;
                                int SlashIndex = CatalogRow.Value.LastIndexOf("\\");
                                if (SlashIndex != -1 && SlashIndex < (CatalogRow.Value.Count()-1))
                                {
                                    NewRow.CategoryName = CatalogRow.Value.Substring(SlashIndex+1);
                                }
                                
                                CategoriesData.Add(NewRow);
                                
                            }

                        }
                    }
   
                }


                
            }

        }

    }

    [Serializable]
    public class СategoriesDataRow
    {
        [XmlElement("CategoryName")]
        public string CategoryName { get; set; }
        [XmlElement("TorrentDirectoryListNumber")]
        public int TorrentDirectoryListNumber { get; set; }
        [XmlElement("Comment")]
        public string Comment { get; set; }                
    }
    
}

