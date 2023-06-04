// *****************************************************************************
//  ISlideShow.cs
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

namespace LifeDrawingClass.Business.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface ISlideShow: ISession
    {
        /// <summary>
        ///     In order to guarantee that an image will not be displayed again, the order list of photos will always remain fixed.
        ///     Every time a new image is requested, the <see cref="LastUsedImage" /> increases. If all the images have been
        ///     displayed once (the index of the last image has reached the end), the list will be scrolled again from the
        ///     beginning.
        /// </summary>
        IReadOnlyList<int> ImagesOrder { get; set; }

        /// <summary>
        ///     The index of last used item in <see cref="ImagesOrder" />. So the last displayed image is
        ///     <see cref="ISession.ImagePaths" />[<see cref="ImagesOrder" />[<see cref="LastUsedImage" />]].
        /// </summary>
        int LastUsedImage { get; set; }

        Dictionary<int, int> SegmentImageIndex { get; set; }
        SlideShowState State { get; set; }
        int CurrentSegmentIndex { get; set; }

        TimeSpan CurrentSegmentElapsedTime { set; get; }

        /// <remarks>
        ///     It includes <see cref="CurrentPauseDuration" />. In other words, when <see cref="State" /> is
        ///     <see cref="SlideShowState.Paused" />, both <see cref="TotalPauseDuration" /> and
        ///     <see cref="CurrentPauseDuration" /> increase.
        /// </remarks>
        TimeSpan TotalPauseDuration { get; set; }

        TimeSpan CurrentPauseDuration { get; set; }
    }
}