using System.IO;


namespace Helpers
{
    public static class FileAndFolderExtensions
    { 
        public static void WriteToFile(this byte[] content, string DestinationPath) => File.WriteAllBytes(DestinationPath, content);
    }
}
