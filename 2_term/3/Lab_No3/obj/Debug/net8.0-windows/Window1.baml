   M S B A M L   `   `   `  ����B  >Lab_No3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:Lab_No3Lab_No3  5   	       Lab_No3.Window1   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:Lab_No3     5   	   -   5          � Title$  	TimerTool��5   	   $	��420��6   $	��300��6(   .��5	      � .+�5
   
   �� $��Имя:��6   $
��Left=�6   $��	30,17,0,0q�6:   $	��Top=�6M   �� 5   
   -   5        $��	TimerName��5      $
��Left=�6#   $��	71,21,0,0q�6>    �� TextWrapping$
 Wrap=�6Q   $	��Top=�6e   $��Center=�6}   $	��200��6�   $��22��6�   �� 5   
   $��	Часы:��6   $
��Left=�6    $��	28,74,0,0q�6;   $	��Top=�6N   �� 5   
   $��Минуты:��6   $��Center=�6"   $��0,74,0,0q�6?   $	��Top=�6Q   �� 5   
   $��Секунды:��6   $
��Left=�6#   $��
211,74,0,0q�6>   $	��Top=�6R   �� 5   
   -   5        $��
InputHours��5      $
��Left=�6$   $��
10,105,0,0q�6?   $
 Wrap=�6S   $
��0-23��6g   $	��Top=�6s   $��Center=�6�   $��80��6�   $��30��6�   6�   �� 5   
   -   5        $��InputMinutes��5      $��Center=�6&   $��	0,105,0,0q�6C   $
 Wrap=�6V   $
��0-59��6j   $	��Top=�6v   $��Center=�6�   $��80��6�   $��30��6�   6�   �� 5   
   -   5        $��InputSeconds��5      $
��Left=�6&   $��210,105,0,0q�6A   $
 Wrap=�6V   $
��0-59��6j   $	��Top=�6v   $��Center=�6�   $��80��6�   $��30��6�   6�   &   System.Windows.Controls.Calendar  5   
   -   5        $��	InputDate��5      $��Center=�6$   $��	0,149,0,0q�6A   $	��Top=�6T   �� 5   
   -   5        $��AcceptInput��5      $��Подтвердить��6$   $
��Left=�6:   $��
30,339,0,0q�6U   $	��Top=�6i   $��30��6�   $��90��6�   �� 5   
   -   5        $��CancelInput��5      $��Отменить��6$   $
��Left=�67   $��181,339,0,0q�6R   $	��Top=�6g   $��30��6�   $��90��6�   5      5      