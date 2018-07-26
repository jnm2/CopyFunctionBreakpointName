using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

#pragma warning disable VSTHRD200, UseAsyncSuffix // Test methods don’t need async suffix

namespace CopyFunctionBreakpointName.Tests
{
    public static class FunctionBreakpointUtilsTests
    {
        [Test]
        public static async Task Namespace_not_needed()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    void [|B|]() { }
}", "A.B");
        }

        [Test]
        public static async Task Simple_namespace()
        {
            await AssertFunctionBreakpointName(@"
namespace A
{
    class B
    {
        void [|C|]() { }
    }
}", "A.B.C");
        }

        [Test]
        public static async Task Dotted_namespace()
        {
            await AssertFunctionBreakpointName(@"
namespace A.B
{
    class C
    {
        void [|D|]() { }
    }
}", "A.B.C.D");
        }

        [Test]
        public static async Task Nested_namespace()
        {
            await AssertFunctionBreakpointName(@"
namespace A
{
    namespace B1 { }

    namespace B2
    {
        class C
        {
            void [|D|]() { }
        }
    }
}", "A.B2.C.D");
        }

        [Test]
        public static async Task Nested_dotted_namespace()
        {
            await AssertFunctionBreakpointName(@"
namespace A.B
{
    namespace C.D
    {
        class E
        {
            void [|F|]() { }
        }
    }
}", "A.B.C.D.E.F");
        }

        [Test]
        public static async Task Nested_classes()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    class B
    {
        void [|C|]() { }
    }
}", "A.B.C");
        }

        [Test]
        public static async Task Struct()
        {
            await AssertFunctionBreakpointName(@"
struct A
{
    void [|B|]() { }
}", "A.B");
        }

        [Test]
        public static async Task Nested_structs()
        {
            await AssertFunctionBreakpointName(@"
struct A
{
    struct B
    {
        void [|C|]() { }
    }
}", "A.B.C");
        }

        [Test]
        public static async Task Namespace_identifier_selection_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
namespace [|A|]
{
    class B
    {
        void C() { }
    }
}", null);
        }

        [Test]
        public static async Task Class_identifier_selection_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
class [|A|]
{
    void B() { }
}", null);
        }

        [Test]
        public static async Task Zero_width_selection_at_start_of_method_name()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    void [||]B() { }
}", "A.B");
        }

        [Test]
        public static async Task Zero_width_selection_at_end_of_method_name()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    void B[||]() { }
}", "A.B");
        }

        [Test]
        public static async Task Zero_width_selection_inside_method_name()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    void B[||]B() { }
}", "A.BB");
        }

        [Test]
        public static async Task Selection_past_end_of_method_name_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    void [|B(|]) { }
}", null);
        }

        [Test]
        public static async Task Selection_before_start_of_method_name_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    void[| B|]() { }
}", null);
        }

        [Test]
        public static async Task Class_instance_constructor()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    [|A|]()
    {
    }
}", "A.A");
        }

        [Test]
        public static async Task Class_static_constructor()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    static [|A|]()
    {
    }
}", "A.cctor");
        }

        [Test]
        public static async Task Struct_instance_constructor()
        {
            await AssertFunctionBreakpointName(@"
struct A
{
    [|A|](int x)
    {
    }
}", "A.A");
        }

        [Test(Description = "There does not appear to be a way to set a breakpoint on a struct static constructor by function name.")]
        public static async Task Struct_static_constructor()
        {
            await AssertFunctionBreakpointName(@"
struct A
{
    static [|A|]()
    {
    }
}", null);
        }

        [Test]
        public static async Task Finalizer()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    ~[|A|]()
    {
    }
}", "A.Finalize");
        }

        [Test]
        public static async Task Local_function_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    void B()
    {
        void [|C|]() { }
    }
}", null);
        }

        [Test]
        public static async Task Entire_property()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int [|B|] { get; set; }
}", "A.B");
        }

        [Test]
        public static async Task Entire_property_expression()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int [|B|] => 0;
}", "A.B");
        }

        [Test]
        public static async Task Get_accessor_auto()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int B { [|get|]; }
}", "A.B.get");
        }

        [Test]
        public static async Task Get_accessor_expression()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int B { [|get|] => 0; }
}", "A.B.get");
        }

        [Test]
        public static async Task Get_accessor_statement()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int B { [|get|] { return 0; } }
}", "A.B.get");
        }

        [Test]
        public static async Task Set_accessor_auto()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int B { get; [|set|]; }
}", "A.B.set");
        }

        [Test]
        public static async Task Set_accessor_expression()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int B { [|set|] => _ = 0; }
}", "A.B.set");
        }

        [Test]
        public static async Task Set_accessor_statement()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int B { [|set|] { } }
}", "A.B.set");
        }

        [Test]
        public static async Task Entire_indexed_property()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int [|this|][int index] { get => 0; set { } }
}", "A.Item");
        }

        [Test]
        public static async Task Entire_indexed_property_expression()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int [|this|][int index] => 0;
}", "A.Item");
        }

        [Test]
        public static async Task Indexed_get_accessor_expression()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int this[int index] { [|get|] => 0; }
}", "A.Item.get");
        }

        [Test]
        public static async Task Indexed_get_accessor_statement()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int this[int index] { [|get|] { return 0; } }
}", "A.Item.get");
        }

        [Test]
        public static async Task Indexed_set_accessor_expression()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int this[int index] { [|set|] => _ = 0; }
}", "A.Item.set");
        }

        [Test]
        public static async Task Indexed_set_accessor_statement()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    int this[int index] { [|set|] { } }
}", "A.Item.set");
        }

        [Test]
        public static async Task Named_indexed_property()
        {
            await AssertFunctionBreakpointName(@"
using System.Runtime.CompilerServices;

class A
{
    [IndexerName(""B"")]
    int [|this|][int index] => 0;
}", "A.B");
        }

        [Test]
        public static async Task Named_indexed_get_accessor()
        {
            await AssertFunctionBreakpointName(@"
using System.Runtime.CompilerServices;

class A
{
    [IndexerName(""B"")]
    int this[int index] { [|get|] => 0; }
}", "A.B.get");
        }

        [Test]
        public static async Task Named_indexed_set_accessor()
        {
            await AssertFunctionBreakpointName(@"
using System.Runtime.CompilerServices;

class A
{
    [IndexerName(""B"")]
    int this[int index] { [|set|] { } }
}", "A.B.set");
        }

        [Test]
        public static async Task Entire_event_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    event System.Action [|B|];
}", null);
        }

        [Test]
        public static async Task Entire_event_custom_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    event System.Action [|B|] { add { } remove { } }
}", null);
        }

        [Test]
        public static async Task Add_accessor_expression()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    event System.Action B { [|add|] => _ = value; remove => _ = value; }
}", "A.add_B");
        }

        [Test]
        public static async Task Add_accessor_statement()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    event System.Action B { [|add|] { } remove { } }
}", "A.add_B");
        }

        [Test]
        public static async Task Remove_accessor_expression()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    event System.Action B { add => _ = value; [|remove|] => _ = value; }
}", "A.remove_B");
        }

        [Test]
        public static async Task Remove_accessor_statement()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    event System.Action B { add { } [|remove|] { } }
}", "A.remove_B");
        }

        private static async Task AssertFunctionBreakpointName(string annotatedSource, string expected)
        {
            Assert.That(await GetFunctionBreakpointNameAsync(annotatedSource), Is.EqualTo(expected));
        }

        private static async Task<string> GetFunctionBreakpointNameAsync(string annotatedSource, bool permitCompilationErrors = false)
        {
            var (source, span) = AnnotatedSourceUtils.Parse(annotatedSource, nameof(annotatedSource));

            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var compilation = new Lazy<CSharpCompilation>(() =>
            {
                var result = CSharpCompilation.Create(
                    nameof(GetFunctionBreakpointNameAsync),
                    new[] { syntaxTree },
                    new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                Assert.That(result.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error), Is.Empty);

                return result;
            });

            if (!permitCompilationErrors) _ = compilation.Value;

            var factory = await FunctionBreakpointUtils.GetFunctionBreakpointNameFactoryAsync(
                await syntaxTree.GetRootAsync().ConfigureAwait(false),
                span,
                c => Task.FromResult(compilation.Value.GetSemanticModel(syntaxTree, ignoreAccessibility: false)),
                CancellationToken.None).ConfigureAwait(false);

            return factory?.ToString();
        }
    }
}
