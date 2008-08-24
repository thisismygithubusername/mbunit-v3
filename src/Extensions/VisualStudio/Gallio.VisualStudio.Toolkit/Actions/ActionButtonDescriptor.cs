// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Toolkit.Actions
{
    /// <summary>
    /// Describes an action button to be installed.
    /// </summary>
    public class ActionButtonDescriptor
    {
        public Type ActionType { get; set; }
        public string CommandName { get; set; }
        public string CommandPath { get; set; }

        public string Caption { get; set; }
        public string Tooltip { get; set; }
        public ActionButtonStatus ButtonStatus { get; set; }
        public ActionButtonStyle ButtonStyle { get; set; }
        public ActionButtonType ButtonType { get; set; }
    }
}