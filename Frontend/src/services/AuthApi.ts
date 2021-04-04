import { User, UserPassword } from "@/ResponseTypes";
import { request } from "./Request";

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

    async Login(credentials: UserPassword): Promise<void> {
        await request.post<void>(`${this.endpoint}/Login`, JSON.stringify(credentials), {
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }
}

export default new AuthApi();