using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace Calculator_Window
{
  public class CalculatorModel
  {    
    public double CurrentResult { get; private set; } = 0.0;

    public double IntegerFromCurrentResult => Math.Truncate(this.CurrentResult);

    public double FractionFromCurrentResult
     => Double.Parse($"0.{this.CurrentResult.ToString().Split('.')[1]}");

    protected static readonly Regex operrandRegex =
      new Regex(@"^\s*(?<sign>[+-]*)\s*(?<number>(\d+)+([\.,](\d+))?)(?<powerSign>\^)?");
    protected static readonly Regex numberPowerRegex =
      new Regex(@"^(?<sign>[+-]*)(?<number>\d+)+");

    protected static readonly Regex operratorRegex =
      new Regex(@"^\s*(?<operator>[+*\-/])");

    protected static readonly Regex parentheseOpeningRegex =
      new Regex(@"^\s*\(");

    protected static readonly Regex parantheseClosedNoSpaceRegex =
      new Regex(@"^\(");

    protected static readonly Regex parentheseCloseRegex =
      new Regex(@"^\s*\)");

    protected static readonly Regex parentheseCloseAnywereRegex =
      new Regex(@"\s*\)");


    protected static readonly HashSet<string> prioOperands = 
      new HashSet<string>( new string[] { "*", "/" } );
          
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

      try
      {
        string textForm = inputForCalc.Trim();
        this.CurrentResult = this.ProcessTextTerm(ref textForm);
      }
      catch (DivideByZeroException e)
      {
        throw e;
      }
      catch (CalculationParseException e)
      {
        throw e;
      }

      return this.CurrentResult;
    }

    private int _stackCounter = 0;

    private double ProcessTextTerm(
      ref string textTerm, bool looksCloseParanthese = false
      )
    {
      this._stackCounter++;
      var expectsOpperand = true;
      var lastOperandsWasPrio = false;
      var foundClosingParanthese = false;
      var operands = new List<double>();
      var operators = new List<string>();
      var prioStartIndexes = new List<int>();
      var operandsPrio = new List< List<double> >();
      var operatorsPrio = new List< List<string> >();
      List<double> currentOperandsPrio = null;
      List<string> currentOperatorsPrio = null;
      var result = 0.0;

      Match currentMatch;

      // Parsing

      while (textTerm != String.Empty)
      {
        if (looksCloseParanthese)
        {
          currentMatch = parentheseCloseRegex.Match(textTerm);
          
          if (currentMatch.Success)
          {
            textTerm = MoveToNextTextPart(textTerm, currentMatch);
            foundClosingParanthese = true;
            break;
          }
        }

        currentMatch = parentheseOpeningRegex.Match(textTerm);

        if (currentMatch.Success)
        {
          textTerm = MoveToNextTextPart(textTerm, currentMatch);
          // Getting result from inner parentheses.
          double subResult = this.ProcessTextTerm(ref textTerm, true);

          if (!expectsOpperand)
          {            
            DigestPrioOperator("*");
          }

          AddOperand(subResult);

          expectsOpperand = false;          
        }
        else if (expectsOpperand)
        {
          currentMatch = operrandRegex.Match(textTerm);

          if (currentMatch.Success)
          {
            double number = GetNumericOperandFromMatch(currentMatch);

            // Checks if an operand contains a power operation.
            if (currentMatch.Groups["powerSign"].Value == "^")
            {
              textTerm = MoveToNextTextPart(textTerm, currentMatch);

              // Check if factor for power operation is a valid whole number
              Match powerFactor = numberPowerRegex.Match(textTerm);

              if (powerFactor.Success)
              {
                double powerNumber = GetNumericOperandFromMatch(powerFactor);

                number = Math.Pow(number, powerNumber);
                
                // valid power factor as whole number after "^" is processed
                // and will be now removed.
                currentMatch = powerFactor;
                textTerm = MoveToNextTextPart(textTerm, currentMatch);
              }
              else
              {
                powerFactor = parantheseClosedNoSpaceRegex.Match(textTerm);

                if (powerFactor.Success)
                {
                  textTerm = MoveToNextTextPart(textTerm, powerFactor);
                  double subResult = this.ProcessTextTerm(ref textTerm, true);
                  number = Math.Pow(number, subResult);
                }
                else
                {
                  // String after '^' is no valid whole number 
                  // or term surrounded by ( ) as a power factor 
                  throw new CalculationParseException(
                  "Syntax Error: power factor"
                  );
                }
              }
            }
            else
            {
              textTerm = MoveToNextTextPart(textTerm, currentMatch);
            }

            AddOperand(number);
          }
          else
          {
            throw new CalculationParseException("Syntax Error: invalid operand");
          }

          // textTerm = MoveToNextTextPart(textTerm, currentMatch);
          expectsOpperand = false;
        }
        else
        {
          currentMatch = operratorRegex.Match(textTerm);

          if (currentMatch.Success)
          {
            string currentOperator = currentMatch.Groups["operator"].Value;

            if (prioOperands.Contains(currentOperator))
            {
              DigestPrioOperator(currentOperator);
            }
            else
            {
              lastOperandsWasPrio = false;
              operators.Add(currentMatch.Groups["operator"].Value);
            }
          }
          else
          {
            throw new CalculationParseException("Syntax Error: invalid operator");
          }
          
          textTerm = MoveToNextTextPart(textTerm, currentMatch);
          expectsOpperand = true;
        }        
      }

      if (looksCloseParanthese && !foundClosingParanthese)
      {
        throw new CalculationParseException("Syntax Error: Closing parentheses !");
      }
      else if (expectsOpperand)
      {
        throw new CalculationParseException("Syntax Error: missing operand !");
      }


      // Calculating priority terms aka point before line calculation ..
      for (int i = 0, count = prioStartIndexes.Count; i < count; i++)
      {
        operands[prioStartIndexes[i]] = ProcessMacroTerm(
          operandsPrio[i], operatorsPrio[i], CalculateOnePrioOperation
          );
      }

      this._stackCounter--;

      return result + ProcessMacroTerm(operands, operators, CalculateOneOperation);

      // Parameter: operandMatch is a match of a valid operand in a string.
      // Returns numeric signed value of an operand
      static double GetNumericOperandFromMatch(Match operandMatch)
      {
        string sign = operandMatch.Groups["sign"].Value;
        var number = Double.Parse(operandMatch.Groups["number"].Value);
        return ProcessPlusMinusSeq(sign, number);
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

      static string MoveToNextTextPart(string text, Match currentMath)
        => text[(currentMath.Index + currentMath.Length)..];

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

        return firstOperand;
      }

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
            if (secondOperand == 0.0)
            {
              throw new DivideByZeroException(
                "Mathematical Error: One denominator is zero in a fraction !"
                );
            }
            firstOperand /= secondOperand;
            break;
          default:
            break;
        }

        return firstOperand;
      }

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

      // 
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


    }

    public void Clear() => this.CurrentResult = 0.0;
  }
}
