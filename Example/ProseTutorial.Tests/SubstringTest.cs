using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Compiler;
using Microsoft.ProgramSynthesis.Diagnostics;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Learning.Strategies;
using Microsoft.ProgramSynthesis.Specifications;
using Microsoft.ProgramSynthesis.VersionSpace;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProseTutorial
{
    [TestClass]
    public class SubstringTest
    {
        private const string GrammarPath = @"../../../../ProseTutorial/synthesis/grammar/substring.grammar";
        private const string SavedProgramAST = @"../../../../ProgramAST.xml";

        [TestMethod]
        public void TestLearnSubstringOneExample()
        {
            Result<Grammar> grammar = CompileGrammar();
            SynthesisEngine prose = ConfigureSynthesis(grammar.Value);

            State input = State.CreateForExecution(grammar.Value.InputSymbol, "Toby Miller");
            var examples = new Dictionary<State, object> {{input, "Miller"}};

            var spec = new ExampleSpec(examples);

            var scoreFeature = new RankingScore(grammar.Value);
            ProgramSet topPrograms = prose.LearnGrammarTopK(spec, scoreFeature, 1, null);
            ProgramNode topProgram = topPrograms.RealizedPrograms.First();

            var x = topProgram.PrintAST();
            var y = ProgramNode.Parse(x,grammar.Value); // var y is null. ==> SDK method

            var se = new ASTSerialization.Serialization(grammar.Value);
            var xe = se.PrintXML(topProgram);
            xe.Save(SavedProgramAST);
            topProgram = se.Parse(xe); // var topProgram is ok. ==> ASTSerialization.Serialization method

            var output = topProgram.Invoke(input) as string;
            Assert.AreEqual("Miller", output);

            State input2 = State.CreateForExecution(grammar.Value.InputSymbol, "Courtney Lynch");
            var output2 = topProgram.Invoke(input2) as string;
            Assert.AreEqual("Lynch", output2);
        }
        [TestMethod]
        public void TestLearnSubstringOneExampleWithASTLoad()
        {
            Result<Grammar> grammar = CompileGrammar();
            SynthesisEngine prose = ConfigureSynthesis(grammar.Value);

            State input = State.CreateForExecution(grammar.Value.InputSymbol, "Toby Miller");
            var examples = new Dictionary<State, object> {{input, "Miller"}};

            var se = new ASTSerialization.Serialization(grammar.Value);
            var xe = System.Xml.Linq.XElement.Load(new FileStream(SavedProgramAST,FileMode.Open));
            var topProgram = se.Parse(xe); // var topProgram is ok. ==> ASTSerialization.Serialization method

            var output = topProgram.Invoke(input) as string;
            Assert.AreEqual("Miller", output);

            State input2 = State.CreateForExecution(grammar.Value.InputSymbol, "Courtney Lynch");
            var output2 = topProgram.Invoke(input2) as string;
            Assert.AreEqual("Lynch", output2);
        }

        public static SynthesisEngine ConfigureSynthesis(Grammar grammar)
        {
            var witnessFunctions = new WitnessFunctions(grammar);
            var deductiveSynthesis = new DeductiveSynthesis(witnessFunctions);
            var synthesisExtrategies = new ISynthesisStrategy[] {deductiveSynthesis};
            var synthesisConfig = new SynthesisEngine.Config {Strategies = synthesisExtrategies};
            var prose = new SynthesisEngine(grammar, synthesisConfig);
            return prose;
        }

        private static Result<Grammar> CompileGrammar()
        {
            return DSLCompiler.Compile(new CompilerOptions
            {
                InputGrammarText = File.ReadAllText(GrammarPath),
                References = CompilerReference.FromAssemblyFiles(typeof(Semantics).GetTypeInfo().Assembly)
            });
        }
    }
}