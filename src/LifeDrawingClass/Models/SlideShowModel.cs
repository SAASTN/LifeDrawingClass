// *****************************************************************************
//  SlideShowModel.cs
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
    using System;
    using System.Collections.Generic;
    using CommunityToolkit.Mvvm.ComponentModel;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Image;
    using SkiaSharp;

    internal class SlideShowModel: ObservableObject
    {
        #region Constants & Statics

        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        #endregion

        #region Properties & Fields - Non-Public

        private IReadOnlyList<string> _imageList;
        private IReadOnlyList<ISessionSegment> _segments;
        private SKBitmap _image;
        private IReadOnlyList<int> _imagesOrder;
        private int _lastUsedImage;
        private Dictionary<int, int> _segmentImageIndex;
        private SlideShowState _state;
        private int _currentSegmentIndex;
        private TimeSpan _currentSegmentElapsedTime;
        private TimeSpan _totalPauseDuration;
        private TimeSpan _currentPauseDuration;

        #endregion

        #region Constructors

        public SlideShowModel(ISlideShow slideShow)
        {
            this.Initialize(slideShow);
        }

        #endregion

        #region Properties & Fields - Public

        /// <inheritdoc cref="ISlideShow.ImagesOrder" />
        public IReadOnlyList<int> ImagesOrder
        {
            get => this._imagesOrder;
            set => this.SetProperty(ref this._imagesOrder, value, nameof(this.ImagesOrder));
        }

        /// <inheritdoc cref="ISlideShow.LastUsedImage" />
        public int LastUsedImage
        {
            get => this._lastUsedImage;
            set => this.SetProperty(ref this._lastUsedImage, value, nameof(this.LastUsedImage));
        }

        /// <inheritdoc cref="ISlideShow.SegmentImageIndex" />
        public Dictionary<int, int> SegmentImageIndex
        {
            get => this._segmentImageIndex;
            set => this.SetProperty(ref this._segmentImageIndex, value, nameof(this.SegmentImageIndex));
        }

        /// <inheritdoc cref="ISlideShow.State" />
        public SlideShowState State
        {
            get => this._state;
            set => this.SetProperty(ref this._state, value, nameof(this.State));
        }

        /// <inheritdoc cref="ISlideShow.CurrentSegmentIndex" />
        public int CurrentSegmentIndex
        {
            get => this._currentSegmentIndex;
            set => this.SetProperty(ref this._currentSegmentIndex, value, nameof(this.CurrentSegmentIndex));
        }


        /// <inheritdoc cref="ISlideShow.CurrentSegmentElapsedTime" />
        public TimeSpan CurrentSegmentElapsedTime
        {
            get => this._currentSegmentElapsedTime;
            set => this.SetProperty(ref this._currentSegmentElapsedTime, value, nameof(this.CurrentSegmentElapsedTime));
        }

        /// <inheritdoc cref="ISlideShow.TotalPauseDuration" />
        public TimeSpan TotalPauseDuration
        {
            get => this._totalPauseDuration;
            set => this.SetProperty(ref this._totalPauseDuration, value, nameof(this.TotalPauseDuration));
        }

        /// <inheritdoc cref="ISlideShow.CurrentPauseDuration" />
        public TimeSpan CurrentPauseDuration
        {
            get => this._currentPauseDuration;
            set => this.SetProperty(ref this._currentPauseDuration, value, nameof(this.CurrentPauseDuration));
        }

        public SKBitmap Image
        {
            get => this._image;
            private set => this.SetProperty(ref this._image, value, nameof(this.Image));
        }

        #endregion

        #region Methods - Public

        #region Methods Other

        /// <summary>
        ///     Runs every 1 seconds.
        /// </summary>
        /// <param name="speedMultiplier">
        ///     Normally is equal tp 1.0, but can be positive value for testing purposes to run the whole session faster or slower.
        ///     This indicates how many seconds has passed from the last call of <see cref="Tick" />.
        /// </param>
        public void Tick(double speedMultiplier)
        {
            TimeSpan timePassed = OneSecond * speedMultiplier;
            switch (this.State)
            {
                case SlideShowState.NotStarted:
                case SlideShowState.Finished:
                    throw new InvalidOperationException("Slide-show is not running.");
                case SlideShowState.Running:
                    this.CurrentSegmentElapsedTime += timePassed;
                    this.CheckSegment();
                    break;
                case SlideShowState.Paused:
                    this.CurrentPauseDuration += timePassed;
                    this.TotalPauseDuration += timePassed;
                    break;
                default: throw new InvalidOperationException("Unknown state.");
            }
        }

        public void ReStartSession(bool resetImagesOrder)
        {
            if (resetImagesOrder)
            {
                this.LastUsedImage = -1;
            }

            this.TotalPauseDuration = TimeSpan.Zero;
            this.CurrentPauseDuration = TimeSpan.Zero;
            this.StartSegment(0);
        }

        public void OnPaintImage(SKCanvas canvas, int width, int height) =>
            ImageEditor.PaintCanvas(canvas, this.Image, width, height);

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Other

        private void Initialize(ISlideShow slideShow)
        {
            this._imageList = slideShow.ImagePaths;
            this._segments = slideShow.Segments;
            this._imagesOrder = slideShow.ImagesOrder;
            this._lastUsedImage = slideShow.LastUsedImage;
            this._segmentImageIndex = slideShow.SegmentImageIndex;
            this._state = slideShow.State;
            this._currentSegmentIndex = slideShow.CurrentSegmentIndex;
            this._currentSegmentElapsedTime = slideShow.CurrentSegmentElapsedTime;
            this._totalPauseDuration = slideShow.TotalPauseDuration;
            this._currentPauseDuration = slideShow.CurrentPauseDuration;
            this.OnPropertyChanged();
        }

        private void CheckSegment()
        {
            if (this.State != SlideShowState.Running)
            {
                throw new InvalidOperationException("Slide-show is not running.");
            }

            if (this.CurrentSegmentElapsedTime > this._segments[this.CurrentSegmentIndex].Duration)
            {
                this.StartSegment(this.CurrentSegmentIndex + 1);
            }
        }

        private void StartSegment(int segmentIndex)
        {
            this.CurrentSegmentIndex = segmentIndex;
            this.CurrentSegmentElapsedTime = TimeSpan.Zero;
            this.GetNextImage();
            this.State = SlideShowState.Running;
        }

        private void GetNextImage()
        {
            SKBitmap bmp = null;
            int tryCount = 0;
            while (tryCount < this._imageList.Count)
            {
                tryCount++;
                this.LastUsedImage = (this.LastUsedImage + 1) % this._imageList.Count;
                string imagePath = this._imageList[this.ImagesOrder[this.LastUsedImage]];
                bmp = ImageEditor.ImportImageFromFile(imagePath);
                if (bmp != null)
                {
                    break;
                }
            }

            if (this._image != null)
            {
                this._image.Dispose();
            }

            this.Image = bmp;
        }

        #endregion

        #endregion
    }
}