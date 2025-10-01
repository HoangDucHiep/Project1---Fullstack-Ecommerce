// src/store.ts
import { configureStore } from "@reduxjs/toolkit";
import useReducer from "./slices/userSlice.ts";
 // ví dụ slice quản lý user

export const store = configureStore({
    reducer: {
        user: useReducer, // bạn có thể thêm nhiều reducer khác
    },
});

// ✅ Export type để dùng cho hooks
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
