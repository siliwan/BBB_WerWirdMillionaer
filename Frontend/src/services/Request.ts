import axios from "axios";
import Settings from "@/Settings";

export let sessionId = localStorage.getItem("x-quiz-session-id") || undefined;

export let request = (() => { 

let baseUrl = `${Settings.protocol}://${Settings.hostname}:${Settings.port}${Settings.apiEndpoint}`

let instance = axios.create({
    baseURL: baseUrl,
    headers: {
        'x-quiz-session-id': sessionId
    },
})

instance.interceptors.response.use((res) => {
    if(res.headers['x-quiz-session-id'] != undefined && res.headers['x-quiz-session-id'] != '') {
        localStorage.setItem("x-quiz-session-id", res.headers['x-quiz-session-id'])
    }
    return res;
});

return instance;
})()