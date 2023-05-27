// *****************************************************************************
//  SessionSegmentDesigner.cs
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
    using System.Runtime.Serialization;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Serialization;

    [DataContract]
    public class SessionSegmentDesigner: ISessionSegmentDesigner
    {
        #region Properties Impl - Public

        /// <inheritdoc />
        [DataMember]
        public SessionSegmentDesignType DesignType { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool SessionDuration { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool AddWarmUp { get; set; }

        /// <inheritdoc />
        [DataMember]
        public List<int> AvailableWarmUpDurations { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double WarmUpPercent { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool AddCoolDown { get; set; }

        /// <inheritdoc />
        [DataMember]
        public List<int> AvailableCoolDownDurations { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double CoolDownPercent { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int NumberOfLongPoses { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool AddBreaks { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double BreakPercent { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double NumberOfBreak { get; set; }

        /// <inheritdoc />
        public string ManualSegmentsDefinition { get; set; }

        #endregion

        #region Methods - Public

        #region Methods Impl

        /// <inheritdoc />
        public void SerializeToXml(string fileName) => XmlSerializationUtils.SerializeToXml(this, fileName);

        /// <inheritdoc />
        public void SerializeToStream(Stream stream) => XmlSerializationUtils.SerializeToStream(this, stream);

        /// <inheritdoc />
        public List<ISessionSegment> GetSegments() => throw new System.NotImplementedException();

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Impl

        /// <inheritdoc />
        ISerializableObject ISerializableObject.DeserializeFromStream(Stream stream) =>
            XmlSerializationUtils.DeserializeFromStream<SessionSegmentDesigner>(stream);

        #endregion

        #endregion
    }
}