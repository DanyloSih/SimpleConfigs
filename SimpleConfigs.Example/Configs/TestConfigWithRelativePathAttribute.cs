﻿using System.Drawing;
using SimpleConfigs.Attributes;

namespace SimpleConfigs.Example.Configs
{
    [RelativePath("RelativePath/Test")]
    public class TestConfigWithRelativePathAttribute
    {
        public ConsoleColor ForegroundColor = ConsoleColor.Green;
        public ConsoleColor BackgroundColor = ConsoleColor.Blue;
    }
}
