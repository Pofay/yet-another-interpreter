namespace SharpLox
{
    class Lox
    {
        private static readonly Interpreter interpreter = new Interpreter();
        private static bool hadError = false;
        private static bool hadRuntimeError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: sharpLox [script]");
                Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
            // Checkpoint code for Chapter 5 Crafting Interpreters
            // var expression = new Expr.Binary(
            //     new Expr.Unary(
            //         new Token(TokenType.MINUS, "-", null, 1),
            //         new Expr.Literal(123)),
            //         new Token(TokenType.STAR, "*", null, 1),
            //         new Expr.Grouping(
            //             new Expr.Literal(45.67)));

            // Console.WriteLine(new AstPrinter().Print(expression));k
        }

        static void RunPrompt()
        {
            for (; ; )
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null) break;
                Run(line);
                hadError = false;
            }
        }

        static void RunFile(string path)
        {
            var reader = new StreamReader(path);
            var script = reader.ReadToEnd();
            Run(script);
            if (hadError) Exit(65);
            if (hadRuntimeError) Exit(70);
        }

        static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();

            if (hadError) return;

            interpreter.Interpret(statements);
            // foreach (var token in tokens)
            // {
            //     Console.WriteLine(token);
            // }
            // Console.WriteLine(new AstPrinter().Print(expression));
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, String message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }

        static void Exit(int code)
        {
            System.Environment.Exit(code);
        }

        internal static void RuntimeError(RuntimeError error)
        {
            Console.WriteLine(error.Message, $"\n[line {error.Token.Line}]");
            hadRuntimeError = true;
        }
    }
}