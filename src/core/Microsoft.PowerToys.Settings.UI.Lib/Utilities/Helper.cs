﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.PowerToys.Settings.UI.Lib.CustomAction;

namespace Microsoft.PowerToys.Settings.UI.Lib.Utilities
{
    public static class Helper
    {
        public static bool AllowRunnerToForeground()
        {
            var result = false;
            var processes = Process.GetProcessesByName("PowerToys");
            if (processes.Length > 0)
            {
                var pid = processes[0].Id;
                result = NativeMethods.AllowSetForegroundWindow(pid);
            }

            return result;
        }

        public static string GetSerializedCustomAction(string moduleName, string actionName, string actionValue)
        {
            var customAction = new CustomActionDataModel
            {
                Name = actionName,
                Value = actionValue,
            };

            var moduleCustomAction = new ModuleCustomAction
            {
                ModuleAction = customAction,
            };

            var sendCustomAction = new SendCustomAction(moduleName)
            {
                Action = moduleCustomAction,
            };

            return sendCustomAction.ToJsonString();
        }

        public static FileSystemWatcher GetFileWatcher(string moduleName, string fileName, Action onChangedCallback)
        {
            var path = Path.Combine(LocalApplicationDataFolder(), $"Microsoft\\PowerToys\\{moduleName}");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var watcher = new FileSystemWatcher
            {
                Path = path,
                Filter = fileName,
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true,
            };

            watcher.Changed += (o, e) => onChangedCallback();

            return watcher;
        }

        private static string LocalApplicationDataFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        private static readonly interop.LayoutMapManaged LayoutMap = new interop.LayoutMapManaged();

        public static string GetKeyName(uint key)
        {
            return LayoutMap.GetKeyName(key);
        }

        public static string GetProductVersion()
        {
            return interop.CommonManaged.GetProductVersion();
        }

        public static int CompareVersions(string version1, string version2)
        {
            try
            {
                // Split up the version strings into int[]
                // Example: v10.0.2 -> {10, 0, 2};
                if (version1 == null)
                {
                    throw new ArgumentNullException(nameof(version1));
                }
                else if (version2 == null)
                {
                    throw new ArgumentNullException(nameof(version2));
                }

                var v1 = version1.Substring(1).Split('.').Select(int.Parse).ToArray();
                var v2 = version2.Substring(1).Split('.').Select(int.Parse).ToArray();

                if (v1.Length != 3 || v2.Length != 3)
                {
                    throw new FormatException();
                }

                if (v1[0] - v2[0] != 0)
                {
                    return v1[0] - v2[0];
                }

                if (v1[1] - v2[1] != 0)
                {
                    return v1[1] - v2[1];
                }

                return v1[2] - v2[2];
            }
            catch (Exception)
            {
                throw new FormatException("Bad product version format");
            }
        }

        public const uint VirtualKeyWindows = interop.Constants.VK_WIN_BOTH;
    }
}
