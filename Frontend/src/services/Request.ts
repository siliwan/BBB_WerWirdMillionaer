const host = "localhost"
const port = 44313
const apiPath = "/api"

import axios from "axios";

export const request = axios.create({
    baseURL: `https://${host}:${port}${apiPath}`
});