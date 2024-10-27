namespace OLT.Utility.AssemblyScanner.Tests
{
    public class AssemblyScanBuilderTests
    {
        [Fact]
        public void NoFilters_ShouldBeEmty()
        {
            var result = new OltAssemblyScanBuilder()
                .DeepScan()
                .LoadAssemblies()
                .Build();

            Assert.Empty(result);
        }

      
     
    }
}