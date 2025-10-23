import axios from "axios";

const api = axios.create({
    baseURL: "https://localhost:5001", // backend .NET của bạn
    headers: {
        "Content-Type": "application/json",
    },
});
export default api;
