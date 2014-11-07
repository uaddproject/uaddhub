using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using BencodeLibrary;
using System.Threading;

namespace uAdd
{

    #region TreeView of files

    public class FileNode : INotifyPropertyChanged
    {
        public FileNode()
        {
            this.id = Guid.NewGuid().ToString();
        }

        private ObservableCollection<FileNode> children = new ObservableCollection<FileNode>();
        private ObservableCollection<FileNode> parent = new ObservableCollection<FileNode>();
        private string text;
        private string filepath;
        private string id;
        private long   length;        
        private string filesize;
        private string iconimageuri;
                      
        private bool? isChecked = true;
        private bool isExpanded;

        public ObservableCollection<FileNode> Children
        {
            get { return this.children; }
            set
            {
                this.children = value;
                RaisePropertyChanged("Сhildren");
            }
        }

        public ObservableCollection<FileNode> Parent
        {
            get { return this.parent; }
        }

        public bool? IsChecked
        {
            get {
                return this.isChecked; 
            }
            set
            {
                this.isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                RaisePropertyChanged("Text");
            }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        public string Id
        {
            get { return this.id; }
            set
            {
                this.id = value;
            }
        }

        public long Length
        {
            get { return this.length; }
            set
            {
                this.length = value;
            }
        }

        public string FileSize
        {
            get { return this.filesize; }
            set
            {
                this.filesize = value;
                RaisePropertyChanged("FileSize");
            }
        }
        
        public string FilePath
        {
            get { return this.filepath; }
            set
            {
                this.filepath = value;
            }
        }

        public string IconImageUri
        {
            get { return iconimageuri;}
            set { 
                this.iconimageuri = value;
                RaisePropertyChanged("IconImageUri");                
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
                
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName) );

            int countCheck = 0;

            if (propertyName == "IsChecked")
            {

                if (this.Id == CheckBoxId.checkBoxId && this.Parent.Count == 0 && this.Children.Count != 0)
                {
                    CheckedTreeParent(this.Children, this.IsChecked);
                }
                if (this.Id == CheckBoxId.checkBoxId && this.Parent.Count > 0 && this.Children.Count > 0)
                {
                    CheckedTreeChildMiddle(this.Parent, this.Children, this.IsChecked);
                }
                if (this.Id == CheckBoxId.checkBoxId && this.Parent.Count > 0 && this.Children.Count == 0)
                {
                    CheckedTreeChild(this.Parent, countCheck);
                }

                //RefreshFileListView();
            }            
        }

        private void CheckedTreeChildMiddle(ObservableCollection<FileNode> itemsParent, ObservableCollection<FileNode> itemsChild, bool? isChecked)
        {
            int countCheck = 0;
            CheckedTreeParent(itemsChild, isChecked);
            CheckedTreeChild(itemsParent, countCheck);
        }

        public void CheckedTreeParent(ObservableCollection<FileNode> items, bool? isChecked)
        {
            foreach (FileNode item in items)
            {
                item.IsChecked = isChecked;
                if (item.Children.Count != 0) CheckedTreeParent(item.Children, isChecked);
            }
        }

        private void CheckedTreeChild(ObservableCollection<FileNode> items, int countCheck)
        {            
            bool isNull = false;
            foreach (FileNode paren in items)
            {
                foreach (FileNode child in paren.Children)
                {
                    if (child.IsChecked == true || child.IsChecked == null)
                    {
                        countCheck++;
                        if (child.IsChecked == null)
                            isNull = true;
                    }
                }
                if (countCheck != paren.Children.Count && countCheck != 0) paren.IsChecked = null;
                else if (countCheck == 0) paren.IsChecked = false;
                else if (countCheck == paren.Children.Count && isNull) paren.IsChecked = null;
                else if (countCheck == paren.Children.Count && !isNull) paren.IsChecked = true;
                if (paren.Parent.Count != 0) CheckedTreeChild(paren.Parent, 0);
            }
        }




    }

    public struct CheckBoxId
    {
        public static string checkBoxId;
    }

    #endregion TreeView of files
      
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static BackgroundWorker BWorker;
        private string filepath = "";
        private string torrentContent = "";
        private string contentSize = "";
        private string contentinfo = "";
        private bool forcestart = false;
        private bool notstartdownload = false;
        private string categoryFontWeight = "Normal";
        private double categoryFontSize = 12;

        private TorrentAPI TConnector;

        private static object Tlocker = new object();
        private bool _connected = false;
        private bool _mainStatusOut = false;
                
        public struct StatusData
        {
            public string Text;
            public SolidColorBrush Color;
        }

        private TorrentInfo TInfo=null;
        
        ObservableCollection<string> _categoriesList = new ObservableCollection<string>();

        public ObservableCollection<string> CategoriesList { get { return _categoriesList; } set { _categoriesList = value; OnPropertyChanged("CategoriesList"); } }
        public string FilePath { get { return filepath; } set { filepath = value; OnPropertyChanged("FilePath"); LoadTorrentContent(); } }
        public string TorrentContent { get { return torrentContent;} set { torrentContent = value; OnPropertyChanged("TorrentContent"); } }
        public string ContentSize { get { return contentSize; } set { contentSize = value; OnPropertyChanged("ContentSize"); } }
        public string СontentInfo { get { return contentinfo; } set { contentinfo = value; OnPropertyChanged("СontentInfo"); } }
        public bool ForceStart { get { return forcestart; } set { forcestart = value; OnPropertyChanged("ForceStart"); } }
        public bool NotStartDownload { get { return notstartdownload; } set { notstartdownload = value; OnPropertyChanged("NotStartDownload"); } }

        public string CategoryFontWeight { get { return categoryFontWeight; } set { categoryFontWeight = value; OnPropertyChanged("CategoryFontWeight"); } }
        public double CategoryFontSize { get { return categoryFontSize; } set { categoryFontSize = value; OnPropertyChanged("CategoryFontSize"); } }

        public ObservableCollection<FileNode> TFileNodes { get; set ; }
      
        public bool Connected {
                                get { return _connected; }
                                set {
                                    if (_connected != value || _mainStatusOut == false)
                                      {                                          
                                          lock (Tlocker) 
                                          {                                            
                                            _connected = value; 
                                          }
                                          UpdateMainStatus();
                                      } 
                                     }
                              }
                
        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion        

        public MainWindow(string mStartupFilePath = null)
        {
           // set current culture for debug
           //System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
           
           InitializeComponent();

           if (Thread.CurrentThread.Name == null)
           {
               Thread.CurrentThread.Name = "MainThread";
           }
           
           this.DataContext = this;           
                      
           BWorker = new BackgroundWorker();
           BWorker.WorkerReportsProgress = true;
           BWorker.DoWork += BackgroundWorkerThread;
           BWorker.ProgressChanged += BackgroundWorkerProgressChanged;
           BWorker.RunWorkerAsync(null);

           TFileNodes = new ObservableCollection<FileNode>();
           FilesTreeView.ItemsSource = TFileNodes;

           UpdateVisibleControl();
            
           GetTorentFilePath(mStartupFilePath);

           LoadCatageriesList();

           LoadTorrentContent();
           
        }
            
        public void UpdateVisibleControl()
        {
            if (uAdd.Properties.Settings.Default.BigFontCategory == true)
            { CategoryFontSize = 14; }
            else { CategoryFontSize = 12; }

            if (uAdd.Properties.Settings.Default.BoldFontCategory == true)
            { CategoryFontWeight = "Bold"; }
            else { CategoryFontWeight = "Normal"; }

        }

        #region Background worker

        private void BackgroundWorkerThread(object sender, DoWorkEventArgs e)        
        {
            Thread.CurrentThread.Name = "BackgroundWorker";

            while (true)
            {
                if (Connected == false)
                {
                    Connect();
                }

                Thread.Sleep(3000);
            }

                        
        }

        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateMainStatus((StatusData)e.UserState);                        
        }
        
        #endregion
        
        #region control events

        private void FilePathBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OfdDlg = new OpenFileDialog();
            if (filepath.Length > 0)
            {
                OfdDlg.FileName = FilePath;
            }

            OfdDlg.DefaultExt = ".torrent";
            OfdDlg.Filter = "Torrent files (.torrent)|*.torrent";

            Nullable<bool> DlgResult = OfdDlg.ShowDialog();

            if (DlgResult == true)
            {
                FilePath = OfdDlg.FileName;

            }
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            AddTorrentToServer();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void AboutLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;
            uAdd.AboutWindow AboutWindow = new uAdd.AboutWindow();
            AboutWindow.Owner = this;
            AboutWindow.Show();

        }

        private void OptionLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;
            uAdd.SettingsWindow SetWindow = new uAdd.SettingsWindow();
            SetWindow.Owner = this;
            SetWindow.Show();

        }
        
        private void ServerLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;
            uAdd.ServerWindow ServWindow = new uAdd.ServerWindow();            
            ServWindow.Owner = this;
            ((App)Application.Current).pServerWindow = ServWindow;
            ServWindow.Show();
        }
   
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            CheckBoxId.checkBoxId = currentCheckBox.Uid;            
        }

        private void IsChecked_Checked(object sender, RoutedEventArgs e)
        {
            RefreshFileListView();
        }

        private void ContentHelpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TInfo != null && TInfo.ContentInfo.Length>0)
            {
                try
                {
                    System.Diagnostics.Process.Start(TInfo.ContentInfo);
                }
                catch
                {
                }
            }
        }

        private void NotStartDownload_Checked(object sender, RoutedEventArgs e)
        {            
            if (NotStartDownload == true)
            {
                ForceStart = !NotStartDownload;
            }

        }

        private void ForceStart_Checked(object sender, RoutedEventArgs e)
        {
            if (ForceStart == true)
            {
                NotStartDownload = !ForceStart;
            }
        }
        
        private void CommandCheckAllFiles(object sender, RoutedEventArgs e)
        {
            CommandCheckUnCheckFileTree(true);
        }
        
        private void CommandUnCheckAllFiles(object sender, RoutedEventArgs e)
        {
            CommandCheckUnCheckFileTree(false);
        }

        private void CommandCheckUnCheckFileTree(bool action)
        {

            if (TFileNodes != null)
            {
                foreach (FileNode item in TFileNodes)
                {
                    item.IsChecked = action;
                    
                    if (item.Children.Count != 0) 
                    {
                        item.CheckedTreeParent(item.Children, action);
                    }
                }
        
            }            

        }                


        #endregion control events

        #region basic functionality

        public bool Connect(bool ForceConnect = false)
        {            
            lock (Tlocker)
            {
                if (Connected == false || ForceConnect == true )
                {                
                                                                
                    try
                    {
                        bool Tresult = false;
                                    
                        TConnector = new TorrentAPI();
                        Tresult = TConnector.Connect(uAdd.Properties.Settings.Default.Host, uAdd.Properties.Settings.Default.User, uAdd.Properties.Settings.Default.Password);

                        if (Connected != Tresult || _mainStatusOut == false)
                        {
                            Connected = Tresult;
                        }
                        
                    }
                    catch (Exception)
                    {
                        if (Connected != false )
                        {
                            Connected = false;
                        }
                        
                        if (_mainStatusOut==false)
                        {
                            _mainStatusOut = true;
                            UpdateMainStatus();
                        }                        

                    }
                }
            }

            return Connected;
        }

        private void UpdateMainStatus(StatusData? MainStatus = null)
        {
            StatusData MainStatusDataSet;

            if (MainStatus == null)
            {
                MainStatusDataSet = new StatusData();
                MainStatusDataSet.Text = GetMainStatusText();
                MainStatusDataSet.Color = GetMainStatusColor();
            }
            else
            {
                MainStatusDataSet = (StatusData) MainStatus;
            }

            if (Thread.CurrentThread.Name == "BackgroundWorker")
            {

                BWorker.ReportProgress(0, MainStatusDataSet);
            }
            else
            {
                if (_mainStatusOut == false)
                {
                    _mainStatusOut = true;
                }

                MainStatusBarItem1.Content = MainStatusDataSet.Text;
                MainStatusBarItem1.Foreground = MainStatusDataSet.Color;
            }            
     
        }

        
        public void LoadCatageriesList()
        {
            string TempValue =  CategoriesSelect.SelectedValue==null?"":CategoriesSelect.SelectedValue.ToString();

            CategoriesList.Clear();

            if (Properties.Settings.Default.CategoriesTable != "")
            {
                ObservableCollection<СategoriesDataRow> CategoriesDataList = uAdd.App.DeserializeString<ObservableCollection<СategoriesDataRow>>(Properties.Settings.Default.CategoriesTable);

                foreach(СategoriesDataRow iRow in CategoriesDataList )
                {
                    if (iRow.CategoryName.Length != 0)
                    {
                        CategoriesList.Add(iRow.CategoryName.Trim());
                    }
                }
                if (CategoriesList.Count() > 0 && uAdd.Properties.Resources.CategoriesDefaultValue !="" && uAdd.Properties.Settings.Default.NotAddDefaultCategory == false )
                {

                    CategoriesList.Insert(0, uAdd.Properties.Resources.CategoriesDefaultValue);
                }
            }

            CategoriesSelect.SelectedValue = "";

            if (TempValue.Length > 0 && CategoriesList.Contains(TempValue) )
            {
                CategoriesSelect.SelectedValue = TempValue;                
            }

        }

        private void AddTorrentToServer()
        {

            if ((TorrentContent.Trim()).Length == 0)
            {
                MessageBox.Show(uAdd.Properties.Resources.MsgEmptyContentText, uAdd.Properties.Resources.MsgAddingCaption);
                return;
            }

            string CurentCategoryValue = CategoriesSelect.SelectedValue == null ? "" : CategoriesSelect.SelectedValue.ToString();

            if (CurentCategoryValue.Length == 0 || CurentCategoryValue == uAdd.Properties.Resources.CategoriesDefaultValue)
            {
                if (uAdd.Properties.Settings.Default.NotAllowEmptyCategory == true)
                {
                    MessageBox.Show(uAdd.Properties.Resources.MsgEmptyCategoryTextNotAllow, uAdd.Properties.Resources.MsgAddingCaption);
                    return;
                }
                else if (CurentCategoryValue.Length == 0 && Properties.Settings.Default.CategoriesTable != "")
                {

                    MessageBoxResult result = MessageBox.Show(uAdd.Properties.Resources.MsgEmptyCategoryText, uAdd.Properties.Resources.MsgAddingCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            break;
                        case MessageBoxResult.No:
                            return;
                    }
                }
            }
                        
            // add .torrent to server

            bool AddResult = false;
            
            Connect();           

            if (Connected == true && TConnector.IsConnected()==true)
            {
                 
                int CategoryNumber = 0;
                string CategoryLabel ="";

                if (Properties.Settings.Default.CategoriesTable != "")
                {
                    if (CurentCategoryValue.Length > 0 && CurentCategoryValue != uAdd.Properties.Resources.CategoriesDefaultValue)
                    {
                        ObservableCollection<СategoriesDataRow> CategoriesData = uAdd.App.DeserializeString<ObservableCollection<СategoriesDataRow>>(Properties.Settings.Default.CategoriesTable);

                        foreach (СategoriesDataRow CategoriesRow in CategoriesData)
                        {
                            if (CategoriesRow.CategoryName == CurentCategoryValue)
                            {
                                CategoryNumber = CategoriesRow.TorrentDirectoryListNumber;
                                CategoryLabel = CategoriesRow.CategoryName;
                            }
                        }
                    }

                }
                
                // add .torrent file to server           
                AddResult = TConnector.AddTorrent(TInfo, TFileNodes, CategoryNumber, ForceStart,NotStartDownload);
           
                if (AddResult == false && uAdd.Properties.Settings.Default.FailedRetries == true)
                {
                    AddResult = TConnector.AddTorrent(TInfo, TFileNodes, CategoryNumber, ForceStart, NotStartDownload);
                }

            }

            if (AddResult == false)
            {
                MessageBox.Show(uAdd.Properties.Resources.MsgAddingError + "\"" + TConnector.GetLastHost() + "\"", uAdd.Properties.Resources.MsgAddingCaption);
                return;
            }
            else
            {
                if (((System.Windows.Window)(this)).Owner != null && ((System.Windows.Window)(this)).Owner.GetType() == typeof(uAdd.ServerWindow))
                {
                    ((uAdd.ServerWindow)(((System.Windows.Window)(this)).Owner)).UpdateData();

                }
            }
                        
            
            this.Close();
        }
        
        private void LoadTorrentContent()
        {
            ClearContent();
                                    
            if (FilePath.Length >0)
            {
                ObservableCollection<FileNode> TempFileNodes = new ObservableCollection<FileNode>();
                                
                TInfo = new TorrentInfo(FilePath, ref TempFileNodes);  // reead torrent info file
                                
                TFileNodes = TempFileNodes;

                FilesTreeView.ItemsSource = TFileNodes;

                if (TInfo.IsEmpty())
                {
                    ClearContent();
                }
                else
                { 
                    // interface output
                    TorrentContent = TInfo.ContentName;
                    СontentInfo    = TInfo.ContentInfo;
                    ContentSize    = TInfo.ContentSize;
                }
            }

            SingleMultiFileMode();

        }

        private void SingleMultiFileMode()
        {
            
            if ( TInfo != null && TInfo.MultiFile == true)    // multi file mode
            {
                if (FileListGrid.Visibility != System.Windows.Visibility.Visible)
                {
                    FileListGrid.Visibility = System.Windows.Visibility.Visible;
                    WindowGridRow2.Height = new GridLength(1, GridUnitType.Star);
                    WindowGridRow4.Height = System.Windows.GridLength.Auto;
                }
            }
            else                                             // single file mode
            {
                if (FileListGrid.Visibility == System.Windows.Visibility.Visible)
                {
                    FileListGrid.Visibility = System.Windows.Visibility.Collapsed;
                    WindowGridRow2.Height = System.Windows.GridLength.Auto;
                    WindowGridRow4.Height = new GridLength(1, GridUnitType.Star);
                }                                                       
            
            }
                
        }

        private void RefreshFileListView()
        {
            if (TInfo != null)
            {
                if (TInfo.MultiFile == true)
                {
                    TInfo.FileLength = TInfo.CalculateTotal(TFileNodes);
                    TInfo.ContentSize = TInfo.GetFileSizeFormat(TInfo.FileLength);
                    ContentSize = TInfo.ContentSize;
                }

            }
        }
                
        #endregion basic functionality

        #region additional functionality

        private void ClearContent()
        {
            TorrentContent = "";
            ContentSize    = "";
            TInfo          = null;
            TFileNodes.Clear();    
            
        }

        private void GetTorentFilePath(string mStartupFilePath)
        {
            if (mStartupFilePath != null && mStartupFilePath != "")
            {
                FilePath = mStartupFilePath;                
            }
        }
        
        private string GetMainStatusText()
        {
            return (Connected == true) ? uAdd.Properties.Resources.StateWebUIConnected : uAdd.Properties.Resources.StateWebUINotConnected;            
        }

        private SolidColorBrush GetMainStatusColor()
        {                    
            return (Connected == true) ? Brushes.DarkGreen : Brushes.DarkRed;
        }


        #endregion additional functionality

    };


}

