using System;
using Microsoft.CodeAnalysis.Text;

namespace CopyFunctionBreakpointName.Tests
{
    internal static class AnnotatedSourceUtils
    {
        public const string AnnotationStartMarker = "[|";
        public const string AnnotationEndMarker = "|]";

        public static (string source, TextSpan span) Parse(string annotatedSource, string paramName)
        {
            if (!TryParse(annotatedSource, out var source, out var span))
            {
                throw new ArgumentException(
                    "The source must be annotated with \"" + AnnotationStartMarker + "\" and \"" + AnnotationEndMarker + "\" around the selected text.",
                    paramName);
            }

            return (source, span);
        }

        public static bool TryParse(string annotatedSource, out string unannotatedSource, out TextSpan span)
        {
            if (TrySingleIndexOf(annotatedSource, AnnotationStartMarker, out var annotationStart)
                && TrySingleIndexOf(annotatedSource, AnnotationEndMarker, out var annotationEnd))
            {
                var innerSubstringStart = annotationStart + AnnotationStartMarker.Length;
                var innerSubstringLength = annotationEnd - innerSubstringStart;

                if (innerSubstringLength >= 0)
                {
                    unannotatedSource =
                        annotatedSource.Substring(0, annotationStart)
                        + annotatedSource.Substring(innerSubstringStart, innerSubstringLength)
                        + annotatedSource.Substring(annotationEnd + AnnotationEndMarker.Length);

                    span = new TextSpan(annotationStart, innerSubstringLength);

                    return true;
                }
            }

            unannotatedSource = null;
            span = default;
            return false;
        }

        private static bool TrySingleIndexOf(string str, string value, out int index)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (value == null) throw new ArgumentNullException(nameof(value));

            index = str.IndexOf(value, StringComparison.Ordinal);
            if (index == -1) return false;

            if (str.IndexOf(value, index + value.Length, StringComparison.Ordinal) == -1) return true;

            index = -1;
            return false;
        }
    }
}
