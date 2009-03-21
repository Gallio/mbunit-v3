using System.IO;

namespace Gallio.Utilities
{
    ///<summary>
    /// Default implementation of IFileSystem using System.IO classes.
    ///</summary>
    public class FileSystem : IFileSystem
    {
        ///<summary>
        /// Checks if a given file exists (File.Exists).
        ///</summary>
        ///<param name="path">The path of the file.</param>
        ///<returns>True if the file exists, otherwise False.</returns>
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        ///<summary>
        /// Checks if a path is relative or absolute (Path.IsPathRooted).
        ///</summary>
        ///<param name="path">The path to check.</param>
        ///<returns>True if the path is absolute, otherwise False.</returns>
        public bool IsPathRooted(string path)
        {
            return Path.IsPathRooted(path);
        }

        ///<summary>
        /// Checks if a directory exists (Directory.Exists)
        ///</summary>
        ///<param name="path">The location of the directory.</param>
        ///<returns>True if the directory exists, otherwise False.</returns>
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        ///<summary>
        /// Attempts to create a directory.
        ///</summary>
        ///<param name="path">The location of the directory.</param>
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        ///<summary>
        /// Attempts to delete a file.
        ///</summary>
        ///<param name="path">The location of the file.</param>
        public void DeleteFile(string path)
        {
            File.Delete(path);
        }
    }
}
