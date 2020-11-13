﻿using System.Windows.Controls;
using Smart_school_bell.ViewModel;

namespace Smart_school_bell.View
{
    /// <summary>
    /// Логика взаимодействия для DialogRenameTimetable.xaml
    /// </summary>
    public partial class DialogRenameTimetable : UserControl
    {
        public DialogRenameTimetable(DialogRenameTimetableVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
