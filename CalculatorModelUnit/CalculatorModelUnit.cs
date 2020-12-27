using System;
using System.Collections.Generic;
using Xunit;

using Calculator_Window;
using Calculator_Window.CalculatorControl;

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
    [MemberData(nameof(EquationWithSyntaxError))]
    public void CalculateFromText_ShouldThrowExceptionForErrorError(
      string invalidEquation, SyntaxError expectedErrorType
      )
    {
      var calculator = new CalculatorModel();
      CalculationParseSyntaxException thrownException = 
        Assert.Throws<CalculationParseSyntaxException>(
          () => calculator.CalculateFromText(invalidEquation)
          );
      SyntaxError actualErrorType = thrownException.SyntaxErrorType;
      Assert.Equal(expectedErrorType, actualErrorType);
    }

    [Theory]
    [MemberData(nameof(EquationWithMathematicalError))]
    public void CalculateFromText_ShouldThrowExceptionForMathematicalError(
      string invalidEquation, MathematicalError expectedErrorType
      )
    {
      var calculator = new CalculatorModel();
      CalculationParseMathematicalException thrownException =
        Assert.Throws<CalculationParseMathematicalException>(
          () => calculator.CalculateFromText(invalidEquation)
          );
      MathematicalError actualErrorType = thrownException.MathematicalErrorType;
      Assert.Equal(expectedErrorType, actualErrorType);
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
    public void IntegerFromCurrentResult_ShouldReturnIntegralPart(
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
    public void FractionFromCurrentResult_ShouldReturnFractionalPart(
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

    // Tests if calculator stores all given equation and the respective results correctly.
    // Tests if the last invalid equation is not stored and the most recent result is at
    // index 0 aka list is reverted.
    [Fact]
    public void Results_ShouldHaveAllCalculatedResults()
    {
      List<EquationCalculation> equations = EquationsToBeStored;
      var calucaltorModel = new CalculatorModel();
      
      foreach (EquationCalculation data in equations)
      {
        calucaltorModel.CalculateFromText(data.Equation);
      }

      for (int i = 0, j = 1, count = equations.Count; i < count; i++, j++)
      {
        // An equation is trimmed before saved in the history in the calculator model.
        string trimedEquation = equations[i].Equation.Trim();

        Assert.Equal(equations[i].Result, calucaltorModel.Results[count - j].Result);
        Assert.Equal(trimedEquation, calucaltorModel.Results[count - j].Equation);
      }

      try
      {
        calucaltorModel.CalculateFromText(null);
      }
      catch (ArgumentNullException)
      {
        // Error provoked to test if invalid equation is not added to history.    
      }

      Assert.Equal(3, calucaltorModel.Results.Count);
    }

    // Tests if property Result of an instance CalculatorModel empty is after the 
    // several invocations of CalculateFromText and a single call of ClearHistory 
    [Fact]
    public void ClearHistory_ShouldHaveNoResultsAfterClearHistory()
    {
      List<EquationCalculation> equations = EquationsToBeStored;
      var calucaltorModel = new CalculatorModel();

      foreach (EquationCalculation data in equations)
      {
        calucaltorModel.CalculateFromText(data.Equation);
      }

      calucaltorModel.ClearHistory();

      Assert.Empty(calucaltorModel.Results);
    }

    // Tests if list only contains the last calculation 
    // after performing 3 calculations and then setting property MaxNumberOfResult to 1.
    [Fact]
    public void MaxNumberOfResults_ShouldNotMoreThanMaxNumber()
    {
      List<EquationCalculation> equations = EquationsToBeStored;
      var calucaltorModel = new CalculatorModel();


      foreach (EquationCalculation data in equations)
      {
        calucaltorModel.CalculateFromText(data.Equation);
      }

      calucaltorModel.MaxNumberOfResult = 1;

      Assert.Single<EquationCalculation>(calucaltorModel.Results);

      Assert.Equal("2", calucaltorModel.Results[0].Result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-85)]
    [InlineData(Int32.MinValue)]
    public void MaxNumberOfResults_ShouldThrowIfNegative(int negativeValue)
    {
      var calculator = new CalculatorModel();
      Assert.Throws<ArgumentOutOfRangeException>(
        () => calculator.MaxNumberOfResult = negativeValue
        );
    }

    [Fact]
    public void CalculateFromText_ShouldCalculateWithRadians()
    {
      var calculator = new CalculatorModel() { UsesRadians = true };
      
      double actualResult = calculator.CalculateFromText("sin(2.0)");

      Assert.Equal(0.91, Math.Round(actualResult, 2));

      actualResult = calculator.CalculateFromText("cotan(0.5)");

      Assert.Equal(0.46, Math.Round(actualResult, 2));
    }

    [Fact]
    public void UsesPointAsDecimalSeperator_ShouldCalculateWithCommas()    
    {
      var calculator = new CalculatorModel() { UsesPointAsDecimalSeperator = false };

      double actualResult = calculator.CalculateFromText("2,6 + 2,5");

      Assert.Equal(5.1, actualResult);

      var expectedCalculation = new EquationCalculation("5,1", "2,6 + 2,5");

      EquationCalculation actualCalculation = calculator.Results[0];

      Assert.Equal(
          expectedCalculation.Result, actualCalculation.Result
        );

      Assert.Equal(
        expectedCalculation.Equation, actualCalculation.Equation
      );
    }

    [Fact]
    public void RoundingPrecision_ShouldReturnResultWithRoundedDigits()
    {
      var calculator = new CalculatorModel();
      calculator.RoundingPrecision = 3;
      double actualResult = calculator.CalculateFromText("1/3");
      Assert.Equal(0.333, actualResult);
    }

    [Theory]
    [InlineData(-80)]
    [InlineData(-1)]
    [InlineData(Int32.MinValue)]
    [InlineData(16)]
    [InlineData(250)]
    [InlineData(Int32.MaxValue)]
    public void RoundingPrecision_ShouldThrowIfNegativeOrOver15(int invalidPrecision)
    {
      var calculator = new CalculatorModel();
      Assert.Throws<ArgumentOutOfRangeException>(
        () => calculator.RoundingPrecision = invalidPrecision
        );
    }

    // Data used for fact tests Results_ShouldHaveAllCalculatedResults, 
    // ClearHistory_ShouldHaveNoResultsAfterClearHistory and 
    // MaxNumberOfResults_ShouldNotMoreThanMaxNumber
    public static List<EquationCalculation> EquationsToBeStored =>
      new List<EquationCalculation>()
      {
        new EquationCalculation("4", "2+2"),
        new EquationCalculation("18", "8*2+2"),
        new EquationCalculation("2", "4-2"),
      };

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
        {
          "(2*2)^4",
          256.0
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
        {
          "3!",
          6.0
        },
        {
          "-4!",
          -24
        },
        {
          "sin(0)",
          0.0
        },
        {
          "sin(90)",
          1.0
        },
        {
          "cos(0)",
          1.0
        },
        {
          "cotan(1)",
          45
        }
      };



    public static TheoryData<string, double, int> CalculationWithRounding
      => new TheoryData<string, double, int>()
        {
          {
            "16.5 * -14.2",
            -234.3,
            1
          },
          {
            "12!",
            479001600.0,
            1
          },
          {
            "tan(30)",
            0.58,
            2
          },
          {
            "sin(80)",
            0.98,
            2
          },
          {
            "sin(-45)",
            -0.71,
            2
          },
          {
            "cos(-190)",
            -0.98,
            2
          },
          {
            "cos(300)",
            0.5,
            1
          },
          {
            "cosin(-0.2)",
            -11.54,
            2
          },
          {
            "cocos(0.6)",
            53.13,
            2
          },
          {
            "cotan(2.5)",
            68.20,
            2
          },
          {
            "cocos(0.5)",
            60.0,
            1
          },
          {
            "cosin(0.5)",
            30,
            1
          },
          {
            "pi",
            3.14,
            2
          },
          {
            "π",
            3.14,
            2
          },
          {
            "2pi",
            6.28,
            2
          },
          {
            "2.5pi",
            7.85,
            2
          },
          {
            "-2.5pi^2",
            61.69,
            2
          },
          {
            "2.5pipiπ",
            77.52,
            2
          },
          {
            "---pi",
            3.14,
            2
          },
          {
            "e^3",
            20.09,
            2
          },
          {
            "(2*4-8)^2+(8+9)R85236",
            1.95,
            2
          },
          {
            "ln(5)",
            1.61,
            2
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
        },
      };

    public static TheoryData<string, MathematicalError> EquationWithMathematicalError
      => new TheoryData<string, MathematicalError>()
      {
        {
          "0R24",
          MathematicalError.RootBaseZero
        },
        {
          "4R0",
          MathematicalError.RootParamZeroOrSmaller
        },
        {
          "4R-8",
          MathematicalError.RootParamZeroOrSmaller
        },
        // Testing logarithm
        {
          "log-2(2)",
          MathematicalError.LogBaseZeroOrSmaller
        },
        {
          "log0(2)",
          MathematicalError.LogBaseZeroOrSmaller
        },
        {
          "log2(-2)",
          MathematicalError.LogParamZeroOrSmaller
        },
        {
          "log2(0)",
          MathematicalError.LogParamZeroOrSmaller
        },
        {
          "tan(90)",
          MathematicalError.TanInvalidAngle
        },
        {
          "tan(270)",
          MathematicalError.TanInvalidAngle
        },
        {
          "cocos(2)",
          MathematicalError.SinCosInvalidAngle
        },
        {
          "cosin(-2)",
          MathematicalError.SinCosInvalidAngle
        }
      };

    public static TheoryData<string, SyntaxError> EquationWithSyntaxError
      => new TheoryData<string, SyntaxError>()
      {
        {
          "+",
          SyntaxError.InvalidOperand
        },
        {
          "25.. + 5",
          SyntaxError.InvalidOperator
        },
        {
          "24 +",
          SyntaxError.InvalidOperand
        },
        {
          "24 + 2a5",
          SyntaxError.InvalidOperator
        },
        {
          "24. + 25.25",
          SyntaxError.InvalidOperator
        },
        {
          "2 * (2 - 4)24",
          SyntaxError.InvalidOperator
        },
        {
          "2 * (2 - 4 + 24",
          SyntaxError.MissingClosingParanthese
        },
        {
          "2 * (2 - ( 4 ) + 24",
          SyntaxError.MissingClosingParanthese
        },
        {
          "2 * )2 - 4 + 24",
          SyntaxError.InvalidOperand
        },
        {
          "25 + ^-25",
          SyntaxError.InvalidOperand
        },
        {
          "12 !",
          SyntaxError.InvalidOperator
        },
        {
          "log(2)",
          SyntaxError.MissingBaseForFunction
        },
        {
          "log2",
          SyntaxError.MissingParantheseParamForFunction
        },
        {
          "log2 (24)",
          SyntaxError.MissingParantheseParamForFunction
        },
        {
          "log 2(5)",
          SyntaxError.MissingBaseForFunction
        },
        {
          "tan()",
          SyntaxError.InvalidOperand
        },
        {
          "tan (80)",
          SyntaxError.MissingParantheseParamForFunction
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

    public static TheoryData<int, int> FacultyCalculation
      => new TheoryData<int, int>()
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
