import { IReferencableArray, Question } from "@/ResponseTypes";
import { request } from "@/services/Request";

export class QuestionCrud {
    endpoint: string;

    constructor() {
        this.endpoint = "/Question";
    }

    async getAll(): Promise<Question[]> {
        let res = await request.get<IReferencableArray<Question>>(this.endpoint);
        return res.data.$values;
    }

    async get(id: number): Promise<Question> {
        let res = await request.get<Question>(`${this.endpoint}/${id}`);
        return res.data;
    }

    async post(questionText: string, categoryId: number, answers: {answerText: string, isCorrect: boolean}[]): Promise<Question> {
        let sendObject = {
            questionText,
            categoryId,
            answers
        }
        let res = await request.post<Question>(`${this.endpoint}`, JSON.stringify(sendObject), {
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return res.data;
    }
    
    async put(id: number, categoryId: number, questionText: string): Promise<void> {
        await request.put<void>(`${this.endpoint}/${id}`, JSON.stringify({
            categoryId,
            questionText
        }), {
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }

    async delete(id: number): Promise<void> {
        await request.delete<void>(`${this.endpoint}/${id}`);
    }
}

export default new QuestionCrud();