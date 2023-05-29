// *****************************************************************************
//  SessionSegment.cs
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
    using System.IO;
    using System.Runtime.Serialization;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Serialization;

    [DataContract]
    public class SessionSegment: ISessionSegment
    {
        #region Constants & Statics

        private const int MinimumSegmentDuration = 10000; // in ms

        #endregion

        #region Properties & Fields - Non-Public

        [DataMember] private int _durationMilliseconds = MinimumSegmentDuration;

        #endregion

        #region Properties Impl - Public

        /// <inheritdoc />
        [DataMember]
        public int GroupId { get; set; }

        /// <inheritdoc />
        [DataMember]
        public SessionSegmentType Type { get; set; }

        /// <inheritdoc />
        public int DurationMilliseconds
        {
            get => this._durationMilliseconds;
            set
            {
                if (value < MinimumSegmentDuration)
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Segment must be longer than {MinimumSegmentDuration / 1000} seconds.");
                }

                this._durationMilliseconds = value;
            }
        }

        #endregion

        #region Methods - Public

        #region Methods Stat

        public static IEnumerable<int> GetStartTimes(IEnumerable<ISessionSegment> segments)
        {
            int sum = 0;

            foreach (ISessionSegment segment in segments)
            {
                yield return sum;

                sum += segment.DurationMilliseconds;
            }
        }

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
            XmlSerializationUtils.DeserializeFromStream<SessionSegment>(stream);

        #endregion

        #endregion
    }
}