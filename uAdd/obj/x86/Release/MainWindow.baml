   M S B A M L   `   `   `  ���� ?  ;uAdd, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:uAdd.PropertiesuAdd.Properties  clr-namespace:uAdduAdd      uAdd.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -resclr-namespace:uAdd.Properties     $localclr-namespace:uAdd     -   $��
uAddWindow��   uAdd.Properties.Resources    	TitleText � Title# Z  $	��Top=�$	��550��$	��450��$	��400��$	��250��$	��800��$	��800��$��Stretch=� � Icon$ utorrent.ico�� � WindowStartupLocation$ CenterScreen=�
������� � 	Resources  �� DataType   uAdd.FileNode �� ItemsSource � ShowGridLines � ColumnDefinitions	 d� 	IsChecked
 �� Mode
 J� Uid �� 	UriSource �� 
TargetType �� Property �� Value �� Path��%  (��       *, +)'Q� �    �� "   �� *
Children+.R�� $��TreeViewDataGrid�� .  $��Stretch=�$	��150�� �� $
��Auto ��� $
��Auto ��� $��* ��� $
��auto �.+��� -   $��0��$��0��$��0,0,3,0q�	 �� *	IsChecked+$
 TwoWay=� �� *Id+�� $��0��$��1��$��16��$��16������  �� *IconImageUri+�� $��0��$��2��$��3,0,5,0q����� *Text+�� $��0��$��3��$��30,0,3,0q����� *
FileSize+�� " Q�.���  �@Q�
IsExpanded �� $ 
IsExpanded�$
 TwoWay=�.��� -   $��
WindowGrid��$
��auto��$��Stretch=� .   � RowDefinitions �� -    �Name$ WindowGridRow1��$
��auto ��� -   $ WindowGridRow2��$��* ��� -   $ WindowGridRow3��$
��auto ��� -   $ WindowGridRow4��$
��auto �.+�� -   $��DataGrid��$��0�� .  $
��auto��$	��Top=�$��Stretch=� �� $
��Auto ��� $
��auto ��� $
��auto � �� $
��Auto ��� $��* ��� $
��Auto �.+��� -	   $��FilePathLabel��$��0��$��0��   FilePathText#��Z $��Center=�$
��auto��$��Right=�$��3,2,2,2q�$��2q�$��Center=��� -
   $��FilePathBox��$��0��$��1��$��3,2,2,2q�$��Center=�$��Stretch=�$
��Left=����� *
FilePath+$
 TwoWay=��� -   $��FilePathBtn��$��0��$��2��$	��...��$��20��$��20��$��3,2,2,2q�$��Right=�$��Center=��� -   $��TorrentNameLabel��$��1��$��0��   TorrentNameText#��Z $��Center=�$
��auto��$��Right=�$��3,2,2,2q�$��2q�$��Center=��� -   $��TorrentNameBox��$��1��$��1��$	��350�� �� AcceptsReturn .   �� 
AcceptsTab .  $��Center=�$��Center=�$��3,2,2,2q� �� 
IsReadOnly .  ^� HasDropShadow . ���� *TorrentContent+$
 TwoWay=��� -   $��ContentHelpBtn��$��1��$��2��$��20��$��20��$��3,2,2,2q�$��Right=�$��Center=� � ToolTip �� *СontentInfo+.����  �� Orientation$ 
Horizontal=�$��Center=�$��Center=�$��25��$��25��.+��� $��Helpalt.ico��$��25��$��25��$��Center=�$��Center=��� -   $��ContentSizeLabel��$��2��$��0��   ContentSizeText#��Z $��Center=�$
��auto��$��Right=�$��3,2,2,2q�$��2q�$��Center=��� -   $��ContentSizeBox��$��2��$��1��$��2��$��Center=�$
��auto��$
��Left=�$��3,2,2,2q�$��Center=����� *ContentSize+$
 OneWay=�� -   $��FileListGrid��$��1�� .  $
��auto��$��Stretch=�$��Stretch=� �� $
��auto ��� $��* � �� $
��auto ��� $��* ��� $
��Auto �.+��� -   $��FileListLabel��$��0��$��0��   FileListText#��Z $	��Top=�$��Right=�$��3,2,2,2q�$��2q�$	��top=�$��Right=����� *ActualWidth+ �� ElementName$ FilePathLabel��$
 OneWay=��� $ 
Horizontal=�$��0��$��1��$��3,2,2,2q�.+��� -   $y�Visible=�   CommandCheckAllFiles# Z 
������ ���� $ 
uAddWindow��$ 
Background�$
 OneWay=�.���� $��	check.ico��$��16��$��16���� -   $y�Visible=�   CommandUnCheckAllFiles# Z $��1,0,0,0q�
������ ���� $ 
uAddWindow��$ 
Background�$
 OneWay=�.���� $��uncheck.ico��$��16��$��16��R� -   $��FilesTreeView��$��1��$��1��$��2��$
��auto��$��Stretch=�$��Stretch=�$��3,2,2,2q�� -   $��
ActionGrid��$��2��$
��auto�� .  $��0,3,0,0q� �� $
��auto ��� $
��auto ��� $
��auto � �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+��� -   $��CategoryNameLabel��$��0��$��0��    CategoryNameText#��Z  $��center=�$��Right=�$��3,2,2,2q�$��2q�$	��Top=�$��Right=����� *ActualWidth+$ FilePathLabel��$
 OneWay=��� -   $��CategoriesSelect��$��0��$��1��$��3��! �� Text$!  ��$
��auto��$��Stretch=�$��3,2,2,2q�$��Center=�$
��auto��" �� 
IsEditable" .  ���� *CategoriesList+$
 TwoWay=����� *CategoryFontWeight+���� *CategoryFontSize+�� -   $��ForceStartCheck��$��1��$��1��$��3��#   ForceStartCheckText#��Z# $
��Left=�$
��Left=�$��3,2,2,2q�	 �� *
ForceStart+$
 TwoWay=��� -   $��NotStartDownloadCheck��$��2��$��1��$��3�� $   NotStartDownloadCheckText#��Z$ $
��Left=�$
��Left=�$��3,2,2,2q�	 �� *NotStartDownload+$
 TwoWay=�� -   $��
ButtonGrid��$��3�� .  $
��auto��$
��auto��$��Bottom=�$��Stretch=�$��0,10,0,5q� �� $
��auto ��� $
��auto ��� $
��auto ��� $��* ��� $
��auto ��� $
��auto � �� $
��Auto �.+��� $��0��$��0��$��3,0,0,0q�$��Center=�$��Center=�|�. ���� *ActualWidth+$ FilePathLabel��$
 OneWay=��� -   $ 	AboutLink��$��AboutNavigate?�.�.��� %   AboutLabelText#�Z% �� $��0��$��1��$��10,0,0,0q�$��Center=�$��Center=�|�. ���� *ActualWidth+$ FilePathLabel��$
 OneWay=��� -   $ 
OptionLink��$��OptionNavigate?�.�.��� &   OptionLabelText#�Z& �� $��0��$��2��$��3,0,0,0q�$��Center=�$��Center=�|�. ���� *ActualWidth+$ FilePathLabel��$
 OneWay=��� -   $ 
ServerLink��$��OptionNavigate?�' �� BaselineAlignment$' Center=�.�.��� (   ServerLabelText#�Z( �� -   $��MainStatusBar��$��0��$��3��$
��auto��$
��auto��$��Stretch=�$��0,0,0,0q�$��Stretch=�$��Center=�$
��Left=�$��0q�
�������.@��� -    $��MainStatusBarItem1��$��0,0,0,0q��� -!   $��OKBtn��$��0��$��4��)   OKButtonText#��Z) $��4,1,4,1q�$
��auto��$
��auto��$��Center=�$��Right=�$��6,2,6,2q�* �� 	IsDefault* . �� -"   $��	CancelBtn��$��0��$��5��+   CancelButtonText#��Z+ $��4,1,4,1q�$
��auto��$
��auto��$��Center=�$��Right=�$��6,2,6,2q�, �� IsCancel, . 