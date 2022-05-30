using System.Reflection;

namespace Movies.Persistence
{
    public static class AssemblyPersistence
    {
        public static Assembly Assembly => Assembly.GetAssembly(typeof(AssemblyPersistence));
    }
}
