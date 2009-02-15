// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.IO;

namespace Gallio.Icarus.Utilities
{
    public class FileSystem : IFileSystem
    {
        string userDataPath;

        public string UserDataPath
        {
            get
            {
                if (userDataPath == null)
                {
                    userDataPath = CombinePath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Gallio", "Icarus");
                    EnsurePath(userDataPath);
                }
                return userDataPath;
            }
            set { userDataPath = value; }
        }

        public void AppendAllText(string path, string contents)
        {
            File.AppendAllText(path, contents);
        }

        public void CombineAttributes(string path, FileAttributes attributes)
        {
            FileAttributes existing = GetAttributes(path);

            existing |= attributes;

            SetAttributes(path, existing);
        }

        public string CombinePath(params string[] pathElements)
        {
            string result = "";

            foreach (string pathElement in pathElements)
                result = Path.Combine(result, pathElement);

            return result;
        }

        public void CopyFile(string sourcePath, string targetPath)
        {
            CopyFile(sourcePath, targetPath, false);
        }

        public void CopyFile(string sourcePath, string targetPath, bool overwrite)
        {
            File.Copy(sourcePath, targetPath, overwrite);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteDirectory(string path, bool force)
        {
            if (!Directory.Exists(path))
                return;

            if (force)
                RecursiveMakeNormal(path);

            Directory.Delete(path, force);
        }

        public void DeleteFile(string path)
        {
            if (!File.Exists(path))
                return;

            SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void EnsurePath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public FileAttributes GetAttributes(string path)
        {
            return File.GetAttributes(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string GetDirectoryName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            char ending = path[path.Length - 1];

            if (ending == Path.DirectorySeparatorChar || ending == Path.AltDirectorySeparatorChar)
                path = path.Substring(0, path.Length - 1);

            return Path.GetDirectoryName(path);
        }

        public byte[] GetFileHash(string path)
        {
            using (Stream stream = OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                return EncryptionUtil.GetHash(stream);
        }

        public byte[] GetFileHash(byte[] contents)
        {
            using (MemoryStream stream = new MemoryStream(contents, false))
                return EncryptionUtil.GetHash(stream);
        }

        public string GetFileHashAsString(string path)
        {
            return Convert.ToBase64String(GetFileHash(path));
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        public long GetFileSize(string path)
        {
            return new FileInfo(path).Length;
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public string GetName(string path)
        {
            return Path.GetFileName(path);
        }

        public Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return new FileStream(path, mode, access, share);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        void RecursiveMakeNormal(string path)
        {
            foreach (string dir in GetDirectories(path))
            {
                RecursiveMakeNormal(dir);
                SetAttributes(dir, FileAttributes.Normal | FileAttributes.Directory);
            }

            foreach (string file in GetFiles(path))
                SetAttributes(file, FileAttributes.Normal);
        }

        public void RemoveAttributes(string path, FileAttributes attributes)
        {
            FileAttributes existing = GetAttributes(path);

            existing &= ~attributes;

            SetAttributes(path, existing);
        }

        public void SetAttributes(string path, FileAttributes attributes)
        {
            File.SetAttributes(path, attributes);
        }

        public void WriteAllBytes(string path, byte[] contents)
        {
            File.WriteAllBytes(path, contents);
        }

        public void WriteAllLines(string path, string[] contents)
        {
            File.WriteAllLines(path, contents);
        }

        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
    }
}
