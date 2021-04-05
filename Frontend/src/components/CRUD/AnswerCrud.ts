import { Answer, IReferencableArray } from '@/ResponseTypes';
import { request } from '@/services/Request';

export class AnswerCrud  {
    endpoint: string;

    constructor() {
        this.endpoint = "/Answer";
    }
    async getAll(): Promise<Answer[]> {
        let res = await request.get<IReferencableArray<Answer>>(this.endpoint);
        return res.data.$values;
    }
    async get(id: number): Promise<Answer> {
        let res = await request.get<Answer>(`${this.endpoint}/${id}`);
        return res.data;
    }
    async create(answerText: string, isCorrect: boolean) {
        let res = await request.post(this.endpoint, JSON.stringify({
            answerText,
            isCorrect
        }));
        return res;
    }
    async update(id: number, answerText: string, isCorrect: boolean) {
        let res = await request.put(`${this.endpoint}/${id}`, JSON.stringify({
            answerText,
            isCorrect
        }), {
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return res;
    }
    async delete(id: number) {
        let res = await request.delete(`${this.endpoint}/${id}`);
        return res;
    }
}

export default new AnswerCrud();