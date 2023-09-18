internal class Program
{
    private static void Main(string[] args)
    {
        var outputPath = @"\";
        // DefineAst(outputPath, "Expr", new[] {
        //     "Binary   : Expr left, Token opr, Expr right",
            // "Grouping : Expr expression",
        //     "Literal  : Object value",
        //     "Unary    : Token opr, Expr right"
        // });
        DefineAst(outputPath, "Stmt", new [] {
            "Expression : Expr xpression",
            "Print      : Expr xpression",
            
        });
    }

    private static void DefineAst(string path, string baseName, string[] types)
    {
        using (var writer = new StreamWriter($"{path}{baseName}.cs"))
        {
            DefineNamespace(writer);
            writer.WriteLine();
            writer.WriteLine($"    public abstract class {baseName} {{");
            DefineVisitor(writer, baseName, types);
            writer.WriteLine();
            writer.WriteLine($"      public abstract T Accept<T>(I{baseName}Visitor<T> visitor);");
            foreach (var type in types)
            {
                var className = type.Split(":")[0].Trim();
                var fields = type.Split(":")[1].Trim();
                DefineType(writer, baseName, className, fields);
            }

            writer.WriteLine("}");
            writer.WriteLine("}");
        }
    }

    private static void DefineNamespace(StreamWriter writer)
    {
        writer.WriteLine("using System.Collections.Generic;");
        writer.WriteLine();
        writer.WriteLine("namespace SharpLox {");
    }

    private static void DefineVisitor(StreamWriter writer, string baseName, string[] types)
    {
        writer.WriteLine();
        writer.WriteLine($"        public interface I{baseName}Visitor<T> {{");
        foreach (var type in types)
        {
            var typeName = type.Split(":")[0].Trim();
            writer.WriteLine($"            T Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
        }
        writer.WriteLine("        }");
    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine();
        writer.WriteLine($"        public class {className} : {baseName} {{");
        var fields = fieldList.Split(",");

        DefineFields(writer, fields);
        DefineCtor(writer, className, fieldList, fields);
        DefineVisitorImplementation(writer, baseName, className);

        writer.WriteLine("      }");
    }

    private static void DefineFields(StreamWriter writer, string[] fields)
    {
        foreach (var field in fields)
        {
            var trimmed = field.Trim();
            var type = trimmed.Split(' ')[0].Trim();
            var name = trimmed.Split(' ')[1].Trim();
            writer.WriteLine($"            public {type} {Capitalize(name)} {{ get; }}");
        }
    }

    private static void DefineCtor(StreamWriter writer, string className, string fieldList, string[] fields)
    {
        writer.WriteLine($"           public {className}({fieldList}) {{");
        foreach (var field in fields)
        {
            var trimmed = field.Trim();
            var name = trimmed.Split(' ')[1].Trim();
            writer.WriteLine($"                this.{Capitalize(name)} = {name};");
        }
        writer.WriteLine("            }");
    }

    private static void DefineVisitorImplementation(StreamWriter writer, string baseName, string className)
    {
        writer.WriteLine();
        writer.WriteLine($"         public override T Accept<T>(I{baseName}Visitor<T> visitor) {{");
        writer.WriteLine($"            return visitor.Visit{className}{baseName}(this);");
        writer.WriteLine("         }");
    }

    private static string Capitalize(string source)
    {
        return source.Substring(0, 1).ToUpper() + source.Substring(1);
    }
}