// *****************************************************************************
//  SessionProperties.cs
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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Serialization;

    [DataContract]
    public class SessionProperties: ISessionProperties
    {
        #region Constructors

        public SessionProperties()
        {
            this.DesignType = SessionSegmentDesignType.Automatic;
            this.SessionDuration = TimeSpan.FromMinutes(120);
            this.NumberOfLongPoses = 3;
            this.AddWarmUp = true;
            this.AddCoolDown = false;
            this.AddBreaks = false;
            this.IsSimplified = true;
        }

        #endregion

        #region Properties Impl - Public

        /// <inheritdoc />
        [DataMember]
        public SessionSegmentDesignType DesignType { get; set; }

        /// <inheritdoc />
        [DataMember]
        public TimeSpan SessionDuration { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool AddWarmUp { get; set; }

        /// <inheritdoc />
        [DataMember]
        public List<TimeSpan> AvailableWarmUpDurations { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double WarmUpPercent { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool AddCoolDown { get; set; }

        /// <inheritdoc />
        [DataMember]
        public List<TimeSpan> AvailableCoolDownDurations { get; set; }

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
        public TimeSpan BreaksDuration { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int NumberOfBreaks { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool IsSimplified { get; set; }

        /// <inheritdoc />
        public string ManualSegmentsDefinition { get; set; }

        #endregion

        #region Methods - Public

        #region Methods Stat

        public static List<ISessionSegment> GetSegments(ISessionProperties sessionProperties) =>
            sessionProperties.DesignType switch
            {
                SessionSegmentDesignType.Automatic => SessionSegmentsDesigner.DesignSessionSegments(sessionProperties),
                SessionSegmentDesignType.Manual => throw new NotImplementedException(),
                _ => throw new InvalidEnumArgumentException("Unknown design type.")
            };

        #endregion

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
            XmlSerializationUtils.DeserializeFromStream<SessionProperties>(stream);

        #endregion

        #endregion
    }
}