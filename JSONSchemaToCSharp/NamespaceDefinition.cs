using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONSchemaToCSharp
{
    internal class NamespaceDefinition : ObjectDefinition
    {
        public string Source
        {
            get;
            set;
        }

        public string Name { get; set; }

        protected Dictionary<string, EnumDefinition> EnumDefinitions { get; set; } = new Dictionary<string, EnumDefinition>();

        protected Dictionary<string, ClassDefinition> ClassDefinitions { get; set; } = new Dictionary<string, ClassDefinition>();

        protected Dictionary<string, ValueDefinition> ValueDefinitions { get; set; } = new Dictionary<string, ValueDefinition>();

        public Dictionary<string, CommonDefinition> CommonDefinitions { get; set; }

        public static IReadOnlyList<string>? UsingNamespaces { get; set; }

        public NamespaceDefinition(string source, string namespaceName)
        {
            Source = source;

            Name = namespaceName;

            CommonDefinitions = ReadCommonDefinitions(Path.GetDirectoryName(Source)!, Path.GetFileNameWithoutExtension(source));
        }

        public NamespaceDefinition(string source)
        {
            Source = source;

            var directoryName = Path.GetDirectoryName(Source)!;

            var namespaceName = Path.GetFileName(directoryName);
            Name = !string.IsNullOrEmpty(namespaceName) ? ToDefinitionName(namespaceName) : "Default";

            CommonDefinitions = ReadCommonDefinitions(directoryName, Path.GetFileNameWithoutExtension(source));
        }

        public ObjectDefinition AddClassDefinition(ClassDefinition classDefinition)
        {
            var signature = classDefinition.GetSignature();
            if (CommonDefinitions.TryGetValue(signature, out var commonDefinition))
            {
                if (commonDefinition.ObjectDefinition != null)
                {
                    return commonDefinition.ObjectDefinition;
                }

                classDefinition.Title = commonDefinition.Name;
                if (commonDefinition.IsValueType)
                {
                    var valueDefinition = new ValueDefinition(classDefinition);
                    ValueDefinitions.Add(valueDefinition.GetName(), valueDefinition);
                    commonDefinition.ObjectDefinition = valueDefinition;
                    return valueDefinition;
                }
                else
                {
                    commonDefinition.ObjectDefinition = classDefinition;
                    ClassDefinitions.Add(classDefinition.GetName(), classDefinition);
                    return classDefinition;
                }
            }
            else
            {
                var className = classDefinition.GetName();
                if (ClassDefinitions.TryGetValue(className, out var existingClassDefinition))
                {
                    if (existingClassDefinition.GetSignature() == signature)
                    {
                        return existingClassDefinition;
                    }

                    (int suffix, bool isExistingDefinition) = GetNameSuffix(ClassDefinitions.ToDictionary(d => d.Key, d => d.Value.GetSignature()), className, signature);

                    if (isExistingDefinition)
                    {
                        return ClassDefinitions[className + suffix];
                    }

                    classDefinition.NameSuffix = suffix.ToString();
                }

                if (ValueDefinitions.TryGetValue(className, out _))
                {
                    classDefinition.NameSuffix = GetNameSuffix(ValueDefinitions.Keys, className);
                }

                if (EnumDefinitions.TryGetValue(className, out _))
                {
                    classDefinition.NameSuffix = GetNameSuffix(EnumDefinitions.Keys, className);
                }

                className = classDefinition.GetName();

                ClassDefinitions.Add(className, classDefinition);

                return classDefinition;
            }
        }

        public EnumDefinition AddEnumDefinition(EnumDefinition enumDefinition)
        {
            var signature = enumDefinition.GetSignature();
            if (CommonDefinitions.TryGetValue(signature, out var definitionOverride))
            {
                if (definitionOverride.ObjectDefinition is EnumDefinition existingEnumDefinition)
                {
                    return existingEnumDefinition;
                }

                enumDefinition.Title = definitionOverride.Name;
                enumDefinition.EnumValueNames = definitionOverride.Values;
                definitionOverride.ObjectDefinition = enumDefinition;
                EnumDefinitions.Add(enumDefinition.GetName(), enumDefinition);
                return enumDefinition;
            }
            else
            {
                var enumName = enumDefinition.GetName();
                if (EnumDefinitions.TryGetValue(enumName, out var existingEnumDefinition))
                {
                    if (existingEnumDefinition.GetSignature() == signature)
                    {
                        return existingEnumDefinition;
                    }

                    (int suffix, bool isExistingDefinition) = GetNameSuffix(EnumDefinitions.ToDictionary(d => d.Key, d => d.Value.GetSignature()), enumName, signature);

                    if (isExistingDefinition)
                    {
                        return EnumDefinitions[enumName + suffix];
                    }

                    enumDefinition.NameSuffix = suffix.ToString();
                }

                if (ValueDefinitions.TryGetValue(enumName, out _))
                {
                    enumDefinition.NameSuffix = GetNameSuffix(ValueDefinitions.Keys, enumName);
                }

                if (ClassDefinitions.TryGetValue(enumName, out _))
                {
                    enumDefinition.NameSuffix = GetNameSuffix(ClassDefinitions.Keys, enumName);
                }

                enumName = enumDefinition.GetName();

                EnumDefinitions.Add(enumName, enumDefinition);

                return enumDefinition;
            }
        }

        private static (int, bool) GetNameSuffix(Dictionary<string, string> signatures, string name, string signature)
        {
            Int32 index = 1;
            string nameWithSuffix = name + index;
            for (; ; )
            {
                if (!signatures.TryGetValue(nameWithSuffix, out var sig))
                {
                    return (index, false);
                }

                if (sig == signature)
                {
                    return (index, true);
                }

                nameWithSuffix = name + ++index;
            }
        }

        private static string? GetNameSuffix(IEnumerable<string> keys, string name)
        {
            Int32 index = 1;
            string nameWithSuffix = name + index;
            for (; ; )
            {
                if (!keys.Any(k => k == nameWithSuffix))
                {
                    return index.ToString();
                }

                nameWithSuffix = name + ++index;
            }
        }

        private Dictionary<string, CommonDefinition> ReadCommonDefinitions(string directoryName, string fileName)
        {
            var commonDefinitionFile = Path.Combine(directoryName, fileName + ".Common.json");
            if (!File.Exists(commonDefinitionFile))
            {
                return new Dictionary<string, CommonDefinition>();
            }

            try
            {
                JsonDocument commonDefDoc;
                using (var inputStream = new FileStream(commonDefinitionFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    commonDefDoc = JsonDocument.Parse(inputStream);
                    return commonDefDoc.RootElement.EnumerateObject().ToDictionary(a => a.Name, a => new CommonDefinition(a.Value));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Dictionary<string, CommonDefinition>();
            }
        }

        internal override void Write(StreamWriter sw)
        {
            sw.WriteLine("using System.ComponentModel.DataAnnotations;");
            sw.WriteLine("using System.Runtime.Serialization;");
            sw.WriteLine("using System.Text.Json.Serialization;");
            if (UsingNamespaces != null)
            {
                foreach (var u in UsingNamespaces)
                {
                    sw.WriteLine("using " + u + ';');
                }
            }

            sw.WriteLine();

            sw.WriteLine("/// <summary>");
            sw.WriteLine("/// Generated from \"" + Path.GetFileName(Source));
            sw.WriteLine("/// </summary>");
            sw.WriteLine("namespace " + Name + ';');
            sw.WriteLine();

            sw.WriteLine("#region EnumDefinitions");
            var index = 0;
            foreach (var enumDefinition in EnumDefinitions.OrderBy(d => d.Key))
            {
                enumDefinition.Value.Write(sw);
                if (++index != EnumDefinitions.Count)
                {
                    sw.WriteLine();
                }
            }
            sw.WriteLine("#endregion // EnumDefinitions");
            sw.WriteLine();

            sw.WriteLine("#region ValueDefinitions");
            index = 0;
            foreach (var valueDefinition in ValueDefinitions.OrderBy(d => d.Key))
            {
                valueDefinition.Value.Write(sw);
                if (++index != ValueDefinitions.Count)
                {
                    sw.WriteLine();
                }
            }
            sw.WriteLine("#endregion //ValueDefinitions");
            sw.WriteLine();

            sw.WriteLine("#region ClassDefinitions");
            index = 0;
            foreach (var classDefinition in ClassDefinitions.OrderBy(d => d.Key))
            {
                classDefinition.Value.Write(sw);
                if (++index != ClassDefinitions.Count)
                {
                    sw.WriteLine();
                }
            }
            sw.WriteLine("#endregion //ClassDefinitions");

            foreach (var classGroup in ClassDefinitions.Values.GroupBy(c => c.GetSignature()))
            {
                if (classGroup.Count() == 1)
                {
                    continue;
                }

                Console.WriteLine("The following classes have the same contents:");
                Console.WriteLine("\t" + string.Join(",", classGroup.Select(c => '"' + c.GetName() + '"')));
                Console.WriteLine("\tThe signature is \"" + classGroup.Key + "\".");
            }

            Console.WriteLine("Number of enum definitions processed: " + EnumDefinitions.Count);
            Console.WriteLine("Number of value definitions processed: " + ValueDefinitions.Count);
            Console.WriteLine("Number of class definitions processed: " + ClassDefinitions.Count);
        }

        internal void AddRootClass(JsonElement rootElement)
        {
            if (rootElement.ValueKind != JsonValueKind.Object)
            {
                throw new ArgumentException("No root element found.", nameof(rootElement));
            }

            var rootClassDefinition = new ClassDefinition(rootElement.EnumerateObject(), this);

            if (string.IsNullOrEmpty(rootClassDefinition.Title))
            {
                rootClassDefinition.Title = Path.GetFileName(Source);
            }

            if (string.IsNullOrEmpty(rootClassDefinition.Title))
            {
                throw new Exception("Cannot determine root class name.");
            }

            ClassDefinitions[rootClassDefinition.GetName()] = rootClassDefinition;

            rootClassDefinition.ParseObjectDefinitions(this);
        }

        internal JsonDocument LoadRef(string refname)
        {
            var directoryName = Path.GetDirectoryName(Source)!;

            var refPath = Path.Combine(directoryName, refname);

            using (var inputStream = new FileStream(refPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return JsonDocument.Parse(inputStream);
            }
        }
    }
}