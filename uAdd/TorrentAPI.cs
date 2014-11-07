
using System.Net;
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Jayrock;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using BencodeLibrary;


namespace uAdd
{

    public static class TorrentEnumConverter
    {
        public static T ToEnumValue<T, E>(this E enumType)
        {
            return (T)Enum.ToObject(typeof(E), enumType);
        }

        public static E ToEnumType<T, E>(this T enumString)
        {
            return (E)Enum.Parse(typeof(E), enumString.ToString());
        }

    }

    class TorrentAPI
    {
        public string Token { get { return token; } }
        public string Cookie { get {return cookie; } }

        private string host, username, password, token, cookie;
                
        CredentialCache credentials;

        delegate List<FileNode> GetUnCheckedFilesAsync(ObservableCollection<FileNode> FileTree);


        public TorrentAPI()
        {
            token = "";
            cookie = "";
            host = "";
            username = "";
            password = "";

        }
                   
        public bool IsConnected()
        {
            return (token.Length == 0) ? false : true;
        }

        public string GetLastHost()
        {
            return host;
        }

        public bool Connect(string _host, string _userName, string _password)
        {
            host = "http://" + _host + "/gui/";
            username = _userName;
            password = _password;

            token = "";

            credentials = new CredentialCache();
            credentials.Add(new Uri(host), "Basic", new NetworkCredential(username, password));
            GetToken();
            
            return IsConnected();
        }

        private void GetToken()
        {
            try
            {
                HttpWebRequest getTokenRequest = (HttpWebRequest)(HttpWebRequest.Create(host + "token.html"));
                getTokenRequest.KeepAlive = false;
                getTokenRequest.Method = "GET";
                getTokenRequest.Credentials = credentials;

                HttpWebResponse response = (HttpWebResponse)getTokenRequest.GetResponse();

                cookie = response.GetResponseHeader("Set-Cookie");
                cookie = cookie.Substring(0, cookie.IndexOf(';'));

                StreamReader sr = new StreamReader(response.GetResponseStream());
                Regex r = new Regex(".*<div[^>]*id=[\"\']token[\"\'][^>]*>([^<]*)</div>.*");
                Match m = r.Match(sr.ReadToEnd());
                token = m.Result("$1");

                response.Close();
            }
            catch
            {
                token = "";
            }
         }

        public bool AddTorrent(TorrentInfo TInfo, ObservableCollection<FileNode> TFileNodes, int DirectoryNumber = 0, bool ForceStart = false, bool NotStartDownload = false)
        {               
            if (IsConnected() )
            {
                GetUnCheckedFilesAsync UnCheckedFilesAsyncFunc = TInfo.GetUnCheckedFiles;        // create async delegate object
                IAsyncResult AsyncFilesKey = null;
                bool NeedStopAsync = false;
                
                try
                {
                    List<FileNode> UnCheckedFiles = null;                
                    bool PrevTorrentsStartStoppedOption = false;
                    bool NeedAutoStopTorrent = false;
                    bool flag = false;
                                    
                    if (NotStartDownload == true)
                    {
                        NeedAutoStopTorrent = true;
                        AsyncFilesKey = UnCheckedFilesAsyncFunc.BeginInvoke(TFileNodes, null, null);  // start async delegate
                        NeedStopAsync = true;
                    }
                    else
                    {
                        UnCheckedFiles = TInfo.GetUnCheckedFiles(TFileNodes);

                        if (UnCheckedFiles.Count > 0)                    
                        {
                            NeedAutoStopTorrent = true;
                        }
                    }

                    if (NeedAutoStopTorrent == true)
                    {                    
                        object TorrentsStartStoppedOptionObject = null;

                        TorrentsStartStoppedOptionObject = GetTorrentServerSettings("torrents_start_stopped");   // get current value of server option "torrents_start_stopped"

                        if (TorrentsStartStoppedOptionObject == null)
                        {                             
                            if (NeedStopAsync == true)   { UnCheckedFilesAsyncFunc.EndInvoke(AsyncFilesKey); }
                            return false; 
                        }
                        else
                        { PrevTorrentsStartStoppedOption = (bool)(TorrentsStartStoppedOptionObject); }

                        if (PrevTorrentsStartStoppedOption == false)  // if option true, then no action do
                        { 
                            // set option in "true" for disable auto start torrents on adding file
                            flag = SetTorrentServerSettings("torrents_start_stopped", "1");
                            
                            if (flag == false)
                            {  
                                if (NeedStopAsync == true)   { UnCheckedFilesAsyncFunc.EndInvoke(AsyncFilesKey); }
                                return false;
                            }
                        }
                    }

                    // add .torrent file to uTorrent server

                    flag = AddTorrentFileToServer(TInfo, DirectoryNumber);

                    if (flag == true)
                    {
                        if (NeedAutoStopTorrent == true && PrevTorrentsStartStoppedOption == false)
                        {
                             flag = SetTorrentServerSettings("torrents_start_stopped", "0");
                        }

                        if (NeedStopAsync == true)
                        {
                            UnCheckedFiles = UnCheckedFilesAsyncFunc.EndInvoke(AsyncFilesKey);
                        }
                        
                        if (UnCheckedFiles !=null && UnCheckedFiles.Count > 0)    // this is disabled files
                        {
                            List<TorrentFile> TorrentFiles = new List<TorrentFile>();

                            flag = GetTorrentFiles(TInfo.Hash, TorrentFiles);

                            if (flag == true)
                            {
                                flag = SetFilesPriority(TInfo.Hash, UnCheckedFiles, TorrentFiles, 0);

                                if (flag == true)
                                {
                                    
                                }
                                else
                                {
                                    flag = ActionTorrent(TInfo.Hash, "removedata");     // delete torrent from server

                                    return false;
                                }                                
                            }
                            else
                            {
                                flag = ActionTorrent(TInfo.Hash, "removedata");     // delete torrent from server

                                return false;
                            }                            
                        }

                        // start download

                        if (NotStartDownload == false )
                        {
                            if (ForceStart == true)
                            {
                                flag = ActionTorrent(TInfo.Hash, "forcestart");     // force start
                            }
                            else
                            {                                
                                flag = ActionTorrent(TInfo.Hash, "start");          // ordinary start
                            }

                            if (flag == false)
                            { 
                                flag = ActionTorrent(TInfo.Hash, "removedata");     // delete torrent from server
                                return false;
                            }

                        }

                        return true;

                    }
                    else
                    {
                        if (NeedAutoStopTorrent == true && PrevTorrentsStartStoppedOption == false )
                        {                            
                            flag = SetTorrentServerSettings("torrents_start_stopped", "0");
                        }

                        if (NeedStopAsync == true)   { UnCheckedFilesAsyncFunc.EndInvoke(AsyncFilesKey); }
                        return false;
                    }
                }
                catch (Exception)
                {
                    if (NeedStopAsync == true)
                    {
                        try
                        { UnCheckedFilesAsyncFunc.EndInvoke(AsyncFilesKey); }
                        catch
                        { }
                    }

                    return false;
                }

            }

            return false;
        }

    
        private bool AddTorrentFileToServer(TorrentInfo TInfo, int DirectoryNumber)
        {
            if (IsConnected() )
            {
                try
                {
                    Stream TorrentStream = File.OpenRead(TInfo.FilePath);

                    string RequestText = "";
                    
                    RequestText = host + "?action=add-file&token=" + token + ((DirectoryNumber > 0)?"&download_dir=" + DirectoryNumber.ToString() : "") ;
                    
                    HttpWebRequest PostRequest = (HttpWebRequest)(HttpWebRequest.Create(RequestText));
                    PostRequest.KeepAlive = false;
                    PostRequest.Credentials = credentials;
                    string guid = Guid.NewGuid().ToString("N");
                    PostRequest.ContentType = "multipart/form-data; boundary=" + guid;
                    PostRequest.Method = "POST";
                    PostRequest.Headers.Add("Cookie", cookie);

                    using (BinaryWriter Writer = new BinaryWriter(PostRequest.GetRequestStream()))
                    {
                        byte[] FileBytes = new byte[TorrentStream.Length];
                        TorrentStream.Read(FileBytes, 0, FileBytes.Length);

                        Writer.Write(Encoding.ASCII.GetBytes(String.Format("--{0}\r\nContent-Disposition: form-data; name=\"torrent_file\"; filename=\"{0}\"\r\nContent-Type: application/x-bittorrent\r\n\r\n", guid, TInfo.FilePath)));
                        Writer.Write(FileBytes, 0, FileBytes.Length);
                        Writer.Write(Encoding.ASCII.GetBytes(String.Format("\r\n--{0}\r\n", guid)));
                    }

                    HttpWebResponse Response = (HttpWebResponse)PostRequest.GetResponse();
                    StreamReader ResponseStream = new StreamReader(Response.GetResponseStream());
                    string ResponseContent = ResponseStream.ReadToEnd();                    
                    ResponseStream.Close();
                    
                    if (ResponseContent.Contains("error") == false)
                    {
                        return true;
                    }
                   
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }


        public bool ActionTorrent(string TorrentHash, string Command)
        {
            if (IsConnected())
            {
                try
                {
                    HttpWebRequest getTorrentRequest = (HttpWebRequest)(HttpWebRequest.Create(host + "?action="+ Command + "&hash=" + TorrentHash + "&token=" + token));
                    getTorrentRequest.Credentials = credentials;
                    getTorrentRequest.Headers.Add("Cookie", cookie);
                    HttpWebResponse getTorrentResponse = (HttpWebResponse)getTorrentRequest.GetResponse();
                    StreamReader ResponseStream = new StreamReader(getTorrentResponse.GetResponseStream());
                    string ResponseContent = ResponseStream.ReadToEnd();
                    ResponseStream.Close();
                    getTorrentResponse.Close();

                    if (ResponseContent.Contains("error") == false)
                    {
                        return true;
                    }

                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        public bool GetTorrentsWebDirectory(ObservableCollection<СategoriesDataRow> CategoriesData)
        {
            if (IsConnected())
            {
                try
                {
                    HttpWebRequest getTorrentRequest = (HttpWebRequest)(HttpWebRequest.Create(host + "?action=list-dirs&token=" + token));
                    getTorrentRequest.Credentials = credentials;
                    getTorrentRequest.ContentType = @"text/html; charset=windows-1251";
                    
                    getTorrentRequest.Headers.Add("Cookie", cookie);
                    HttpWebResponse getTorrentResponse = (HttpWebResponse)getTorrentRequest.GetResponse();
                    StreamReader ResponseStream = new StreamReader(getTorrentResponse.GetResponseStream(), Encoding.Default);
                    string ResponseContent = ResponseStream.ReadToEnd();
                    ResponseStream.Close();
                    getTorrentResponse.Close();

                    if (ResponseContent.Contains("error") == false)
                    {
                        JsonObject res = (JsonObject)JsonConvert.Import(ResponseContent);

                        if (res.Contains("download-dirs"))
                        {
                            string s;

                            JsonArray WebDirectorySet = (JsonArray)res["download-dirs"];

                            foreach (JsonObject Dir in WebDirectorySet)
                            {
                                s = Dir["path"].ToString();

                                if (s != "Default download dir")
                                {
                                    СategoriesDataRow NewRow = new СategoriesDataRow();
                                    NewRow.CategoryName = "";
                                    NewRow.TorrentDirectoryListNumber =  WebDirectorySet.IndexOf(Dir);
                                    NewRow.Comment = s;
                                    int SlashIndex = s.LastIndexOf("\\");
                                    if (SlashIndex != -1 && SlashIndex < (s.Count() - 1))
                                    {
                                        NewRow.CategoryName = s.Substring(SlashIndex + 1);
                                    }

                                    CategoriesData.Add(NewRow);

                                }
                            }

                            return true;
                        }



                        return true;
                    }

                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }


        private bool GetTorrentFiles(string TorrentHash, List<TorrentFile> TorrentFiles)
        {
            if (IsConnected())
            {
                try
                {
                    HttpWebRequest getTorrentRequest = (HttpWebRequest)(HttpWebRequest.Create(host + "?action=getfiles&hash=" + TorrentHash + "&token=" + token));
                    getTorrentRequest.Credentials = credentials;
                    getTorrentRequest.Headers.Add("Cookie", cookie);
                    HttpWebResponse getTorrentResponse = (HttpWebResponse)getTorrentRequest.GetResponse();
                    StreamReader ResponseStream = new StreamReader(getTorrentResponse.GetResponseStream());
                    string ResponseContent = ResponseStream.ReadToEnd();
                    ResponseStream.Close();
                    getTorrentResponse.Close();

                    if (ResponseContent.Contains("error") == false)
                    {
                        JsonObject res = (JsonObject)JsonConvert.Import(ResponseContent);

                        if (res.Contains("files"))
                        {

                            JsonArray tFilesSet = (JsonArray)((JsonArray)res["files"])[1];

                            foreach (JsonArray tfile in tFilesSet)
                            {
                                TorrentFile TFile = new TorrentFile();
                                TFile.Name = (string)tfile[0];
                                TFile.WebIndex = tFilesSet.IndexOf(tfile);
                                
                                TorrentFiles.Add(TFile);                                

                            }

                            return true;
                        }

                    }

                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

                    
        public bool SetFilesPriority(string TorrentHash, List<FileNode> UserFiles, List<TorrentFile> TorrentFiles, int Priority)
        {
            if (IsConnected() )
            {
                try
                {
                    string FilesText="";

                    if (UserFiles != null)
                    {
                        foreach (FileNode UserFile in UserFiles)
                        {
                            foreach (TorrentFile TFile in TorrentFiles)
                            {
                                if (UserFile.FilePath == TFile.Name)
                                {
                                    FilesText += "&f=" + TFile.WebIndex.ToString();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (TorrentFile TFile in TorrentFiles)
                        {                            
                            FilesText += "&f=" + TFile.WebIndex.ToString();                            
                        }
                    }

                    HttpWebRequest getTorrentRequest = (HttpWebRequest)(HttpWebRequest.Create(host + "?action=setprio&hash=" + TorrentHash + "&p=" + Priority + FilesText + "&token=" + token));
                    getTorrentRequest.Credentials = credentials;
                    getTorrentRequest.Headers.Add("Cookie", cookie);
                    HttpWebResponse getTorrentResponse = (HttpWebResponse)getTorrentRequest.GetResponse();
                    StreamReader ResponseStream = new StreamReader(getTorrentResponse.GetResponseStream());
                    string ResponseContent = ResponseStream.ReadToEnd();
                    ResponseStream.Close();
                    getTorrentResponse.Close();

                    if (ResponseContent.Contains("error") == false)
                    {
                        return true;                    
                        
                    }

                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        public object GetTorrentServerSettings(string SettingsName)
        {
            object SettingsValue = null;

            if (IsConnected())
            {
                try
                {
                    HttpWebRequest getTorrentRequest = (HttpWebRequest)(HttpWebRequest.Create(host + "?action=getsettings&token=" + token));
                    getTorrentRequest.Credentials = credentials;
                    getTorrentRequest.Headers.Add("Cookie", cookie);
                    HttpWebResponse getTorrentResponse = (HttpWebResponse)getTorrentRequest.GetResponse();
                    StreamReader ResponseStream = new StreamReader(getTorrentResponse.GetResponseStream());
                    string ResponseContent = ResponseStream.ReadToEnd();
                    ResponseStream.Close();
                    getTorrentResponse.Close();

                    if (ResponseContent.Contains("error") == false)
                    {
                        JsonObject res = (JsonObject)JsonConvert.Import(ResponseContent);

                        if (res.Contains("settings"))
                        {

                            string typeParam = "";

                            JsonArray tSettingsSet = (JsonArray)res["settings"];


                            foreach (JsonArray tSetting in tSettingsSet)
                            {
                                if ((string)tSetting[0] == SettingsName)
                                {
                                    Jayrock.Json.JsonNumber typeParamJson = (Jayrock.Json.JsonNumber)(tSetting[1]);

                                    typeParam = typeParamJson.ToString();

                                    if (typeParam == "0")         // type number
                                    {
                                        SettingsValue = Convert.ToDecimal((string)tSetting[2]);
                                    }
                                    else if (typeParam == "1")   // type bool
                                    {
                                        SettingsValue = Convert.ToBoolean((string)tSetting[2]);
                                    }
                                    else if (typeParam == "2")   // type string
                                    {
                                        SettingsValue = Convert.ToString((string)tSetting[2]);
                                    }

                                    break;
                                }

                            }

                        }

                    }
                    
                }
                catch (Exception)
                {
                    SettingsValue = null;
                }
            }
            
            return SettingsValue;
        }

        public bool SetTorrentServerSettings(string SettingsName, string SettingsValue)
        {            
            if (IsConnected())
            {
                try
                {
                    HttpWebRequest getTorrentRequest = (HttpWebRequest)(HttpWebRequest.Create(host + "?action=setsetting&s=" + SettingsName + "&v=" + SettingsValue + "&token=" + token));
                    getTorrentRequest.Credentials = credentials;
                    getTorrentRequest.Headers.Add("Cookie", cookie);
                    HttpWebResponse getTorrentResponse = (HttpWebResponse)getTorrentRequest.GetResponse();
                    StreamReader ResponseStream = new StreamReader(getTorrentResponse.GetResponseStream());
                    string ResponseContent = ResponseStream.ReadToEnd();
                    ResponseStream.Close();
                    getTorrentResponse.Close();

                    if (ResponseContent.Contains("error") == false)
                    {

                        return true;
                    }

                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        private List<TorrentFile> GetTorrentFilesServer(List<string> TorrentHashList)
        {
            List<TorrentFile> TorrentFilesList = null;

            if (IsConnected() && TorrentHashList.Count>0)
            {
                try
                {
                    string hashstring="";

                    foreach (string mhash in TorrentHashList)
                    {
                        hashstring += "&hash=" + mhash;
                    }

                    HttpWebRequest getTorrentRequest = (HttpWebRequest)(HttpWebRequest.Create(host + "?action=getfiles" + hashstring + "&token=" + token));
                    getTorrentRequest.Credentials = credentials;
                    getTorrentRequest.Headers.Add("Cookie", cookie);
                    HttpWebResponse getTorrentResponse = (HttpWebResponse)getTorrentRequest.GetResponse();
                    StreamReader ResponseStream = new StreamReader(getTorrentResponse.GetResponseStream());
                    string ResponseContent = ResponseStream.ReadToEnd();
                    ResponseStream.Close();
                    getTorrentResponse.Close();

                    if (ResponseContent.Contains("error") == false)
                    {
                        JsonObject res = (JsonObject)JsonConvert.Import(ResponseContent);

                        if (res.Contains("files"))
                        {

                            JsonArray tFilesSet = (JsonArray)((JsonArray)res["files"]);
                            
                            TorrentFilesList = new List<TorrentFile>();
                            string lasthash = "";

                            for (int i = 0; i < tFilesSet.Count; i++)
                            {
                                if ((i % 2) == 0)   // hash
                                {
                                    lasthash = tFilesSet[i].ToString();
                                    
                                }
                                else                // files array data
                                {
                                    foreach (JsonArray tfile in (JsonArray)tFilesSet[i])
                                    {
                                        
                                        TorrentFile TFile = new TorrentFile(lasthash,
                                                                            tfile[0].ToString(),                                                                            
                                                                            ((JsonArray)tFilesSet[i]).IndexOf(tfile) ,
                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tfile[1]).ToString()),
                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tfile[2]).ToString()),
                                                                            Convert.ToInt32(((Jayrock.Json.JsonNumber)tfile[3]).ToString())
                                                                            );
                                        
                                                                                

                                        TorrentFilesList.Add(TFile);                                                                      
                                    
                                        
                                    }
                                   
                                }
                            }
                          

                            return TorrentFilesList;
                        }

                    }

                }
                catch (Exception)
                {
                    return null;
                }
            }

            return TorrentFilesList;
        }


        public ObservableCollection<ServerTorrentDataRow> GetTorrentListServer()
        {         
            ObservableCollection<ServerTorrentDataRow> ServerDataSet = null;

            if (IsConnected())
            {
                try
                {

                    HttpWebRequest getTorrentRequest = (HttpWebRequest)(HttpWebRequest.Create(host + "?list=1&token=" + token));
                                        
                    getTorrentRequest.Credentials = credentials;
                    getTorrentRequest.Headers.Add("Cookie", cookie);
                    HttpWebResponse getTorrentResponse = (HttpWebResponse)getTorrentRequest.GetResponse();
                    StreamReader ResponseStream = new StreamReader(getTorrentResponse.GetResponseStream());
                    string ResponseContent = ResponseStream.ReadToEnd();
                    ResponseStream.Close();
                    getTorrentResponse.Close();

                    if (ResponseContent.Contains("error") == false)
                    {
                        JsonObject res = (JsonObject)JsonConvert.Import(ResponseContent);

                       
                        if (res.Contains("torrents") == true)
                        {                                                        
                            JsonArray tTorrentsSet = (JsonArray)res["torrents"];

                            string localCacheId = "";
                            
                            if (res.Contains("torrentc") == true)
                            {
                                localCacheId = res["torrentc"].ToString();
                            }

                            // get categories

                            List<СategoriesDataRow> LocalCategoriesData = uAdd.App.DeserializeString<List<СategoriesDataRow>>(Properties.Settings.Default.CategoriesTable);

                            bool flag = true;

                            if (LocalCategoriesData.Count >0)
                            {
                                ObservableCollection<СategoriesDataRow> ServerCategoriesDataTemp = new ObservableCollection<СategoriesDataRow>();

                                flag = GetTorrentsWebDirectory(ServerCategoriesDataTemp);

                                if (flag==true)
                                {
                                    List<СategoriesDataRow> ServerCategoriesData = ServerCategoriesDataTemp.ToList();

                                    foreach(СategoriesDataRow lc in LocalCategoriesData)
                                    {
                                        СategoriesDataRow findCategory = ServerCategoriesData.Find(
                                                delegate(СategoriesDataRow sc)
                                                {
                                                    return (sc.TorrentDirectoryListNumber == lc.TorrentDirectoryListNumber);
                                                }
                                            );

                                        if (findCategory !=null)
                                        {
                                            lc.Comment = findCategory.Comment;
                                        }
                                        else
                                        {
                                            lc.Comment = "";
                                        }

                                    }

                                }
                                
                             
                            }
                           

                            if (flag == true)
                            {
                                                               
                                ServerDataSet = new ObservableCollection<ServerTorrentDataRow>();

                                List<string> TorrentHashList = new List<string>();
                                                                                             

                                string TorrentCategory ="";
                                string TorrentDir = "";
                                string TemplateTorrentDir = "";


                                foreach (JsonArray tTorrent in tTorrentsSet)
                                {
                                    TemplateTorrentDir = "";                                    
                                    TorrentDir = tTorrent[26].ToString();

                                    TemplateTorrentDir = (TorrentDir.Substring(0, TorrentDir.LastIndexOf("\\"))).Trim().ToUpper();

                                    if (LocalCategoriesData.Count > 0 && TemplateTorrentDir!="")
                                    {                                        
                                        СategoriesDataRow findCategory = LocalCategoriesData.Find(
                                                delegate(СategoriesDataRow localCat)
                                                {
                                                    return (localCat.Comment.Trim().ToUpper() == TemplateTorrentDir);
                                                }
                                            );

                                        if (findCategory != null)
                                        {
                                            TorrentCategory = findCategory.CategoryName;
                                        }
                                        else
                                        {
                                            TorrentCategory = "";
                                        }
                                        
                                     }
                                     else
                                     {
                                        TorrentCategory = "";
                                     }

                                    ServerTorrentDataRow NewRow = new ServerTorrentDataRow(tTorrent[0].ToString(),                                                    // hash
                                                                                            Convert.ToInt16(((Jayrock.Json.JsonNumber)tTorrent[1]).ToString()),       // data state
                                                                                            tTorrent[21].ToString(),                                                  // status
                                                                                            tTorrent[2].ToString(),                                                   // name
                                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tTorrent[3]).ToString()),       // size
                                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tTorrent[4]).ToString()),       // execution percent
                                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tTorrent[5]).ToString()),       // downloaded size
                                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tTorrent[6]).ToString()),       // uploaded size
                                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tTorrent[8]).ToString()),       // upload speed
                                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tTorrent[9]).ToString()),       // download speed
                                                                                            tTorrent[11].ToString(),                                                  // label
                                                                                            TorrentCategory,                                                          // category
                                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tTorrent[12]).ToString()),      // connected peers
                                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tTorrent[14]).ToString()),      // connected seed
                                                                                            Convert.ToInt32(((Jayrock.Json.JsonNumber)tTorrent[17]).ToString()),      // order queue
                                                                                            Convert.ToInt64(((Jayrock.Json.JsonNumber)tTorrent[18]).ToString()),      // remaining size
                                                                                            localCacheId);                                                            // cache id




                                    if ( NewRow.OrderQueue == -1)
                                    {
                                        
                                        if (Properties.Settings.Default.ShowDownloaded == true)
                                        {
                                            ServerDataSet.Add(NewRow);
                                        }
                                    }
                                    else
                                    {
                                        ServerDataSet.Add(NewRow);
                                    }    

                                    TorrentHashList.Add(NewRow.Hash);

                                }

                                if (ServerDataSet != null & ServerDataSet.Count() > 0)
                                {
                                    int MaxIndex = ServerDataSet.Count();
                                    bool isReCalc = false;

                                    // re calc order number
                                    for (int i = 0; i < MaxIndex; i++)
                                    {
                                        if (ServerDataSet[i].OrderQueue == (-1))
                                        {
                                            ServerDataSet[i].OrderQueue = (MaxIndex + 1);
                                            isReCalc = true;
                                        }
                                    }

                                    ServerDataSet = new ObservableCollection<ServerTorrentDataRow>(ServerDataSet.OrderBy(x => x.OrderQueue));

                                    if (isReCalc == true)
                                    {
                                        int cIndex = 1;

                                        for (int i = 0; i < MaxIndex; i++, cIndex++)
                                        {
                                            ServerDataSet[i].OrderQueue = cIndex;
                                        }
                                    }
                                }                                    


                                if (ServerDataSet.Count > 0)
                                {
                                    List<TorrentFile> TorrentFileList = GetTorrentFilesServer(TorrentHashList);

                                    if (TorrentFileList != null)
                                    {
                                        foreach (ServerTorrentDataRow mTorrentRow in ServerDataSet)
                                        {
                                            List<TorrentFile> findfiles = TorrentFileList.FindAll(
                                                    delegate(TorrentFile tf)
                                                    {
                                                        return (tf.Hash == mTorrentRow.Hash);
                                                    }
                                                );

                                            if (findfiles.Count > 0)
                                            {

                                                findfiles = new List<TorrentFile>(findfiles.OrderBy(x => x.Name));
                                                
                                                mTorrentRow.Files = findfiles;

                                            }


                                        }

                                    }
                                    else
                                    {
                                        ServerDataSet = null;
                                    }
                                }
                                
                            }
                            else
                            {
                                ServerDataSet = null;
                            }

                        }
                        
                    }

                }
                catch (Exception)
                {
                    ServerDataSet = null;
                }
            }

            return ServerDataSet;
        }


    }      // class TorrentAPI

    public class TorrentInfo
    {
        public string FileName;
        public string FilePath;
        public string ContentName;
        public string ContentInfo;
        public string ContentSize;
        public string Hash;
        public bool   MultiFile;
        public long   FileLength;

        public TorrentInfo()
        {
            Clear();
        } // TorrentInfo constructor 1

        private void Clear()
        {
            FileName = "";
            FilePath = "";
            ContentName = "";
            ContentInfo = "";
            ContentSize = "";
            Hash        = "";
            MultiFile = false;
            FileLength = 0;
        }

        public TorrentInfo(string TorrentInfoFilePath, ref ObservableCollection<FileNode> FileTree)
        {
            ReadInfoFile(TorrentInfoFilePath, ref FileTree);

        } // TorrentInfo constructor 2

        public void ReadInfoFile(string TorrentInfoFilePath, ref ObservableCollection<FileNode> FileTree)
        {
            Clear();
            FileTree.Clear();

            if (TorrentInfoFilePath.Length >0)       
            {
                IBencodingType BEObject = null;

                try
                {
                    BEObject = BencodeLibrary.BencodingUtils.DecodeFile(TorrentInfoFilePath, Encoding.GetEncoding(437));
                }
                catch
                {
                    return;
                }
                               
                if (BEObject != null)
                {
                    BDict BTree = (BDict)BEObject;
                    if (BTree != null)
                    {
                        if (BTree.ContainsKey("info") == true)
                        {
                            BDict BInfo = (BDict)BTree["info"];
                            if (BInfo != null && BInfo.ContainsKey("name") == true && BInfo.ContainsKey("pieces") == true)
                            {   
                                FilePath = TorrentInfoFilePath;
                                FileName = (new FileInfo(TorrentInfoFilePath)).Name;

                                Hash = BitConverter.ToString(BencodeLibrary.BencodingUtils.CalculateTorrentInfoHash(BInfo)).Replace("-", string.Empty);
                                                                
                                BEObject = BencodeLibrary.BencodingUtils.DecodeFile(TorrentInfoFilePath, Encoding.UTF8);
                                BTree = (BDict)BEObject;
                                BInfo = (BDict)BTree["info"];

                                BString BKeyName = (BString)BInfo["name"];
                                ContentName = BKeyName.Value.ToString().Trim();
                                                    
                                
                                if (BTree.ContainsKey("comment") == true)
                                {   
                                    BString BComment = (BString)BTree["comment"];
                                    ContentInfo = BComment.Value.ToString().Trim();
                                }

                                if (BInfo.ContainsKey("files") == true) // multi file torrent
                                {
                                    MultiFile = true;

                                    BList InfoFiles = (BList)BInfo["files"];

                                    foreach (BDict file in InfoFiles)
                                    {
                                        BList filePaths = (BList)file["path"];
                                        long length = ((BInt)file["length"]).Value;
                                                                                                                                                                
                                        string fullPath = "";

                                        FileNode LastRootNode = null;

                                        foreach (BString partOfPath in filePaths)
                                        {
                                            fullPath += (fullPath.Length>0 ? @"\" : "" ) + partOfPath.Value;

                                            if (filePaths.IndexOf(partOfPath) < (filePaths.Count - 1))   // this is folder
                                            {
                                                // find folder in file tree
                                                FileNode CurrentFolder = FindFolder(FileTree, partOfPath.Value, LastRootNode);

                                                if (CurrentFolder == null)   // create folder in file tree
                                                {
                                                    CurrentFolder = new FileNode();
                                                    CurrentFolder.Text = partOfPath.Value;
                                                    CurrentFolder.FilePath = fullPath;
                                                    CurrentFolder.IsChecked = true;
                                                    CurrentFolder.IsExpanded = false;
                                                    CurrentFolder.IconImageUri = "folderalt.ico";

                                                    if (LastRootNode != null)
                                                    {
                                                        CurrentFolder.Parent.Add(LastRootNode);
                                                        LastRootNode.Children.Add(CurrentFolder);
                                                    }
                                                    else
                                                    {
                                                        FileTree.Add(CurrentFolder);
                                                    }
                                                    
                                                }
                                                LastRootNode = CurrentFolder;
                                                
                                            }
                                            else   // this is file
                                            {
                                                
                                                FileNode CurrentFile = new FileNode();
                                                CurrentFile.FilePath = fullPath;
                                                CurrentFile.Text = partOfPath.Value;
                                                CurrentFile.Length = length;
                                                CurrentFile.IsChecked = true;
                                                CurrentFile.IsExpanded = false;
                                                CurrentFile.IconImageUri = "document.ico";
                                                                                               
                                                if (LastRootNode != null)
                                                {
                                                    CurrentFile.Parent.Add(LastRootNode);
                                                    LastRootNode.Children.Add(CurrentFile);
                                                }
                                                else
                                                {
                                                    FileTree.Add(CurrentFile);
                                                }
                                            }

                                            
                                        }
                                        

                                    }

                                    
                                    FileTree = SortFileTree(FileTree);
                                    
                                }
                                else                                    // single file torrent
                                {
                                    MultiFile = false;

                                    if (BInfo.ContainsKey("length") == true)
                                    {
                                        FileLength = ((BInt)BInfo["length"]).Value;
                                    }
                                    else
                                    {
                                        ContentName = "";
                                        Hash        = "";
                                    }
                                                                       
                                }

                                                              
                                if (MultiFile == true)
                                {
                                    FileLength = CalculateTotal(FileTree);
                                }

                                ContentSize = GetFileSizeFormat(FileLength);                                


                            }
                        }

                    }
                }
            }


        }

        public ObservableCollection<FileNode> SortFileTree(ObservableCollection<FileNode> FileTree)
        {
            if (FileTree != null)
            {
                
                ObservableCollection<FileNode> FoldersTemp = new ObservableCollection<FileNode>();
                ObservableCollection<FileNode> FilesTemp = new ObservableCollection<FileNode>();

                foreach (FileNode item in FileTree)
                {
                    if (item.Children.Count()>0)  // this is folder
                    {                        
                        item.Children = SortFileTree((ObservableCollection<FileNode>)item.Children);
                        FoldersTemp.Add(item);
                    }
                    else                          // this is file
                    {
                        FilesTemp.Add(item);
                    }
                }

                FoldersTemp = new ObservableCollection<FileNode>(FoldersTemp.OrderBy(x => x.Text));
                FilesTemp = new ObservableCollection<FileNode>(FilesTemp.OrderBy(x => x.Text));
                                
                FileTree = new ObservableCollection<FileNode>(FoldersTemp.Concat(FilesTemp));
                                
            }

            return FileTree;
        }
    
        public bool IsEmpty()
        {
            return (ContentName.Length == 0 || Hash.Length == 0);
        }

        public List<FileNode> GetUnCheckedFiles(ObservableCollection<FileNode> FileTree)
        {
            List<FileNode> UnCheckedFiles = new List<FileNode>();
                                   
            if (FileTree != null)
            {

                foreach (FileNode fNode in FileTree)
                {
                    if (fNode.Children != null && fNode.Children.Count > 0)  // this is a folder
                    {
                        List<FileNode> ChildrenFiles = GetUnCheckedFiles(fNode.Children);
                                                
                        UnCheckedFiles = (List<FileNode>)(IEnumerable<FileNode>)UnCheckedFiles.Concat(ChildrenFiles).ToList();                        
                        
                    }
                    else                                                     // this is a file
                    {
                        if (fNode.IsChecked == false)
                        { 
                            UnCheckedFiles.Add(fNode);
                        }
                        
                    }

                }

            }

            return UnCheckedFiles;
        }

        public long CalculateTotal(ObservableCollection<FileNode> FileTree)
        {
            long NodeSetLength = 0;

            if (FileTree !=null)
            {

                foreach (FileNode fNode in FileTree)
                {
                    if (fNode.Children != null && fNode.Children.Count > 0)  // this is a folder
                    {
                        long nodesize = CalculateTotal(fNode.Children);
                        fNode.Length = nodesize;
                        fNode.FileSize = GetFileSizeFormat(fNode.Length);                        
                        NodeSetLength += fNode.IsChecked == false ? 0 : fNode.Length;
                    }
                    else                                                     // this is a file
                    { 
                        fNode.FileSize = GetFileSizeFormat(fNode.Length);
                        NodeSetLength += fNode.IsChecked ==false ? 0:fNode.Length;
                    }
                    
                }  

            }

            return NodeSetLength;
        }
        
        public string GetFileSizeFormat(long FileSizeByte)
        {
            return App.GetSizeReadable(FileSizeByte);
        }

        public FileNode FindFolder(ObservableCollection<FileNode> FileTree, string SearchName, FileNode RootNode)
        {
            ObservableCollection<FileNode> SearchArea = FileTree;

            if (RootNode != null)
            {
                SearchArea = RootNode.Children; 
                               
            }

            foreach (FileNode fNode in SearchArea)
            {
                if (fNode.Text == SearchName)
                {
                    return fNode;
                }
            }

            return null;
        }


    }

    
    public class TorrentFile
    {
        private string _Hash;
        private string _Name;
        private int _WebIndex;
        private string _Size;
        private string _Downloaded;
        private string _Priority;
        private TorrentFilePriority _PriorityData;
        private string _executionPercent;
        private Int32 _ExecutionPercentData;

        public string Hash { get { return _Hash; } set { _Hash = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public int WebIndex { get { return _WebIndex; } set { _WebIndex = value; } }
        public string Size { get { return _Size; } set { _Size = value; } }
        public string Downloaded { get { return _Downloaded; } set { _Downloaded = value; } }
        public string Priority { get { return _Priority; } set { _Priority = value; } }
        public TorrentFilePriority PriorityData { get { return _PriorityData; } set { _PriorityData = value; } }

        public string ExecutionPercent { get { return _executionPercent; } set { _executionPercent = value; } }
        public Int32 ExecutionPercentData { get { return _ExecutionPercentData; } set { _ExecutionPercentData = value; } }

        public TorrentFile()
        { 
            Hash = "";
            Name = "";
            WebIndex = 0;
            Size = "";
            Downloaded = "";
            Priority = "";
            PriorityData = (TorrentFilePriority)0;
            ExecutionPercent = "";
            ExecutionPercentData = 0;
        }

        public TorrentFile(string NewHash, string NewName, int NewWebIndex, long NewSize, long NewDownloaded, int NewPriorityData)
        {
            Hash = NewHash;
            Name = NewName;
            WebIndex = NewWebIndex;
            Size = App.GetSizeReadable(NewSize);
            Downloaded = App.GetSizeReadable(NewDownloaded);
            PriorityData = (TorrentFilePriority)NewPriorityData;
            Priority = App.GetCulturePresentationEnum(PriorityData);

            if (NewSize == 0 && NewDownloaded == 0)
            {
                ExecutionPercentData = 100;
            }
            else if (NewSize == 0 && NewDownloaded != 0)
            {
                ExecutionPercentData = 0;
            }
            else
            {
                ExecutionPercentData = (Int32)Math.Ceiling(((double)NewDownloaded / (double)NewSize) * 100);
            }
            
            ExecutionPercent = (ExecutionPercentData).ToString() + " %";            
        }

        public bool IsEmpty()
        {
            return (Hash == "") ? true : false;
        }


    }

    [Flags]
    public enum TorrentServerState: short
    {
        None = 0,
        Started = 1,
        Check = 2,
        RunningAfterChecking = 4,
        Checked = 8,
        Error = 16,
        Suspended = 32,
        Queued = 64,
        Uploaded = 128        
    }

    public enum TorrentFilePriority : int
    {
        DontDownload = 0,
        LowPriority = 1,
        NormalPriority = 2,
        HighPriority = 3
    }

    public class ServerTorrentDataRow
    {
        private string _hash;
        private TorrentServerState _dataState;
        private string _status;
        private string _name;
        private string _size;
        private string _executionPercent;
        private string _downloadedSize;
        private string _uploadedSize;
        private string _uploadSpeed;
        private string _downloadSpeed;        
        private string _label;
        private string _category;
        private long _connectedPeers;
        private long _connectedSeed;
        private int _orderQueue;
        private string _remainingSize;
        private string _cacheID;
        private List<TorrentFile> _files;
        private double _ExecutionPercentData;
             
        
        public string Hash { get {return _hash;} set { _hash = value; } }
        public TorrentServerState DataState { get { return _dataState; } set { _dataState = value; } }
        public string Status { get {return _status;} set { _status = value; } }
        public string Name { get {return _name;} set { _name = value; } }
        public string Size { get {return _size;} set { _size = value; } }
        public string ExecutionPercent { get { return _executionPercent; } set { _executionPercent = value; } }
        public string DownloadedSize { get { return _downloadedSize; } set { _downloadedSize = value; } }
        public string UploadedSize { get { return _uploadedSize; } set { _uploadedSize = value; } }
        public string UploadSpeed { get { return _uploadSpeed; } set { _uploadSpeed = value; } }
        public string DownloadSpeed { get { return _downloadSpeed; } set { _downloadSpeed = value; } }
        
        public string Label { get {return _label;} set { _label = value; } }
        public string Category { get { return _category; } set { _category = value; } }
        public long ConnectedPeers { get {return _connectedPeers;} set { _connectedPeers = value; } }
        public long ConnectedSeed { get {return _connectedSeed;} set { _connectedSeed = value; } }
        public int OrderQueue { get {return _orderQueue;} set { _orderQueue = value; } }
        public string RemainingSize { get { return _remainingSize; } set { _remainingSize = value; } }
        public string CacheID { get {return _cacheID;} set { _cacheID = value; } }
        public List<TorrentFile> Files { get { return _files; } set { _files = value; } }
        public double ExecutionPercentData { get { return _ExecutionPercentData; } set { _ExecutionPercentData = value; } }
        public string SizeWithDownload { get {
                                                return (_size + " (" + DownloadedSize + " " + uAdd.Properties.Resources.TorrentListCaptionDownloadedSize.ToLower() + ")");
                                              }
                                        }
                    
        public ServerTorrentDataRow()
        { 
            Hash = "";
            DataState = TorrentServerState.None;  
            Status = "";
            Name = "";
            Size = "";
            ExecutionPercent = "";
            DownloadedSize = "";
            UploadedSize = "";
            UploadSpeed = "";
            DownloadSpeed = "";
            Label = "";
            Category = "";
            ConnectedPeers = 0;
            ConnectedSeed = 0;
            OrderQueue = 0;
            RemainingSize = "";
            CacheID = "";
            ExecutionPercentData = 0;
            Files = null;
        }

        public ServerTorrentDataRow(string NewHash, Int16 NewDataState, string NewStatus, string NewName, long NewSize, long NewExecutionPercent,
                                    long NewDownloadedSize, long NewUploadedSize, long NewUploadSpeed, long NewDownloadSpeed, string NewLabel,string NewCategory, long NewConnectedPeers,
                                    long NewConnectedSeed, int NewOrderQueue, long NewRemainingSize, string NewCacheID)
        {
            Hash = NewHash;
            DataState = (TorrentServerState)NewDataState;
            Status = NewStatus;
            Name = NewName;
            Size = App.GetSizeReadable(NewSize);
            ExecutionPercent = (NewExecutionPercent/10).ToString() + " %";
            DownloadedSize = App.GetSizeReadable(NewDownloadedSize);
            UploadedSize = App.GetSizeReadable(NewUploadedSize);
            UploadSpeed = App.GetSizeReadable(NewUploadSpeed) + "/" + uAdd.Properties.Resources.Seconds;
            DownloadSpeed = App.GetSizeReadable(NewDownloadSpeed) + "/" + uAdd.Properties.Resources.Seconds;
            Label = NewLabel;
            ConnectedPeers = NewConnectedPeers;
            ConnectedSeed = NewConnectedSeed;
            OrderQueue = NewOrderQueue;
            RemainingSize = App.GetSizeReadable(NewRemainingSize);
            CacheID = NewCacheID;
            Files = null;
            Category = NewCategory;
            ExecutionPercentData = NewExecutionPercent / 10;
        }

        public bool IsEmpty()
        {
            return (Hash=="") ? true : false;
        }
              
    }

        
}
