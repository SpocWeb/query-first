using System.IO;

namespace QueryFirst
{
    public class QfTextFileWriter : IQfTextFileWriter
    {
        public void WriteFile(QfTextFile fileToWrite)
        {
            // directory may not exist
            Directory.CreateDirectory(Path.GetDirectoryName(fileToWrite.Filename));
            File.WriteAllText(fileToWrite.Filename, fileToWrite.FileContents);
        }
    }
}
