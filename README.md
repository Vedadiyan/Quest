# Quest 
*Quest is a simple querying language for RESTful APIs. It is so simple as it does not even need any documentation!* 

## Getting Started

Quest has a JSON like syntax and supports a variety of options: 

|keyword|description  | example|
|--|--|--|
| eq | Equal to | `{ eq : 'abc' }`
| gt | Greater than | `{ gt : 1 }`
| gte| Greater than or Equal to| `{ gte : 1 }`
| lt | Less than | `{ lt: 1 }`
| lte| Less than or Equal to| `{ lte: 1 }`
| not| Not Equal to| `{ not : 1 }`
| in | Include| `{ in: [1,2,3] }` 
| matches| Regex Pattern Matching| `{ matches : '[Aa-Zz]'}`
| and *| And Operator| `{ and: [{ lte : 1 }, { not: 0 }] }`
| or | Or Operator | `{ or: [{ lte : 1 }, { not: 0 }] }`
 

 \* the `and` operator is not required in most cases as an `and` expression is equivalent to `{ lte : 1 , not : 0 }`

## Evaluation
A Quest expression can be evaluated by calling the `Eval` method: 

	int year = 2019;
    QuestReader readExpreesion = new QuestReader("{ gt : 2017 , lt : 2021 }");
    bool result = readExpression.Eval(year);

--- 
Support: vedadiyan@gmail.com
