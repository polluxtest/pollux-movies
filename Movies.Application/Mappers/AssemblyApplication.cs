using System.Reflection;

namespace Movies.Application.Mappers
{
    public static class AssemblyApplication
    {
        public static Assembly Assembly => Assembly.GetAssembly(typeof(AssemblyApplication));
    }
}
