using SimpleConfigs.Core;

namespace SimpleConfigs.Example.Core 
{
    public class PathOverrideSettingsTest
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void SetRelativeFilePath()
        {
            PathSettings pathOverrideSettings = new PathSettings();

            string relativeFilePath = Path.Combine("Proxies", "Google", "available.cfg");
            pathOverrideSettings.SetRelativeFilePath(relativeFilePath);
            Assert.That(relativeFilePath, Is.EqualTo(pathOverrideSettings.RelativeFilePath));
            Assert.That(pathOverrideSettings.RelativeDirectoryPath, Is.EqualTo(Path.Combine("Proxies", "Google")));
            Assert.That(pathOverrideSettings.FileName, Is.EqualTo("available.cfg"));

            relativeFilePath = Path.Combine("available.cfg");
            pathOverrideSettings.SetRelativeFilePath(relativeFilePath);
            Assert.That(relativeFilePath, Is.EqualTo(pathOverrideSettings.FileName));
            Assert.That(pathOverrideSettings.RelativeDirectoryPath, Is.EqualTo(null));
            Assert.That(relativeFilePath, Is.EqualTo(pathOverrideSettings.RelativeFilePath));

            relativeFilePath = Path.Combine("Proxies", "Google");
            Assert.Throws<ArgumentException>(
                () => pathOverrideSettings.SetRelativeFilePath(relativeFilePath));

            relativeFilePath = string.Empty;
            pathOverrideSettings.SetRelativeFilePath(relativeFilePath);
            Assert.That(relativeFilePath, Is.EqualTo(pathOverrideSettings.RelativeFilePath));

            relativeFilePath = Path.Combine("   ", "     ");
            Assert.Throws<ArgumentException>(
                () => pathOverrideSettings.SetRelativeFilePath(relativeFilePath));
        }

        [Test]
        public void SetFileName()
        {
            PathSettings pathOverrideSettings = new PathSettings();

            string fileName = "available.cfg";
            pathOverrideSettings.SetFileName(fileName);
            Assert.That(pathOverrideSettings.FileName, Is.EqualTo(fileName));
            Assert.That(pathOverrideSettings.RelativeFilePath, Is.EqualTo(fileName));
            Assert.That(pathOverrideSettings.RelativeDirectoryPath, Is.EqualTo(null));

            pathOverrideSettings.SetFileName(null);
            Assert.That(pathOverrideSettings.FileName, Is.EqualTo(null));
            Assert.That(pathOverrideSettings.RelativeDirectoryPath, Is.EqualTo(null));
            Assert.That(pathOverrideSettings.RelativeFilePath, Is.EqualTo(string.Empty));

            fileName = Path.Combine("Proxies", "Google", "available.cfg");
            Assert.Throws<ArgumentException>(() => { 
                pathOverrideSettings.SetFileName(fileName);  
            });

            fileName = Path.Combine("Proxies", "Google");
            Assert.Throws<ArgumentException>(() => {
                pathOverrideSettings.SetFileName(fileName);
            });

            fileName = "available";
            Assert.Throws<ArgumentException>(() => {
                pathOverrideSettings.SetFileName(fileName);
            });
        }

        [Test]
        public void SetRelativeDirectoryPath()
        {
            PathSettings pathOverrideSettings = new PathSettings();

            string relativeDirectoryPath = Path.Combine("Proxies 2", "Google Folder 2");
            pathOverrideSettings.SetRelativeDirectoryPath(relativeDirectoryPath);
            Assert.That(pathOverrideSettings.FileName, Is.EqualTo(null));
            Assert.That(pathOverrideSettings.RelativeDirectoryPath, Is.EqualTo(relativeDirectoryPath));
            Assert.That(pathOverrideSettings.RelativeFilePath, Is.EqualTo(relativeDirectoryPath));

            pathOverrideSettings.SetRelativeDirectoryPath(null);
            Assert.That(pathOverrideSettings.FileName, Is.EqualTo(null));
            Assert.That(pathOverrideSettings.RelativeDirectoryPath, Is.EqualTo(null));
            Assert.That(pathOverrideSettings.RelativeFilePath, Is.EqualTo(string.Empty));


            relativeDirectoryPath = Path.Combine("Proxies", "Google", "available.cfg");
            Assert.Throws<ArgumentException>(() => {
                pathOverrideSettings.SetFileName(relativeDirectoryPath);
            });

            relativeDirectoryPath = Path.Combine("P: --|ro>xies", "Goog><>le");
            Assert.Throws<ArgumentException>(() => {
                pathOverrideSettings.SetFileName(relativeDirectoryPath);
            });
        }
    }
}