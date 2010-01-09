// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Concurrency;
using Gallio.Common.Policies;
using Gallio.Runtime.Preferences.Schema;

namespace Gallio.Runtime.Preferences
{
    /// <summary>
    /// A preference set implementation based on storing preference settings within a file.
    /// </summary>
    public class FilePreferenceSet : IPreferenceSet
    {
        private readonly FileInfo preferenceSetFile;
        private readonly LockBox<PreferenceSetData> dataLockBox;

        /// <summary>
        /// Creates a file preference set.
        /// </summary>
        /// <param name="preferenceSetFile">The preference set file.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="preferenceSetFile"/> is null.</exception>
        public FilePreferenceSet(FileInfo preferenceSetFile)
        {
            if (preferenceSetFile == null)
                throw new ArgumentNullException("preferenceSetFile");

            this.preferenceSetFile = preferenceSetFile;

            dataLockBox = new LockBox<PreferenceSetData>(new PreferenceSetData(preferenceSetFile));
        }

        /// <summary>
        /// Gets the preference set file.
        /// </summary>
        public FileInfo PreferenceSetFile
        {
            get { return new FileInfo(preferenceSetFile.ToString()); }
        }

        /// <inheritdoc />
        public void Read(ReadAction<IPreferenceSetReader> readAction)
        {
            if (readAction == null)
                throw new ArgumentNullException("readAction");

            dataLockBox.Read(data =>
            {
                data.Refresh();
                readAction(new PreferenceSetReader(data));
            });
        }

        /// <inheritdoc />
        public TResult Read<TResult>(ReadFunc<IPreferenceSetReader, TResult> readFunc)
        {
            if (readFunc == null)
                throw new ArgumentNullException("readFunc");

            return dataLockBox.Read(data =>
            {
                data.Refresh();
                return readFunc(new PreferenceSetReader(data));
            });
        }

        /// <inheritdoc />
        public void Write(WriteAction<IPreferenceSetWriter> writeAction)
        {
            if (writeAction == null)
                throw new ArgumentNullException("writeAction");

            dataLockBox.Write(data =>
            {
                data.Refresh();

                try
                {
                    writeAction(new PreferenceSetWriter(data));
                }
                finally
                {
                    data.Commit();
                }
            });
        }

        /// <inheritdoc />
        public TResult Write<TResult>(WriteFunc<IPreferenceSetWriter, TResult> writeFunc)
        {
            if (writeFunc == null)
                throw new ArgumentNullException("writeFunc");

            return dataLockBox.Write(data =>
            {
                data.Refresh();

                try
                {
                    return writeFunc(new PreferenceSetWriter(data));
                }
                finally
                {
                    data.Commit();
                }
            });
        }

        private static string ConvertToRawValue<T>(T value)
        {
            return value != null ? Convert.ToString(value) : null;
        }

        private static T ConvertFromRawValue<T>(string rawValue)
        {
            if (typeof(T).IsEnum)
                return (T) Enum.Parse(typeof(T), rawValue);

            return (T)Convert.ChangeType(rawValue, typeof(T));
        }

        private sealed class PreferenceSetData
        {
            private Memoizer<XmlSerializer> xmlSerializerMemoizer = new Memoizer<XmlSerializer>();
            private readonly FileInfo preferenceSetFile;
            private readonly Dictionary<string, string> contents;

            private DateTime? cachedTimestamp;
            private bool modified;

            public PreferenceSetData(FileInfo preferenceSetFile)
            {
                this.preferenceSetFile = preferenceSetFile;

                contents = new Dictionary<string,string>();
            }

            public void Refresh()
            {
                if (cachedTimestamp.HasValue)
                {
                    DateTime? newTimestamp = GetTimestamp();
                    if (!newTimestamp.HasValue || newTimestamp.Value != cachedTimestamp.Value)
                    {
                        contents.Clear();
                        cachedTimestamp = null;
                    }
                }
            }

            private void EnsureLoaded()
            {
                if (!cachedTimestamp.HasValue)
                {
                    cachedTimestamp = GetTimestamp();
                    if (cachedTimestamp.HasValue)
                    {
                        PreferenceContainer container = LoadPreferenceContainer();
                        if (container != null)
                        {
                            foreach (var setting in container.Settings)
                                contents[setting.Name] = setting.Value;
                        }
                    }
                }
            }

            public void Commit()
            {
                if (modified)
                {
                    PreferenceContainer container = new PreferenceContainer();
                    foreach (var pair in contents)
                        container.Settings.Add(new PreferenceSetting(pair.Key, pair.Value));

                    SavePreferenceContainer(container);

                    cachedTimestamp = GetTimestamp();
                    modified = false;
                }
            }

            public string GetSetting<T>(Key<T> preferenceSettingKey)
            {
                EnsureLoaded();

                string value;
                contents.TryGetValue(preferenceSettingKey.Name, out value);
                return value;
            }

            public void SetSetting<T>(Key<T> preferenceSettingKey, string rawValue)
            {
                EnsureLoaded();

                if (rawValue == null)
                {
                    if (contents.Remove(preferenceSettingKey.Name))
                    {
                        modified = true;
                    }
                }
                else
                {
                    string oldRawValue;
                    if (!contents.TryGetValue(preferenceSettingKey.Name, out oldRawValue)
                        || oldRawValue != rawValue)
                    {
                        modified = true;
                        contents[preferenceSettingKey.Name] = rawValue;
                    }
                }
            }

            private XmlSerializer XmlSerializer
            {
                get { return xmlSerializerMemoizer.Memoize(() => new XmlSerializer(typeof(PreferenceContainer))); }
            }

            private DateTime? GetTimestamp()
            {
                preferenceSetFile.Refresh();
                return preferenceSetFile.Exists ? preferenceSetFile.LastWriteTimeUtc : (DateTime?) null;
            }

            private PreferenceContainer LoadPreferenceContainer()
            {
                preferenceSetFile.Refresh();
                if (preferenceSetFile.Exists)
                {
                    try
                    {
                        using (var reader = new StreamReader(preferenceSetFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
                        {
                            var container = (PreferenceContainer) XmlSerializer.Deserialize(reader);
                            container.Validate();
                            return container;
                        }
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report(string.Format("Failed to load preferences from file '{0}'.", preferenceSetFile.FullName), ex);
                    }
                }

                return null;
            }

            private void SavePreferenceContainer(PreferenceContainer container)
            {
                if (! preferenceSetFile.Directory.Exists)
                    preferenceSetFile.Directory.Create();

                using (var writer = new StreamWriter(preferenceSetFile.Open(FileMode.Create, FileAccess.Write, FileShare.None)))
                {
                    XmlSerializer.Serialize(writer, container);
                }
            }
        }

        private class PreferenceSetReader : IPreferenceSetReader
        {
            protected readonly PreferenceSetData data;

            public PreferenceSetReader(PreferenceSetData data)
            {
                this.data = data;
            }

            public T GetSetting<T>(Key<T> preferenceSettingKey)
            {
                return GetSetting(preferenceSettingKey, default(T));
            }

            public T GetSetting<T>(Key<T> preferenceSettingKey, T defaultValue)
            {
                string rawValue = data.GetSetting(preferenceSettingKey);
                if (rawValue == null)
                    return defaultValue;
                return ConvertFromRawValue<T>(rawValue);
            }

            public bool HasSetting<T>(Key<T> preferenceSettingKey)
            {
                return data.GetSetting(preferenceSettingKey) != null;
            }
        }

        private sealed class PreferenceSetWriter : PreferenceSetReader, IPreferenceSetWriter
        {
            public PreferenceSetWriter(PreferenceSetData data)
                : base(data)
            {
            }

            public void SetSetting<T>(Key<T> preferenceSettingKey, T value)
            {
                string rawValue = ConvertToRawValue(value);
                data.SetSetting(preferenceSettingKey, rawValue);
            }

            public void RemoveSetting<T>(Key<T> preferenceSettingKey)
            {
                data.SetSetting(preferenceSettingKey, null);
            }
        }
    }
}
