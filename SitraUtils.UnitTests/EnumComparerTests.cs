using System.Collections.Generic;

namespace SitraUtils.UnitTests
{
    public class EnumComparerTests : BaseEnumComparerTests
    {
        protected override IEqualityComparer<TEnum> getEnumComparer<TEnum>()
        {
            return EnumComparer.For<TEnum>();
        }
    }
}