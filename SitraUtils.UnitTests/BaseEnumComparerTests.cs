using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;

namespace SitraUtils.UnitTests
{
    [TestFixture]
    public abstract class BaseEnumComparerTests
    {
        [Test]
        public void Equals_should_return_false_for_different_values()
        {
            var enumComparer = getEnumComparer<DayOfWeek>();
            const DayOfWeek left = DayOfWeek.Monday;
            const DayOfWeek right = DayOfWeek.Friday;
            var result = enumComparer.Equals(left, right);

            Assert.AreEqual(left == right, result);
        }

        protected abstract IEqualityComparer<TEnum> getEnumComparer<TEnum>()
            where TEnum : struct, IComparable, IConvertible, IFormattable;

        [Test]
        public void Equals_should_return_true_for_matching_values()
        {
            var enumComparer = getEnumComparer<DayOfWeek>();
            const DayOfWeek left = DayOfWeek.Monday;
            const DayOfWeek right = DayOfWeek.Monday;
            var result = enumComparer.Equals(left, right);

            Assert.AreEqual(left == right, result);
        }

        [Test]
        public void GetHashCode_should_return_hashCode_of_literal()
        {
            var enumComparer = getEnumComparer<DayOfWeek>();
            var hashCode = enumComparer.GetHashCode(DayOfWeek.Monday);

            Assert.AreEqual(DayOfWeek.Monday.GetHashCode(), hashCode);
        }

        [Test]
        [ExpectedException(typeof(TypeInitializationException))]
        public void EnumComparer_should_throw_when_using_type_other_than_Enum()
        {
            getEnumComparer<int>();
        }

        [Test]
        [ExpectedException(typeof(TargetInvocationException))]
        public void EnumComparer_should_throw_when_using_enum_with_char_underlying_type()
        {
            var charEnumType = generateCharEnum();
            var invalidEnumComparerType = getEnumComparerType().MakeGenericType(charEnumType);
            var property = invalidEnumComparerType.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
            property.GetValue(null);
        }

        protected Type getEnumComparerType()
        {
            var comparer = getEnumComparer<DayOfWeek>();
            var closedGenericType = comparer.GetType();
            var openGenericType = closedGenericType.GetGenericTypeDefinition();
            return openGenericType;
        }

        [Test]
        [ExpectedException(typeof(TargetInvocationException))]
        public void EnumComparer_should_throw_when_using_enum_with_bool_underlying_type()
        {
            var boolEnumType = generateBoolEnum();
            var invalidEnumComparerType = getEnumComparerType().MakeGenericType(boolEnumType);
            var property = invalidEnumComparerType.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
            property.GetValue(null);
        }

        public Type generateCharEnum()
        {
            var domain = AppDomain.CurrentDomain;
            var assemblyName = new AssemblyName("CharEnum");
            var assembly = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("CharEnum.dll");
            var type = module.DefineEnum("CharEnum", TypeAttributes.Public, typeof(char));
            type.DefineLiteral("LastLetter", 'z');
            return type.CreateType();
        }
        public Type generateBoolEnum()
        {
            var domain = AppDomain.CurrentDomain;
            var assemblyName = new AssemblyName("BoolEnum");
            var assembly = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("BoolEnum.dll");
            var type = module.DefineEnum("BoolEnum", TypeAttributes.Public, typeof(bool));
            type.DefineLiteral("True", true);
            return type.CreateType();
        }
        [Test]
        public void Test_Flagged_Enum()
        {
            var enumComparer = getEnumComparer<ByteEnum>();
            ByteEnum[] edgeValues = { 0, (ByteEnum)byte.MaxValue };
            foreach (var value in edgeValues)
            {
                Assert.AreEqual(value.GetHashCode(), enumComparer.GetHashCode(value));
            }

            foreach (var left in edgeValues)
            {
                foreach (var right in edgeValues)
                {
                    Assert.AreEqual(left == right, enumComparer.Equals(left, right));
                }
            }
        }
        [Test]
        public void Test_Enum_with_underlying_type_of_sbyte()
        {
            var enumComparer = getEnumComparer<SByteEnum>();
            SByteEnum[] edgeValues = { (SByteEnum)sbyte.MinValue, 0, (SByteEnum)sbyte.MaxValue };
            foreach (var value in edgeValues)
            {
                var actual = enumComparer.GetHashCode(value);
                var expected = ((sbyte) value).GetHashCode();
                Assert.AreEqual(expected, actual);
            }

            foreach (var left in edgeValues)
            {
                foreach (var right in edgeValues)
                {
                    Assert.AreEqual(left == right, enumComparer.Equals(left, right));
                }
            }
        }
        [Test]
        public void Test_Enum_with_underlying_type_of_short()
        {
            var enumComparer = getEnumComparer<ShortEnum>();
            ShortEnum[] edgeValues = { (ShortEnum) short.MinValue, 0, (ShortEnum) short.MaxValue };
            foreach (var value in edgeValues)
            {
                Assert.AreEqual(value.GetHashCode(), enumComparer.GetHashCode(value));
            }

            foreach (var left in edgeValues)
            {
                foreach (var right in edgeValues)
                {
                    Assert.AreEqual(left == right, enumComparer.Equals(left, right));
                }
            }
        }
        [Test]
        public void Test_Enum_with_underlying_type_of_ushort()
        {
            var enumComparer = getEnumComparer<UShortEnum>();
            UShortEnum[] edgeValues = {
                                          0, (UShortEnum) 0x1234, (UShortEnum) short.MaxValue,
                                          (UShortEnum) (0x1234 | 0x8000), (UShortEnum) ushort.MaxValue
                                      };
            foreach (var value in edgeValues)
            {
                Assert.AreEqual(value.GetHashCode(), enumComparer.GetHashCode(value));
            }

            foreach (var left in edgeValues)
            {
                foreach (var right in edgeValues)
                {
                    Assert.AreEqual(left == right, enumComparer.Equals(left, right));
                }
            }
        }

        [Test]
        public void Test_Enum_with_underlying_type_of_int()
        {
            var enumComparer = getEnumComparer<IntEnum>();
            IntEnum[] edgeValues = {
                                        (IntEnum) int.MinValue, 0, (IntEnum) int.MaxValue
                                    };
            foreach (var value in edgeValues)
            {
                Assert.AreEqual(value.GetHashCode(), enumComparer.GetHashCode(value));
            }

            foreach (var left in edgeValues)
            {
                foreach (var right in edgeValues)
                {
                    Assert.AreEqual(left == right, enumComparer.Equals(left, right));
                }
            }
        }
        [Test]
        public void Test_Enum_with_underlying_type_of_uint()
        {
            var enumComparer = getEnumComparer<UIntEnum>();
            UIntEnum[] edgeValues = {
                                        0, (UIntEnum) 0x12345678, (UIntEnum) int.MaxValue,
                                        (UIntEnum) (0x12345678 | 0x80000000), (UIntEnum) uint.MaxValue
                                    };
            foreach (var value in edgeValues)
            {
                Assert.AreEqual(value.GetHashCode(), enumComparer.GetHashCode(value));
            }

            foreach (var left in edgeValues)
            {
                foreach (var right in edgeValues)
                {
                    Assert.AreEqual(left == right, enumComparer.Equals(left, right));
                }
            }
        }
        [Test]
        public void Test_Enum_with_underlying_type_of_long()
        {
            var enumComparer = getEnumComparer<LongEnum>();
            LongEnum[] edgeValues = {
                                        (LongEnum) long.MinValue, (LongEnum) int.MinValue, 0, (LongEnum) 0x0000000012345678
                                        , (LongEnum) int.MaxValue, (LongEnum) 0x1234567812345678, (LongEnum) long.MaxValue
                                    };
            foreach (var value in edgeValues)
            {
                Assert.AreEqual(value.GetHashCode(), enumComparer.GetHashCode(value));
            }

            foreach (var left in edgeValues)
            {
                foreach (var right in edgeValues)
                {
                    Assert.AreEqual(left == right, enumComparer.Equals(left, right));
                }
            }
        }
        [Test]
        public void Test_Enum_with_underlying_type_of_ulong()
        {
            var enumComparer = getEnumComparer<ULongEnum>();
            ULongEnum[] edgeValues = { 0, (ULongEnum) int.MaxValue, (ULongEnum) ulong.MaxValue };
            foreach (var value in edgeValues)
            {
                Assert.AreEqual(value.GetHashCode(), enumComparer.GetHashCode(value));
            }

            foreach (var left in edgeValues)
            {
                foreach (var right in edgeValues)
                {
                    Assert.AreEqual(left == right, enumComparer.Equals(left, right));
                }
            }
        }

        [Test]
        public void Test_flagged_enum()
        {
            var enumComparer = getEnumComparer<FlaggedEnum>();
            FlaggedEnum[] edgeValues = {0, FlaggedEnum.One, FlaggedEnum.Two, FlaggedEnum.One | FlaggedEnum.Two};
            foreach (var value in edgeValues)
            {
                Assert.AreEqual(value.GetHashCode(), enumComparer.GetHashCode(value));
            }

            foreach (var left in edgeValues)
            {
                foreach (var right in edgeValues)
                {
                    Assert.AreEqual(left == right, enumComparer.Equals(left, right));
                }
            }
            var map = new Dictionary<FlaggedEnum, int>(enumComparer);
            map[FlaggedEnum.One] = 1;
            map[FlaggedEnum.Two] = 2;
            map[FlaggedEnum.One | FlaggedEnum.Two] = 3;

        }
    }

    #region Enums

    public enum ByteEnum : byte
    {
    }
    public enum SByteEnum : sbyte
    {
    }
    public enum ShortEnum : short
    {
    }
    public enum UShortEnum : ushort
    {
    }
    public enum IntEnum
    {
    }
    public enum UIntEnum : uint
    {
    }
    public enum LongEnum : long
    {
    }
    public enum ULongEnum : ulong
    {
    }
    [Flags]
    public enum FlaggedEnum 
    {
        One = 1,
        Two = 2,
        Four = 4,
        Eight = 8,
    }

    #endregion

}

/*
class EnumComparer<TEnum> : IEqualityComparer<TEnum>
{
    public bool Equals(TEnum x, TEnum y)
    {
        // error CS0019: Operator '==' cannot be applied to operands of type 'TEnum' and 'TEnum'
        return (x == y);
    }
    public int GetHashCode(TEnum obj)
    {
        // error CS0030: Cannot convert type 'TEnum' to 'int'
        return (int)obj;
    }
}
*/