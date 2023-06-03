// *****************************************************************************
//  SessionPropertiesWindow.xaml.cs
//   Copyright (C) 2023 SAASTN <saastn@gmail.com>
//   This file is part of LifeDrawingClass.
// 
//   LifeDrawingClass is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
// 
//   LifeDrawingClass is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//   GNU General Public License for more details.
// 
//   You should have received a copy of the GNU General Public License
//   along with LifeDrawingClass. If not, see <https://www.gnu.org/licenses/>.
// *****************************************************************************

namespace LifeDrawingClass.Views.Windows
{
    using System.Windows.Controls;
    using LifeDrawingClass.Business;

    /// <summary>
    ///     Interaction logic for SessionPropertiesWindow.xaml
    /// </summary>
    public partial class SessionPropertiesWindow
    {
        #region Constructors

        public SessionPropertiesWindow()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void ListBox_LostFocus(object sender, System.Windows.RoutedEventArgs e) =>
            ((ListBox) sender).SelectedIndex = -1;

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox) sender)?.SelectedItem != null)
            {
                SessionDefinitionParser.ParserMessageItem selectedMessage =
                    (SessionDefinitionParser.ParserMessageItem) ((ListBox) sender).SelectedItem;
                string text = this.ManualDefinitionTextBox.Text;
                int selStart = 0;
                for (int i = 0; i < selectedMessage.SectionIndex; i++)
                {
                    selStart++;
                    selStart = text.IndexOf(',', selStart);
                }

                this.ManualDefinitionTextBox.SelectionStart = selStart + 1;
                this.ManualDefinitionTextBox.SelectionLength = selectedMessage.Section.Length;
                this.ManualDefinitionTextBox.Focus();
            }
        }

        private void ManualDefinitionTextBox_PreviewTextInput(object sender,
            System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!SessionDefinitionParser.AreValidChars(e.Text))
            {
                e.Handled = true;
            }
        }

        #endregion

        #endregion
    }
}