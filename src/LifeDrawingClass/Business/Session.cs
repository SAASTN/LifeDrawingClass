// *****************************************************************************
//  Session.cs
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

namespace LifeDrawingClass.Business
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Image;
    using LifeDrawingClass.Core.Serialization;

    [DataContract]
    public class Session: ISession
    {
        #region Properties & Fields - Non-Public

        [DataMember] private ImageList _imageList;

        /// <remarks>
        ///     The internal field is declared as a list of <see cref="SessionSegment" />s instead of
        ///     <see cref="ISessionSegment" />s for serialization, as <see cref="DataContractSerializer" /> does not support
        ///     interfaces. if there was other <see cref="ISessionSegment" /> implementations, the serialization part must be
        ///     rewritten.
        /// </remarks>
        [DataMember] private List<SessionSegment> _segments;

        #endregion

        #region Constructors

        public Session()
        {
            this._segments = SessionSegmentsDesigner.DesignSessionSegments(new SessionProperties())
                .Cast<SessionSegment>().ToList();
        }

        #endregion

        #region Properties Impl - Public

        public IReadOnlyList<string> ImagePaths
        {
            get => (this._imageList ??= new ImageList()).AsList();
            set => this._imageList = new ImageList(value?.ToList());
        }

        /// <inheritdoc />

        public IReadOnlyList<ISessionSegment> Segments
        {
            get => this._segments;
            set => this._segments = value.Cast<SessionSegment>().ToList();
        }

        #endregion

        #region Methods - Public

        #region Methods Impl

        /// <inheritdoc />
        public void SerializeToXml(string fileName) => XmlSerializationUtils.SerializeToXml(this, fileName);

        /// <inheritdoc />
        public void SerializeToStream(Stream stream) => XmlSerializationUtils.SerializeToStream(this, stream);

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Impl

        /// <inheritdoc />
        ISerializableObject ISerializableObject.DeserializeFromStream(Stream stream) =>
            XmlSerializationUtils.DeserializeFromStream<Session>(stream);

        #endregion

        #endregion
    }
}