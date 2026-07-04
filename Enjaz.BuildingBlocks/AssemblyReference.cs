using System.Reflection;

namespace Enjaz.BuildingBlocks;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
