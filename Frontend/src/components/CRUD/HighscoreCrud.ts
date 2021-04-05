import { HighScore, HighScoreResult } from "@/ResponseTypes";
import { request } from "@/services/Request";

export class HighscoreCrud {
    endpoint: string;

    constructor() {
        this.endpoint = "/HighScore";
    }

    async getAll(): Promise<HighScore[]> {
        let res = await request.get<HighScoreResult>(this.endpoint);
        return res.data.$values;
    }

    async get(id: number): Promise<HighScore> {
        let res = await request.get<HighScore>(`${this.endpoint}/${id}`);
        return res.data;
    }

    async delete(id: number): Promise<void> {
        await request.delete<HighScoreResult>(`${this.endpoint}/${id}`);
    }
}

export default new HighscoreCrud();