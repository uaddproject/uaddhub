   M S B A M L   `   `   `  ���� ?  ;uAdd, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:uAdd.PropertiesuAdd.Properties      uAdd.SettingsWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -resclr-namespace:uAdd.Properties        uAdd.Properties.Resources    SettingsWindowTitle � Title# Z  $	��350��$	��250��$	��600��$	��380��$	��600�� � SizeToContent$ WidthAndHeight=� � Icon$ utorrent.ico�� � WindowStartupLocation$ CenterScreen=�.��� -   $��MainGrid��$
��auto��$��Stretch=�$��Stretch=� � ColumnDefinitions �� $��* � � RowDefinitions �� $
��auto ��� $
��auto ��� $��* ��� $
��auto �.+�9� -   $��	HostGroup��$��0��$	��Top=�   SettingsHostGroupTitle#��Z $��2,3,2,2q�$��Stretch=�$��Stretch=� 9� 
IsExpanded . .��� -   $��SettingGridMain��$
��auto��$	��Top=�$��Stretch=� �� $
��auto ��� $
��auto ��� $
��auto � �� $
��Auto ��� $��* �.+��� -   $��SettingsHostLabel��$��0��$��0��	   SettingsHostTitle#��Z	 $��Center=�$
��auto��$��Right=�$��10,2,2,2q�$��2q�$��Center=��� -   $��SettingsUserLabel��$��1��$��0��
   SettingsUserTitle#��Z
 $��Center=�$
��auto��$��Right=�$��10,2,2,2q�$��2q�$��Center=��� -   $��SettingsPasswordLabel��$��2��$��0��   SettingsPasswordTitle#��Z $��Center=�$
��auto��$��Right=�$��10,2,2,2q�$��2q�$��Center=��� -   $��SettingsHostBox��$��0��$��1��$��3,2,2,2q�$��Center=�$��Stretch=�$
��Left=����� *Host+ �� Mode$ TwoWay=� � DataContext �� �� -   $��SettingsUserBox��$��1��$��1��$��3,2,2,2q�$��Center=�$��Stretch=�$
��Left=����� *User+$ TwoWay=�G� -	   $��SettingsPasswordBox��$��2��$��1��$��3,2,2,2q�$��Center=�$��Stretch=�$
��Left=�9� -
   $��AdditionalSettingsGroup��$��1��$	��Top=�"   SettingAdditionalGroupTitle#��Z $��2,3,2,2q�$��Stretch=�$��Stretch=�.��]� $��0,5,0,0q���. $��Stretch=�$��Stretch=�.+��� -   $#��AddSettingsFailedRetriesCheck��$	��Top=�$	��Top=�   AddSettingsFailedRetries#��Z $��10,4,2,2q�$��Center=� d� 	IsChecked �� *FailedRetries+$ TwoWay=��� -   $+��%AddSettingsNotAllowEmptyCategoryCheck��$	��Top=�$	��Top=�'    AddSettingsNotAllowEmptyCategory#��Z $��10,4,2,2q�$��Center=� �� *NotAllowEmptyCategory+$ TwoWay=��� -   $+��%AddSettingsNotAddDefaultCategoryCheck��$	��Top=�$	��Top=�'    AddSettingsNotAddDefaultCategory#��Z $��10,4,2,2q�$��Center=� �� *NotAddDefaultCategory+$ TwoWay=��� -   $&�� AddSettingsCategoryBoldFontCheck��$	��Top=�$	��Top=�"   AddSettingsBoldFontCategory#��Z $��10,4,2,2q�$��Center=� �� *BoldFontCategory+$ TwoWay=��� -   $%��AddSettingsCategoryBigFontCheck��$	��Top=�$	��Top=�!   AddSettingsBigFontCategory#��Z $��10,4,2,2q�$��Center=� �� *BigFontCategory+$ TwoWay=�9� -   $��TabGroup��$��2��$	��Top=�#   SettingsCategoriesTableTitle#��Z $��2,3,2,2q�$��Stretch=�$��Stretch=� . .���  �� $��* � �� $
��auto ��� $��* �.+�`� $��0��$��0��$	��Top=�$��3,1,4,0q�$	��Top=�$��Stretch=�.�c� -   $��CategoriesGridToolbar��$	��Top=� � UseLayoutRounding .  $��Stretch=�.@��� -   $!��BtnCategoriesGridCommandAdd��$y�Visible=�#   SettingsCategoriesCommandAdd � ToolTip# Z .���� $��add.ico��$��16��$��16���� -   $y�Visible=�&   SettingsCategoriesCommandDelete# Z .���� $��
cancel.ico��$��16��$��16���� -   $y�Visible=�&   SettingsCategoriesCommandMoveUp# Z .���� $��up.ico��$��16��$��16���� -   $y�Visible=�(   !SettingsCategoriesCommandMoveDown# Z .���� $��down.ico��$��16��$��16���� �� -   $y�Visible=�,   %SettingsCategoriesCommandLoadFromFile# Z .���� $��folderalt.ico��$��16��$��16���� -   $y�Visible=�,   %SettingsCategoriesCommandLoadFromLink# Z .���� $��globealt.ico��$��16��$��16��&   System.Windows.Controls.DataGrid  -   $��CategoriesDataGrid��$��1��$��0��$	��Top=�$��3,0,4,3q�$��Stretch=�$��Stretch=�   AutoGenerateColumns .     HeadersVisibility$ Column=�    CanUserAddRows  . !   CanUserDeleteRows! . "   CanUserResizeRows" . #   SelectionMode$# Single=�$��3q�$��1q�$   HorizontalGridLinesBrush$ �	#FFF0F0F0%   VerticalGridLinesBrush% �	#FFF0F0F0#��["������ *CategoriesData+$ TwoWay=�&   Columns& 0  *System.Windows.Controls.DataGridTextColumn  +'   $SettingsCategoriesColumnCategoryName,  &System.Windows.Controls.DataGridColumn(   Header#( Z' )   Width5  /System.Windows.Controls.DataGridLengthConverter$	) 120 1  +System.Windows.Controls.DataGridBoundColumn*   Binding* �� + �� Path$+ CategoryName�  9,   2SettingsCategoriesColumnTorrentDirectoryListNumber#( Z, $
) auto * �� $ + TorrentDirectoryListNumber�  &-   SettingsCategoriesColumnComment#( Z- $
) auto * �� $+ Comment�� -   $��
ButtonGrid��$��3��$��Stretch=�$��Stretch=�$
��auto��$
��auto��$��0,7,0,5q� �� $��* ��� $
��auto ��� $
��auto ��� $
��auto � �� $
��auto �.+��� -   $��SettingsTestBtn��.   TestButtonText#��Z. $��0��$��1��$��3,1,3,1q�$
��auto��$
��auto��$��Bottom=�$��Right=�$��6,2,6,2q��� -   $��SettingsOKBtn��/   OKButtonText#��Z/ $��0��$��2��$��3,1,3,1q�$
��auto��$
��auto��$��Bottom=�$��Right=�$��6,2,6,2q�0 �� 	IsDefault0 . �� -   $��SettingsCancelBtn��1   CancelButtonText#��Z1 $��0��$��3��$��3,1,3,1q�$
��auto��$
��auto��$��Bottom=�$��Right=�$��6,2,6,2q�2 �� IsCancel2 . 