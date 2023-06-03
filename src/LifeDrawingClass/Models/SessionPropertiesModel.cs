// *****************************************************************************
//  SessionPropertiesModel.cs
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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using CommunityToolkit.Mvvm.ComponentModel;
    using LifeDrawingClass.Business;
    using LifeDrawingClass.Business.Interfaces;
    using LifeDrawingClass.Core.Configuration;

    public class SessionPropertiesModel: ObservableObject
    {
        #region Properties & Fields - Non-Public

        private SessionSegmentDesignType _designType;
        private int _sessionDurationMinutes;
        private int _numberOfLongPoses;
        private bool _addWarmUp;
        private bool _addCoolDown;
        private bool _addBreaks;
        private bool _isSimplified;
        private string _manualSegmentsDefinition;
        private bool _hasParsingIssue;

        #endregion

        #region Constructors

        public SessionPropertiesModel(ISessionProperties properties)
        {
            this.Initialize(properties);
        }

        #endregion

        #region Properties & Fields - Public

        public IReadOnlyList<SessionSegmentModel> MergedSegments
        {
            get
            {
                List<SessionSegmentModel> segments = new(SessionSegmentModel.MergeSegment(
                    SessionProperties.GetSegments(this.GetProperties(),
                        out List<SessionDefinitionParser.ParserMessageItem> parsingMessages)));
                this.SetParsingMessages(parsingMessages);
                return segments;
            }
        }

        public ObservableCollection<SessionDefinitionParser.ParserMessageItem> ParsingMessages { get; } = new();

        public SessionSegmentDesignType DesignType
        {
            get => this._designType;
            set
            {
                string manualSegmentsDefinition = this._manualSegmentsDefinition;
                if ((this._designType == SessionSegmentDesignType.Automatic) &&
                    (value == SessionSegmentDesignType.Manual))
                {
                    manualSegmentsDefinition = ConvertAutomaticToManual(this.MergedSegments);
                }

                this.SetProperty(ref this._designType, value, nameof(this.DesignType));
                if (manualSegmentsDefinition != this._manualSegmentsDefinition)
                {
                    this.ManualSegmentsDefinition = manualSegmentsDefinition;
                }
            }
        }


        /// <inheritdoc cref="ISessionProperties.SessionDuration" />
        public int SessionDurationMinutes
        {
            get => this._sessionDurationMinutes;
            set
            {
                value = (int) (Math.Round(value / 5.0) * 5.0);
                this.SetProperty(ref this._sessionDurationMinutes, value, nameof(this.SessionDurationMinutes));
            }
        }

        /// <inheritdoc cref="ISessionProperties.NumberOfLongPoses" />
        public int NumberOfLongPoses
        {
            get => this._numberOfLongPoses;
            set => this.SetProperty(ref this._numberOfLongPoses, value, nameof(this.NumberOfLongPoses));
        }

        /// <inheritdoc cref="ISessionProperties.AddWarmUp" />
        public bool AddWarmUp
        {
            get => this._addWarmUp;
            set => this.SetProperty(ref this._addWarmUp, value, nameof(this.AddWarmUp));
        }

        /// <inheritdoc cref="ISessionProperties.AddCoolDown" />
        public bool AddCoolDown
        {
            get => this._addCoolDown;
            set => this.SetProperty(ref this._addCoolDown, value, nameof(this.AddCoolDown));
        }

        /// <inheritdoc cref="ISessionProperties.AddBreaks" />
        public bool AddBreaks
        {
            get => this._addBreaks;
            set => this.SetProperty(ref this._addBreaks, value, nameof(this.AddBreaks));
        }

        /// <inheritdoc cref="ISessionProperties.IsSimplified" />
        public bool IsSimplified
        {
            get => this._isSimplified;
            set => this.SetProperty(ref this._isSimplified, value, nameof(this.IsSimplified));
        }

        /// <inheritdoc cref="ISessionProperties.ManualSegmentsDefinition" />
        public string ManualSegmentsDefinition
        {
            get => this._manualSegmentsDefinition;
            set => this.SetProperty(ref this._manualSegmentsDefinition, value.ToUpperInvariant().Replace('X', 'x'),
                nameof(this.ManualSegmentsDefinition));
        }

        public bool HasParsingIssue
        {
            get => this._hasParsingIssue;
            set => this.SetProperty(ref this._hasParsingIssue, value, nameof(this.HasParsingIssue));
        }

        #endregion

        #region Methods - Public

        #region Methods Other

        internal void SaveToConfigs() =>
            this.GetProperties().SerializeToXml(Configurator.GetLastSessionPropertiesFileName());

        public ISessionProperties GetProperties() => new SessionProperties()
        {
            DesignType = this.DesignType,
            SessionDuration = TimeSpan.FromMinutes(this.SessionDurationMinutes),
            NumberOfLongPoses = this.NumberOfLongPoses,
            AddWarmUp = this.AddWarmUp,
            AddCoolDown = this.AddCoolDown,
            AddBreaks = this.AddBreaks,
            IsSimplified = true,
            ManualSegmentsDefinition = this.ManualSegmentsDefinition
        };

        #endregion

        #endregion

        #region Methods - Non-Public

        #region Methods Stat

        private static string ConvertAutomaticToManual(IEnumerable<SessionSegmentModel> mergedSegments) =>
            SessionDefinitionParser.GetDefinitionTextAutomaticSession(mergedSegments);

        #endregion

        #region Methods Impl

        /// <inheritdoc />
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (!string.IsNullOrEmpty(e.PropertyName) && (e.PropertyName != nameof(this.MergedSegments)) &&
                (e.PropertyName != nameof(this.ParsingMessages)) && (e.PropertyName != nameof(this.HasParsingIssue)))
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.MergedSegments)));
            }
        }

        #endregion

        #region Methods Other

        private void SetParsingMessages(List<SessionDefinitionParser.ParserMessageItem> parsingMessages)
        {
            this.ParsingMessages.Clear();
            if (this.DesignType != SessionSegmentDesignType.Manual)
            {
                this._hasParsingIssue = false;
            }
            else
            {
                parsingMessages?.ForEach(m => this.ParsingMessages.Add(m));
                this._hasParsingIssue = parsingMessages?.Any() ?? false;
                if (!this._hasParsingIssue)
                {
                    this.ParsingMessages.Add(
                        new SessionDefinitionParser.ParserMessageItem("", -1, "Everything looks fine."));
                }
            }

            this.OnPropertyChanged(nameof(this.ParsingMessages));
            this.OnPropertyChanged(nameof(this.HasParsingIssue));
        }

        private void Initialize(ISessionProperties properties)
        {
            this._designType = properties.DesignType;
            this._sessionDurationMinutes = (int) properties.SessionDuration.TotalMinutes;
            this._numberOfLongPoses = properties.NumberOfLongPoses;
            this._addWarmUp = properties.AddWarmUp;
            this._addCoolDown = properties.AddCoolDown;
            this._addBreaks = properties.AddBreaks;
            this._isSimplified = properties.IsSimplified;
            this.OnPropertyChanged();
        }

        #endregion

        #endregion
    }
}