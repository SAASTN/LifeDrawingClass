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

namespace LifeDrawingClass.Views
{
    using LifeDrawingClass.Core;
    using LifeDrawingClass.Models;
    using LifeDrawingClass.ViewModels;

    /// <summary>
    ///     Interaction logic for NewSessionWindow.xaml
    /// </summary>
    public partial class NewSessionWindow
    {
        #region Constructors

        public NewSessionWindow()
        {
            this.InitializeComponent();
            this.DataContext = new NewSessionViewModel(GetSessionModel());
        }

        #endregion

        #region Methods - Non-Public

        #region Methods Stat

        private static SessionModel GetSessionModel()
        {
            ISession session = new Session()
            {
                Interval = 1000
            };
            session.ImportFolder(@"..\..\..\SampleGallery\", false);
            return new SessionModel(session);
        }

        #endregion

        #endregion
    }
}