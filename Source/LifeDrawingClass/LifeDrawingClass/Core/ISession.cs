// *****************************************************************************
//  ISession.cs
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

namespace LifeDrawingClass.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface ISession: INotifyPropertyChanged
    {
        IReadOnlyList<string> ImagePaths { get; }
        int Interval { get; set; }
        void ImportFolder(string path, bool importSubfolders);
        void ClearPaths();
        void AddPaths(string[] fileNames);
        int CurrentSegmentIndex { get; }
    }
}