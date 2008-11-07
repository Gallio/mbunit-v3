// Taken from CodePlexClient - http://www.codeplex.com/CodePlexClient/SourceControl/FileView.aspx?itemId=59623&changeSetId=17983

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gallio.Icarus.Utilities
{
    public interface IFileSystem
    {
        string UserDataPath { get; }

        void AppendAllText(string path, string contents);
        void CombineAttributes(string path, FileAttributes attributes);
        string CombinePath(params string[] pathElements);
        void CopyFile(string sourcePath, string targetPath);
        void CopyFile(string sourcePath, string targetPath, bool overwrite);
        void CreateDirectory(string path);
        void DeleteDirectory(string path, bool force);
        void DeleteFile(string path);
        bool DirectoryExists(string path);
        void EnsurePath(string path);
        bool FileExists(string path);
        FileAttributes GetAttributes(string path);
        string[] GetDirectories(string path);
        string GetDirectoryName(string path);
        byte[] GetFileHash(string path);
        byte[] GetFileHash(byte[] contents);
        string GetFileHashAsString(string path);
        string GetFileName(string path);
        string[] GetFiles(string path);
        string[] GetFiles(string path, string searchPattern);
        long GetFileSize(string path);
        string GetFullPath(string path);
        DateTime GetLastWriteTime(string path);
        string GetName(string path);
        Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);
        byte[] ReadAllBytes(string path);
        string[] ReadAllLines(string path);
        string ReadAllText(string path);
        void RemoveAttributes(string path, FileAttributes attributes);
        void SetAttributes(string path, FileAttributes attributes);
        void WriteAllBytes(string path, byte[] contents);
        void WriteAllLines(string path, string[] contents);
        void WriteAllText(string path, string contents);
    }
}
