using Other.Assembly;
using Test.ExcludeAssembly;

namespace Test.AssemblyOne
{
    public class ClassOne 
    {
        private Blog _blog = new Blog();
        private OtherClass _otherClass = new OtherClass();

    }

    public class AutoMapperClass : AutoMapper.Profile
    {
        public AutoMapperClass()
        {
            
        }
    }
}
