using System;
using Xunit;

using Calculator_Window;

namespace CalculatorModelUnit
{
  public class CalculatorModelUnit
  {
    [Fact]
    public void CalculateFromText_ShouldThrowExceptionForNull()
    {
      var calculator = new CalculatorModel();
      Assert.Throws<ArgumentNullException>(
        () => calculator.CalculateFromText(null)
        );
    }

    [Fact]
    public void CalculateFromText_ShouldThrowExceptionForEmptyString()
    {
      var calculator = new CalculatorModel();
      Assert.Throws<ArgumentException>(
        () => calculator.CalculateFromText(String.Empty)
        );
    }

    [Theory]
    [MemberData(nameof(InvalidEqautions))]
    public void CalculateFromText_ShouldThrowExceptionForInvalidEquation(
      string invalidEquation
      )
    {
      var calculator = new CalculatorModel();
      Assert.Throws<CalculationParseException>(
        () => calculator.CalculateFromText(invalidEquation)
        );
    }

    [Theory]
    [MemberData(nameof(BasicCalculation))]
    public void CalculateFromText_ShouldReturnExactResult(
      string text, 
      double expectedResult
      )
    {
      var calculator = new CalculatorModel();
      double actualResult = calculator.CalculateFromText(text);
      Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [MemberData(nameof(CalculationWithRounding))]
    public void CalculateFromText_ShouldReturnRoundedResult(
      string text,
      double expectedResult,
      int fractionalDigits
      )
    {
      var calculator = new CalculatorModel();
      double actualResult = calculator.CalculateFromText(text);
      actualResult = Math.Round(actualResult, fractionalDigits);
      Assert.Equal(expectedResult, actualResult);
    }
    // 1. test element as string = equation as a text
    // 2. test element as double = expected result from equation
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
        },        
        {                
          "800",
          800.0
        },
        {
          "---800 + --50",
          -750.0
        },
        {
          "2 * 4 + 8 * 16",
          136.0
        },
        {
          "2 + 2 * 4 - 8 * 16 - 45",
          -163.0
        },
        {
          "2 / 4",
          0.5
        },

      };

    public static TheoryData<string, double, int> CalculationWithRounding
      => new TheoryData<string, double, int>()
        {
          {
            "16.5 * -14.2",
            -234.3,
            1
          }
        };

    public static TheoryData<string> InvalidEqautions
      => new TheoryData<string>()
      {
        {
          "+"
        },
        {
          "24 +"
        },
        {
          "24 + 2x5"
        },
        {
          "24. + 25.25"
        }
      };

  }
}
