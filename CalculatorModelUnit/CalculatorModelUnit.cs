using System;
using Xunit;

using Calculator_Window;

namespace CalculatorModelUnit
{
  public class CalculatorModelUnit
  {
    [Theory]
    [MemberData(nameof(BasicCalculation))]
    public void CalculateFromText_ShouldReturnRightResult(
      string text, 
      double expectedResult
      )
    {
      var calculator = new CalculatorModel();
      double actualResult = calculator.CalculateFromText(text);
      Assert.Equal(actualResult, expectedResult);
    }

    public static TheoryData<string, double> BasicCalculation
      => new TheoryData<string, double>()
      {
        {
          "2 + 2", 
          4.0
        },
        {                
          "40 - 2 + 58 - 800",
          -704.0
        }
      };
  }
}
