using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Calculator_Window
{
  public class CalculatorModel
  {
    public double CurrentResult { get; private set; } = 0.0;

    protected readonly Regex operrandRegex = 
      new Regex(@"^\s*(?<sign>[+-]*)(?<number>(\d+)+([\.,](\d+))?)");

    protected readonly Regex operratorRegex = 
      new Regex(@"^\s*(?<operator>[\+\*-/])");
      
    public double CalculateFromText(string inputForCalc)
    {
      try
      {
        this.CurrentResult = this.ProcessTextTerm(inputForCalc.Trim());        
      }
      catch (CalculationParseException e)
      {
        throw e;
      }

      return this.CurrentResult;
    }

    private double ProcessTextTerm(string textTerm)
    {
      var expectsOpperand = true;
      var operands = new List<double>();
      var operators = new List<string>();

      Match currentMatch;

      // Parsing

      while (textTerm != String.Empty)
      {
        if (expectsOpperand)
        {
          currentMatch = this.operrandRegex.Match(textTerm);
          
          if (currentMatch.Success)
          {            
            string sign = currentMatch.Groups["sign"].Value;
            var number = Double.Parse(currentMatch.Groups["number"].Value);
            operands.Add(number * ProcessPlusMinusSeq(sign));
            textTerm = MoveToNextTextPart(textTerm, currentMatch);
          }
          else
          {
            throw new CalculationParseException("Syntax Error: invalid operand");
          }

          expectsOpperand = false;
        }
        else
        {
          currentMatch = this.operratorRegex.Match(textTerm);

          if (currentMatch.Success)
          {
            operators.Add(currentMatch.Groups["operator"].Value);
            textTerm = MoveToNextTextPart(textTerm, currentMatch);
          }
          else
          {
            throw new CalculationParseException("Syntax Error: invalid operator");
          }

          expectsOpperand = true;
        }
      }

      var result = 0.0;

      if (operands.Count == 1)
      {
        result = operands[operands.Count - 1];
      }
      else
      {
        result = operands[0];
        operands.RemoveAt(0);

        while (operands.Count > 0)
        {

          switch (operators[0])
          {
            case "+":
              result += operands[0];
              break;
            case "-":
              result -= operands[0];
              break;
            default:
              break;
          }

          operands.RemoveAt(0);
          operators.RemoveAt(0);
        }

      }

      return result;

      static double ProcessPlusMinusSeq(string signSeq)
      {
        var currentSign = 1.0;

        foreach (char sign in signSeq)
        {
          if (sign == '-')
          {
            currentSign *= -1.0;
          }
        }

        return currentSign;
      }

    }

    static string MoveToNextTextPart(string text, Match currentMath) 
      => text[(currentMath.Index + currentMath.Length)..];

    public void Clear() => this.CurrentResult = 0.0;
  }
}
