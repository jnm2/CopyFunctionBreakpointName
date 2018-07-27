using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using Shouldly;

#pragma warning disable VSTHRD200, UseAsyncSuffix // Test methods don’t need async suffix

namespace CopyFunctionBreakpointName.Tests
{
    public static class FunctionBreakpointUtilsTests
    {
        [Test]
        public static async Task Null_syntax_root_argument_exception()
        {
            var ex = await Should.ThrowAsync<ArgumentNullException>(
                () => FunctionBreakpointUtils.GetFunctionBreakpointNameFactoryAsync(syntaxRoot: null, new TextSpan(0, 0), c => null, default));

            ex.ParamName.ShouldBe("syntaxRoot");
        }

        [Test]
        public static async Task Null_semantic_model_accessor_argument_exception()
        {
            var ex = await Should.ThrowAsync<ArgumentNullException>(
                () => FunctionBreakpointUtils.GetFunctionBreakpointNameFactoryAsync(SyntaxFactory.IdentifierName(""), new TextSpan(0, 0), semanticModelAccessor: null, default));

            ex.ParamName.ShouldBe("semanticModelAccessor");
        }

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

        [Test]
        public static async Task Operator_select_only_token()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A operator [|+|](A a) => a;
}", "A.op_UnaryPlus");
        }

        [Test]
        public static async Task Operator_select_before_token()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A operator [||]+(A a) => a;
}", "A.op_UnaryPlus");
        }

        [Test]
        public static async Task Operator_select_after_token()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A operator +[||](A a) => a;
}", "A.op_UnaryPlus");
        }

        [Test]
        public static async Task Operator_select_before_keyword()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [||]operator +(A a) => a;
}", "A.op_UnaryPlus");
        }

        [Test]
        public static async Task Operator_select_after_keyword()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A operator[||] +(A a) => a;
}", "A.op_UnaryPlus");
        }

        [Test]
        public static async Task Operator_select_keyword_and_token()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator +|](A a) => a;
}", "A.op_UnaryPlus");
        }

        [Test]
        public static async Task Operator_select_partial_token_and_space_after_keyword()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A operator[| +|]+(A a) => a;
}", "A.op_Increment");
        }

        [Test]
        public static async Task Operator_select_partial_keyword_and_space_before_token()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A operato[|r |]+(A a) => a;
}", "A.op_UnaryPlus");
        }

        [Test]
        public static async Task Operator_nonzero_whitespace_selection_between_keyword_and_token_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A operator[| |]+(A a) => a;
}", null);
        }

        [Test]
        public static async Task Operator_zero_whitespace_selection_touching_neither_keyword_nor_token_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A operator [||] +(A a) => a;
}", null);
        }

        [Test]
        public static async Task Operator_zero_whitespace_selection_touching_both_keyword_and_token()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A operator[||]+(A a) => a;
}", "A.op_UnaryPlus");
        }

        [Test]
        public static async Task Operator_character_before_keyword()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A[| operator|] +(A a) => a;
}", null);
        }

        [Test]
        public static async Task Conversion_operator_character_before_keyword()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A[| operator|] +(A a) => a;
}", null);
        }

        [Test]
        public static async Task Operator_character_after_token()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A operator [|+(|]A a) => a;
}", null);
        }

        [Test]
        public static async Task Conversion_operator_type_selection_returns_nothing()
        {

            await AssertFunctionBreakpointName(@"
class A
{
    public static implicit operator [|bool|](A a) => true;
}", null);
        }

        [Test]
        public static async Task Conversion_operator_implicit_keyword_selection_returns_nothing()
        {

            await AssertFunctionBreakpointName(@"
class A
{
    public static [|implicit|] operator bool(A a) => true;
}", null);
        }

        [Test]
        public static async Task Operator_Explicit()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static explicit [|operator|] bool(A a) => true;
}", "A.op_Explicit");
        }

        [Test]
        public static async Task Operator_Implicit()
        {

            await AssertFunctionBreakpointName(@"
class A
{
    public static implicit [|operator|] bool(A a) => true;
}", "A.op_Implicit");
        }

        [Test]
        public static async Task Operator_UnaryPlus()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] +(A a) => a;
}", "A.op_UnaryPlus");
        }

        [Test]
        public static async Task Operator_UnaryNegation()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] -(A a) => a;
}", "A.op_UnaryNegation");
        }

        [Test]
        public static async Task Operator_LogicalNot()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] !(A a) => a;
}", "A.op_LogicalNot");
        }

        [Test]
        public static async Task Operator_OnesComplement()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] ~(A a) => a;
}", "A.op_OnesComplement");
        }

        [Test]
        public static async Task Operator_Increment()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] ++(A a) => a;
}", "A.op_Increment");
        }

        [Test]
        public static async Task Operator_Decrement()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] --(A a) => a;
}", "A.op_Decrement");
        }

        [Test]
        public static async Task Operator_True()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static bool [|operator|] true(A a) => true;
    public static bool operator false(A a) => false;
}", "A.op_True");
        }

        [Test]
        public static async Task Operator_False()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static bool [|operator|] false(A a) => false;
    public static bool operator true(A a) => true;
}", "A.op_False");
        }

        [Test]
        public static async Task Operator_Addition()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] +(A left, A right) => left;
}", "A.op_Addition");
        }

        [Test]
        public static async Task Operator_Subtraction()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] -(A left, A right) => left;
}", "A.op_Subtraction");
        }

        [Test]
        public static async Task Operator_Multiply()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] *(A left, A right) => left;
}", "A.op_Multiply");
        }

        [Test]
        public static async Task Operator_Division()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] /(A left, A right) => left;
}", "A.op_Division");
        }

        [Test]
        public static async Task Operator_Modulus()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] %(A left, A right) => left;
}", "A.op_Modulus");
        }

        [Test]
        public static async Task Operator_BitwiseAnd()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] &(A left, A right) => left;
}", "A.op_BitwiseAnd");
        }

        [Test]
        public static async Task Operator_BitwiseOr()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] |(A left, A right) => left;
}", "A.op_BitwiseOr");
        }

        [Test]
        public static async Task Operator_ExclusiveOr()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] ^(A left, A right) => left;
}", "A.op_ExclusiveOr");
        }

        [Test]
        public static async Task Operator_LeftShift()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] <<(A a, int shift) => a;
}", "A.op_LeftShift");
        }

        [Test]
        public static async Task Operator_RightShift()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static A [|operator|] >>(A a, int shift) => a;
}", "A.op_RightShift");
        }

        [Test]
        public static async Task Operator_Equality()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static bool [|operator|] ==(A left, A right) => true;
    public static bool operator !=(A left, A right) => false;
}", "A.op_Equality");
        }

        [Test]
        public static async Task Operator_Inequality()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static bool [|operator|] !=(A left, A right) => true;
    public static bool operator ==(A left, A right) => false;
}", "A.op_Inequality");
        }

        [Test]
        public static async Task Operator_LessThan()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static bool [|operator|] <(A left, A right) => true;
    public static bool operator >(A left, A right) => false;
}", "A.op_LessThan");
        }

        [Test]
        public static async Task Operator_GreaterThan()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static bool [|operator|] >(A left, A right) => true;
    public static bool operator <(A left, A right) => false;
}", "A.op_GreaterThan");
        }

        [Test]
        public static async Task Operator_LessThanOrEqual()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static bool [|operator|] <=(A left, A right) => true;
    public static bool operator >=(A left, A right) => false;
}", "A.op_LessThanOrEqual");
        }

        [Test]
        public static async Task Operator_GreaterThanOrEqual()
        {
            await AssertFunctionBreakpointName(@"
class A
{
    public static bool [|operator|] >=(A left, A right) => true;
    public static bool operator <=(A left, A right) => false;
}", "A.op_GreaterThanOrEqual");
        }

        [Test]
        public static async Task Explicit_method_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
interface ITest
{
    void Test();
}

class A : ITest
{
    void ITest.[|Test|]() { }
}", null);
        }

        [Test]
        public static async Task Explicit_property_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
interface ITest
{
    int Test { get; }
}

class A : ITest
{
    int ITest.[|Test|] => 0;
}", null);
        }

        [Test]
        public static async Task Explicit_property_getter_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
interface ITest
{
    int Test { get; }
}

class A : ITest
{
    int ITest.Test { [|get|] => 0; }
}", null);
        }

        [Test]
        public static async Task Explicit_indexed_property_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
interface ITest
{
    int this[int index] { get; }
}

class A : ITest
{
    int ITest.[|this|][int index] => 0;
}", null);
        }

        [Test]
        public static async Task Explicit_indexed_property_getter_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
interface ITest
{
    int this[int index] { get; }
}

class A : ITest
{
    int ITest.this[int index] { [|get|] => 0; }
}", null);
        }

        [Test]
        public static async Task Explicit_event_add_accessor_returns_nothing()
        {
            await AssertFunctionBreakpointName(@"
interface ITest
{
    event System.EventHandler Test;
}

class A : ITest
{
    event System.EventHandler ITest.Test { [|add|] { } remove { } }
}", null);
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
                CSharpCompilation.Create(
                    nameof(GetFunctionBreakpointNameAsync),
                    new[] { syntaxTree },
                    new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)));

            if (!permitCompilationErrors)
            {
                Assert.That(compilation.Value.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error), Is.Empty);
            }

            var factory = await FunctionBreakpointUtils.GetFunctionBreakpointNameFactoryAsync(
                await syntaxTree.GetRootAsync().ConfigureAwait(false),
                span,
                c => Task.FromResult(compilation.Value.GetSemanticModel(syntaxTree, ignoreAccessibility: false)),
                CancellationToken.None).ConfigureAwait(false);

            return factory?.ToString();
        }
    }
}
