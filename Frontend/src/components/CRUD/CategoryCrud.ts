import { Category, IReferencableArray } from '@/ResponseTypes';
import { request, sessionId } from '@/services/Request';

export class CategoryCrud  {
    endpoint: string;

    constructor() {
        this.endpoint = "/Category";
    }
    async getAll(): Promise<Category[]> {
        let res = await request.get<IReferencableArray<Category>>(this.endpoint);
        return res.data.$values;
    }
    async get(id: number): Promise<Category> {
        let res = await request.get<Category>(`${this.endpoint}/${id}`);
        return res.data;
    }
    async create(categoryName: string) {
        let res = await request.post(this.endpoint, categoryName);
        return res;
    }
    async update(id: number, newCategoryName: string) {
        let res = await request.put(`${this.endpoint}/${id}`, JSON.stringify(newCategoryName), {
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

export default new CategoryCrud();