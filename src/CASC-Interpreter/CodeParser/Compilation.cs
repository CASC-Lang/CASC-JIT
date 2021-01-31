using System.Collections.Generic;
using CASC.CodeParser.Syntax;
using CASC.CodeParser.Binding;
using System.Linq;
using System.Collections.Immutable;
using System.Threading;
using System.IO;
using CASC.CodeParser.Symbols;
using System;

namespace CASC.CodeParser
{
    public sealed class Compilation
    {
        private BoundGlobalScope _globalScope;

        public Compilation(params SyntaxTree[] syntaxTree)
            : this(null, syntaxTree)
        {
        }

        private Compilation(Compilation previous, params SyntaxTree[] syntaxTree)
        {
            Previous = previous;
            SyntaxTrees = syntaxTree.ToImmutableArray();
        }

        public Compilation Previous { get; }
        public ImmutableArray<SyntaxTree> SyntaxTrees { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTrees);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }

                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree) => new Compilation(this, syntaxTree);

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            var parseDiagnostics = SyntaxTrees.SelectMany(st => st.Diagnostics);

            var diagnostics = parseDiagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();

            if (diagnostics.Any())
                return new EvaluationResult(diagnostics, null);

            var program = Binder.BindProgram(GlobalScope);

            var appPath = Environment.GetCommandLineArgs()[0];
            var appDirectory = Path.GetDirectoryName(appPath);
            var cfgPath = Path.Combine(appDirectory, "cfg.dot");
            var cfgStatement = !program.Statement.Statements.Any() && program.Functions.Any()
                               ? program.Functions.Last().Value
                               : program.Statement;
            var cfg = ControlFlowGraph.Create(cfgStatement);
            
            using (var streamWriter = new StreamWriter(cfgPath))
                cfg.WriteTo(streamWriter);

            if (program.Diagnostics.Any())
                return new EvaluationResult(program.Diagnostics.ToImmutableArray(), null);

            var evaluator = new Evaluator(program, variables);

            try
            {
                var value = evaluator.Evaluate();

                return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
            }
            catch (EvaluatorException ex)
            {
                return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, ex);
            }
        }

        public void EmitTree(TextWriter writer)
        {
            var program = Binder.BindProgram(GlobalScope);

            if (program.Statement.Statements.Any())
                program.Statement.WriteTo(writer);
            else
                foreach (var functionBody in program.Functions)
                {
                    if (!GlobalScope.Functions.Contains(functionBody.Key))
                        continue;

                    functionBody.Key.WriteTo(writer);
                    functionBody.Value.WriteTo(writer);
                }
        }
    }
}