using System;
using System.IO;

internal class Paths
{
    internal static readonly string DotModTekDirectory = Path.Combine(
        Environment.GetEnvironmentVariable("DOORSTOP_MANAGED_FOLDER_DIR"),
        "..", "..",
        "Mods",
        ".modtek"
    );
}