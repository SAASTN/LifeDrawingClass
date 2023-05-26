// *****************************************************************************
//  SessionSegmentDesignerModel.cs
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
    using System.ComponentModel;
    using CommunityToolkit.Mvvm.ComponentModel;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Configuration;
    using LifeDrawingClass.Core.Serialization;

    public class SessionSegmentDesignerModel: ObservableObject
    {
        #region Properties & Fields - Non-Public

        /// <summary>
        ///     Key is the name of property of th <see cref="ISessionSegmentDesigner" /> instance, and value is the name of
        ///     property of the
        ///     <see cref="SessionSegmentDesignerModel" /> instance.
        ///     When <see cref="SessionModel" /> gets notified that one of properties of its <see cref="ISessionSegmentDesigner" />
        ///     has change, it
        ///     notifies its ViewModels by calling
        ///     <see cref="ObservableObject.OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs)" /> with the name of
        ///     its own property.
        /// </summary>
        private readonly Dictionary<string, string> _designerPropertyBindings = new()
        {
            { nameof(ISessionSegmentDesigner.DesignType), nameof(DesignType) }
        };

        private readonly ISessionSegmentDesigner _designer;

        #endregion

        #region Constructors

        public SessionSegmentDesignerModel(ISessionSegmentDesigner designer)
        {
            this._designer = designer;
            this._designer.PropertyChanged += this.DesignerPropertyChanged;
        }

        #endregion

        #region Properties & Fields - Public

        public SessionSegmentDesignType DesignType
        {
            get => this._designer.DesignType;
            set => this._designer.DesignType = value;
        }

        #endregion

        #region Methods - Public

        #region Methods Other

        internal void SaveToConfigs() =>
            this._designer.SerializeToXml(Configurator.GetLastSessionSegmentDesignerFileName());

        internal SessionSegmentDesignerModel DeepCopy() =>
            new(XmlSerializationUtils.DeepCopy(this._designer));

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void DesignerPropertyChanged(object sender, PropertyChangedEventArgs e) =>
            this.OnPropertyChanged(e.PropertyName != null ? this._designerPropertyBindings[e.PropertyName] : null);

        #endregion

        #endregion
    }
}