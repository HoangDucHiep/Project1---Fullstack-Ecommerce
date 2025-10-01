// src/slices/userSlice.ts
import { createSlice } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit"; // ✅ import type riêng


interface UserState {
    isAdmin: boolean;
    name: string;
}

const initialState: UserState = {
    isAdmin: false,
    name: "",
};

const userSlice = createSlice({
    name: "user",
    initialState,
    reducers: {
        login(state, action: PayloadAction<{ name: string; isAdmin: boolean }>) {
            state.name = action.payload.name;
            state.isAdmin = action.payload.isAdmin;
        },
        logout(state) {
            state.name = "";
            state.isAdmin = false;
        },
    },
});

export const { login, logout } = userSlice.actions;
export default userSlice.reducer;
