   M S B A M L   `   `   `  ���� ?  ;uAdd, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:uAdd.PropertiesuAdd.Properties  clr-namespace:uAdduAdd      uAdd.ServerWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -resclr-namespace:uAdd.Properties     $localclr-namespace:uAdd        uAdd.Properties.Resources    ServerTitleText � Title# Z   � Icon$ utorrent.ico�� � WindowStartupLocation$ CenterScreen=�$��Stretch=�$��Stretch=�$	��400��$	��450��$	��550��$	��800�� J� CommandBindings )  #System.Windows.Input.CommandBinding  -      Command$
 Copy��  -   $% local:CommandLibrary.RowCommand��  -   $( "local:CommandLibrary.ColumnCommand��  -   $) #local:CommandLibrary.FileRowCommand�� � 	Resources  %  !ContextMenuServerColumnsItemStyle �� 
TargetType �� Property	 �� Value
 �� RelativeSource �� Mode �� Path �� AncestorType " ContextMenuRowCommandItemStyle & "ContextMenuFileRowCommandItemStyle ��Name  HeaderColumnsMenu �� 	StaysOpen �� Mode��%�  &	        &	 J    &	 A    &	 <    7   �� " m�.���  � ��	 �� *Header+��  �@m�IsCheckable	 . ��  �@m�	IsChecked	 �� *	IsChecked+��  �@m�Command$(	 "local:CommandLibrary.ColumnCommand����  �@m�CommandParameter	 �� 
 �� $
 Self=���  �@m�CommandTarget	 �� $ PlacementTarget�
 �� $ ContextMenuL��� " m�.���  � ��	 �� *Name+��  �@m�Command$%	 local:CommandLibrary.RowCommand����  �@m�CommandParameter	 �� 
 �� $
 Self=���  �@m�CommandTarget	 �� $ PlacementTarget�
 �� $ ContextMenuL��� " m�.���  � ��	 �� *Name+��  �@m�Command$)	 #local:CommandLibrary.FileRowCommand����  �@m�CommandParameter	 �� 
 �� $
 Self=���  �@m�CommandTarget	 �� $ PlacementTarget�
 �� $ ContextMenuL��� $ HeaderColumnsMenu��8��   . ���� *ServerColumns+$ TwoWay=�.��� -   $��ServerWindowGrid��$
��auto��$
��auto��$��Stretch=� � RowDefinitions �� $
��Auto ��� $	��100��$��* ��� $
��auto ��� $	��170 �$	��100��$	��350���� $
��auto � � ColumnDefinitions �� $��* �.+�`� $��0��$��0��$	��Top=�$	��Top=�$��Stretch=�.�c� $	��Top=� � UseLayoutRounding .  $��Stretch=�.@��� -   $y�Visible=�#   SettingsCategoriesCommandAdd � ToolTip# Z .���� $��add.ico��$��16��$��16���� -   $y�Visible=�&   SettingsCategoriesCommandDelete# Z .���� $��
cancel.ico��$��16��$��16���� �� -   $y�Visible=�   RowMenuActionStart# Z .���� $��play.ico��$��16��$��16���� -	   $y�Visible=�   RowMenuActionPause# Z .���� $��	pause.ico��$��16��$��16���� -
   $y�Visible=�   RowMenuActionStop# Z .���� $��stop.ico��$��16��$��16���� �� -   $y�Visible=�   OptionShowDownloaded# Z .���� $��checkmark.ico��$��16��$��16���� -   $y�Visible=�   OptionLabelText# Z .���� $��tool.ico��$��16��$��16���� -   $y�Visible=�   OptionRefreshText# Z .���� $��refresh.ico��$��16��$��16��&   System.Windows.Controls.DataGrid  -     Name$ ServerDataGrid��$
��auto��$��1��$��0��   SelectionMode$ Single=�$��0,0,0,0q�$��Stretch=�$��Stretch=�   AutoGenerateColumns .      HeadersVisibility$  Column=�!   CanUserAddRows! . "   CanUserDeleteRows" . #   CanUserResizeRows# . $��3q�$��1q�$   HorizontalGridLinesBrush$ �	#FFF0F0F0%   VerticalGridLinesBrush% �	#FFF0F0F0#��["��&   CanUserReorderColumns& . '   
IsReadOnly' . ���� *
ServerData+$ TwoWay=� G  ASystem.Windows.Controls.Primitives.DataGridColumnHeadersPresenter��%9   '        7  �� "  .���  �@�ContextMenu8	   (   Columns( 0  *System.Windows.Controls.DataGridTextColumn  )   TorrentListCaptionIndex,  &System.Windows.Controls.DataGridColumn*   Header#* Z) +   Width5  /System.Windows.Controls.DataGridLengthConverter$
+ auto ,   
IsReadOnly, . 1  +System.Windows.Controls.DataGridBoundColumn-   Binding- �� $ 
OrderQueue�  .   TorrentListCaptionName#* Z. $
+ auto /   MinWidth$	/ 200Q�, . - �� $
 Name�  0   TorrentListCaptionSize#* Z0 $
+ auto , . - �� $
 Size�  1   TorrentListCaptionStatus#* Z1 $
+ auto , . - �� $ Status�  !2   TorrentListCaptionCategory#* Z2 $
+ auto , . - �� $ Category�  3   TorrentListCaptionLabel#* Z3 $
+ auto , . - �� $ Label�  '4    TorrentListCaptionDownloadedSize#* Z4 $
+ auto , . - �� $ DownloadedSize�  &5   TorrentListCaptionRemainingSize#* Z5 $
+ auto , . - �� $ RemainingSize�  )6   "TorrentListCaptionExecutionPercent#* Z6 $
+ auto , . - �� $ ExecutionPercent�  &7   TorrentListCaptionDownloadSpeed#* Z7 $
+ auto , . - �� $ DownloadSpeed�  $8   TorrentListCaptionUploadSpeed#* Z8 $
+ auto , . - �� $ UploadSpeed�  %9   TorrentListCaptionUploadedSize#* Z9 $
+ auto , . - �� $ UploadedSize�  ':    TorrentListCaptionConnectedPeers#* Z: $
+ auto , . - �� $ ConnectedPeers�  &;   TorrentListCaptionConnectedSeed#* Z; $
+ auto , . - �� $ ConnectedSeed�< � ContextMenu< �� -   $��RowCommandMenu��#��[ ���� $ RowCommands�$ TwoWay=��� = �� ResizeDirection$
= Rows=�$��2��$��0��$��6��$
��auto��$��Stretch=�$��Stretch=�$��0q��� -   $��ServerTorrentTab��$��3��$��0��.@��� -   $��ServerTorrentTabFile��!>   ServerTorrentTabFileHeader#��Z> .���� $
��Auto=�.��  -   $��ServerFilesGrid��$
��auto��$ Single=�$��0,0,0,0q�$��Stretch=�$��Stretch=� .  $  Column=�$��3q�$��1q�$ �	#FFF0F0F0% �	#FFF0F0F0#��["��& . ' . ���� $ 	FilesData�$ TwoWay=�(   #* Z. $
+ auto $	/ 300Q�, . - �� $
 Name�  #* Z0 $
+ auto , . $/ 80Q�- �� $
 Size�  #* Z4 $
+ auto , . $/ 80Q�- �� $ 
Downloaded�  #* Z6 $
+ auto , . $/ 80Q�- �� $ ExecutionPercent�  %?   TorrentListCaptionFilePriority#* Z? $
+ auto , . $	/ 100Q�- �� $ Priority�< �� -   $��FileRowCommandMenu��#��[ ���� $ FileRowCommands�$ TwoWay=��� -   $��ServerTorrentTabInfo��!@   ServerTorrentTabInfoHeader#��Z@ .���� $
��Auto=�.��� $	��100�� �� $
��auto �$	��100���� $��* �$	��170���� $
��auto �$	��100���� $��* �$	��120���� $
��auto �$	��100���� $��* �$	��120�� �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto �.+��� $��0��$��0��A   ServerTorrentInfoName#��ZA $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=��� $��0��$��1��$��5��$��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=�|�. B J� IsManipulationEnabledB . ���� $ SelectedRow.Name�$ OneWay=�< �� .@�m� C m� Command$
C Copy��D m� CommandParameterD �� $ SelectedRow.Name��� $��1��$��0��"E   ServerTorrentInfoDownloaded#��ZE $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=�� -   $��InfoProgress��$��1��$��1��$��5��F � Maximum$	F 100Q�G � Minimum$G 0Q�H � IsIndeterminateH .  I � ValueI �� $% SelectedRowInfoExecutionPercent�$ OneWay=� �� $" SelectedRow.ExecutionPercent�$ OneWay=��� $��1��$��1��$��5��$��Center=�$
��auto��$��Center=�$��2,1,1,1q�$��2q�$��Center=�$��Center=����� $" SelectedRow.ExecutionPercent�$ OneWay=�< �� .@�m� $
C Copy��D �� $" SelectedRow.ExecutionPercent��� $��2��$��0��J   ServerTorrentInfoSize#��ZJ $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=��� $��2��$��1��$��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=����� $" SelectedRow.SizeWithDownload�$ OneWay=�< �� .@�m� $
C Copy��D �� $" SelectedRow.SizeWithDownload��� $��3��$��0��K   ServerTorrentInfoStatus#��ZK $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=��� $��3��$��1��$��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=����� $ SelectedRow.Status�$ OneWay=�< �� .@�m� $
C Copy��D �� $ SelectedRow.Status��� $��2��$��2�� L   ServerTorrentInfoCategory#��ZL $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=��� $��2��$��3��$��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=����� $ SelectedRow.Category�$ OneWay=�< �� .@�m� $
C Copy��D �� $ SelectedRow.Category��� $��3��$��2��M   ServerTorrentInfoLabel#��ZM $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=��� $��3��$��3��$��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=����� $ SelectedRow.Label�$ OneWay=�< �� .@�m� $
C Copy��D �� $ SelectedRow.Label��� $��2��$��4��%N   ServerTorrentInfoDownloadSpeed#��ZN $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=��� $��2��$��5��$��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=����� $ SelectedRow.DownloadSpeed�$ OneWay=�< �� .@�m� $
C Copy��D �� $ SelectedRow.DownloadSpeed��� $��3��$��4��#O   ServerTorrentInfoUploadSpeed#��ZO $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=��� $��3��$��5��$��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=����� $ SelectedRow.UploadSpeed�$ OneWay=�< �� .@�m� $
C Copy��D �� $ SelectedRow.UploadSpeed��� $��4��$��0��&P   ServerTorrentInfoConnectedPeers#��ZP $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=��� $��4��$��1��$��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=����� $  SelectedRow.ConnectedPeers�$ OneWay=�< �� .@�m� $
C Copy��D �� $  SelectedRow.ConnectedPeers��� $��4��$��2��%Q   ServerTorrentInfoConnectedSeed#��ZQ $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=��� $��4��$��3��$��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=����� $ SelectedRow.ConnectedSeed�$ OneWay=�< �� .@�m� $
C Copy��D �� $ SelectedRow.ConnectedSeed��� $��5��$��0��R   ServerTorrentInfoHash#��ZR $��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=��� -   $��ServerHashLabel��$��5��$��1��$��5��$��Center=�$
��auto��$
��Left=�$��2,1,1,1q�$��2q�$��Center=����� $ SelectedRow.Hash�$ OneWay=�< �� .@�m� $
C Copy��D �� $ SelectedRow.Hash�� -   $��StatusBarGrid��$��4��$��Stretch=�$��Stretch=�$
��auto��$
��auto�� �� $��* � �� $
��auto �.+��� $��0��$��0��.@��� .���� -   $��ConnectionStatusBar��S   ServerStatusBarCaption#��ZS �� .���� T �� Orientation$T 
Horizontal=�.+��� -   $!��ServerStatusBarImageConnect��$��Ballgreen.ico��$��16��$��16��$��Center=�$��Center=�y��� *VisibilityConnect+�� -   $$��ServerStatusBarImageNotConnect��$��Ballred.ico��$��16��$��16��$��Center=�$��Center=�y��� *VisibilityNotConnect+