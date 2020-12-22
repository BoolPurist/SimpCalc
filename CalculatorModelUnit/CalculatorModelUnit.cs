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
    [MemberData(nameof(EquationWithDenominatorAsZero))]
    public void CalculateFromText_ShouldThrowExceptionZeroDivition(
      string invalidEquation
      )
    {
      var calculator = new CalculatorModel();
      Assert.Throws<DivideByZeroException>(
          () => calculator.CalculateFromText(invalidEquation)
        );
    }

    [Theory]
    [MemberData(nameof(EquationWithTooBigNumbers))]
    public void CalculateFromText_ShouldThrowExceptionForTooBigNbr(
      string invalidEquation
      )
    {
      var calculator = new CalculatorModel();
      Assert.Throws<OverflowException>(
          () => calculator.CalculateFromText(invalidEquation)
        );
    }

    [Theory]
    [MemberData(nameof(InvalidEquations))]
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

    [Theory]
    [MemberData(nameof(CalculationForInteger))]
    public void CalculateFromText_ShouldReturnIntegralPart(
      string text,
      double expectedResult      
      )
    {
      var calculator = new CalculatorModel();
      calculator.CalculateFromText(text);      
      Assert.Equal(expectedResult, calculator.IntegerFromCurrentResult);
    }

    [Theory]
    [MemberData(nameof(CalculationForFragtion))]
    public void CalculateFromText_ShouldReturnFractionalPart(
      string text,
      double expectedResult
      )
    {
      var calculator = new CalculatorModel();
      calculator.CalculateFromText(text);
      Assert.Equal(expectedResult, calculator.FractionFromCurrentResult);
    }

    [Theory]
    [MemberData(nameof(FacultyCalculation))]
    public void CalculateFaculty_ShouldReturnExactResult(
      int parameter, 
      int expectedResult
      )
    {      
      Assert.Equal(expectedResult, CalculatorModel.CalculateFaculty(parameter));
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
        {
          "(1)",
          1.0
        },
        {
          "2(4)",
          8.0
        },
        {
          "2-(4)*2",
          -6.0
        },
        {
          "2 * (2 - 4) - 24",
          -28.0
        },
        {
          "2( 2/ (8*4) +2)-89",
          -84.875
        },                {
          "2*( 2/ (8*4) +2)-89",
          -84.875
        },
        {
          "2  +  ( 2/ (8*4) +2)-89",
          -84.9375
        },
        // Testing functionality for power        
        {
          "-5^0",
          1.0
        },
        {
          "2^4",
          16.0
        },
        {
          "2^-1",
          0.5
        },
        {
          "5^--1",
          5
        },
        {
          "4^(2-1)",
          4.0
        },
        {
          "2^(2(2-1)+(10+8))",
          1048576.0
        },
        {
          "2R9",
          3.0
        },
        {
          "4R16",
          2.0
        },
        {
          "3√27",
          3.0
        },
        {
          "3R(26+(3/3))",
          3.0
        },
        {
          "2√16",
          4.0
        },
        {
          "R16",
          4.0
        },
        {
          "12 + R16",
          16.0
        },
        // Testing modular calculation
        {
          "2-(5%4+2)*2",
          -4.0
        },
        {
          "4%(2+1)",
          1.0
        },
        // Testing Logarithm
        {
          "log10(10)",
          1.0
        },
        {
          "log2(8)",
          3.0
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

    public static TheoryData<string, double> CalculationForInteger
     => new TheoryData<string, double>()
      {
        {
          "0",
          0.0
        },
        {
          "24",
          24.0
        },
        { 
          "0.45",
          0.0
        },
        {
          "78.04",
          78.0
        }
      };

    public static TheoryData<string, double> CalculationForFragtion
      => new TheoryData<string, double>()
      {
        {
          "0",
          0.0
        },
        {
          "24",
          0.0
        },
        {
          "0.45",
          0.45
        },
        {
          "78.04",
          0.04
        }
      };

    public static TheoryData<string> InvalidEquations
      => new TheoryData<string>()
      {
        {
          "+"
        },
        {
          "25.. + 5"
        },
        {
          "24 +"
        },
        {
          "24 + 2x5"
        },
        {
          "24. + 25.25"
        },        
        {
          "2 * (2 - 4)24"
        },
        {
          "2 * (2 - 4 + 24"
        },
        {
          "2 * (2 - ( 4 ) + 24"
        },
        {
          "2 * )2 - 4 + 24"
        },
        {
          "25 + ^-25"
        },
        {
          "0R24"
        },
        {
          "4R0"
        },
        {
          "4R-8"
        },
        // Testing syntax errors for logarithm
        {
          "log(2)"
        },
        {
          "log2"
        },
        {
          "log2 (2)"
        },
        // Testing mathematical errors for logarithm
        {
          "log-2(2)"
        },
        {
          "log0(2)"
        },
        {
          "log2(-2)"
        },
        {
          "log2(0)"
        }
      };

    public static TheoryData<string> EquationWithDenominatorAsZero
      => new TheoryData<string>()
      {
        "2/0",
        "2(2/(4-2*2))",                
        "8%0"        
      };

    private const string tooBigNumber = 
        "10000000000000000000000000000000000000000000000000000000000000000000000" +
        "00000000000000000000000000000000000000000000000000000000000000000000000" +
        "00000000000000000000000000000000000000000000000000000000000000000000000" +
        "0000000000000000000000000000000000000000000000000000000000000000000000000" +
        "0000000000000000000000000000000000000000000000000000000000000000000000000" +
        "000000000000000";

    public static TheoryData<string> EquationWithTooBigNumbers
      => new TheoryData<string>()
      {

        "456^4564654654654654654654",
        tooBigNumber,
        $"2^{tooBigNumber}",
        "465465465465454665446546546546545664654654654654546654465465465465456646" +
        "5465465465454665446546546546545664654654654654546654465465465465456646546" +
        "54654654546654465465465465456646546546546545466544654654654654566" +
        "*" +
        "465465465465454665446546546546545664654654654654546654465465465465456" +
        "6465465465465454665446546546546545664654654654654546654465465465465456666"
      };

    public static TheoryData<int,int> FacultyCalculation
      => new TheoryData<int,int>()
      {
        {
          0,
          1
        },
        {
          1,
          1
        },
        {
          2,
          2
        },
        {
          5,
          120
        },
        {
          -4,
          -24
        },
      };

  }
}
