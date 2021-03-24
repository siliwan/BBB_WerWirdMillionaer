export interface IReferencable { $id: string }
export interface IReferencableArray<T = Reference> extends IReferencable { $values: T[] }

export type Reference<T = IReferencable> = { $ref: string, value: () => T }

function isReference(obj: any): obj is Reference { return nameof<Reference>('$ref') in obj }
export const nameof = <T>(name: keyof T) => name;
export const typedUnitialized = <T>() => null as unknown as T

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

export enum PlayState {
  Menu,
  Playing,
  Won,
  Lost
}

export enum SubmissionResult {
  Won,
  Lost,
  Correct,
  TimeUp,
  Invalid
}

export class SubmissionResultWithMessage implements IReferencable {
  $id!: string;
  object!: SubmissionResult;
  message!: string;
}

export class HighScore implements IReferencable {
  $id!: string;
  id!: number;
  timeStamp!: Date;
  pointsAchieved!: number;
  duration!: number;
  rank!: number;
  pointsWeighted!: number;
  categories!: IReferencableArray<Reference<Category>>;
}

export class HighScoreResult implements IReferencableArray<HighScore> {
  $id!: string;
  $values!: HighScore[];
}

export class User implements IReferencable{
  $id!: string;
  id!: number;
  username!: string;
}

export class ValidationErrorResponse {
  type!: string;
  title!: string;
  status!: number;
  traceId!: string;
  errors!: {
    $: string[]
  }
}

export class ReferencableJsonWrapper<T> extends String {

  public parseJSON(parser: IReferencableJsonParser<T>) : T {
    return parser.parse((this as String).valueOf());
  }

}
export interface IReferencableJsonParser<T> { parse(json: string): T};
export class ReferencableJsonParser<T> implements IReferencableJsonParser<T> {

    constructor() {

    }

    public parse(json: string): T {
        var $refTable: Map<string, any> = new Map();

        if(typeof json !== 'string') {
          json = JSON.stringify(json);
        }

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