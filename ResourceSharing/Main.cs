using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HarmonyLib;

[assembly: IgnoresAccessChecksTo("Cosmoteer")]
namespace ResourceSharing;

public static class Main
{
    public static Harmony? Harmony;

    [UnmanagedCallersOnly]
    public static void InitializePatches()
    {
        Harmony = new("ResourceSharing");
        Harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}