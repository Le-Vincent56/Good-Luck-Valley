using System;
using System.Collections.Generic;
using System.Reflection;

namespace GoodLuckValley.Utilities.PredefinedAssembly
{
    public static class PredefinedAssemblyUtils
    {
        enum AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpEditorFirstPass,
            AssemblyCSharpFirstPass
        }

        /// <summary>
        /// Maps an assembly name to the corresponding AssemblyType
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private static AssemblyType? GetAssemblyType(string assemblyName)
        {
            return assemblyName switch
            {
                "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
                "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
                "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
                _ => null
            };
        }

        /// <summary>
        /// Looks through a given assembly and add types that fulfill a certain
        /// interface to the provided collection
        /// </summary>
        private static void AddTypesFromAssembly(Type[] assembly, ICollection<Type> types, Type interfaceType)
        {
            // Exit case - if the assembly is null
            if (assembly == null) return;

            // Iterate through the assembly types
            for (int i = 0; i < assembly.Length; i++)
            {
                // Store the current type
                Type type = assembly[i];

                // Check if the type is not the same as the interface type and that the interface type
                // is assignable from the stored type
                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                {
                    // Add the type to the collection
                    types.Add(type);
                }
            }
        }

        /// <summary>
        /// Get all types from all assemblies in the current AppDomain that implement
        /// the provided interface type
        /// </summary>
        public static List<Type> GetTypes(Type interfaceType)
        {
            // Get the assemblies in the current AppDomain
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Create a dictionary to store the assemblies and their types
            Dictionary<AssemblyType, Type[]> assemblyTypes = new();

            // Create a list to store the types
            List<Type> types = new List<Type>();

            // Iterate over each assembly
            for (int i = 0; i < assemblies.Length; i++)
            {
                // Get the assembly type
                AssemblyType? assemblyType = GetAssemblyType(assemblies[i].GetName().Name);

                // Check if the assembly type exists
                if (assemblyType != null)
                {
                    // Add the assembly to the dictionary along with its corresponding types
                    assemblyTypes.Add((AssemblyType)assemblyType, assemblies[i].GetTypes());
                }
            }

            // Add types specifically from the AssemblyCSharp and AssemblyCSharpFirstPass assemblies
            assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out Type[] assemblyCSharpTypes);
            AddTypesFromAssembly(assemblyCSharpTypes, types, interfaceType);

            assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out Type[] assemblyCSharpFirstPassTypes);
            AddTypesFromAssembly(assemblyCSharpFirstPassTypes, types, interfaceType);

            return types;
        }
    }
}