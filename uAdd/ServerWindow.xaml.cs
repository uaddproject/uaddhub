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
using System.Collections.ObjectModel;
using System.Threading;

namespace uAdd
{

    public partial class ServerWindow : Window, INotifyPropertyChanged
    {
        private static BackgroundWorker BWorker;
        private static object Tlocker = new object();

        private bool _connected = false;                
        private TorrentAPI TConnector;

        public string StartupFilePath = "";

        public bool Connected
        {
            get { return _connected; }
            set
            {
                if (_connected != value)
                {
                    lock (Tlocker)
                    {
                        _connected = value;
                        OnPropertyChanged("Connected");
                        OnPropertyChanged("VisibilityConnect");
                        OnPropertyChanged("VisibilityNotConnect");
                        
                    }                    
                }                
            }
        }

        public Visibility VisibilityConnect
        {
            get { return (Connected == true) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility VisibilityNotConnect
        {
            get { return (Connected == true) ? Visibility.Collapsed : Visibility.Visible; }
        }

        private ObservableCollection<MenuItem> _ServerColumns = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> ServerColumns { get { return _ServerColumns; } set { _ServerColumns = value; OnPropertyChanged("ServerColumns"); } }

        private ObservableCollection<MenuItem> _RowCommands = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> RowCommands { get { return _RowCommands; } set { _RowCommands = value; OnPropertyChanged("RowCommands"); } }

        private ObservableCollection<MenuItem> _FileRowCommands = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> FileRowCommands { get { return _FileRowCommands; } set { _FileRowCommands = value; OnPropertyChanged("FileRowCommands"); } }
                        
        private ObservableCollection<ServerTorrentDataRow> _ServerData = new ObservableCollection<ServerTorrentDataRow>();        
        public ObservableCollection<ServerTorrentDataRow> ServerData { get { return _ServerData; } set { _ServerData = value; OnPropertyChanged("ServerData"); } }

        private ObservableCollection<TorrentFile> _FilesData = new ObservableCollection<TorrentFile>();
        public ObservableCollection<TorrentFile> FilesData { get { return _FilesData; } set { 
            _FilesData = value; OnPropertyChanged("FilesData"); 
        } }

        private ServerTorrentDataRow _SelectedRow = new ServerTorrentDataRow();

        public ServerTorrentDataRow SelectedRow { get { return _SelectedRow; } 
            set { 
                    _SelectedRow = value; 
                    OnPropertyChanged("SelectedRow");
                    SelectedRowInfoExecutionPercent = (_SelectedRow == null) ? 0 : _SelectedRow.ExecutionPercentData;
                    
                    FilesData = (SelectedRow !=null && SelectedRow.Files!=null) ? new ObservableCollection<TorrentFile>(SelectedRow.Files):null;
                    
                    ServerFilesGrid.SelectedItem = null;
                } 
        }

        private TorrentFile _SelectedFileRow = new TorrentFile();

        public TorrentFile SelectedFileRow { get { return _SelectedFileRow; } set { _SelectedFileRow = value; OnPropertyChanged("SelectedFileRow"); } }
        
        private double _SelectedRowInfoExecutionPercent = 0;

        public double SelectedRowInfoExecutionPercent { get { return _SelectedRowInfoExecutionPercent; } 
            set { 
                    _SelectedRowInfoExecutionPercent = value; 
                    OnPropertyChanged("SelectedRowInfoExecutionPercent");                    
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


        public ServerWindow(string mStartupFilePath = null)
        {
            // set current culture for debug
            //System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            // build a menu                        
            BuildContextMenu();
                      
            InitializeComponent();

            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "MainThread";
            }

            this.DataContext = this;

            ReCheckContextMenu();
            
            SetVisibleServerColumns();
              
            TConnector = new TorrentAPI();

            BWorker = new BackgroundWorker();
            BWorker.WorkerReportsProgress = true;
            BWorker.DoWork += BackgroundWorkerThread;
            BWorker.ProgressChanged += BackgroundWorkerProgressChanged;
            BWorker.RunWorkerAsync(null);

            // server mode

            if (((App)Application.Current).ServerMode == true && mStartupFilePath != null && mStartupFilePath != "")
            {
                StartupFilePath = mStartupFilePath;
            }
            
        }

        #region Background worker

        private void BackgroundWorkerThread(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = "BackgroundWorker";

            while (true)
            {
                UpdateData();

                Thread.Sleep(5000); // 5000
            }


        }

        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateData((ServerStatusData)e.UserState);
        }

        #endregion

        public void ShowStartupAdd()
        {
            if (StartupFilePath != "")
            {
                uAdd.MainWindow AddWindow = new uAdd.MainWindow(StartupFilePath);
                AddWindow.Owner = this;
                AddWindow.Show();
                
                StartupFilePath = "";
            }
        }

        private void BuildContextMenu()
        { 
            // build row command context menu
            RowCommands.Clear();

            MenuItem mRowComand;
            mRowComand = new MenuItem();
            mRowComand.Header = uAdd.Properties.Resources.RowMenuActionForceStart;
            mRowComand.Tag = "RowCmdForceStart";
            mRowComand.IsCheckable = false;

            RowCommands.Add(mRowComand);
            
            mRowComand = new MenuItem();
            mRowComand.Header = uAdd.Properties.Resources.RowMenuActionStart;
            mRowComand.Tag = "RowCmdStart";
            mRowComand.IsCheckable = false;

            RowCommands.Add(mRowComand);

            mRowComand = new MenuItem();
            mRowComand.Header = uAdd.Properties.Resources.RowMenuActionPause;
            mRowComand.Tag = "RowCmdPause";
            mRowComand.IsCheckable = false;
            
            RowCommands.Add(mRowComand);

            mRowComand = new MenuItem();
            mRowComand.Header = uAdd.Properties.Resources.RowMenuActionStop;
            mRowComand.Tag = "RowCmdStop";
            mRowComand.IsCheckable = false;

            RowCommands.Add(mRowComand);

            mRowComand = new MenuItem();
            mRowComand.Header = uAdd.Properties.Resources.RowMenuActionRemove;
            mRowComand.Tag = "RowCmdRemove";
            mRowComand.IsCheckable = false;

            RowCommands.Add(mRowComand);

            mRowComand = new MenuItem();
            mRowComand.Header = uAdd.Properties.Resources.RowMenuActionRemoveData;
            mRowComand.Tag = "RowCmdRemoveData";
            mRowComand.IsCheckable = false;                  

            RowCommands.Add(mRowComand);
           
            // build columns command context menu

            ServerColumns.Clear();

            MenuItem mColumnComand;
            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionIndex;
            mColumnComand.Tag = "ColumnCmdIndex";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionName;
            mColumnComand.Tag = "ColumnCmdName";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionSize;
            mColumnComand.Tag = "ColumnCmdSize";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionStatus;
            mColumnComand.Tag = "ColumnCmdStatus";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionCategory;
            mColumnComand.Tag = "ColumnCmdCategory";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionLabel;
            mColumnComand.Tag = "ColumnCmdLabel";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionDownloadedSize;
            mColumnComand.Tag = "ColumnCmdDownloadedSize";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionRemainingSize;
            mColumnComand.Tag = "ColumnCmdRemainingSize";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionExecutionPercent;
            mColumnComand.Tag = "ColumnCmdExecutionPercent";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionDownloadSpeed;
            mColumnComand.Tag = "ColumnCmdDownloadSpeed";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionUploadSpeed;
            mColumnComand.Tag = "ColumnCmdUploadSpeed";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionUploadedSize;
            mColumnComand.Tag = "ColumnCmdUploadedSize";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionConnectedPeers;
            mColumnComand.Tag = "ColumnCmdConnectedPeers";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            mColumnComand = new MenuItem();
            mColumnComand.Header = uAdd.Properties.Resources.TorrentListCaptionConnectedSeed;
            mColumnComand.Tag = "ColumnCmdConnectedSeed";
            mColumnComand.IsCheckable = true;
            mColumnComand.IsChecked = true;

            ServerColumns.Add(mColumnComand);

            // build file row context menu
            FileRowCommands.Clear();
                 
            MenuItem mFileRowComand = new MenuItem();
            mFileRowComand.Header = uAdd.Properties.Resources.EnumTorrentFilePriorityHighPriority;
            mFileRowComand.Tag = "FileRowCmdHighPriority";
            mFileRowComand.IsCheckable = false;

            FileRowCommands.Add(mFileRowComand);

            mFileRowComand = new MenuItem();
            mFileRowComand.Header = uAdd.Properties.Resources.EnumTorrentFilePriorityNormalPriority;
            mFileRowComand.Tag = "FileRowCmdNormalPriority";
            mFileRowComand.IsCheckable = false;

            FileRowCommands.Add(mFileRowComand);

            mFileRowComand = new MenuItem();
            mFileRowComand.Header = uAdd.Properties.Resources.EnumTorrentFilePriorityLowPriority;
            mFileRowComand.Tag = "FileRowCmdLowPriority";
            mFileRowComand.IsCheckable = false;

            FileRowCommands.Add(mFileRowComand);

            mFileRowComand = new MenuItem();
            mFileRowComand.Header = uAdd.Properties.Resources.EnumTorrentFilePriorityDontDownload;
            mFileRowComand.Tag = "FileRowCmdDontDownload";
            mFileRowComand.IsCheckable = false;

            FileRowCommands.Add(mFileRowComand);
            
            if (Properties.Settings.Default.ColumnsServer != "" && ServerColumns !=null)
            {                
                List<String> ColumnsArray = new List<String>();

                ColumnsArray = uAdd.App.DeserializeString< List<String> >(Properties.Settings.Default.ColumnsServer);
                
                foreach (MenuItem sc in ServerColumns)
                {
                    if ((string)sc.Tag != "ColumnCmdIndex" && (string)sc.Tag != "ColumnCmdName")
                    {
                        String findrow = ColumnsArray.Find(

                                                        delegate(String sm)
                                                        {
                                                            return (sm == (string)sc.Tag);
                                                        });
                    
                        if (findrow != null )
                        {
                            sc.IsChecked = true;
                        }
                        else
                        {
                            sc.IsChecked = false;
                        }
                    }
                }
            }
            
        }

        private void ReCheckContextMenu()
        {
            if (RowCommands != null && RowCommands.Count > 0)
            {
                foreach (MenuItem mnu in RowCommands)
                {
                    mnu.CommandTarget = ServerDataGrid;

                    if (mnu.Items.Count > 0)
                    {
                        foreach (MenuItem mchild in mnu.Items)
                        {
                            mchild.CommandTarget = ServerDataGrid;
                        }
                    }
                }
            }

        }

        private void SetVisibleServerColumns()
        {

            if (ServerColumns != null && ServerDataGrid != null)
            {   
                foreach(System.Windows.Controls.DataGridTextColumn sc in ServerDataGrid.Columns)
                {
                    
                    List<MenuItem> ServerColumnsList = ServerColumns.ToList();
                    
                    MenuItem findRow = ServerColumnsList.Find(
                                        delegate(MenuItem sm)
                                        {
                                            return (sm.Header == sc.Header);
                                        }
                                    );

                    if (findRow != null)
                    {

                        if (findRow.IsChecked == true || (string)findRow.Tag == "ColumnCmdIndex" || (string)findRow.Tag == "ColumnCmdName")
                        {
                            sc.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            sc.Visibility = System.Windows.Visibility.Hidden;
                        }
                    }
                    

                }
                 
            }

        }

        private ServerStatusData GetServerData()
        {
            ServerStatusData ServerStatusDataSet = null;

            if (TConnector.IsConnected() == false)    // get connection if need
            {

                TConnector.Connect(uAdd.Properties.Settings.Default.Host, uAdd.Properties.Settings.Default.User, uAdd.Properties.Settings.Default.Password);

                // get torrents
             }

            if (TConnector.IsConnected() == true)     // get data if connection set
            {
                
                ObservableCollection<ServerTorrentDataRow> ListTorrentServer = TConnector.GetTorrentListServer();

                if (ListTorrentServer != null)
                {        
                    ServerStatusDataSet = new ServerStatusData();
                    ServerStatusDataSet.Connected = true;
                    ServerStatusDataSet.ServerData = ListTorrentServer;
                }

            }

            if (ServerStatusDataSet == null)
            {
                ServerStatusDataSet = new ServerStatusData();   // empty data, no connection flag
            }

            return ServerStatusDataSet;

        }

        public void UpdateData(ServerStatusData ServerStatus = null)
        {
            ServerStatusData ServerStatusDataSet;
            
            if (ServerStatus == null)
            {
                // получить данные от сервера
                                               
                ServerStatusDataSet = GetServerData(); 

            }
            else
            {
                ServerStatusDataSet = (ServerStatusData)ServerStatus;
            }

            if (Thread.CurrentThread.Name == "BackgroundWorker")
            {

                BWorker.ReportProgress(0, ServerStatusDataSet);
            }
            else
            {
                
                // обновить локальные данные из данных сервера
                // MainStatusBarItem1.Content = MainStatusDataSet.Text;
                // MainStatusBarItem1.Foreground = MainStatusDataSet.Color;
                
                this.Connected = ServerStatusDataSet.Connected;

                if (ServerStatusDataSet.ServerData != null)
                {
                    
                    ServerTorrentDataRow SaveSelectedRow = null;
                    TorrentFile SaveSelectedFileRow = null;
                    
                    if (SelectedRow != null && !SelectedRow.IsEmpty())   // selected item is set, save current value
                    {
                        SaveSelectedRow = SelectedRow;
                    }

                    if (SelectedFileRow != null && !SelectedFileRow.IsEmpty())   
                    {
                        SaveSelectedFileRow = SelectedFileRow;
                    }

                    
                    this.ServerData = ServerStatusDataSet.ServerData;

                    
                    if (RowCommandMenu.IsOpen == true && RowCommandMenu.Items.Count>0)
                    {
                         foreach (System.Windows.Controls.MenuItem CurItem in RowCommandMenu.Items)
                         {
                             if (CurItem.CommandTarget == null)
                             {
                                 CurItem.CommandTarget = ServerDataGrid;
                             }
                         }
                    }                    

                    
                    if (ServerData.Count > 0)
                    {

                        SelectedFileRow = null;

                        // try update old selection from SaveSelectedRow

                        if (SaveSelectedRow != null && !SaveSelectedRow.IsEmpty())
                        {

                            List<ServerTorrentDataRow> ServerDataList = ServerData.ToList();

                            ServerTorrentDataRow findRow = ServerDataList.Find(
                                                delegate(ServerTorrentDataRow sc)
                                                {
                                                    return (sc.Hash == SaveSelectedRow.Hash);
                                                }
                                            );

                            if (findRow != null)
                            {
                                SelectedRow = findRow;

                                if (SaveSelectedFileRow != null && !SaveSelectedFileRow.IsEmpty())
                                {
                                    if (SelectedRow.Files != null)
                                    {
                                        List<TorrentFile> ServerDataFileList = SelectedRow.Files.ToList();

                                        TorrentFile findFileRow = ServerDataFileList.Find(
                                                delegate(TorrentFile sc)
                                                {
                                                    return (sc.Hash == SaveSelectedFileRow.Hash && sc.Name == SaveSelectedFileRow.Name);
                                                }
                                            );

                                        if (findFileRow != null)
                                        {
                                            SelectedFileRow = findFileRow;
                                        }


                                    }

                                }
                                
                            }
                            else
                            {
                                SelectedRow = null;
                            }
                         }

                        if (!(SelectedRow != null && !SelectedRow.IsEmpty()))
                        {
                            SelectedRow = ServerData[0];
                        }
                        
                        ServerDataGrid.SelectedItem = SelectedRow;
                        ServerFilesGrid.SelectedItem = SelectedFileRow;
                                                
                    }
                    else
                    {
                        SelectedRow = new ServerTorrentDataRow();
                        SelectedFileRow = new TorrentFile();
                    }
                     
                    
                }

            }
            
        }

        private void ServerDataCommandAdd(object sender, RoutedEventArgs e)
        {
            uAdd.MainWindow AddWindow = new uAdd.MainWindow();
            AddWindow.Owner = this;
            AddWindow.Show();                       
          
        }
        private void ServerDataCommandDelete(object sender, RoutedEventArgs e)
        {
            if (SelectedRow != null && ServerData !=null)
            {
                // delete torrent on the server
                bool flag = TConnector.ActionTorrent(SelectedRow.Hash, "remove");     // delete torrent from server
                if (flag == true)
                {
                    int OldIndex = ServerData.IndexOf(SelectedRow);
                    ServerData.Remove(SelectedRow);
                    
                    if (OldIndex > 0)           // select element above
                    {
                        SelectedRow = ServerData[OldIndex - 1];
                        ServerDataGrid.SelectedItem = SelectedRow;
                    }
                    
                }
                
            }
        }

        private void ServerDataCommandStart(object sender, RoutedEventArgs e)
        {
            if (SelectedRow != null && ServerData !=null)
            {
                // delete torrent on the server
                bool flag = TConnector.ActionTorrent(SelectedRow.Hash, "start");     // start torrent
                if (flag == true)
                {
                    UpdateData();                    
                }
                
            }

        }

        private void ServerDataCommandPause(object sender, RoutedEventArgs e)
        {
            if (SelectedRow != null && ServerData != null)
            {
                // delete torrent on the server
                bool flag = TConnector.ActionTorrent(SelectedRow.Hash, "pause");     // pause torrent
                if (flag == true)
                {
                    UpdateData();
                }

            }

        }

        private void ServerDataCommandShowDownloaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowDownloaded = ! Properties.Settings.Default.ShowDownloaded;
            Properties.Settings.Default.Save();                    
                        
            UpdateData();                  
        }

        private void ServerDataCommandStop(object sender, RoutedEventArgs e)
        {
            if (SelectedRow != null && ServerData != null)
            {
                // delete torrent on the server
                bool flag = TConnector.ActionTorrent(SelectedRow.Hash, "stop");     // stop torrent
                if (flag == true)
                {
                    UpdateData();
                }

            }

        }


        private void ServerDataCommandRefresh(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }
                
        private void ServerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ServerTorrentDataRow)ServerDataGrid.SelectedItem != null)
            {
                SelectedRow = (ServerTorrentDataRow)ServerDataGrid.SelectedItem;
            }
            else
            {
                SelectedRow = new ServerTorrentDataRow();
            }

        }

        private void ServerFilesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((TorrentFile)ServerFilesGrid.SelectedItem != null)
            {
                SelectedFileRow = (TorrentFile)ServerFilesGrid.SelectedItem;
            }
            else
            {
                SelectedFileRow = new TorrentFile();
            }

        }

        private void ServerDataCommandSettings(object sender, RoutedEventArgs e)
        {
            uAdd.SettingsWindow SetWindow = new uAdd.SettingsWindow();
            SetWindow.Owner = this;
            SetWindow.Show();
        }

        void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            string command;
            command = ((RoutedCommand)e.Command).Name;

            if (command == (String)"Copy" && e.Parameter != null && (string)e.Parameter != "")
            {
                try
                {
                    Clipboard.SetText((string)e.Parameter);
                }
                catch
                { }

            }            
        }
        void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RowCommandCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {                        
            if (e.Command !=null)
            {          
                if ( (string)((RoutedCommand)e.Command).Name == "RowCommand" && (System.Windows.Controls.MenuItem)(e.Parameter) != null && e.Parameter.GetType() == typeof(System.Windows.Controls.MenuItem) )
                {                     
                    string CommandName = (string)((System.Windows.FrameworkElement)(e.Parameter)).Tag;

                    if (CommandName != "" && SelectedRow != null)
                    {
                        ExecTorrentCommand(CommandName, SelectedRow);

                    }
                }
            }


        }

        private void RowCommandCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (e.Command != null && (System.Windows.Controls.MenuItem)(e.Parameter) != null && e.Parameter.GetType() == typeof(System.Windows.Controls.MenuItem))
            {
                e.CanExecute = MenuItemVisible((System.Windows.Controls.MenuItem)(e.Parameter), ((System.Windows.Input.RoutedUICommand)(e.Command)).Text);
            }
        }

        private void ColumnCommandCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {            
            if (e.Command != null && (System.Windows.Controls.MenuItem)(e.Parameter) != null && e.Parameter.GetType() == typeof(System.Windows.Controls.MenuItem)
                && ((RoutedCommand)e.Command).Name == "ColumnCommand")
            {                                                
                if (ServerColumns.Contains((System.Windows.Controls.MenuItem)(e.Parameter)) == true)
                {
                    string mTag = (string)(((System.Windows.FrameworkElement)(e.Parameter)).Tag);

                    if (mTag != "ColumnCmdIndex" && mTag != "ColumnCmdName")
                    {
                        SaveColumnsServer();
                        SetVisibleServerColumns();
                    }
                    else 
                    {
                        ((System.Windows.Controls.MenuItem)(e.Parameter)).IsChecked = true;
                    }
                }                 

            }


        }

        private void ColumnCommandCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (e.Command != null && (System.Windows.Controls.MenuItem)(e.Parameter) != null && e.Parameter.GetType() == typeof(System.Windows.Controls.MenuItem))
            {
                e.CanExecute = MenuItemVisible((System.Windows.Controls.MenuItem)(e.Parameter), ((System.Windows.Input.RoutedUICommand)(e.Command)).Text);
            }
        }

        private void FileRowCommandCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (e.Command != null)
            {
                if ((string)((RoutedCommand)e.Command).Name == "FileRowCommand" && (System.Windows.Controls.MenuItem)(e.Parameter) != null && e.Parameter.GetType() == typeof(System.Windows.Controls.MenuItem))
                {
                    string CommandName = (string)((System.Windows.FrameworkElement)(e.Parameter)).Tag;

                    if (CommandName != "" && SelectedFileRow != null)
                    {
                        ExecTorrentFileCommand(CommandName, SelectedFileRow);

                    }
                }
            }
            
        }

        private void FileRowCommandCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (e.Command != null && (System.Windows.Controls.MenuItem)(e.Parameter) != null && e.Parameter.GetType() == typeof(System.Windows.Controls.MenuItem))
            {
                e.CanExecute = MenuItemVisible((System.Windows.Controls.MenuItem)(e.Parameter), ((System.Windows.Input.RoutedUICommand)(e.Command)).Text);
            }
        }

        private bool MenuItemVisible(MenuItem mItem, string mNameCommand)
        {
            bool CanExecute = false;
               
            if (mNameCommand == "RowCommand")
            {                
                if (SelectedRow != null)                
                {
                    string CommandMenuName = (string)mItem.Tag;
                    if (CommandMenuName != "")
                    {
                        switch (CommandMenuName)
                        {
                            case "RowCmdForceStart":
                                {
                                    CanExecute = true;
                                    break;
                                }
                            case "RowCmdStart":
                                {
                                    CanExecute = true;
                                    break;
                                }
                            case "RowCmdPause":
                                {
                                    if ((SelectedRow.DataState & TorrentServerState.Suspended) == TorrentServerState.Suspended)
                                    {
                                        CanExecute = false;
                                    }
                                    else
                                    {
                                        CanExecute = true;
                                    }
                                    break;
                                }
                            case "RowCmdStop":
                                {
                                    CanExecute = true;
                                    break;
                                }
                            case "RowCmdRemove":
                                {
                                    CanExecute = true;
                                    break;
                                }
                            case "RowCmdRemoveData":
                                {
                                    CanExecute = true;
                                    break;
                                }

                            default:
                                {
                                    break;
                                }
                        }
                                                
                    }
                }               
                
                    
            }
            else if (mNameCommand == "ColumnCommand")
            {
                CanExecute = true;
            }
            else if (mNameCommand == "FileRowCommand")
            {
                CanExecute = true;
            }
                            
            return CanExecute;
        }

        private void SaveColumnsServer()
        {

            if (ServerColumns != null)
            {
                List<String> ColumnsArray = new List<String>();

                foreach (MenuItem sc in ServerColumns)
                {
                    if (sc.IsChecked == true)
                    {
                        ColumnsArray.Add((string)sc.Tag);
                    }
                }
                Properties.Settings.Default.ColumnsServer = uAdd.App.SerializeToString(ColumnsArray);
                Properties.Settings.Default.Save();
            }
        }

        private void ExecTorrentCommand(string CommandName, ServerTorrentDataRow mSelectedRow)
        {
            if (mSelectedRow != null && CommandName != "" && TConnector.IsConnected() == true)
            {
                bool flagCommand = false;
            
                switch (CommandName)
                {
                    case "RowCmdForceStart":
                        {      
                            flagCommand = TConnector.ActionTorrent(mSelectedRow.Hash, "forcestart");     // force start
                            break;                       
                        }
                    case "RowCmdStart":
                        {
                            flagCommand = TConnector.ActionTorrent(mSelectedRow.Hash, "start");          // start
                            break;
                        }
                    case "RowCmdPause":
                        {
                            flagCommand = TConnector.ActionTorrent(mSelectedRow.Hash, "pause");          // pause
                            break;
                        }
                    case "RowCmdStop":
                        {
                            flagCommand = TConnector.ActionTorrent(mSelectedRow.Hash, "stop");          // stop
                            break;
                        }                    
                    case "RowCmdRemove":
                        {
                            flagCommand = TConnector.ActionTorrent(mSelectedRow.Hash, "remove");        // remove
                            break;
                        }
                    case "RowCmdRemoveData":
                        {
                            flagCommand = TConnector.ActionTorrent(mSelectedRow.Hash, "removedata");    // remove data
                            break;
                        }                       

                    default:
                        {
                            break;
                        }
                }

                if (flagCommand == true)
                {
                    UpdateData();
                }

            }

        }

        private void ExecTorrentFileCommand(string CommandName, TorrentFile mSelectedFileRow)
        {
            if (mSelectedFileRow != null && CommandName != "" && TConnector.IsConnected() == true)
            {
                bool flagCommand = false;

                List<FileNode> UserFiles = null;
                List<TorrentFile> tempTorrentFiles = new List<TorrentFile>();
                tempTorrentFiles.Add(mSelectedFileRow);

                switch (CommandName)
                {        
                    case "FileRowCmdHighPriority":
                        {
                            flagCommand = TConnector.SetFilesPriority(mSelectedFileRow.Hash, UserFiles, tempTorrentFiles, (int)TorrentFilePriority.HighPriority);     // set high priority
                            break;
                        }
                    case "FileRowCmdNormalPriority":
                        {
                            flagCommand = TConnector.SetFilesPriority(mSelectedFileRow.Hash, UserFiles, tempTorrentFiles, (int)TorrentFilePriority.NormalPriority);   // set normal priority
                            break;
                        }
                    case "FileRowCmdLowPriority":
                        {
                            flagCommand = TConnector.SetFilesPriority(mSelectedFileRow.Hash, UserFiles, tempTorrentFiles, (int)TorrentFilePriority.LowPriority);   // set low priority
                            break;
                        }
                    case "FileRowCmdDontDownload":
                        {
                            flagCommand = TConnector.SetFilesPriority(mSelectedFileRow.Hash, UserFiles, tempTorrentFiles, (int)TorrentFilePriority.DontDownload);   // set dont download priority
                            break;
                        }             
                    default:
                        {
                            break;
                        }
                }

                if (flagCommand == true)
                {
                    UpdateData();
                }

            }

        }


    }
    
    public class ServerStatusData
    {
        public ObservableCollection<ServerTorrentDataRow> ServerData;
        public bool Connected;
        
        public ServerStatusData()
        {
            ServerData  = null;
            Connected = false;

        }
        
                        
    }

    public static class CommandLibrary
    {
        private static RoutedUICommand _RowCommand = new RoutedUICommand("RowCommand", "RowCommand", typeof(CommandLibrary));
        private static RoutedUICommand _ColumnCommand = new RoutedUICommand("ColumnCommand", "ColumnCommand", typeof(CommandLibrary));
        private static RoutedUICommand _FileRowCommand = new RoutedUICommand("FileRowCommand", "FileRowCommand", typeof(CommandLibrary));

        public static RoutedUICommand RowCommand { get { return _RowCommand; } }
        public static RoutedUICommand ColumnCommand { get { return _ColumnCommand; } }
        public static RoutedUICommand FileRowCommand { get { return _FileRowCommand; } }

    } 


}
