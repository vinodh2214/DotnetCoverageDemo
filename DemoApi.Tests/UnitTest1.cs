using Xunit;
using DemoApi;

namespace DemoApi.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Add_ShouldReturnCorrectSum()
        {
            var calc = new Calculator();

            var result = calc.Add(2, 3);

            Assert.Equal(5, result);
        }
    }
}