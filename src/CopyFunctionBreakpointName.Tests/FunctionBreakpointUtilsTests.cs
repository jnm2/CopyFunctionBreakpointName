using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace CopyFunctionBreakpointName.Tests
{
    public static class FunctionBreakpointUtilsTests
    {
        [Test]
        public static void Namespace_not_needed()
        {
            AssertFunctionBreakpointName(@"
class A
{
    void [|B|]() { }
}", "A.B");
        }

        [Test]
        public static void Simple_namespace()
        {
            AssertFunctionBreakpointName(@"
namespace A
{
    class B
    {
        void [|C|]() { }
    }
}", "A.B.C");
        }

        [Test]
        public static void Dotted_namespace()
        {
            AssertFunctionBreakpointName(@"
namespace A.B
{
    class C
    {
        void [|D|]() { }
    }
}", "A.B.C.D");
        }

        [Test]
        public static void Nested_namespace()
        {
            AssertFunctionBreakpointName(@"
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
        public static void Nested_dotted_namespace()
        {
            AssertFunctionBreakpointName(@"
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
        public static void Nested_classes()
        {
            AssertFunctionBreakpointName(@"
class A
{
    class B
    {
        void [|C|]() { }
    }
}", "A.B.C");
        }

        [Test]
        public static void Struct()
        {
            AssertFunctionBreakpointName(@"
struct A
{
    void [|B|]() { }
}", "A.B");
        }

        [Test]
        public static void Nested_structs()
        {
            AssertFunctionBreakpointName(@"
struct A
{
    struct B
    {
        void [|C|]() { }
    }
}", "A.B.C");
        }

        [Test]
        public static void Namespace_identifier_selection_returns_nothing()
        {
            AssertFunctionBreakpointName(@"
namespace [|A|]
{
    class B
    {
        void C() { }
    }
}", null);
        }

        [Test]
        public static void Class_identifier_selection_returns_nothing()
        {
            AssertFunctionBreakpointName(@"
class [|A|]
{
    void B() { }
}", null);
        }

        [Test]
        public static void Zero_width_selection_at_start_of_method_name()
        {
            AssertFunctionBreakpointName(@"
class A
{
    void [||]B() { }
}", "A.B");
        }

        [Test]
        public static void Zero_width_selection_at_end_of_method_name()
        {
            AssertFunctionBreakpointName(@"
class A
{
    void B[||]() { }
}", "A.B");
        }

        [Test]
        public static void Zero_width_selection_inside_method_name()
        {
            AssertFunctionBreakpointName(@"
class A
{
    void B[||]B() { }
}", "A.BB");
        }

        [Test]
        public static void Selection_past_end_of_method_name_returns_nothing()
        {
            AssertFunctionBreakpointName(@"
class A
{
    void [|B(|]) { }
}", null);
        }

        [Test]
        public static void Selection_before_start_of_method_name_returns_nothing()
        {
            AssertFunctionBreakpointName(@"
class A
{
    void[| B|]() { }
}", null);
        }

        [Test]
        public static void Local_function_returns_nothing()
        {
            AssertFunctionBreakpointName(@"
class A
{
    void B()
    {
        void [|C|]() { }
    }
}", null);
        }

        [Test]
        public static void Entire_property()
        {
            AssertFunctionBreakpointName(@"
class A
{
    int [|B|] { get; set; }
}", "A.B");
        }

        [Test]
        public static void Entire_property_expression()
        {
            AssertFunctionBreakpointName(@"
class A
{
    int [|B|] => 0;
}", "A.B");
        }

        [Test]
        public static void Get_accessor_auto()
        {
            AssertFunctionBreakpointName(@"
class A
{
    int B { [|get|]; }
}", "A.B.get");
        }

        [Test]
        public static void Get_accessor_expression()
        {
            AssertFunctionBreakpointName(@"
class A
{
    int B { [|get|] => 0; }
}", "A.B.get");
        }

        [Test]
        public static void Get_accessor_statement()
        {
            AssertFunctionBreakpointName(@"
class A
{
    int B { [|get|] { return 0; } }
}", "A.B.get");
        }

        [Test]
        public static void Set_accessor_auto()
        {
            AssertFunctionBreakpointName(@"
class A
{
    int B { [|set|]; }
}", "A.B.set");
        }

        [Test]
        public static void Set_accessor_expression()
        {
            AssertFunctionBreakpointName(@"
class A
{
    int B { [|set|] => _ = 0; }
}", "A.B.set");
        }

        [Test]
        public static void Set_accessor_statement()
        {
            AssertFunctionBreakpointName(@"
class A
{
    int B { [|set|] { } }
}", "A.B.set");
        }

        [Test]
        public static void Entire_event_returns_nothing()
        {
            AssertFunctionBreakpointName(@"
class A
{
    event System.Action [|B|];
}", null);
        }

        [Test]
        public static void Entire_event_custom_returns_nothing()
        {
            AssertFunctionBreakpointName(@"
class A
{
    event System.Action [|B|] { add { } remove { } }
}", null);
        }

        [Test]
        public static void Add_accessor_expression()
        {
            AssertFunctionBreakpointName(@"
class A
{
    event System.Action B { [|add|] => _ = value; remove => _ = value; }
}", "A.add_B");
        }

        [Test]
        public static void Add_accessor_statement()
        {
            AssertFunctionBreakpointName(@"
class A
{
    event System.Action B { [|add|] { } remove { } }
}", "A.add_B");
        }

        [Test]
        public static void Remove_accessor_expression()
        {
            AssertFunctionBreakpointName(@"
class A
{
    event System.Action B { add => _ = value; [|remove|] => _ = value; }
}", "A.remove_B");
        }

        [Test]
        public static void Remove_accessor_statement()
        {
            AssertFunctionBreakpointName(@"
class A
{
    event System.Action B { add { } [|remove|] { } }
}", "A.remove_B");
        }

        private static void AssertFunctionBreakpointName(string annotatedSource, string expected)
        {
            Assert.That(GetFunctionBreakpointName(annotatedSource), Is.EqualTo(expected));
        }

        private static string GetFunctionBreakpointName(string annotatedSource)
        {
            var (source, span) = AnnotatedSourceUtils.Parse(annotatedSource, nameof(annotatedSource));

            var factory = FunctionBreakpointUtils.GetFunctionBreakpointNameFactory(
                CSharpSyntaxTree.ParseText(source).GetRoot(),
                span);

            return factory?.ToString();
        }
    }
}
