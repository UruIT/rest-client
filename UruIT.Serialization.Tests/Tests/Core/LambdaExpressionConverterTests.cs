using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UruIT.Serialization.Utilities;

namespace UruIT.Serialization.Tests
{
    [TestClass]
    public class LambdaExpressionConverterTests
    {
        protected IJsonSerializer jsonSerializer;

        public LambdaExpressionConverterTests()
        {
            jsonSerializer = new JsonSerializer();
            jsonSerializer.Settings.Formatting = Formatting.None;
            jsonSerializer.Settings.Converters.Add(new JsonConverters.LambdaExpressionConverter());
        }

        [TestClass]
        public class Serialization : LambdaExpressionConverterTests
        {
            private void WhenPropertyBasicTypeThenOk<T>()
            {
                // Arrange
                Expression<Func<BasicType<T>, T>> expr = (x => x.ValueProp);

                // Act
                string json = jsonSerializer.SerializeObject(expr);

                // Assert
                string expected = "{" + string.Format("\"ExpFieldName\":\"{0}\",\"ExpFieldType\":\"{1}\",\"ExpFieldDeclaringType\":\"{2}\"",
                    "ValueProp", typeof(T).AssemblyQualifiedName, typeof(BasicType<T>).AssemblyQualifiedName) + "}";

                Assert.AreEqual(expected, json);
            }

            private void WhenPropertyComplexTypeThenOk<T>()
            {
                // Arrange
                Expression<Func<ComplexType<T>, BasicType<T>>> expr = (x => x.ValueProp);

                // Act
                string json = jsonSerializer.SerializeObject(expr);

                // Assert
                string expected = "{" + string.Format("\"ExpFieldName\":\"{0}\",\"ExpFieldType\":\"{1}\",\"ExpFieldDeclaringType\":\"{2}\"",
                    "ValueProp", typeof(BasicType<T>).AssemblyQualifiedName, typeof(ComplexType<T>).AssemblyQualifiedName) + "}";

                Assert.AreEqual(expected, json);
            }

            [TestMethod]
            public void WhenPropertyBasicTypeThenOk()
            {
                WhenPropertyBasicTypeThenOk<int>();
                WhenPropertyBasicTypeThenOk<uint>();
                WhenPropertyBasicTypeThenOk<short>();
                WhenPropertyBasicTypeThenOk<long>();
                WhenPropertyBasicTypeThenOk<double>();
                WhenPropertyBasicTypeThenOk<decimal>();
                WhenPropertyBasicTypeThenOk<DateTime>();
                WhenPropertyBasicTypeThenOk<string>();
            }

            [TestMethod]
            public void WhenPropertyComplexTypeThenOk()
            {
                WhenPropertyComplexTypeThenOk<int>();
                WhenPropertyComplexTypeThenOk<uint>();
                WhenPropertyComplexTypeThenOk<short>();
                WhenPropertyComplexTypeThenOk<long>();
                WhenPropertyComplexTypeThenOk<double>();
                WhenPropertyComplexTypeThenOk<decimal>();
                WhenPropertyComplexTypeThenOk<DateTime>();
                WhenPropertyComplexTypeThenOk<string>();
            }

            [TestMethod]
            public void WhenPropertySubClassThenOk()
            {
                // Arrange
                Expression<Func<BasicSubType, int>> expr = (x => x.ValueProp);

                // Act
                string json = jsonSerializer.SerializeObject(expr);

                // Assert
                string expected = "{" + string.Format("\"ExpFieldName\":\"{0}\",\"ExpFieldType\":\"{1}\",\"ExpFieldDeclaringType\":\"{2}\"",
                    "ValueProp", typeof(int).AssemblyQualifiedName, typeof(BasicSubType).AssemblyQualifiedName) + "}";

                Assert.AreEqual(expected, json);
            }

            [TestMethod]
            public void WhenNullThenOk()
            {
                // Act
                string json = jsonSerializer.SerializeObject(null);

                // Assert
                Assert.AreEqual("null", json);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException), "The expression should be a member access")]
            public void WhenExpressionIncompatibleThenError()
            {
                // Arrange
                ParameterExpression paramExpr = Expression.Parameter(typeof(int), "arg");

                // arg => arg + 1
                LambdaExpression lambdaExpr = Expression.Lambda(Expression.Add(paramExpr, Expression.Constant(1)),
                    new List<ParameterExpression>() { paramExpr }
                );

                // Act
                string json = jsonSerializer.SerializeObject(lambdaExpr);
            }

            [TestMethod]
            public void WhenAnotherTypeThenDoesntExecute()
            {
                // Arrange
                int data = 10;

                // Act
                string json = jsonSerializer.SerializeObject(data);

                // Assert
                Assert.AreEqual("10", json);
            }
        }

        [TestClass]
        public class Deserialization : LambdaExpressionConverterTests
        {
            private static ExpressionEqualityComparer expressionComparer = new ExpressionEqualityComparer();

            private void WhenPropertyBasicTypeThenOk<T>()
            {
                // Arrange
                string json = "{" + string.Format("\"ExpFieldName\":\"{0}\",\"ExpFieldType\":\"{1}\",\"ExpFieldDeclaringType\":\"{2}\"",
                    "ValueProp", typeof(T).AssemblyQualifiedName, typeof(BasicType<T>).AssemblyQualifiedName) + "}";

                Expression<Func<BasicType<T>, T>> expected = (x => x.ValueProp);

                // Act
                var result = jsonSerializer.DeserializeObject<Expression<Func<BasicType<T>, T>>>(json);

                // Assert
                Assert.IsTrue(expressionComparer.Equals(expected, result));
            }

            private void WhenPropertyComplexTypeThenOk<T>()
            {
                // Arrange
                string json = "{" + string.Format("\"ExpFieldName\":\"{0}\",\"ExpFieldType\":\"{1}\",\"ExpFieldDeclaringType\":\"{2}\"",
                    "ValueProp", typeof(BasicType<T>).AssemblyQualifiedName, typeof(ComplexType<T>).AssemblyQualifiedName) + "}";

                Expression<Func<ComplexType<T>, BasicType<T>>> expected = (x => x.ValueProp);

                // Act
                var result = jsonSerializer.DeserializeObject<Expression<Func<ComplexType<T>, BasicType<T>>>>(json);

                // Assert
                Assert.IsTrue(expressionComparer.Equals(expected, result));
            }

            [TestMethod]
            public void WhenPropertyBasicTypeThenOk()
            {
                WhenPropertyBasicTypeThenOk<int>();
                WhenPropertyBasicTypeThenOk<uint>();
                WhenPropertyBasicTypeThenOk<short>();
                WhenPropertyBasicTypeThenOk<long>();
                WhenPropertyBasicTypeThenOk<double>();
                WhenPropertyBasicTypeThenOk<decimal>();
                WhenPropertyBasicTypeThenOk<DateTime>();
                WhenPropertyBasicTypeThenOk<string>();
            }

            [TestMethod]
            public void WhenPropertyComplexTypeThenOk()
            {
                WhenPropertyComplexTypeThenOk<int>();
                WhenPropertyComplexTypeThenOk<uint>();
                WhenPropertyComplexTypeThenOk<short>();
                WhenPropertyComplexTypeThenOk<long>();
                WhenPropertyComplexTypeThenOk<double>();
                WhenPropertyComplexTypeThenOk<decimal>();
                WhenPropertyComplexTypeThenOk<DateTime>();
                WhenPropertyComplexTypeThenOk<string>();
            }

            [TestMethod]
            public void WhenPropertySubclassThenOk()
            {
                // Arrange
                string json = "{" + string.Format("\"ExpFieldName\":\"{0}\",\"ExpFieldType\":\"{1}\",\"ExpFieldDeclaringType\":\"{2}\"",
                    "ValueProp", typeof(int).AssemblyQualifiedName, typeof(BasicSubType).AssemblyQualifiedName) + "}";

                Expression<Func<BasicSubType, int>> expected = (x => x.ValueProp);

                // Act
                var result = jsonSerializer.DeserializeObject<Expression<Func<BasicSubType, int>>>(json);

                // Assert
                Assert.IsTrue(expressionComparer.Equals(expected, result));
            }

            [TestMethod]
            public void WhenNullThenOk()
            {
                // Act
                var result = jsonSerializer.DeserializeObject<LambdaExpression>("null");

                // Assert
                Assert.IsNull(result);
            }

            [TestMethod]
            public void WhenAnotherTypeThenDoesntExecute()
            {
                // Act
                var result = jsonSerializer.DeserializeObject<int>("10");

                // Assert
                Assert.AreEqual(10, result);
            }
        }

        public class BasicType<T>
        {
            public T ValueProp { get; set; }
        }

        public class ComplexType<T>
        {
            public BasicType<T> ValueProp { get; set; }
        }

        public class BasicSubType : BasicType<int>
        {
        }
    }
}