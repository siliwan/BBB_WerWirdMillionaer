import _Vue from "vue";
import AuthApi from "@/services/AuthApi";

export default async function AuthPluginFunction (Vue: typeof _Vue, options?: any): Promise<void>  {
    const instance = new AuthInstance();
    await instance.Refresh();
    Vue.prototype.$auth = instance;
}

class AuthInstance {

    private isAuthenticated = false;

    public async Refresh() {
        this.isAuthenticated = await AuthApi.IsLoggedIn();
    }

    public get IsAuthenticated() {
        return this.isAuthenticated;
    }

}