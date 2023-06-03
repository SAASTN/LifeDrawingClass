// *****************************************************************************
//  SlideShowSessionModel.cs
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

namespace LifeDrawingClass.Models
{
    using System.Collections.Generic;
    using CommunityToolkit.Mvvm.ComponentModel;
    using LifeDrawingClass.Business.Interfaces;

    public class SlideShowSessionModel: ObservableObject
    {
        #region Constructors

        public SlideShowSessionModel(ISession session)
        {
            this.Initialize(session);
        }

        #endregion

        #region Properties & Fields - Public

        public IReadOnlyList<string> ImagePaths { get; private set; }

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void Initialize(ISession session)
        {
            this.ImagePaths = session.ImagePaths;
            this.OnPropertyChanged();
        }

        #endregion

        #endregion
    }
}