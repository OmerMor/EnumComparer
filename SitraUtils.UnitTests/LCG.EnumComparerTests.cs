using System.Collections.Generic;
using SitraUtils.UnitTests;

namespace SitraUtils.LCG.UnitTests
{
    public class LcgEnumComparerTests : BaseEnumComparerTests
    {
        protected override IEqualityComparer<TEnum> getEnumComparer<TEnum>()
        {
            return EnumComparer.For<TEnum>();
        }
    }
}