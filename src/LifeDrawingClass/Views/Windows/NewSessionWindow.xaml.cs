// *****************************************************************************
//  NewSessionWindow.xaml.cs
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
    using System;
    using System.Windows;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Configuration;
    using LifeDrawingClass.Core.Log;
    using LifeDrawingClass.Core.Serialization;
    using LifeDrawingClass.Models;
    using LifeDrawingClass.ViewModels;

    /// <summary>
    ///     Interaction logic for NewSessionWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class NewSessionWindow
    {
        #region Constructors

        public NewSessionWindow()
        {
            Application.Current.Resources["MahApps.Font.Size.Content"] = 16.5d;
            Application.Current.Resources["MahApps.Font.Size.Button"] = 16.5d;
            this.InitializeComponent();
            this.DataContext = new NewSessionViewModel(GetLastSessionModel(), GetLastSessionPropertiesModel());
        }

        #endregion

        #region Methods - Non-Public

        #region Methods Stat

        private static NewSessionModel GetLastSessionModel()
        {
            string lastSessionFileName = Configurator.GetLastSessionFileName();
            ISession session;
            try
            {
                session = XmlSerializationUtils.DeserializeFromXml<Session>(lastSessionFileName);
            }
            catch (Exception e)
            {
                Logger.Error("Can not load last session.", e);
                session = new Session();
            }

            return new NewSessionModel(session);
        }

        private static SessionPropertiesModel GetLastSessionPropertiesModel()
        {
            string lastPropertiesFileName = Configurator.GetLastSessionPropertiesFileName();
            ISessionProperties properties;
            try
            {
                properties = XmlSerializationUtils.DeserializeFromXml<SessionProperties>(lastPropertiesFileName);
            }
            catch (Exception e)
            {
                Logger.Error("Can not load last session segment properties.", e);
                properties = new SessionProperties();
            }

            return new SessionPropertiesModel(properties);
        }

        #endregion

        #endregion
    }
}