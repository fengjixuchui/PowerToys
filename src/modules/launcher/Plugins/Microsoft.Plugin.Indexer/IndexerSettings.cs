﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Microsoft.Plugin.Indexer
{
    public class IndexerSettings
    {
        public List<ContextMenu> ContextMenus { get; } = new List<ContextMenu>();

        public int MaxSearchCount { get; set; } = 30;

        public bool UseLocationAsWorkingDir { get; set; } = false;
    }
}
