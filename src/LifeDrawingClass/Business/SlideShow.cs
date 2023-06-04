// *****************************************************************************
//  SlideShow.cs
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
    using System.Linq;
    using System.Runtime.Serialization;
    using LifeDrawingClass.Business.Interfaces;

    [DataContract]
    internal class SlideShow: Session, ISlideShow
    {
        #region Properties Impl - Public

        /// <inheritdoc />
        [DataMember]
        public IReadOnlyList<int> ImagesOrder { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int LastUsedImage { get; set; }

        /// <inheritdoc />
        [DataMember]
        public Dictionary<int, int> SegmentImageIndex { get; set; }

        /// <inheritdoc />
        [DataMember]
        public SlideShowState State { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int CurrentSegmentIndex { get; set; }


        /// <inheritdoc />
        [DataMember]
        public TimeSpan CurrentSegmentElapsedTime { get; set; }

        /// <inheritdoc />
        [DataMember]
        public TimeSpan TotalPauseDuration { get; set; }

        /// <inheritdoc />
        [DataMember]
        public TimeSpan CurrentPauseDuration { get; set; }

        #endregion

        #region Methods - Public

        #region Methods Stat

        public static ISlideShow NewSlideShow(ISession session, bool shuffleImages)
        {
            IReadOnlyList<int> imagesOrder = Enumerable.Range(0, session.ImagePaths.Count).ToList();
            if (shuffleImages)
            {
                Random rng = new();
                imagesOrder = imagesOrder.OrderBy(_ => rng.Next()).ToList();
            }

            return new SlideShow
            {
                ImagePaths = new List<string>(session.ImagePaths),
                Segments = new List<ISessionSegment>(session.Segments),
                ImagesOrder = imagesOrder,
                LastUsedImage = -1,
                SegmentImageIndex = Enumerable.Range(0, session.Segments.Count).ToDictionary(i => i, _ => -1),
                State = SlideShowState.NotStarted,
                CurrentSegmentIndex = -1,
                CurrentSegmentElapsedTime = TimeSpan.Zero,
                TotalPauseDuration = TimeSpan.Zero,
                CurrentPauseDuration = TimeSpan.Zero
            };
        }

        #endregion

        #endregion
    }
}