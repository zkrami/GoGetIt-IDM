   M S B A M L   `   `   `  ����B  >GoGetIt, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:IDMIDM  5   	       IDM.YoutubeVideoSelector   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:IDM     5   	     � Icon$  res/icon.png��5   
    � Title$ Youtube Downloader��5	   	   $	��210��6$   $	��450��64   $	��210��6C   $	��450��6S   $	��450��6b   $	��210��6n    � WindowStyle$ 
ToolWindow=�5
   
    � WindowStartupLocation$ CenterScreen=�5   	   .��5      � $��15q�6    � RowDefinitions 5   
   �� 5      $��28 �6   6*   �� 5      $��20 �6   6*   �� 5      $��28 �6   6*   �� 5      $��20 �6   6*   �� 5      $��10 �6   6*   �� 5      $
��Auto �6   6,    � ColumnDefinitions 5   
   �� 5      $��* �6   6+   �� 5      $��3 �6   6+   �� 5      $��* �6   6+   �� 5      $��3 �6   6+   �� 5      $��* �6   6+   .+�5    
   �� $	��Url��6   $��0��6   $��5��6+   $��Calibri)�6@   $��14(�6U   ���	#FF007DB86c   �� 5!   
   -   5        $��
urlTextBox��5!         textBoxStyle#��[  6$   $��1��6J   $��5��6X    �� 
IsReadOnly . $��Calibri)�6   $��14(�6�   $��0,0,0,22q�6�   $��2��6�   �� 5"   
   $��
Resolution��6   $��2��6%   $��Calibri)�64   $��14(�6I   ���	#FF007DB8$��0,4,0,17q�6n   $��2��6�   �� 5#   
   #��[  6   $��3��68   $��5��6m    . $��Calibri)�6�   $��14(�6�   $
��Auto��6�   $��2��6�   $��0,0,0,5q�6�   ��6E   �� *
FilePath+ �� Mode$ OneWay=�6�   �� 5$   
   -   5        $��resolutionComboBox��5$   %    �� SelectedIndex$ 0��6   $��3��6?   $��Stretch=�6L   $��5��6j   $��0,0,0,5q�6~   $��2��6�   .@�5&      �� .��6   Please Wait5(      5*      �� 5+   
   -   5          buttonStyle#��[ 5+      $��Download��6N   $��5��6a   $��0��6n   $��Calibri)�6~   $��14(�6�   $��	0,0,70,-5q�6�   $��3��6�   ��67   Z� 6�   �� 5,   
   -   5        #��[ 5,      $��Cancel��6M   $��5��6^   $��2��6k   $��Calibri)�6{   $��14(�6�   $��	70,0,0,-5q�6�   $��3��6�   $��26��6�   $	��Top=�6�   ��66   Z� 6�   5/      50      