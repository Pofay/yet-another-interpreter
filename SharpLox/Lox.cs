namespace SharpLox
{
    class Lox
    {
        public static bool hadError = false;

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

            // Console.WriteLine(new AstPrinter().Print(expression));
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
        }

        static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }

        static void Exit(int code)
        {
            Environment.Exit(code);
        }
    }
}