import {type TypedUseSelectorHook, useDispatch, useSelector } from "react-redux";
import type { RootState, AppDispatch } from "../store.ts";

// ✅ Hook thay thế useDispatch
export const useAppDispatch: () => AppDispatch = useDispatch;

// ✅ Hook thay thế useSelector
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;
