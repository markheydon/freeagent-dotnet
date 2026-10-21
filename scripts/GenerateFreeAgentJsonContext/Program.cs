using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

static bool IsExcludedHostType(Type type)
{
    if (type.Namespace != "FreeAgent.Client")
    {
        return type.Namespace?.StartsWith("FreeAgent.Client.Infrastructure.Serialization", StringComparison.Ordinal) != true;
    }

    return type.Name is "FreeAgentClient"
        or "FreeAgentOAuthClient"
        or "FreeAgentEnvironment"
        or "FreeAgentHttpClientOptions"
        || type.Name.EndsWith("Client", StringComparison.Ordinal)
        || type.Name.EndsWith("Exception", StringComparison.Ordinal)
        || type.Name.EndsWith("Options", StringComparison.Ordinal);
}

static bool IsSeedWireType(Type type)
{
    if (type.ContainsGenericParameters || (type.IsAbstract && !type.IsSealed))
    {
        return false;
    }

    if (type.Name is "OAuthTokenResponse")
    {
        return true;
    }

    if (type.Namespace?.StartsWith("FreeAgent.Client.Infrastructure.Serialization", StringComparison.Ordinal) == true)
    {
        if (type.Name.Contains('<', StringComparison.Ordinal)
            || type.Name is "FreeAgentJsonContext"
            || type.Name.EndsWith("JsonConverter", StringComparison.Ordinal)
            || type.Name.EndsWith("JsonConverterFactory", StringComparison.Ordinal))
        {
            return false;
        }

        return type.IsClass && !type.IsAbstract;
    }

    if (type.Namespace != "FreeAgent.Client" || IsExcludedHostType(type))
    {
        return false;
    }

    if (type.IsEnum)
    {
        return true;
    }

    if (typeof(FreeAgent.Client.IFreeAgentResource).IsAssignableFrom(type))
    {
        return true;
    }

    foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
    {
        if (property.GetCustomAttribute<JsonPropertyNameAttribute>() is not null)
        {
            return true;
        }
    }

    return type.Name.EndsWith("Response", StringComparison.Ordinal)
        || type.Name.EndsWith("Request", StringComparison.Ordinal)
        || type.Name.EndsWith("Payload", StringComparison.Ordinal)
        || type.Name.EndsWith("Update", StringComparison.Ordinal);
}

static IEnumerable<Type> CollectSerializableTypes(IEnumerable<Type> assemblyTypes)
{
    var types = new HashSet<Type>();
    var pending = new Queue<Type>();

    foreach (var type in assemblyTypes)
    {
        if (IsSeedWireType(type))
        {
            if (types.Add(type))
            {
                pending.Enqueue(type);
            }
        }
    }

    while (pending.Count > 0)
    {
        var type = pending.Dequeue();

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            EnqueueGraphType(property.PropertyType, types, pending);
        }

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            EnqueueGraphType(field.FieldType, types, pending);
        }
    }

    return types.OrderBy(t => t.FullName, StringComparer.Ordinal);
}

static void EnqueueGraphType(Type type, HashSet<Type> types, Queue<Type> pending)
{
    if (type.IsArray)
    {
        EnqueueGraphType(type.GetElementType()!, types, pending);
        return;
    }

    if (type.IsGenericType)
    {
        var def = type.GetGenericTypeDefinition();
        if (def.FullName is "FreeAgent.Client.Infrastructure.Serialization.ExpandableField`1"
            or "FreeAgent.Client.Infrastructure.Serialization.WriteLink`1")
        {
            if (!type.ContainsGenericParameters && types.Add(type))
            {
                pending.Enqueue(type);
            }
        }

        foreach (var arg in type.GetGenericArguments())
        {
            EnqueueGraphType(arg, types, pending);
        }

        return;
    }

    if (type.Namespace != "FreeAgent.Client"
        && type.Namespace?.StartsWith("FreeAgent.Client.Infrastructure.Serialization", StringComparison.Ordinal) != true)
    {
        return;
    }

    if (type.ContainsGenericParameters || (type.IsAbstract && !type.IsSealed) || IsExcludedHostType(type))
    {
        return;
    }

    if (type.Name.EndsWith("JsonConverter", StringComparison.Ordinal)
        || type.Name.EndsWith("JsonConverterFactory", StringComparison.Ordinal)
        || type.Name.Contains('<', StringComparison.Ordinal))
    {
        return;
    }

    if (types.Add(type))
    {
        pending.Enqueue(type);
    }
}

static string ToTypeReference(Type type) => $"typeof({ToCSharpTypeName(type)})";

static string ToCSharpTypeName(Type type)
{
    if (!type.IsGenericType)
    {
        return type.FullName!.Replace('+', '.');
    }

    var name = type.GetGenericTypeDefinition().FullName!.Replace('+', '.');
    var tick = name.IndexOf('`', StringComparison.Ordinal);
    if (tick >= 0)
    {
        name = name[..tick];
    }

    var args = string.Join(", ", type.GetGenericArguments().Select(ToCSharpTypeName));
    return $"{name}<{args}>";
}

var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
var outputPath = Path.Combine(
    repoRoot,
    "src",
    "FreeAgent.Client",
    "Infrastructure",
    "Serialization",
    "FreeAgentJsonContext.Types.g.cs");

var assembly = typeof(FreeAgent.Client.FreeAgentClient).Assembly;
Type[] assemblyTypes;
try
{
    assemblyTypes = assembly.GetTypes();
}
catch (ReflectionTypeLoadException ex)
{
    assemblyTypes = ex.Types.Where(t => t is not null).Cast<Type>().ToArray();
}

var serializableTypes = CollectSerializableTypes(assemblyTypes).ToList();

var sb = new StringBuilder();
sb.AppendLine("// <auto-generated />");
sb.AppendLine("using System.Text.Json.Serialization;");
sb.AppendLine();
sb.AppendLine("namespace FreeAgent.Client.Infrastructure.Serialization;");
sb.AppendLine();

foreach (var type in serializableTypes)
{
    sb.AppendLine($"[JsonSerializable({ToTypeReference(type)})]");
}

sb.AppendLine("partial class FreeAgentJsonContext;");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
var content = sb.ToString();
var normalised = content.ReplaceLineEndings("\n");
if (File.Exists(outputPath) && File.ReadAllText(outputPath).ReplaceLineEndings("\n") == normalised)
{
    Console.WriteLine($"Up to date: {outputPath} ({serializableTypes.Count} types)");
    return;
}

File.WriteAllText(outputPath, normalised);
Console.WriteLine($"Wrote {serializableTypes.Count} JsonSerializable entries to {outputPath}");
