import { User, UserPassword } from "@/ResponseTypes";
import { AxiosError, AxiosResponse } from "axios";
import { request } from "./Request";

type ResponseWithError = {
    success: false
    error: string
}

type ResponseWithoutError = {
    success: true
    user: User
}

type ResponseResult = ResponseWithError | ResponseWithoutError;

class AuthApi {
    private endpoint!: string;

    constructor() {
        this.endpoint = "/Login";
    }

    async IsLoggedIn(): Promise<boolean> {
        try {
            await this.LoginInformation();
            return true;
        } catch (error) {
            return false;
        }
    }

    async LoginInformation(): Promise<User> {
        const res = await request.get<User>(`${this.endpoint}/LoginInformation`);
        return res.data;
    }

    async Logout(): Promise<void> {
        await request.post<void>(`${this.endpoint}/Logout`);
    }

    async Login(credentials: UserPassword): Promise<ResponseResult> {
        try {
            let res = await request.post<User>(`${this.endpoint}/Login`, JSON.stringify(credentials), {
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            return {
                success: true,
                user: res.data
            };
        } catch (error) {
            const res = error['response'] as AxiosResponse;
            if(res.status == 401 || res.status == 403) {
                return {
                    success: false,
                    error: res.data
                };
            }
            return {
                success: false,
                error: "Something went wrong while authentication. Please try again!"
            };
        }
    }
}

export default new AuthApi();