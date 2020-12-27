using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

using Calculator_Window.CalculatorControl;


namespace Calculator_Window
{
  /// <summary> 

  /// </summary>
  public class CalculatorModel
  {

    #region properties

    /// <summary> Result from last successfully calculated given equation </summary>
    /// <value> Getter for last result  </value>
    public double CurrentResult { get; private set; } = 0.0;
    /// <summary> Integral part of a result </summary>
    /// <value> Getter of the integral part of a result </value>
    /// <example> Returns 22 of the result 22.45646   </example>
    public double IntegerFromCurrentResult => Math.Truncate(this.CurrentResult);
    /// <summary> Fractional part of a result </summary>
    /// <value> Getter of the fractional part of a result </value>
    /// <example> Returns 0.45646 of the result 22.45646 </example>
    public double FractionFromCurrentResult
    {
      get
      {
        if (!this.CurrentResult.ToString().Contains('.'))
        {
          return 0.0;
        }
        else
        {
          return Double.Parse($"0.{this.CurrentResult.ToString().Split('.')[1]}");
        }
      }
    }

    private int maxNumberOfResult = 10;
    /// <summary> 
    /// Maximum number of stored calculations in the property Results 
    /// </summary>
    /// <value> 
    /// Getter/Setter if property is set to number lower than the already number of 
    /// stored calculation then the oldest calculations are removed until number of 
    /// stored calculation is equal to this property
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException"> 
    /// Throws if property is set to value smaller than 1.
    /// </exception>
    public int MaxNumberOfResult
    {
      get => this.maxNumberOfResult;
      set
      {
        const int minValue = 1;

        if (value < minValue)
        {
          const string paramName = nameof(this.MaxNumberOfResult);

          throw new ArgumentOutOfRangeException(
            paramName,
            $"Property {paramName} must not be smaller than {minValue}" 
            );
        }

        this.maxNumberOfResult = value;

        while (this.Results.Count > this.maxNumberOfResult)
        {
          this.Results.RemoveAt(this.Results.Count - 1);
        }
      }
    }

    /// <summary> 
    /// Stores all valid calculations. A calculation is 2 read only properties
    /// The result of an equation and the string of given equation
    /// </summary>
    /// <value> 
    /// Getter only of list of observable calculation.
    /// At index 0 the most recent result is stored and the last element 
    /// holds the oldest equation.
    /// </value>
    public ObservableCollection<EquationCalculation> Results { get; private set; }
      = new ObservableCollection<EquationCalculation>();

    /// <summary> Returns last valid equation given </summary>
    /// <value> 
    /// Getter only of a object of type EquationCalculation. Returns null
    /// if no valid equation has been given so far.
    /// </value>
    public EquationCalculation LastEquation
    {
      get
      {
        if (this.Results.Count == 0)
        {
          return null;
        }

        return this.Results[^1];
      }
    }

    /// <summary> 
    /// Value if radian or degree is used as angle in a calculation 
    /// </summary>
    /// <value> 
    /// Getter/Setter property. Default value is false. 
    /// If true, radians are used or If false, degree is used.
    /// </value>
    public bool UsesRadians { get; set; } = false;
    /// <summary> 
    /// Value if point or comma is used as decimal separator in a number 
    /// </summary>
    /// <value> 
    /// Getter/Setter property. Default value is true. 
    /// if true, point is used or if false comma is used.
    /// </value>
    public bool UsesPointAsDecimalSeperator { get; set; } = true;


    private int roundingPrecision = 15;
    /// <summary> 
    /// Determines to how many digits after the decimal separator 
    /// a result of an equation is rounded up  
    /// </summary>
    /// <value> 
    /// Getter/Setter of digits to round up to. 
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException"> 
    /// Thrown if value is under 0 or above 16
    /// </exception>
    public int RoundingPrecision
    {
      get => this.roundingPrecision;
      set
      {
        const int min = 0;
        const int max = 15;

        if ( value < min || value > max)
        {
          throw new ArgumentOutOfRangeException(
            nameof(RoundingPrecision),
            value,
            $"Value must be between {min} and {max}"
            );
        }

        this.roundingPrecision = value;
      }
    }

    /// <summary> 
    /// Returns result of last validly given calculation with respective 
    /// decimal separator. Either with '.' or ','
    /// </summary>
    /// <value> 
    /// Only getter of result. 
    /// Returns null if no valid equation has been given so far
    /// </value>
    public string LastResult
    {
      get 
      { 
        string result = null;

        if (this.Results.Count != 0)
        {
          result = this.Results[0].Result;
        }

        return result;
      }
    }

    #endregion

    #region public methods

    /// <summary> 
    /// Resets the history. After execution does not contain any 
    /// previous equations anymore and CurrentResult is zero.
    /// </summary>
    public void ClearHistory()
    {
      this.CurrentResult = 0.0;
      this.Results.Clear();
    }

    /// <summary> Calculates the faculty of a number </summary>
    /// <param name="number"> number to use as base for the calculation </param>
    /// <returns> Result of a faculty calculation </returns>
    /// <example> 
    /// double result = CalculateModel.CalculateFaculty(3);
    /// // result = 3!
    /// // result = 6
    /// </example>
    public static int CalculateFaculty(int number)
    {
      var result = 1;
      bool numberIsNegative = number < 0;

      if (numberIsNegative)
      {
        number *= -1;
      }

      for (int i = number; i > 0; i-- )
      {
        result *= i;
      }

      return numberIsNegative ? result * -1 : result;
    }

    /// <summary> Returns conversion from degree to radians </summary>
    /// <param name="degree"> Degree to be converted </param>
    /// <returns> Returns radians as angle </returns>
    public static double DegreeToRadians(double degree)
      => (Math.PI / 180) * degree;

    /// <summary> Returns conversion from radians to degree </summary>
    /// <param name="radians"> Radians to be converted </param>
    /// <returns> Returns degree as angle </returns>
    public static double RadiansToDegree(double radians)
      => (180 / Math.PI) * radians;

    /// <summary> 
    /// Takes a string as an equation and returns a numeric values as the result
    /// of this equation.
    /// </summary>
    /// <param name="inputForCalc"> 
    /// String as the equation to be calculated from 
    /// </param>
    /// <returns> Numeric value of the equation </returns>
    /// <exception cref="OverflowException"> 
    /// Thrown if one operand or a result of an operation 
    /// is too big for a double value
    /// </exception>
    /// <exception cref="DivideByZeroException"> 
    /// Thrown if one operand is a denominator and zero
    /// </exception>
    /// <exception cref="CalculationParseException"> 
    /// If the parameter inputForCalc is an invalid equation 
    /// </exception>
    public double CalculateFromText(string inputForCalc)
    {

      if (inputForCalc == null)
      {
        throw new ArgumentNullException(
          nameof(inputForCalc) ,"Parameter must not be null" 
          );
      }
      else if (inputForCalc == String.Empty)
      {
        throw new ArgumentException(
          "Equation must not be empty", nameof(inputForCalc)
          );
      }

      string textForm = inputForCalc.Trim();
      string equation = textForm;

      try
      {

        if (!this.UsesPointAsDecimalSeperator)
        {
          textForm = textForm.Replace(",", ".");
        }

        this.CurrentResult = Math.Round(
          this.ProcessTextTerm(ref textForm), this.RoundingPrecision
          );
      }
      catch (OverflowException e)
      {
        throw e;
      }
      catch (DivideByZeroException e)
      {
        throw e;
      }
      catch (CalculationParseSyntaxException e)
      {
        throw e;
      }
      catch (CalculationParseMathematicalException e)
      {
        throw e;
      }

      string resultToBeStored = this.CurrentResult.ToString();

      if (!this.UsesPointAsDecimalSeperator)
      {
        resultToBeStored = resultToBeStored.Replace(".", ",");
      }

      this.Results.Insert(
        0, new EquationCalculation(resultToBeStored, equation)
        );

      if (this.Results.Count > this.MaxNumberOfResult)
      {
        this.Results.RemoveAt(this.Results.Count - 1);
      }

      return this.CurrentResult;
    }

    #endregion

    #region regular expressions and pattern for the grammar of an equation

    protected const string signSequence = @"(?<signSequence>[+-]*)";
    protected const string floatingNumber =
      signSequence +
      @"(?<floatingNumber>(\d+)(?<fractionalPartOfNumber>[\.,](\d+))?)";
    private const string piSings = @"(?<constSigns>(pi|π|e)+)";
    private const string floatingNumberPiSings = floatingNumber + piSings;
    private const string surroundedOperatorUnit =
      @"(?<surroundedOperator>[\^ER√])";
    private const string operandPart =
      "(" + signSequence + piSings + "|" + floatingNumber + piSings + "?" + ")" +
      surroundedOperatorUnit + @"?";

    // Regular expression for matching a valid operand as a text unit
    protected static readonly Regex operationOneOperandRegex =
      new Regex(
        @"^\s*(?<function>(?<operandFunctionBaseNeeded>log)|" +
        @"(?<operandFunction>cotan|cosin|cocos|tan|sin|cos|ln))"
      );
    protected static readonly Regex surroundedOperatorPattern =
      new Regex(@"^" + surroundedOperatorUnit);
    protected static readonly Regex operandPartPattern =
      new Regex(@"^" + operandPart);
    protected static readonly Regex spaceOperandPartPattern =
      new Regex(@"^\s*" + operandPart);
    protected static readonly Regex operandAsIntegerFunctionPattern =
      new Regex(@"^((?<signSequence>[+-]*)(?<floatingNumber>\d+)!)");
    // Regular expression for matching a valid whole number for a factor of a power
    protected static readonly Regex floatingNumberPattern =
      new Regex(@"^" + floatingNumber);
    // Used to find cases for root operations with no left factor
    // Example: √9, which is √9 = 3
    protected static readonly Regex operatorWithoutNeededLeftPattern
      = new Regex(@"\s*(?<surroundedOperator>[R√])");
    // Regular expression for matching a valid operator as a text unit.
    protected static readonly Regex operatorPattern =
      new Regex(@"^\s*((?<sign>[+\-])|(?<prioritySign>[*/%]))");
    // Regular expression for matching a valid text unit as an opening parentheses
    protected static readonly Regex whiteSpaceOpeningParathesePattern =
      new Regex(@"^\s*\(");
    // Regular expression for matching a valid text unit as an closing parentheses
    // with no whitespace before.
    protected static readonly Regex openingParathesePattern =
      new Regex(@"^\(");
    // Regular expression for matching a valid text unit as an closing parentheses
    protected static readonly Regex whiteSpaceclosingParathesePattern =
      new Regex(@"^\s*\)");
   
    #endregion

    private const string OverflowOperationErrorMsg =
      "Mathematical Error: one operation resulted in a too big or small number !";

    // Equation as a string is parsed. An equation string is considered to be made 
    // of text units. While parsing, operands and operators are extracted 
    // from the equation string. Every extraction is a text unit which is then removed
    // from the equation text. These extractions are put in lists
    // for later calculation to return a numeric result. 
    private double ProcessTextTerm(
      ref string textTerm, bool looksCloseParanthese = false
      )
    {     
      // If true, next text unit must be a valid operand
      // else next text unit must be a valid operator
      var expectsOpperand = true;      
      var lastOperandsWasPrio = false;
      // if true, a closing parentheses was found 
      // during the parsing of the equation string       
      var foundClosingParanthese = false;
      // List of all extracted operands for the calculation
      var operands = new List<double>();
      // List of all extracted operators for the calculation
      var operators = new List<string>();
      // Priority operators are used for priority calculation
      // Priority calculation is are performed before normal
      // calculation. For example multiplication before adding numbers 
      // List of all positions in operands where sub-results 
      // from priority operations, needed for calculation.
      var prioStartIndexes = new List<int>();
      // Lists of lists which contain operands for priority calculation
      var operandsPrio = new List< List<double> >();
      // List of lists which contain all priority operators
      var operatorsPrio = new List< List<string> >();
      List<double> currentOperandsPrio = null;
      List<string> currentOperatorsPrio = null;      

      // Match for next valid text unit
      Match currentMatch;

      // Parsing equation and extracting text units for calculation
      while (textTerm != String.Empty)
      {        
        // Checks if the next equation unit is ')' in case the current method
        // stack was created because of '(' as equation unit.
        if (looksCloseParanthese)
        {
          currentMatch = whiteSpaceclosingParathesePattern.Match(textTerm);
          
          if (currentMatch.Success)
          {
            // After ')' was found, method stack proceeds to the calculation
            // to return its sub-result to method stack 
            // which invoked this method stack
            textTerm = MoveToNextTextPart(textTerm, currentMatch);
            foundClosingParanthese = true;
            break;
          }
        }

        currentMatch = whiteSpaceOpeningParathesePattern.Match(textTerm);

        if (currentMatch.Success)
        {
          textTerm = MoveToNextTextPart(textTerm, currentMatch);
          // Getting result from inner parentheses.
          double subResult = this.ProcessTextTerm(ref textTerm, true);

          currentMatch = surroundedOperatorPattern.Match(textTerm);

          if (currentMatch.Success)
          {
            textTerm = MoveToNextTextPart(textTerm, currentMatch);
            
            subResult = ProcessSurroundedWithRightSide(
              ref textTerm, 
              currentMatch.Groups["surroundedOperator"].Value,
              subResult
              );
          }

          if (!expectsOpperand)
          {            
            DigestPrioOperator("*");
          }

          AddOperand(subResult);

          expectsOpperand = false;          
        }
        else if (expectsOpperand)
        {          
          
          // Checking for encounter of operandAsFunctionWithBase like log
          currentMatch = operationOneOperandRegex.Match(textTerm);
          expectsOpperand = false;
          var number = 0.0;

          if (currentMatch.Success)
          {
            textTerm = MoveToNextTextPart(textTerm, currentMatch);
            var numberInParanthese = 0.0;
            var baseNumber = 0.0;
            string operandOperator =
              currentMatch.Groups["function"].Value;


            if (currentMatch.Groups["operandFunctionBaseNeeded"].Success)
            {
              if ((currentMatch = operandPartPattern.Match(textTerm)).Success)
              {
                baseNumber = ProcessOneOperand(ref textTerm);
              }
              else
              {
                ThrowSyntaxError(
                  SyntaxError.MissingBaseForFunction, operandOperator
                  );
              }
            }

            if (
              (currentMatch = openingParathesePattern.Match(textTerm)).Success
              )
            {
              textTerm = MoveToNextTextPart(textTerm, currentMatch);
              numberInParanthese = this.ProcessTextTerm(ref textTerm, true);
            }
            else
            {
              ThrowSyntaxError(
                SyntaxError.MissingParantheseParamForFunction,
                operandOperator
                );
            }

            number = CalculateFunctionOperand(
              operandOperator, baseNumber, numberInParanthese
              );

            AddOperand(number);
          }
          else if (
            (currentMatch = operandAsIntegerFunctionPattern.Match(textTerm))
            .Success
            )
          {
            textTerm = MoveToNextTextPart(textTerm, currentMatch);
            number = GetNumericOperandFromMatch(currentMatch);
            AddOperand(CalculateFaculty( (int)number ));
          }
          else if (
            (currentMatch = spaceOperandPartPattern.Match(textTerm)).Success
            )
          {
            // Handling operand like 24^4
            number = ProcessOneOperand(ref textTerm);
            AddOperand(number);
          }
          else if (operatorWithoutNeededLeftPattern.Match(textTerm).Success)
          {
            textTerm = textTerm.Trim();
            textTerm = $"2{textTerm}";
            expectsOpperand = true;
          }
          else
          {
            ThrowSyntaxError(SyntaxError.InvalidOperand);
          }

          double ProcessOneOperand(ref string textTerm)
          {
            double number = 0.0;
            
            try
            {
              if (currentMatch.Groups["floatingNumber"].Success)
              {
                number = GetNumericOperandFromMatch(currentMatch);
              }              
            }
            catch (OverflowException)
            {
              throw new OverflowException(
                "Mathematical Error: One operand is too big for calculation"
                );
            }

            Group constantHit = currentMatch.Groups["constSigns"];

            if (constantHit.Success)
            {
              string constantSequence = constantHit.Value;
              double constantValue = 1.0;
              constantSequence = constantSequence.Replace("pi", "π");

              foreach (char constantSign in constantSequence)
              {
                constantValue *= constantSign switch
                {
                  'π' => Math.PI,
                  'e' => Math.E,
                  _ => throw new ArgumentException(
                          $"Unknown constant sign {constantValue}",
                          nameof(constantSign)
                           ),
                };
              }

              if (constantValue != 1.0)
              {
                if (number == 0.0)
                {
                  number += constantValue;
                }
                else
                {
                  number *= constantValue;
                }
              }

            }

            string surroundedOperator = 
              currentMatch.Groups["surroundedOperator"].Value;
            // Checks if an operand contains a power operation.
            if (surroundedOperator != String.Empty)
            {
              textTerm = MoveToNextTextPart(textTerm, currentMatch);

              number = ProcessSurroundedWithRightSide(
                ref textTerm, 
                surroundedOperator,
                number
                );
            }            
            else
            {
              textTerm = MoveToNextTextPart(textTerm, currentMatch);
            }

            return number;
          }
        }
        else
        {
          currentMatch = operatorPattern.Match(textTerm);

          if (currentMatch.Success)
          {
            string currentOperator = currentMatch.Groups["prioritySign"].Value;

            if (currentOperator != String.Empty)
            {
              DigestPrioOperator(currentOperator);
            }
            else
            {
              lastOperandsWasPrio = false;
              operators.Add(currentMatch.Groups["sign"].Value);
            }
          }
          else
          {
            ThrowSyntaxError(SyntaxError.InvalidOperator);
          }
          
          textTerm = MoveToNextTextPart(textTerm, currentMatch);
          expectsOpperand = true;
        }        
      }

      if (looksCloseParanthese && !foundClosingParanthese)
      {
        ThrowSyntaxError(SyntaxError.MissingClosingParanthese);
      }
      else if (expectsOpperand)
      {
        ThrowSyntaxError(SyntaxError.InvalidOperand);
      }

      // Calculating priority terms aka multiplication before adding numbers ..
      for (int i = 0, count = prioStartIndexes.Count; i < count; i++)
      {
        operands[prioStartIndexes[i]] = ProcessMacroTerm(
          operandsPrio[i], operatorsPrio[i], CalculateOnePrioOperation
          );
      }
      
      // Calculate final result from text units from extractions.
      return ProcessMacroTerm(operands, operators, CalculateOneOperation);

      // Parameter: operandMatch is a match of a valid operand in a string.
      // Returns numeric signed value of an operand
      // Match must have the Group signSequence and floatingNumber
      double GetNumericOperandFromMatch(Match operandMatch)
      {
        string signSequence = operandMatch.Groups["signSequence"].Value;        
        var number = 0.0;

        number = Double.Parse(operandMatch.Groups["floatingNumber"].Value);

        CheckForOverflow(number);
        
        return ProcessPlusMinusSeq(signSequence, number); ;
      }

      // Operand is extracted into 2 parts, sign and its numeric value. 
      // This method returns whole signed operand with these 2 parts.
      // Param signSeq, char sequence made of only '+' or '-'
      // Parma number, numeric value which is should be positive because it is 
      // unsigned yet             
      static double ProcessPlusMinusSeq(string signSeq, double number)
      {
        var currentSign = 1.0;

        foreach (char sign in signSeq)
        {
          if (sign == '-')
          {
            currentSign *= -1.0;
          }
        }

        return currentSign * number;
      }

      // Removes last found text unit from the equation string
      static string MoveToNextTextPart(string text, Match currentMath)
        => text[(currentMath.Index + currentMath.Length)..];
      
      // Makes a calculation based on the extractions of the unit text.
      static double ProcessMacroTerm(
        List<double> operands, 
        List<string> operators, 
        Func<string, double, double, double> calculatorLogic
        )
      {        
        var result = operands[0];
        
        for (int i = 1, j = 0, count = operands.Count; i < count ; i++, j++)
        {
          result = calculatorLogic(operators[j], result, operands[i]);
        }

        return result;
      }
      
      // Takes 2 operands and performs an mathematical operation 
      static double CalculateOneOperation(
        string operatorPart, double firstOperand, double secondOperand
        )
      {
        switch (operatorPart)
        {           
          case "+":
            firstOperand += secondOperand;
            break;
          case "-":
            firstOperand -= secondOperand;
            break;
          default:
            break;
        }

        CheckForOverflow(firstOperand, OverflowOperationErrorMsg);

        return firstOperand;
      }

      // Takes 2 operands and performs an mathematical operation 
      static double CalculateOnePrioOperation(
        string operatorPart, double firstOperand, double secondOperand
      )
      {
        switch (operatorPart)
        {
          case "*":
            firstOperand *= secondOperand;
            break;
          case "/":
            ThrowIfDivededByZero();
            firstOperand /= secondOperand;
            break;
          case "%":
            ThrowIfDivededByZero();
            firstOperand %= secondOperand;
            break;
          default:
            break;
        }

        CheckForOverflow(firstOperand, OverflowOperationErrorMsg);

        return firstOperand;

        void ThrowIfDivededByZero()
        {
          if (secondOperand == 0.0)
          {
            throw new DivideByZeroException(
              $"Mathematical Error: "+ 
              $"One denominator is zero in the operation {operatorPart} !"
              );
          }
        }
      }

      // Places parameter newOperand in the proper list for final calculation.
      void AddOperand(double newOperand)
      {
        if (lastOperandsWasPrio)
        {
          currentOperandsPrio.Add(newOperand);
        }
        else
        {
          operands.Add(newOperand);
        }
      }

      // Integrates priority operand for later calculation
      void DigestPrioOperator(string prioOperator)
      {
        if (!lastOperandsWasPrio)
        {
          operandsPrio.Add(new List<double>());
          currentOperandsPrio = operandsPrio[^1];
          currentOperandsPrio.Add(operands[^1]);

          prioStartIndexes.Add(operands.Count - 1);

          operatorsPrio.Add(new List<string>());
          currentOperatorsPrio = operatorsPrio[^1];
        }

        currentOperatorsPrio.Add(prioOperator);
        lastOperandsWasPrio = true;
      }

      // Checks if double value is infinity and therefore an overflow has happened.
      // Error message is then the message property of the overflow exception
      static void CheckForOverflow(
        double possibleTooBigNbr, string errorMsg = null
        )
      {
        if (Double.IsInfinity(possibleTooBigNbr))
        {
          if (errorMsg == null)
          {
            throw new OverflowException();
          }
          else
          {
            throw new OverflowException(errorMsg);
          }          
        }
      }

      // Calculates number from a surrounded operator like ^ and √
      // A surrounded operator looks like this 2^4
      // Parameter leftSide = 2, operatorSign = ^, rightSide = 4
      static double CalcualteSurroundedOperator(
        double leftSide, double rightSide, string operatorSign
      )
      {
        if (operatorSign == "^" || operatorSign == "E")
        {
          leftSide = Math.Pow(leftSide, rightSide);
          
        }
        else if (operatorSign == "R" || operatorSign == "√")
        {
          if (leftSide == 0.0)
          {
            ThrowMathematicalError(MathematicalError.RootBaseZero);
          }
          else if (rightSide <= 0)
          {
            ThrowMathematicalError(MathematicalError.RootParamZeroOrSmaller);
          }

          leftSide = Math.Pow(rightSide, 1.0 / leftSide);
        }
        

        return leftSide;
      }

      // Calculate an operand as a function like log
      // Example: log2(4)
      // Parameter operandFunction = log, 
      // Parameter firstOperand = 2, 
      // Parameter secondOperand = 4
      double CalculateFunctionOperand(
        string operandFunction, double firstOperand, double secondOperand 
        )
      {
        if (operandFunction == "log")
        {
          if (firstOperand <= 0.0)
          {
            ThrowMathematicalError(MathematicalError.LogBaseZeroOrSmaller);
          }
          else if (secondOperand <= 0.0)
          {
            ThrowMathematicalError(MathematicalError.LogParamZeroOrSmaller);
          }

          return Math.Log(secondOperand, firstOperand);
        }
        else if (operandFunction == "ln")
        {
          if (secondOperand <= 0.0)
          {
            ThrowMathematicalError(MathematicalError.LogParamZeroOrSmaller);
          }

          return Math.Log(secondOperand);
        }
        else if (operandFunction == "tan")
        {
          double degree = this.UsesRadians 
            ? RadiansToDegree(secondOperand) : secondOperand;

          if (degree == 90.0 || degree == 270.0)
          {
            ThrowMathematicalError(MathematicalError.TanInvalidAngle);
          }

          return Math.Tan(
            this.UsesRadians ? secondOperand : DegreeToRadians(secondOperand)
            );
        }
        else if (operandFunction == "sin")
        {
          return Math.Sin(ConvertAngle(secondOperand));
        }
        else if (operandFunction == "cos")
        {
          return Math.Cos(ConvertAngle(secondOperand));
        }
        else if (operandFunction == "cocos")
        {
          ThrowForOverOne();
          return ConvertAngleBack(Math.Acos(secondOperand));
        }
        else if (operandFunction == "cosin")
        {
          ThrowForOverOne();
          return ConvertAngleBack(Math.Asin(secondOperand));
        }
        else if (operandFunction == "cotan")
        {
          return ConvertAngleBack(Math.Atan(secondOperand));
        }
        else
        {
          throw new ArgumentException(
            "No valid function for operator was given !",
            nameof(operandFunction)
            );
        }

        void ThrowForOverOne()
        {
          if (Math.Abs(secondOperand) > 1.0)
          {
            ThrowMathematicalError(MathematicalError.SinCosInvalidAngle);
          }
        }

        double ConvertAngle(double angle)
          => this.UsesRadians ? angle : DegreeToRadians(angle);

        double ConvertAngleBack(double angle)
          => this.UsesRadians ? angle : RadiansToDegree(angle);
      }



      double ProcessSurroundedWithRightSide(
        ref string textTerm, 
        string surroundedOperator,
        double number
        )
      {                
        // Check if factor for power operation is a valid whole number
        Match rightSideOfOperand = floatingNumberPattern.Match(textTerm);

        if (rightSideOfOperand.Success)
        {
          double rightNumber;

          try
          {
            rightNumber = GetNumericOperandFromMatch(rightSideOfOperand);
          }
          catch (OverflowException)
          {
            throw new OverflowException(
              "Mathematical Error: One factor for a power operation is too big !"
              );
          }

          number = CalcualteSurroundedOperator(
            number, rightNumber, surroundedOperator
            );


          if (Double.IsInfinity(number))
          {
            throw new OverflowException(
              "Mathematical Error: one operand is too big" +
              " after raised to a certain power"
              );
          }

          // valid power factor as whole number after "^" is processed
          // and will be now removed.
          currentMatch = rightSideOfOperand;
          textTerm = MoveToNextTextPart(textTerm, currentMatch);
        }
        else
        {
          rightSideOfOperand = openingParathesePattern.Match(textTerm);

          if (rightSideOfOperand.Success)
          {
            textTerm = MoveToNextTextPart(textTerm, rightSideOfOperand);
            double subResult = this.ProcessTextTerm(ref textTerm, true);
            try
            {
              number = CalcualteSurroundedOperator(
                number, subResult, surroundedOperator
                );
            }
            catch (OverflowException)
            {
              throw new OverflowException(
                $"One operand is too big or small" +
                $" after the operation {surroundedOperator}"
                );
            }

          }
          else
          {
            // String after operator is no valid whole number 
            // or term surrounded by ( ) as a power factor 
            ThrowSyntaxError(
              SyntaxError.InvalidSorroundedParamter,
              surroundedOperator
              );
          }
        }

        return number;
      }

    }

    static void ThrowSyntaxError(
      SyntaxError errorType, string errorInfo = ""
      )
    {
      const string prefixForMsg = "Syntax Error: ";
      string errorMsg = prefixForMsg;

      switch (errorType)
      {
        case SyntaxError.InvalidOperand:
          errorMsg += "Encountered invalid operand";
          break;
        case SyntaxError.InvalidOperator:
          errorMsg += "Encountered invalid operator";
          break;
        case SyntaxError.InvalidFloatingNumber:
          errorMsg += "Encountered an invalid fractional part of an operand";
          break;
        case SyntaxError.MissingClosingParanthese:
          errorMsg += "Closing parentheses is missing";
          break;
        case SyntaxError.MissingBaseForFunction:
          errorMsg += 
            $"For function {errorInfo}, no number as base factor is provided";
          break;
        case SyntaxError.MissingParantheseParamForFunction:
          errorMsg += "Missing term as parameter in parentheses";
          break;
        case SyntaxError.InvalidSorroundedParamter:
          errorMsg += $"Invalid parameter for surrounded parameter {errorInfo}";
          break;
        default:
          break;
      }

      throw new CalculationParseSyntaxException(errorMsg, errorType);
    }

    #region Factory methods for parse exceptions

    static void ThrowMathematicalError(MathematicalError errorType)
    {
      const string prefixForMsg = "Mathematical Error: ";
      string errorMsg = prefixForMsg;

      switch (errorType)
      {
        case MathematicalError.RootBaseZero:
          errorMsg += "Left side of a root operation must not be 0 !";
          break;
        case MathematicalError.RootParamZeroOrSmaller:
          errorMsg += "Right side of a root operation must greater than 0 !";
          break;
        case MathematicalError.LogBaseZeroOrSmaller:
          errorMsg += "Base factor of log must be equal or greater than zero !";
          break;
        case MathematicalError.LogParamZeroOrSmaller:
          errorMsg += "Term in parentheses must be equal or greater than zero !";
          break;
        case MathematicalError.TanInvalidAngle:
          errorMsg += "Angle given to tan must not be 90 or 270";
          break;
        case MathematicalError.SinCosInvalidAngle:
          errorMsg += "Angle given to sin or cos must be between -1 and 1";
          break;
        default:
          break;
      }

      throw new CalculationParseMathematicalException(errorMsg, errorType);
    }

    #endregion

  }
}
