// *****************************************************************************
//  NewSessionViewModel.cs
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

namespace LifeDrawingClass.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using ControlzEx.Theming;
    using LifeDrawingClass.Core.Image;
    using LifeDrawingClass.Core.Log;
    using LifeDrawingClass.Models;
    using LifeDrawingClass.Views.Windows;
    using Microsoft.Win32;
    using Microsoft.WindowsAPICodePack.Dialogs;

    public class NewSessionViewModel: ObservableObject
    {
        #region Properties & Fields - Non-Public

        private SessionPropertiesModel _sessionPropertiesModel;

        private ICommand _clearPathsCommand;
        private ICommand _addPathsCommand;
        private ICommand _addPathsFromFolderCommand;
        private ICommand _startSessionCommand;
        private ICommand _alterThemCommand;
        private ICommand _editSessionSegments;

        private bool _darkThemSelected = true;

        #endregion

        #region Constructors

        public NewSessionViewModel(NewSessionModel sessionModel, SessionPropertiesModel sessionPropertiesModel)
        {
            this._sessionPropertiesModel = sessionPropertiesModel;
            this.SessionModel = sessionModel;
        }

        #endregion

        #region Properties & Fields - Public

        public NewSessionModel SessionModel { get; }

        public ICommand StartSessionCommand => this._startSessionCommand ??= new RelayCommand(this.StartSession);
        public ICommand ClearPathsCommand => this._clearPathsCommand ??= new RelayCommand(this.ClearPaths);

        public ICommand AddPathsCommand => this._addPathsCommand ??= new RelayCommand(this.AddPaths);

        public ICommand AddPathsFromFolderCommand =>
            this._addPathsFromFolderCommand ??= new RelayCommand(this.AddPathsFromFolder);

        public ICommand AlterThemeCommand => this._alterThemCommand ??= new RelayCommand(this.AlterTheme);

        public string ImagePaths => string.Join("\n", this.SessionModel.ImagePaths);
        public Visibility LightThemeButtonVisible => this._darkThemSelected ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DarkThemeButtonVisible => this._darkThemSelected ? Visibility.Collapsed : Visibility.Visible;

        public ICommand EditSessionSegmentsCommand =>
            this._editSessionSegments ??= new RelayCommand(this.EditSessionSegments);

        #endregion

        #region Methods - Public

        #region Methods Other

        internal void SaveConfigs()
        {
            try
            {
                this.SessionModel.SaveToConfigs();
                this._sessionPropertiesModel.SaveToConfigs();
            }
            catch (Exception e)
            {
                Logger.Error("Failed to save configs.", e);
            }
        }

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void ClearPaths()
        {
            this.SessionModel.ClearPaths();
            this.OnPropertyChanged(nameof(this.ImagePaths));
        }

        private void EditSessionSegments()
        {
            SessionPropertiesViewModel viewModel = new(this._sessionPropertiesModel);
            SessionPropertiesWindow window = new()
            {
                DataContext = viewModel
            };
            viewModel.ClosingRequest += (_, e) =>
            {
                window.DialogResult = !e?.Cancelled;
                window.Close();
            };

            bool? dialogResult = window.ShowDialog();
            if (dialogResult ?? false)
            {
                this._sessionPropertiesModel = viewModel.Result;
                this.SessionModel.MergedSegments = this._sessionPropertiesModel.MergedSegments;
            }
        }

        private void AlterTheme()
        {
            this._darkThemSelected = !this._darkThemSelected;
            ThemeManager.Current.ChangeTheme(Application.Current, this._darkThemSelected ? "Dark.Blue" : "Light.Blue");
            this.OnPropertyChanged(nameof(this.LightThemeButtonVisible));
            this.OnPropertyChanged(nameof(this.DarkThemeButtonVisible));
        }

        private void StartSession()
        {
            if (this.SessionModel.ImagePaths.Count > 0)
            {
                this.SaveConfigs();
                SlideShowWindow slideShowWindow = new(new SlideShowSessionModel(this.SessionModel.GetSession()));
                slideShowWindow.Show();
                Application.Current?.MainWindow?.Close();
            }
            else
            {
                MessageBox.Show("Please provide at least one image path.");
            }
        }

        private void AddPaths()
        {
            List<string> extensions = ImageList.Extensions.Select(e => "*" + e).ToList();
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Filter = $"Image Files ({string.Join(", ", extensions)})|{string.Join(";", extensions)}"
            };
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                this.SessionModel.AddPaths(openFileDialog.FileNames);
                this.OnPropertyChanged(nameof(this.ImagePaths));
            }
        }

        private void AddPathsFromFolder()
        {
            CommonOpenFileDialog dlg = new();
            dlg.Title = "Select Gallery Folder";
            dlg.IsFolderPicker = true;

            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.SessionModel.ImportFolder(dlg.FileName, true);
                this.OnPropertyChanged(nameof(this.ImagePaths));
            }
        }

        #endregion

        #endregion
    }
}