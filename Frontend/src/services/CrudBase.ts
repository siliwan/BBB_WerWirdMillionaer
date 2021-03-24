import { request } from "../services/Request";

export abstract class Crud<T> {
    private endpoint: string;

    constructor(endpoint: string) {
        this.endpoint = endpoint;
    }

    abstract getAll(): T[]
}