﻿using System;
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
      new Regex(@"^\s*(?<operator>[+*-/])");

    protected readonly HashSet<string> prioOperands = 
      new HashSet<string>( new string[] { "*", "/" } );
    
      
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
      var lastOperandsWasPrio = false;
      var operands = new List<double>();
      var operators = new List<string>();
      var prioStartIndexes = new List<int>();
      var operandsPrio = new List< List<double> >();
      var operatorsPrio = new List< List<string> >();
      List<double> currentOperandsPrio = null;
      List<string> currentOperatorsPrio = null;

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

            if (lastOperandsWasPrio)
            {
              currentOperandsPrio.Add((double)(ProcessPlusMinusSeq(sign, number)));
            }
            else
            {
              operands.Add(ProcessPlusMinusSeq(sign, number));
            }            
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
            string currentOperator = currentMatch.Groups["operator"].Value;

            if (this.prioOperands.Contains(currentOperator))
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
              
              currentOperatorsPrio.Add(currentOperator);
              lastOperandsWasPrio = true;
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

          expectsOpperand = true;
        }

        textTerm = MoveToNextTextPart(textTerm, currentMatch);
      }

      // Calculating prio ..
      for (int i = 0, count = prioStartIndexes.Count; i < count; i++)
      {
        operands[prioStartIndexes[i]] = ProcessMacroTerm(
          operandsPrio[i], operatorsPrio[i], CalculateOnePrioOperation
          );
      }

      return ProcessMacroTerm(operands, operators, CalculateOneOperation);

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
        var currentOperand = operands[0];
        var result = currentOperand;
        operands.RemoveAt(0);

        while (operands.Count > 0)
        {
          result = calculatorLogic(operators[0], result, operands[0]);

          operands.RemoveAt(0);
          operators.RemoveAt(0);
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
            firstOperand /= secondOperand;
            break;
          default:
            break;
        }

        return firstOperand;
      }

    }

    public void Clear() => this.CurrentResult = 0.0;
  }
}
