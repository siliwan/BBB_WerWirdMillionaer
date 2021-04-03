const host = "localhost"
const port = 44313
const apiPath = "/api"

import axios from "axios";

export let sessionId = localStorage.getItem("x-quiz-session-id") || undefined;

export const request = (() => { let instance = axios.create({
    baseURL: `https://${host}:${port}${apiPath}`,
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