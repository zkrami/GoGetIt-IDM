   M S B A M L   `   `   `  ���� B  >GoGetIt, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:IDMIDM      IDM.FileDownloadingWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     #localclr-namespace:IDM     -     �Name$  window��
������� � Icon$ res/icon.png��$	��550��$	��700�� � Title �� *Url+.��� $��0q� � RowDefinitions �� $
��auto ��� $��* �.+�� $��Stretch=�$��Stretch=�$��0�����#FFFFFF$��0q�.+�� $��20q�$��Stretch=�$��Stretch=� � ColumnDefinitions �� $	��150 ��� $��* � �� $��27 ��� $��35 ��� $��27 ��� $��27 ��� $��27 ��� $
��Auto ��� $��27 ��� $��27 �.+��� -    ��Name$ urlLabel��$��0��$��0��$��2��$
��Left=�$	��Top=�$��Calibri)�$��15(����	#FF007DB8���� *Url+�� -   $ label��$��0��$��1��$��status��$
��Left=�$	��Top=�$��Calibri)�$��14(����	#FF007DB8�� -   $ 
label_Copy��$��0��$��2��$��	File Size��$��Stretch=�$��Stretch=�$��Calibri)�$��14(����	#FF007DB8�� -   $ label_Copy1��$��0��$��3��$��
Downloaded��$
��Left=�$	��Top=�$��Calibri)�$��14(����	#FF007DB8�� -   $ label_Copy2��$��0��$��4��$��Transfer rate��$
��Left=�$	��Top=�$��Calibri)�$��14(����	#FF007DB8�� -   $ label_Copy3��$��0��$��5��$��	Time left��$
��Left=�$	��Top=�$��Calibri)�$��14(����	#FF007DB8�� -   $ statusLabel��$��1��$��1��$
��Left=�$	��Top=�$��Calibri)�$��14(����	#FF3E3E3E����  �� Path$ StateFormated��� -	   $ fileSizeLabel��$��1��$��2��$
��Left=�$	��Top=�$��Calibri)�$��14(����	#FF3E3E3E���� $ FormatedFileSize��� -
   $ downloadedLabel��$��1��$��3��$
��Left=�$	��Top=�$��Calibri)�$��14(����	#FF3E3E3E���� $ DownloadedFormatted��� -   $ transferRateLabel��$��1��$��4��$
��Left=�$	��Top=�$��Calibri)�$��14(����	#FF3E3E3E���� $ TransferRateFormated��� -   $ timeleftLabel��$��1��$��5��$
��Left=�$	��Top=�$��Calibri)�$��14(����	#FF3E3E3E���� $ TimeLeftFormatted��� -    ��Name$ pauseResumeBtn��   buttonStyle#��[  $��0��$��7��$��Pauase��$
��Left=�$	��Top=�$��75��$��0,6,0,-5q���Z� � $��Stretch=�$��Stretch=�$��1��.+�&   System.Windows.Controls.DataGrid  -     Name$ dataGrid�����	#FFFDFDFD	 �� AlternationCount$	 2��
   RowHeaderWidth$
 0Q�  HeaderStyle   ColumnHeaderStyle# [    
IsReadOnly . $��Stretch=�$��Stretch=�   AutoGenerateColumns .  ���� $ PartsFileDownloader�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ N��   Width5  /System.Windows.Controls.DataGridLengthConverter$ 12 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Number+  $ State��$	 100  �� *StateFormated+  $
 Size��$	 100  �� *AmountToReadFormatted+  $ 
Downloaded��$	 100  �� *DownloadedFormated+   RowStyle ��  �� 
TargetType)  #System.Windows.Controls.DataGridRow"  .���  �� Property � �� �� Value �#FFF��  � ��$ 22�� �� Triggers P�  P� Property �@��AlternationIndex P� Value$ 0��. ���  � �� �	#FF287A91P�  �@��AlternationIndex$ 1��. ���  � �� �	#FF3DB1D1