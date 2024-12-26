﻿namespace common;
using System.Runtime.InteropServices;

public static class Os
{
    public static string GetPathSeparator() {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ";" : ":";
    }
}