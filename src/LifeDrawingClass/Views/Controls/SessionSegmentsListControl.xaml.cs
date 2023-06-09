﻿// *****************************************************************************
//  SessionSegmentsListControl.xaml.cs
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

namespace LifeDrawingClass.Views.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Models;

    /// <summary>
    ///     Interaction logic for SessionSegmentsListControl.xaml
    /// </summary>
    public partial class SessionSegmentsListControl
    {
        #region Constants & Statics

        public static readonly DependencyProperty SegmentsProperty = DependencyProperty.Register(
            "Segments",
            typeof(IEnumerable<SessionSegmentModel>),
            typeof(SessionSegmentsListControl),
            new PropertyMetadata(null, OnSegmentsChanged));

        public static readonly DependencyProperty SegmentIndexProperty = DependencyProperty.Register(
            "SegmentIndex",
            typeof(int),
            typeof(SessionSegmentsListControl),
            new PropertyMetadata(0, OnSegmentIndexChanged));

        #endregion

        #region Constructors

        public SessionSegmentsListControl()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties & Fields - Public

        public IEnumerable<SessionSegmentModel> Segments
        {
            get => (IEnumerable<SessionSegmentModel>) this.GetValue(SegmentsProperty);
            set => this.SetValue(SegmentsProperty, value);
        }

        public int SegmentIndex
        {
            get => (int) this.GetValue(SegmentIndexProperty);
            set => this.SetValue(SegmentIndexProperty, value);
        }

        #endregion

        #region Methods - Non-Public

        #region Methods Stat

        private static void OnSegmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SessionSegmentsListControl segmentsListControl = (SessionSegmentsListControl) d;
            segmentsListControl.CreateObjects();
        }

        private static void OnSegmentIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SessionSegmentsListControl customControl = (SessionSegmentsListControl) d;
            customControl.UpdateSegmentColors();
        }

        #endregion

        #region Methods Other

        private void CreateObjects()
        {
            Style borderStyle = (Style) this.FindResource("SegmentStyle");
            Brush breaksTextColor = (Brush) this.FindResource("BreakSegmentText");
            this.SegmentsStackPanel.Children.Clear();
            Thickness margin = new(3);

            TimeSpan currentTime = TimeSpan.Zero;
            string toolTipFormat = "h\\:mm" + (this.Segments.Any(s => s.Duration.Seconds >= 10) ? "\\:ss" : "");
            foreach (SessionSegmentModel segment in this.Segments)
            {
                TimeSpan currentTimeCopy = currentTime; // only to remove compiler nags about the captured variable, see https://stackoverflow.com/a/271447/2093077
                string toolTipContent = string.Join(Environment.NewLine,
                    Enumerable.Range(0, segment.Count)
                        .Select(i =>
                            ((i * segment.Duration) + currentTimeCopy).ToString(toolTipFormat,
                                CultureInfo.InvariantCulture)));
                ToolTip toolTip = new()
                {
                    Content = toolTipContent,
                    Placement = PlacementMode.Bottom,
                    VerticalOffset = 5
                };
                Border border = new()
                {
                    Style = borderStyle,
                    DataContext = segment,
                    Background = this.GetSegmentBrush(segment.Type),
                    ToolTip = toolTip
                };

                this.SegmentsStackPanel.Children.Add(border);
                TextBlock text = new()
                {
                    Text = segment.DurationText, VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = margin,
                    Opacity = 1
                };
                if (segment.Type == SessionSegmentType.Break)
                {
                    text.Foreground = breaksTextColor;
                }

                border.Child = text;
                currentTime += segment.Count * segment.Duration;
            }

            TimeSpan warmUps = TimeSpan.FromTicks(this.Segments.Where(s => s.Type == SessionSegmentType.WarmUp)
                .Sum(s => s.Duration.Ticks * s.Count));
            TimeSpan coolDowns = TimeSpan.FromTicks(this.Segments.Where(s => s.Type == SessionSegmentType.CoolDown)
                .Sum(s => s.Duration.Ticks * s.Count));
            TimeSpan longPoses = TimeSpan.FromTicks(this.Segments.Where(s => s.Type == SessionSegmentType.LongPose)
                .Sum(s => s.Duration.Ticks * s.Count));
            TimeSpan breaks = TimeSpan.FromTicks(this.Segments.Where(s => s.Type == SessionSegmentType.Break)
                .Sum(s => s.Duration.Ticks * s.Count));
            TimeSpan total = warmUps + longPoses + coolDowns + breaks;
            this.WarmUpText.Text = SessionSegmentModel.FormatDuration(warmUps, 1);
            this.LongPosesText.Text = SessionSegmentModel.FormatDuration(longPoses, 1);
            this.CoolDownText.Text = SessionSegmentModel.FormatDuration(coolDowns, 1);
            this.BreaksText.Text = SessionSegmentModel.FormatDuration(breaks, 1);
            this.TotalText.Text = SessionSegmentModel.FormatDuration(total, 1);
        }

        private Brush GetSegmentBrush(SessionSegmentType type)
        {
            string brushName = type switch
            {
                SessionSegmentType.WarmUp => "WarmUpSegmentBrush",
                SessionSegmentType.LongPose => "LongPoseSegmentBrush",
                SessionSegmentType.CoolDown => "CoolDownSegmentBrush",
                SessionSegmentType.Break => "BreakSegmentBrush",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return (Brush) this.FindResource(brushName);
        }

        private void UpdateSegmentColors()
        {
            foreach (object border in this.SegmentsStackPanel.Children)
            {
                if (border is Border sessionBorder)
                {
                    if (sessionBorder.DataContext is SessionSegmentModel segment)
                    {
                        sessionBorder.Background = this.GetSegmentBrush(segment.Type);
                    }
                    else
                    {
                        throw new InvalidCastException("Unknown data context.");
                    }
                }
                else
                {
                    throw new InvalidCastException("Unknown child control.");
                }
            }
        }

        #endregion

        #endregion
    }
}