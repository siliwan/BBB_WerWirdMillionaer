export interface IReferencable { $id: string }
export interface IReferencableArray<T> extends IReferencable { $values: T[] }

export type Reference<T = IReferencable> = { $ref: string, value: () => T }

function isReference(obj: any): obj is Reference { return nameof<Reference>('$ref') in obj }
const nameof = <T>(name: keyof T) => name;

export class CurrentQuestion implements IReferencable {
    $id!: string;
    timeLeftUntil!: Date;
    percentCorrect!: number;

    question!: Question;
    category!: Category;
    answers!: IReferencableArray<Answer>
}

export class Question implements IReferencable {
    $id!: string;
    id!: number;
    questionText!: string;
    questionStatistic!: QuestionStatistic
}

export class QuestionStatistic implements IReferencable {
    $id!: string;
    id!: number;
    answeredCorrect!: number;
    answeredWrong!: number;
    queestionId!: number;
    question!: Reference<Question>
}

export class Category implements IReferencable {
    $id!: string;
    id!: number;
    name!: string;
    questions!: IReferencableArray<Reference<Question>>
}

export class Answer implements IReferencable {
    $id!: string;
    id!: number;
    answerText!: string;
    isCorrect!: boolean;
    question!: Reference<Question>
}

export class ReferencableJsonParser<T> {
    

    constructor() {

    }


    public parse(json: string): T {
        var $refTable: Map<string, any> = new Map();
        let $obj: T = JSON.parse(json, (key, value) => {
            console.log(key, value);

            if(key == nameof<IReferencable>("$id")) {
                $refTable.set(value, this);
            }
            return value;

        });

        function refWalk(holder: any, key: string): any {
            var k;
            var v;
            var value = holder[key];
            if (value && typeof value === "object") {
                for (k in value) {
                    if (Object.prototype.hasOwnProperty.call(value, k)) {
                        v = refWalk(value, k);
                        
                        if(isReference(v)) {
                            let $ref = v.$ref;
                            v.value = () => $refTable.get($ref);
                        }
                    }
                }
            }

            return v;
        }

        for(let propKey in Object.getOwnPropertyNames($obj)) {
            refWalk($obj, propKey);
        }

        return $obj;
    }

}



/*{
  "$id": "1",
  "timeLeftUntil": "0001-01-01T00:00:30",
  "question": {
    "$id": "2",
    "id": 1,
    "questionText": "How many moons orbit the earth?",
    "questionStatistic": {
      "$id": "3",
      "id": 1,
      "answeredCorrect": 0,
      "answeredWrong": 0,
      "questionId": 1,
      "question": {
        "$ref": "2"
      }
    },
    "category": {
      "$id": "4",
      "id": 1,
      "name": "Planets",
      "questions": {
        "$id": "5",
        "$values": [
          {
            "$ref": "2"
          }
        ]
      }
    },
    "answers": {
      "$id": "6",
      "$values": [
        {
          "$id": "7",
          "id": 1,
          "answerText": "0",
          "isCorrect": false,
          "question": {
            "$ref": "2"
          }
        },
        {
          "$id": "8",
          "id": 2,
          "answerText": "1",
          "isCorrect": true,
          "question": {
            "$ref": "2"
          }
        },
        {
          "$id": "9",
          "id": 3,
          "answerText": "2",
          "isCorrect": false,
          "question": {
            "$ref": "2"
          }
        },
        {
          "$id": "10",
          "id": 4,
          "answerText": "3",
          "isCorrect": false,
          "question": {
            "$ref": "2"
          }
        }
      ]
    }
  },
  "percentCorrect": 0
}*/