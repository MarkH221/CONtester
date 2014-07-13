using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Isolib.IOPackage
{
    /// <summary>Contains function/s that deals with files and folders</summary>
    public class FileAndFolder
    {
        /// <summary>Backup your file using .bk extension</summary>
        /// <param name="path">The file path</param>
        /// <param name="overwrite">overwrite the file if it currently exist</param>
        public static void SimpleBackup(string path, [Optional, DefaultParameterValue(true)] bool overwrite)
        {
            try
            {
                var str = path + ".bk";
                if (File.Exists(str))
                {
                    if (overwrite)
                    {
                        File.Delete(str);
                        File.Copy(path, str);
                    }
                }
                else
                {
                    File.Copy(path, str);
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Create an empty folder in the current location</summary>
        /// <param name="directoryName">The folder name</param>
        /// <returns>A boolean value depending on how successful the operation was</returns>
        public static bool CreateDirectory(string directoryName)
        {
            try
            {
                var directoryString = Directory.GetCurrentDirectory() + directoryName;
                Directory.CreateDirectory(directoryString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

