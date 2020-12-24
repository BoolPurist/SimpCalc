# Description of the Grammar of a valid equation

Startsymbol = equation

| Symbol| | Subsymbols of Symbol |
|:----|:-:|:-------------------------------------------------------------------|
| equation | = | spaceOperand ( ( operator spaceOperand ) \| equationInParatheseWithSpace \| ( operator equationInParatheseWithSpace)  )\* WhiteSpace\* |
| equationInParatheseWithSpace | = | WhiteSpace* equationInParathese |
| equationInParathese | = | openingParathese equation closingParathese |
| spaceOperand | = | WhiteSpace* operand |
| operand | = | ( operandAsNumber \| operandWithSurroundedOperator \| operandWithRightOperator \| operandAsFunctionWithBase \| operandAsFunction \| operandAsIntegerFunction ) |
| operandWithSurroundedOperator | = | operrandAsNumber surroundedOperator ( integer \| equationInParathese ) |
| operandWithRightOperator | = | operatorWithoutNeededLeft ( integer \| equationInParathese ) |
| operandAsFunctionWithBase | = | operandFunctionBaseNeeded ( equationInParathese \| operand ) equationInParathese |
| operandAsFunction | = | operandFunction equationInParathese |
| operandAsIntegerFunction | = | integer operandFunctionsWithIntegers |
| operator | = | WhiteSpace* ( sign \| prioritySign )  |
| operandAsNumber | = | piSing\* \| (floatingNumber piSign\*)  |
| floatingNumber | = | integer fractionalPartOfNumber? |
| integer | = | signSequence number |
| signSequence | = | sign* |
| fractionalPartOfNumber | = | ( . \| , ) number |
| piSign | = | pi \| π |
| number | = | ( 0 \| 1 \| 2 \| 3 \| 4 \| 5 \| 6 \| 7 \| 8 \| 9 )+ |
| sign | = | + \| - |
| prioritySign  | = | ( * \| / \| % ) |
| operandFunctionsWithIntegers | = | ! |
| surroundedOperator | = | ^ \| E \| surroundedOperatorWithoutNeededLeft |
| operatorWithoutNeededLeft | = |  √ \| R  |
| operandFunctionBaseNeeded | = | log |
| operandFunction | = | cos \| sin \| tan \| cocos \| cosin \| cotan \| ln |
| openingParathese | = | ( |
| closingParathese | = | ) |
