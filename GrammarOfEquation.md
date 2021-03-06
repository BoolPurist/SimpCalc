# Description of the Grammar of a valid equation

Startsymbol = equation

| Symbol| | Subsymbols of Symbol |
|:----|:-:|:-------------------------------------------------------------------|
| equation | = | spaceOperand ( ( operator spaceOperand ) \| equationInParatheseWithSpace \| ( operator equationInParatheseWithSpace)  )\* WhiteSpace\* |
| equationInParatheseWithSpace | = | WhiteSpace* equationInParathese |
| equationInParathese | = | openingParathese equation closingParathese surroundedOperatorWithParam |
| spaceOperand | = | WhiteSpace* operand |
| operand | = | ( operandAsNumber \| operandWithSurroundedOperator \| operandWithRightOperator \| operandAsFunctionWithBase \| operandAsFunction \| operandAsIntegerFunction ) |
| operandWithSurroundedOperator | = | operrandAsNumber surroundedOperatorWithParam |
| surroundedOperatorWithParam | = | surroundedOperator ( integer \| equationInParathese ) |
| operandWithRightOperator | = | operatorWithoutNeededLeft ( integer \| equationInParathese ) |
| operandAsFunctionWithBase | = | operandFunctionBaseNeeded ( equationInParathese \| operand ) equationInParathese |
| operandAsFunction | = | operandFunction equationInParathese |
| operandAsIntegerFunction | = | integer operandFunctionsWithIntegers |
| operator | = | WhiteSpace* ( sign \| prioritySign )  |
| operandAsNumber | = | constSign\* \| (floatingNumber constSign\*)  |
| floatingNumber | = | integer fractionalPartOfNumber? |
| integer | = | signSequence number |
| signSequence | = | sign* |
| fractionalPartOfNumber | = | ( . \| , ) number |
| constSign | = | pi \| π \| e |
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
