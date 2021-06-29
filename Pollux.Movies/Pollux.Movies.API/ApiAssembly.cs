using System.Reflection;

namespace Pollux.Movies
{
    public static class ApiAssembly
    {
        public static Assembly Assembly => Assembly.GetAssembly(typeof(ApiAssembly));
    }
}
