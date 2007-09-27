using System.IO;
using System.Reflection;

namespace MbUnit.Framework.Tests
{
    [TestFixture]
    [TestsOn(typeof(FileAssert))]
    public class FileAssertTest
    {
        [Test]
        public void AreEqualPath()
        {
            string pathFile = Assembly.GetExecutingAssembly().Location;
            FileAssert.AreEqual(pathFile, pathFile);
        }

        [Test]
        public void AreEqualFileInfo()
        {
            string pathFile = Assembly.GetExecutingAssembly().Location;
            FileInfo file = new FileInfo(pathFile);
            FileInfo file2 = new FileInfo(pathFile);
            FileAssert.AreEqual(file, file2);
        }

        [Test]
        public void AreStreamContentEqual()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string path = Path.GetDirectoryName(assemblyPath);
            string pathFile = Path.Combine(path, "MbUnitFileAssert_Test.tmp");


            try
            {
                StreamWriter strWriter = new StreamWriter(pathFile);

                strWriter.WriteLine("Testing MbUnit");
                strWriter.Close();

                Stream str = new FileStream(pathFile, FileMode.Open);

                File.Copy(pathFile, pathFile + "TestCopy", true);
                Stream str2 = new FileStream(pathFile + "TestCopy", FileMode.Open);

                FileAssert.AreStreamContentEqual(str, str2);
            }
            finally
            {
                File.Delete(pathFile + "TestCopy");
            }

        }

        [Test]
        public void Exists()
        {
            string path = Assembly.GetExecutingAssembly().Location;
            FileAssert.Exists(path);
        }

        [Test]
        public void NotExists()
        {
            string path = Assembly.GetExecutingAssembly().Location;
            FileAssert.NotExists(path + "MbUnitTest");
        }
    }
}
