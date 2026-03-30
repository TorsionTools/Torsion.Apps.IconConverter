using System.Reflection;

namespace Torsion.Apps.IconConverter.Models;

internal class AppProps
{
    internal static string AssemblyName => Assembly.GetExecutingAssembly().GetName().Name;
}
