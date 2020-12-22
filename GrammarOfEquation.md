# Description of the Grammar of a valid equation

Startsymbol = equation

| Symbol| | Subsymbols of Symbol |
|:----|:-:|:-------------------------------------------------------------------|
| equation | = |operand ( ( operator operand ) \| equationInParatheseWithSpace \| ( operator equationInParatheseWithSpace)  )* |
| equationInParatheseWithSpace | = | WhiteSpace* equationInParathese |
| equationInParathese | = | openingParathese equation closingParathese |
| operand | = | WhiteSpace* ( operandAsNumber \| operandWithSurroundedOperator \| operandAsFunctionWithBase \| operandAsFunction \| operandAsWholeNumberFunction ) |
| operandWithSurroundedOperator | = | operrandAsNumber surroundedOperator ( wholeNumber \| equationInParathese ) |
| operandAsFunctionWithBase | = | operandFunctionBaseNeeded operandAsNumber equationInParathese |
| operandAsFunction | = | operandFunction equationInParathese |
| operandAsWholeNumberFunction | = | wholeNumber operandFunctionsWithWholeNumbers |
| operator | = | WhiteSpace* ( sign \| prioritySign )  |
| operandAsNumber | = | wholeNumber fractionalPartOfNumber? |
| wholeNumber | = | sign number |
| fractionalPartOfNumber | = | ( . \| , ) number |
| number | = | ( 0 \| 1 \| 2 \| 3 \| 4 \| 5 \| 6 \| 7 \| 8 \| 9 )+ |
| sign | = | + \| - |
| prioritySign  | = | ( * \| / \| % ) |
| operandFunctionsWithWholeNumbers | = | ! |
| surroundedOperator | = | ^ \| E \| √ \| R |
| operandFunctionBaseNeeded | = | log |
| operandFunction | = | cos \| sin \| tan \| cosa \| sina \| tana \| ln |
| openingParathese | = | ( |
| closingParathese | = | ) |